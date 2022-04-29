using GOATCode.node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAT_Compiler
{
    public class TypeMismatchException : CompilerException
    {
        public TypeMismatchException(Node node) : base(node) { }
        public TypeMismatchException(Node node, string message) : base(node, message) { }
        public TypeMismatchException(Node node, string message, Exception inner) : base(node, message, inner) { }
    }
}
