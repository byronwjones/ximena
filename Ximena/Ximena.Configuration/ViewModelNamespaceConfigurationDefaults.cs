using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public sealed class ViewModelNamespaceConfigurationDefaults : ViewModelConfigurationDefaults
    {
        internal ViewModelNamespaceConfigurationDefaults() { }

        public string dest { get; set; }
    }
}
