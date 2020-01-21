using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ViewModelPropertyAttribute : Attribute
    {
        public readonly bool ReadOnly;
        public readonly string Summary;
        public readonly string AccessModifier;

        public ViewModelPropertyAttribute(bool readOnly = false,
            string summary = null,
            string accessModifier = "public")
        {
            AccessModifier = accessModifier;
            ReadOnly = readOnly;
            Summary = summary;
        }
    }
}
