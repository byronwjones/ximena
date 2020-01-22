using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ximena.Configuration;
using Ximena.DataAnnotations;

namespace Ximena.Binary
{
    public static class AssemblyParser
    {
        public static void Parse(string assemblyPath, RenderSettings settings)
        {
            var assembly = LoadAssembly(assemblyPath);
            // get all entities in this assembly
            var allEntities = assembly.GetTypes().Where(t => t.IsClass).ToList();

            PruneSettings(allEntities, settings);

            var vmEntities = DiscoverViewModelEntities(allEntities, settings);
        }


        private static void BuildViewModelSettingsForEntity(Type entity, RenderSettings settings)
        {
            var attr = entity.GetCustomAttribute<ViewModelAttribute>();
            var vms = new ViewModelSettings();
            vms.EntityType = entity.Name;
            vms.EntityNamespace = entity.Namespace;

            if (attr != null)
            {
                ApplyViewModelAttribute(vms, attr);
            }

            //inherit view model settings here

            //DiscoverViewModelProperties
        }
        private static void ApplyViewModelAttribute(ViewModelSettings vm, ViewModelAttribute attr)
        {
            vm.access = attr.AccessModifier;
            vm.emitAllProperties = attr.EmitAllProperties;
            vm.emitCollectionsAsObservable = attr.EmitCollectionsAsObservable;
            vm.emitEntityPropertiesAsViewModels = attr.EmitEntityPropertiesAsViewModels;
            vm.emitTo = attr.EmitToDir;
            vm.inheritUsings = attr.InheritUsings;
            vm.partialClass = attr.PartialClass;
            vm.stubForCustomCode = attr.CreateStubForCustomCode;
            vm.type = attr.ViewModelType;
            foreach(var u in attr.Usings)
            {
                vm.usings.Add(u);
            }
        }
        private static List<PropertyInfo> DiscoverViewModelProperties(ViewModelSettings vm, Type entity)
        {
            List<PropertyInfo> properties = null;
            if (vm.emitAllProperties == true)
            {
                properties = entity.GetProperties()
                    .Where(p => p.GetCustomAttribute<NotViewModelPropertyAttribute>() == null)
                    .ToList();
            }
            else
            {
                properties = entity.GetProperties()
                    .Where(p => p.GetCustomAttribute<ViewModelPropertyAttribute>() != null &&
                    p.GetCustomAttribute<NotViewModelPropertyAttribute>() == null)
                    .ToList();
            }

            return properties;
        }
        private static void ConfigureViewModelProperty(PropertyInfo propInfo, ViewModelSettings vm,
            List<Type> entities)
        {
            MemberDefinition propDef;
            if (PropertyInfoParser.IsPropertyIList(propInfo))
            {
                propDef = PropertyInfoParser.BuildCollectionDefinition(propInfo);
            }
            else
            {
                propDef = PropertyInfoParser.BuildPropertyDefinition(propInfo);
            }

            var attr = propInfo.GetCustomAttribute<ViewModelPropertyAttribute>();
            if (attr != null)
            {
                ApplyViewModelPropertyAttribute(propDef, attr);
            }

            // submit namespaces used by this property as usings
            var deps = propDef.GetDependencies();
            foreach (var ns in deps)
            {
                vm.usings.Add(ns);
            }

            // collections
            if(propDef is CollectionDefinition)
            {
                ConfigureCollectionProperty(propInfo, vm, entities, propDef);
            }
            // entities
            else if(entities.Any(e=>e.Name == propDef.GetPropertyType()))
            {
                ConfigureEntityProperty(propInfo, vm, propDef);
            }

            // attach to view model
        }
        private static void ConfigureCollectionProperty(PropertyInfo propInfo, ViewModelSettings vm, List<Type> entities, MemberDefinition propDef)
        {
            var doNotEmitAsObservable =
                                propInfo.GetCustomAttribute<DoNotEmitAsObservableAttribute>() != null;
            var emitAsObservable = propInfo.GetCustomAttribute<EmitAsObservableAttribute>();

            // if not explicitly restricted from emitting this an observable,
            // and we are implicitly making collections observable or there is a explicit
            // designation of this property as observable, make it so
            if (doNotEmitAsObservable == false &&
                (vm.emitCollectionsAsObservable == true || emitAsObservable != null))
            {
                var coll = propDef as CollectionDefinition;
                coll.collectionType = "ObservableCollectionPlus";

                // determine whether or not to emit this collection's type parameter
                // as a view model.  This is something we'll need to do later on,
                // once we have all of our view model settings in place, as we'll
                // need to do a lookup on these settings to get the view model name
                coll.SetEmitAsViewModel(
                    emitAsObservable?.EmitTypeParamAsViewModel == true &&
                    entities.Any(e => e.Name == coll.GetName()));
            }
        }
        private static void ConfigureEntityProperty(PropertyInfo propInfo, ViewModelSettings vm, MemberDefinition propDef)
        {
            var doNotEmitAsViewModel =
                                propInfo.GetCustomAttribute<DoNotEmitAsViewModelAttribute>() != null;
            var emitAsViewModel = propInfo.GetCustomAttribute<EmitAsViewModelAttribute>() != null;

            // if not explicitly restricted from emitting this as a view model,
            // and we are implicitly emitting entity properties as view models or there is a explicit
            // instruction to emit this property as a view model, make it so
            propDef.SetEmitAsViewModel(doNotEmitAsViewModel == false &&
                (vm.emitEntityPropertiesAsViewModels == true || emitAsViewModel == true));
        }

