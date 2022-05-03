using System;

namespace GOAT_Compiler
{
    internal class CallBuildInWalkException : Exception
    {
        internal CallBuildInWalkException(DijkstraNode node)
        {
            Console.WriteLine("e");
        }

        internal CallBuildInWalkException(Extrude e) 
        {
            Console.WriteLine("Tried to call a Build in a Walk");
        }
        
    }
}
