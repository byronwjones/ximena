using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public sealed class NamespaceSettings : NamespaceSettingsBase
    {
        internal NamespaceSettings() : base() { }

        public string mapToNamespace { get; set; }
        public ViewModelNamespaceSettingDefaults vmDefaults { get; set; } = 
            new ViewModelNamespaceSettingDefaults();
        public Dictionary<string, ViewModelSettings> viewModels { get; set; } = 
            new Dictionary<string, ViewModelSettings>();
        public List<ViewModelSettings> adjunctViewModels { get; set; } =
            new List<ViewModelSettings>();
    }
}
