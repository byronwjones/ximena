using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public sealed class NamespaceSettingDefaults : NamespaceSettingsBase
    {
        internal NamespaceSettingDefaults() : base() { }

        public string vmNamespacePrefix { get; set; }
        public string vmNamespaceSuffix { get; set; }
    }
}
