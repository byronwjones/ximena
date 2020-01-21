using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public abstract class ViewModelSettingsBase
    {
        protected ViewModelSettingsBase() { }

        public string access { get; set; }
        public bool? partialClass
        {
            get
            {
                // this must return true if stubForCustomCode is true
                if(stubForCustomCode.HasValue && stubForCustomCode.Value)
                {
                    return true;
                }

                return _partialClass;
            }
            set { _partialClass = value; }
        }
        public bool? inheritUsings { get; set; }
        public HashSet<string> usings { get; set; }
        public bool? stubForCustomCode { get; set; }
        public bool? mapAllProperties { get; set; }
        public bool? makeCollectionsObservable { get; set; }

        private bool? _partialClass = true;
    }
}
