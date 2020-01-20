using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public sealed class ViewModelSettings : ViewModelAdjunctDefinition
    {
        internal ViewModelSettings() { }

        public string type { get; set; }
    }
}
