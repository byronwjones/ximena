using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ximena.Binary
{
    public sealed class AssemblyParsingException : Exception
    {
        public AssemblyParsingException(string message) : base(message) { }
        public AssemblyParsingException(string message, Exception innerException) :
            base(message, innerException)
        { }
    }
}
