using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAT_Compiler
{
    public class TypeMismatchException : Exception
    {
        public TypeMismatchException() { }
        public TypeMismatchException(string message) : base(message) { }
        public TypeMismatchException(string message, Exception inner) : base(message, inner) { }
        protected TypeMismatchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
