using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public abstract class MemberDefinition
    {
        public string access { get; set; } = "public";
        public string summary { get; set; }
        public bool readOnly { get; set; }

        protected string _type;

        // for internal use
        public string GetPropertyType()
        {
            return _type;
        }
        public string GetName() => _name;
        public void SetName(string name) => _name = name;
        public string[] GetDependencies() => _dependencies;
        public void SetDependencies(string[] deps) => _dependencies = deps;

        private string _name;
        private string[] _dependencies;
    }
}
