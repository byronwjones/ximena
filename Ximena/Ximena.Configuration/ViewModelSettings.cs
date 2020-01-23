using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public sealed class ViewModelSettings : ViewModelSettingsBase
    {
        public ViewModelSettings() { }

        public string type { get; set; }
        public string emitTo { get; set; }
        public Dictionary<string, PropertyDefinition> properties { get; set; }
         = new Dictionary<string, PropertyDefinition>();
        public Dictionary<string, CollectionDefinition> collections { get; set; }
         = new Dictionary<string, CollectionDefinition>();

        // internal use
        public bool HasPublicEntity() => _hasPublicEntity;
        public void HasPublicEntity(bool value) => _hasPublicEntity = value;
        public string EntityNamespace() => _entityNamespace;
        public void EntityNamespace(string es) => _entityNamespace = es;
        public string EntityType() => _entityType;
        public void EntityType(string et) => _entityType = et;
        public string ViewModelNamespace() => _viewModelNamespace;
        public void ViewModelNamespace(string vmn) => _viewModelNamespace = vmn;

        private bool _hasPublicEntity;
        private string _entityNamespace;
        private string _viewModelNamespace;
        private string _entityType;
    }
}