        private static void ApplyViewModelPropertyAttribute(MemberDefinition prop,
            ViewModelPropertyAttribute attr)
        {
            prop.access = attr.AccessModifier;
            prop.readOnly = attr.ReadOnly;
            prop.summary = attr.Summary;
        }

        private static void PruneSettings(List<Type> entities, RenderSettings settings)
        {
            var allNamespaces = new HashSet<string>();
            foreach (var t in entities)
            {
                allNamespaces.Add(t.Namespace);
            }

            PruneNamespaces(allNamespaces, settings);
            PruneEntities(entities, settings);
        }
        private static void PruneNamespaces(HashSet<string> namespaces, RenderSettings settings)
        {
            // prune any namespaces declared in the settings file not actually in the assembly
            List<string> remove = new List<string>();
            foreach(var ns in settings.namespaces)
            {
                if (!namespaces.Contains(ns.Key))
                {
                    remove.Add(ns.Key);
                }
            }
            foreach(var ns in remove)
            {
                PrintWarning($"Namespace '{ns}' does not exist in the target assembly.  Settings declared for this namespace will be ignored");
                settings.namespaces.Remove(ns);
            }
        }
        private static void PruneEntities(List<Type> entities, RenderSettings settings)
        {
            foreach (var ns in settings.namespaces)
            {
                PruneEntitiesForNamespace(ns.Key, entities, settings);
            }
        }
        private static void PruneEntitiesForNamespace(string nameSpace, List<Type> entities,
            RenderSettings settings)
        {
            var ns = settings.namespaces[nameSpace];
            // prune any entities declared in namespace settings not actually in the assembly
            List<string> remove = new List<string>();
            foreach (var vm in ns.viewModels)
            {
                if (!entities.Any(e => e.Namespace == nameSpace && e.Name == vm.Key))
                {
                    remove.Add(vm.Key);
                }
            }
            foreach (var vm in remove)
            {
                PrintWarning($"'{nameSpace}.{vm}' does not exist in the target assembly.  Settings declared for this entity will be ignored");
                ns.viewModels.Remove(vm);
            }
        }

        private static List<Type> DiscoverViewModelEntities(List<Type> entities, RenderSettings settings)
        {
            List<Type> implicitIncludes = null;
            // get all namespaces where all entities therein should have view models made for them
            var namespaces = settings.namespaces.Where(n => n.Value.emitAllEntities == true);
            if (namespaces.Any())
            {
                // get all entities in these namespaces not explicitly excluded from rendering
                implicitIncludes = entities.Where(c => c.IsClass &&
                    namespaces.Any(n => n.Key == c.Namespace) &&
                    c.GetCustomAttribute<NotViewModelAttribute>() == null)
                    .ToList();
            }

            List<Type> explicitIncludes = null;
            // all other namespaces - view models are explicitly marked
            namespaces = settings.namespaces.Where(n => n.Value.emitAllEntities == false);
            if (namespaces.Any())
            {
                // get all entities in these namespaces explicitly and unambiguously 
                // marked for rendering
                explicitIncludes = entities
                    .Where(c => namespaces.Any(n => n.Key == c.Namespace) &&
                    c.GetCustomAttribute<ViewModelAttribute>() != null &&
                    c.GetCustomAttribute<NotViewModelAttribute>() == null)
                    .ToList();
            }
            
            // merge lists
            implicitIncludes.AddRange(explicitIncludes);
            return implicitIncludes;
        }

        private static Assembly LoadAssembly(string assemblyPath)
        {
            try
            {
                var assmName = AssemblyName.GetAssemblyName(assemblyPath);
                var assm = Assembly.Load(assmName);

                return assm;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to load assembly '{assemblyPath}': {e.Message}", e);
            }
        }

        private static void PrintWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"WARN: {message}");
            Console.ResetColor();
        }
    }
}
