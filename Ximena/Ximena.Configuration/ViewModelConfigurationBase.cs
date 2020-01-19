using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public abstract class ViewModelConfigurationBase
    {
        protected ViewModelConfigurationBase() { }

        public string access { get; set; }
        public bool partialClass { get; set; }
        public bool inheritUsings { get; set; } = true;
        public List<string> usings { get; set; }
        public bool stubForCustomCode { get; set; }
        public bool matchAllProperties { get; set; }
    }
}
