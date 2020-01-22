using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ximena.Configuration;

namespace Ximena.Binary
{
    internal static class PropertyInfoParser
    {
        public static PropertyDefinition BuildPropertyDefinition(PropertyInfo info)
        {
            var prop = new PropertyDefinition();
            prop.SetName(info.Name);
            string[] nsc;
            prop.type = ResolveTypeName(info.PropertyType, out nsc);
            prop.SetDependencies(nsc);

            prop.readOnly = info.GetSetMethod() == null;

            return prop;
        }
        public static CollectionDefinition BuildCollectionDefinition(PropertyInfo info)
        {
            var coll = new CollectionDefinition();
            coll.SetName(info.Name);
            coll.collectionType = GetGenericTypeName(info.PropertyType);
            coll.typeParam = GetIListTypeParam(info.PropertyType);
            coll.SetDependencies(GetDependencies(info.PropertyType));

            coll.readOnly = info.GetSetMethod() == null;

            return coll;
        }
        public static bool IsPropertyIList(PropertyInfo info)
        {
            return GetIListInterface(info.PropertyType) != null;
        }

        private static string ResolveTypeName(Type type, out string[] dependencies)
        {
            HashSet<string> nspaces = new HashSet<string>();
            nspaces.Add(type.Namespace);
            string formattedName = string.Empty;
            string name = type.Name;

            //handle generic type
            if (name.Contains("`"))
            {
                name = name.Split('`')[0];
                // handle nullable (e.g. Nullable<int>)
                if (name == "Nullable")
                {
                    // get the Nullable type parameter -- that's what we consider to be the type
                    string[] subNamespaces = null;
                    formattedName =
                        ResolveTypeName(type.GetGenericArguments()[0], out subNamespaces);
                    foreach (var ns in subNamespaces) { nspaces.Add(ns); }

                    // we'll emit nullable types like int?, instead of Nullable<int>
                    formattedName += "?";
                }
                // any other generic
                else
                {
                    var typeParams = type.GetGenericArguments();
                    var typeParamNames = new List<string>();
                    string[] subNamespaces = null;
                    foreach (var tp in typeParams)
                    {
                        var utn = ResolveTypeName(tp, out subNamespaces);
                        foreach (var ns in subNamespaces) { nspaces.Add(ns); }
                        typeParamNames.Add(utn);
                    }

                    formattedName = $"{name}<{string.Join(", ", typeParamNames)}>";
                }
            }
            // non generic types
            else
            {
                switch (name)
                {
                    case "String":
                        formattedName = "string";
                        break;
                    case "Boolean":
                        formattedName = "bool";
                        break;
                    case "Int16":
                        formattedName = "short";
                        break;
                    case "UInt16":
                        formattedName = "ushort";
                        break;
                    case "Int32":
                        formattedName = "int";
                        break;
                    case "UInt32":
                        formattedName = "uint";
                        break;
                    case "Int64":
                        formattedName = "long";
                        break;
                    case "UInt64":
                        formattedName = "ulong";
                        break;
                    case "Single":
                        formattedName = "float";
                        break;
                    case "Double":
                        formattedName = "double";
                        break;
                    case "Decimal":
                        formattedName = "decimal";
                        break;
                    case "Byte":
                        formattedName = "byte";
                        break;
                    default:
                        formattedName = name;
                        break;
                }
            }

            dependencies = nspaces.ToArray();
            return formattedName;
        }
        private static string ResolveTypeName(Type type)
        {
            string[] ns;
            return ResolveTypeName(type, out ns);
        }

        private static Type GetIListInterface(Type type)
        {
            return type.GetInterfaces()
               .FirstOrDefault(i => i.IsGenericType && 
               i.GetGenericTypeDefinition() == typeof(IList<>));
        }
        public static string GetIListTypeParam(Type type)
        {
            var iType = GetIListInterface(type);
            if (iType != null)
            {
                return ResolveTypeName(iType.GetGenericArguments()[0]);
            }

            //not an IList
            return null;
        }

        private static string GetGenericTypeName(Type type)
        {
            return type.Name.Split('`')[0];
        }
        private static string[] GetDependencies(Type type)
        {
            HashSet<string> nspaces = new HashSet<string>();
            nspaces.Add(type.Namespace);

            //handle generic type
            if (type.Name.Contains("`"))
            {
                var typeParams = type.GetGenericArguments();
                var typeParamNames = new List<string>();
                foreach (var tp in typeParams)
                {
                    string[] subNamespaces = GetDependencies(tp);
                    foreach (var ns in subNamespaces) { nspaces.Add(ns); }
                }
            }

            return nspaces.ToArray();
        }
    }
}
