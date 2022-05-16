using GOATCode.node;
using System;

namespace GOAT_Compiler
{
    /// <summary>
    /// Exception inheriented from CompilerException to handle errors in the compiler
    /// </summary>
    public class TypeMismatchException : CompilerException
    {
        public TypeMismatchException(Node node) : base(node) { }
        public TypeMismatchException(Node node, string message) : base(node, message) { }
        public TypeMismatchException(Node node, string message, Exception inner) : base(node, message, inner) { }
    }
}