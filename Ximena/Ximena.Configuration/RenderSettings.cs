using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public sealed class RenderSettings
    {
        internal RenderSettings() { }

        public string sourceAssembly { get; set; }
        public string destRoot { get; set; }
        public GlobalDefaults globalDefaults { get; set; } = new GlobalDefaults();
        public HashSet<string> includeNamespaces { get; set; } = new HashSet<string>();
        public Dictionary<string, NamespaceSettings> namespaces { get; set; }
         = new Dictionary<string, NamespaceSettings>();
    }
}
