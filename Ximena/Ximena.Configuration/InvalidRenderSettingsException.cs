using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Configuration
{
    public sealed class InvalidRenderSettingsException : Exception
    {
        public InvalidRenderSettingsException(string message) : base(message) { }
        public InvalidRenderSettingsException(string message, Exception innerException) :
            base(message, innerException)
        { }
    }
}
