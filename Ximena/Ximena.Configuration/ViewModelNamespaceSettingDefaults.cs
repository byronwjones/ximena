using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public sealed class ViewModelNamespaceSettingDefaults : ViewModelSettingDefaults
    {
        internal ViewModelNamespaceSettingDefaults() { }

        public string emitTo { get; set; }
    }
}
