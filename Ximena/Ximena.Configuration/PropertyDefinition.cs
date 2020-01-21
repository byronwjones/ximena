﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public class PropertyDefinition
    {
        internal PropertyDefinition() { }

        public string access { get; set; } = "public";
        public string type { get; set; }
        public string summary { get; set; }
        public bool readOnly { get; set; }
    }
}