using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ViewModelAttribute: Attribute
    {
        public readonly string ViewModelType;
        public readonly string AccessModifier;
        public readonly bool? PartialClass;
        public readonly bool? InheritUsings;
        public readonly string[] Usings;
        public readonly bool? CreateStubForCustomCode;
        public readonly bool? EmitAllProperties;
        public readonly bool? EmitCollectionsAsObservable;
        public readonly bool? EmitEntityPropertiesAsViewModels;
        public readonly string EmitToDir;

        public ViewModelAttribute(string accessModifier = null,
            bool? partialClass = null,
            bool? inheritUsings = null,
            string[] usings = null,
            bool? createStubForCustomCode = null,
            bool? emitAllProperties = null,
            bool? emitCollectionsAsObservable = null,
            bool? emitEntityPropertiesAsViewModels = null,
            string viewModelType = null,
            string emitToDir = null)
        {
            AccessModifier = accessModifier;
            PartialClass = partialClass;
            InheritUsings = inheritUsings;
            Usings = usings ?? new string[0];
            CreateStubForCustomCode = createStubForCustomCode;
            EmitAllProperties = emitAllProperties;
            EmitCollectionsAsObservable = emitCollectionsAsObservable;
            EmitEntityPropertiesAsViewModels = emitEntityPropertiesAsViewModels;
            ViewModelType = viewModelType;
            EmitToDir = emitToDir;
        }
    }
}
