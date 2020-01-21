using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NotViewModelPropertyAttribute : Attribute
    {
        public NotViewModelPropertyAttribute() { }
    }
}
