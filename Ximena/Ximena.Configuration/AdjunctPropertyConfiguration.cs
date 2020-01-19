using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public sealed class AdjunctPropertyConfiguration
    {
        internal AdjunctPropertyConfiguration() { }

        public string access { get; set; } = "public";
        public string type { get; set; }
    }
}
