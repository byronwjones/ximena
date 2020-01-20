using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public class ViewModelAdjunctDefinition : ViewModelSettingsBase
    {
        internal ViewModelAdjunctDefinition() { }

        public string dest { get; set; }
        public Dictionary<string, AdjunctPropertyDefinition> properties { get; set; }
         = new Dictionary<string, AdjunctPropertyDefinition>();
        public Dictionary<string, AdjunctCollectionDefinition> collections { get; set; }
         = new Dictionary<string, AdjunctCollectionDefinition>();
    }
}
