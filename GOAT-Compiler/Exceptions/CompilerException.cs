using GOATCode.node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAT_Compiler
{
    /// <summary>
    /// Base exception class for all compiler exceptions
    /// </summary>
    public abstract class CompilerException : Exception
    {
        private static string NodePrinter(Node node)
        {
            return "Compiler exception at node " + node.ToString() + "\n";
        }
        private Node _node;
        public CompilerException(Node node) : this(node, ""){ }
        public CompilerException(Node node, string message) : this(node, message, null) { }
        public CompilerException(Node node, string message, Exception inner) : base(NodePrinter(node) + message, inner) { }
    }
}
