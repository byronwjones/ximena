using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public class ViewModelSettingDefaults : ViewModelSettingsBase
    {
        internal ViewModelSettingDefaults() { }

        public string typePrefix { get; set; }
        public string typeSuffix { get; set; }
    }
}
