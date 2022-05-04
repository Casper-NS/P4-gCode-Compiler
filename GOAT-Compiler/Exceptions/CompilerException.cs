using GOATCode.node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAT_Compiler
{
    /// <summary>
    /// Base exception class for all compiler exceptions.
    /// Prints the approximate line and character position
    /// of the node, along with the message given from its
    /// child exception.
    /// </summary>
    public abstract class CompilerException : Exception
    {
        public CompilerException(Node node) : this(node, "") { }
        public CompilerException(Node node, string message) : this(node, message, null) { }
        public CompilerException(Node node, string message, Exception inner) : base(GenerateFullMessage(node, message), inner) { }

        // This lock is necessary when running tests, as the test runner seem to be multithreaded.
        private static readonly object nodePositionVisitorLock = new object();

        // The current AST ands its node positions are saved, to avoid revisiting when throwing multiple exceptions.
        private static NodePositionVisitor nodePositionVisitor = null;
        private static Start currentAst = null;
        private static string NodePrinter(Node node)
        {
            NodePosition pos;
            // This must be locked, as currentAst and nodePositionVisitor are static to avoid revisiting
            lock (nodePositionVisitorLock)
            {
                // if we dont have a nodepositionvisitor, or this node doesnt belong to it, create a new one
                if (nodePositionVisitor == null || !nodePositionVisitor.HasNode(node))
                {
                    RedoLineNumbers(node);
                }
                pos = nodePositionVisitor.GetPosition(node);
            }
            return "Compiler exception at " + pos;
        }

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
