using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public sealed class CollectionDefinition : MemberDefinition
    {
        public string collectionType { get; set; } = "ObservableCollectionPlus";
        public string typeParam
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}
