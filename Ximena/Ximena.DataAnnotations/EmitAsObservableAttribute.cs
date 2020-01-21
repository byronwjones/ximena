using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EmitAsObservableAttribute : Attribute 
    {
        public readonly bool EmitTypeParamAsViewModel;

        public EmitAsObservableAttribute(bool emitTypeParamAsViewModel = false)
        {
            EmitTypeParamAsViewModel = emitTypeParamAsViewModel;
        }
    }
}
