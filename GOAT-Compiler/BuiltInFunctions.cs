using GOATCode.node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAT_Compiler
{
    internal static class BuiltInFunctions
    {
        internal static IReadOnlyDictionary<string, Symbol> FunctionsList => _functionsList;
        private static Dictionary<string, Symbol> _functionsList = new Dictionary<string, Symbol>() 
        {
            { "RelMove", new Symbol("RelMove", Types.Void, Types.Vector) },
            { "AbsMove", new Symbol("AbsMove", Types.Void, Types.Vector) },
        };
        
    }
}
