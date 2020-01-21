using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public sealed class ViewModelSettings : ViewModelSettingsBase
    {
        internal ViewModelSettings() { }

        public string type { get; set; }
        public string emitTo { get; set; }
        public Dictionary<string, PropertyDefinition> properties { get; set; }
         = new Dictionary<string, PropertyDefinition>();
        public Dictionary<string, CollectionDefinition> collections { get; set; }
         = new Dictionary<string, CollectionDefinition>();

        // user can set these, but there's no point - system sets these values
        public string EntityNamespace { get; set; }
        public string ViewModelNamespace { get; set; }
    }
}
