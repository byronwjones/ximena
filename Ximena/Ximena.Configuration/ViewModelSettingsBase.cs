﻿using System;
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
        public bool? partialClass { get; set; }
        public bool inheritUsings { get; set; } = true;
        public HashSet<string> usings { get; set; }
        public bool? stubForCustomCode { get; set; }
        public bool? matchAllProperties { get; set; }
        public bool? makeCollectionsObservable { get; set; }
    }
}
