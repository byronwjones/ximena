using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public class AdjunctPropertyDefinition
    {
        internal AdjunctPropertyDefinition() { }

        public string access { get; set; } = "public";
        public string type { get; set; }
    }
}
