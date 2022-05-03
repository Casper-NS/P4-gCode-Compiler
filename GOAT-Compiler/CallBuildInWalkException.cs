using System;
using System.Collections.Generic;

namespace GOAT_Compiler
{
    internal class CallBuildInWalkException : Exception
    {
        internal CallBuildInWalkException(DijkstraNode node) : base(stringMessage(node))
        {
            
        }

        private static string stringMessage(DijkstraNode node)
        {
            HashSet<DijkstraNode> nodes = new();
            string message = $"{node.Name}";

            while (node.GetWhereItWasCalled() is not null)
            {
                if (!nodes.Contains(node.GetWhereItWasCalled()))
                {
                    node = node.GetWhereItWasCalled();
                    message = $"{node.Name}({node.GetCallStackType()}) -> {message}";
                    nodes.Add(node);
                }
                else break;
            }
            return message;
        }

        internal CallBuildInWalkException(Extrude e) : base("Tried to call a Build in a Walk") 
        {
        }
        
    }
}
