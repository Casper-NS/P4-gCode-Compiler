using System;

namespace GOAT_Compiler
{
    internal class CallBuildInWalkException : Exception
    {
        internal CallBuildInWalkException(DijkstraNode node)
        {
            string message = "";

            while (node.GetWhereItWasCalled() is not null)
            {
               message = $"{node.Name} -> {message}";
            }
            Console.WriteLine(message);
        }

        internal CallBuildInWalkException(Extrude e) 
        {
            Console.WriteLine("Tried to call a Build in a Walk");
        }
        
    }
}
