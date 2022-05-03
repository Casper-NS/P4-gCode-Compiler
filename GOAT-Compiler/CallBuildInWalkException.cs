using System;

namespace GOAT_Compiler
{
    internal class CallBuildInWalkException : Exception
    {
        internal CallBuildInWalkException()
        {
            Console.WriteLine("Cannot call Walk in Build");
        }
    }

}
