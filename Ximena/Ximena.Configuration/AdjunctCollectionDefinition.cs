﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public sealed class AdjunctCollectionDefinition : AdjunctPropertyDefinition
    {
        internal AdjunctCollectionDefinition() : base() { }

        public string typeParam
        {
            get { return base.type; }
            set { base.type = value; }
        }

        // mask property
        private new string type { get; set; }
    }
}
