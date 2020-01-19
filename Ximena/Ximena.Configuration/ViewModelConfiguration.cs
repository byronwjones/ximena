using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public sealed class ViewModelConfiguration : ViewModelAdjunctConfiguration
    {
        internal ViewModelConfiguration() { }

        public string type { get; set; }
    }
}
