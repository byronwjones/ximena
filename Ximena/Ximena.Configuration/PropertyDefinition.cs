using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public sealed class PropertyDefinition : MemberDefinition
    {
        public string type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}
