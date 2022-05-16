using System;
using System.Collections.Generic;

namespace GOAT_Compiler
{
    internal class CallBuildInWalkException : Exception
    {
        internal CallBuildInWalkException(BFSNode node) : base(stringMessage(node))
        {
        }

        /// <summary>
        /// Takes the current node and its parent/where it was called from, and puts them in a string, to be printed.
        /// </summary>
        /// <param name="node">The node wich called the exception</param>
        /// <returns>Returns the string to be printed.</returns>
        private static string stringMessage(BFSNode node)
        {
            HashSet<BFSNode> nodes = new();
            string message = $"{node.Name}";

            //Assembles a string with all the names of the function call stack
            while (node.CallerNode is not null)
            {
                if (!nodes.Contains(node.CallerNode))
                {
                    node = node.CallerNode;
                    message = $"{node.Name}({node.TheExtrudeTypeFromCallStack}) -> {message}";
                    nodes.Add(node);
                }
                else
                {
                    break;
                }
            }
            return message;
        }
    }
}