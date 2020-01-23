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
        public string PropertyType() => _type;

        public string Name() => _name;
        public void Name(string name) => _name = name;

        public string[] Dependencies() => _dependencies;
        public void Dependencies(string[] deps) => _dependencies = deps;

        public bool EmitAsViewModel() => _emitAsViewModel;
        public void EmitAsViewModel(bool value) => _emitAsViewModel = value;


        private string _name;
        private string[] _dependencies;
        private bool _emitAsViewModel;
    }
}
