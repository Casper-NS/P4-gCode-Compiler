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
        private static NodePositionVisitor nodePositionVisitor = null;
        private static Start currentAst = null;
        private static string NodePrinter(Node node)
        {
            if (nodePositionVisitor == null || !nodePositionVisitor.HasNode(node))
            {
                RedoLineNumbers(node);
            }
            return "Compiler exception at " + nodePositionVisitor.GetPosition(node);
        }
        public CompilerException(Node node) : this(node, ""){ }
        public CompilerException(Node node, string message) : this(node, message, null) { }
        public CompilerException(Node node, string message, Exception inner) : base(GenerateFullMessage(node, message), inner) { }


        private static string GenerateFullMessage(Node node, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return $"{NodePrinter(node)}";
            }
            else
            {
                return $"{NodePrinter(node)}: \"{message}\"";
            }
        }
        private static void RedoLineNumbers(Node nodeInAst)
        {

            currentAst = FindRootNode(nodeInAst);
            nodePositionVisitor = new NodePositionVisitor();
            currentAst.Apply(nodePositionVisitor);
        }
        private static Start FindRootNode(Node n)
        {
            Node root = n;
            while (root.Parent() != null)
            {
                root = root.Parent();
            }
            return (Start)root;
        }
    }
}
