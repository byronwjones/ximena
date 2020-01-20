using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public sealed class GlobalDefaults
    {
        internal GlobalDefaults() { }

        public ViewModelSettingDefaults viewModel { get; set; } =
            new ViewModelSettingDefaults();
        public NamespaceSettingDefaults nameSpace { get; set; } =
            new NamespaceSettingDefaults();
    }
}
