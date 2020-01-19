using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public class ViewModelAdjunctConfiguration : ViewModelConfigurationBase
    {
        internal ViewModelAdjunctConfiguration() { }

        public string dest { get; set; }
        public Dictionary<string, AdjunctPropertyConfiguration> properties { get; set; }
         = new Dictionary<string, AdjunctPropertyConfiguration>();
        public Dictionary<string, AdjunctCollectionConfiguration> collections { get; set; }
         = new Dictionary<string, AdjunctCollectionConfiguration>();
    }
}
