using GOATCode.node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAT_Compiler
{
    static class BuiltInFunctions
    {
        public static IReadOnlyDictionary<string, List<Types>> FunctionsList => _functionsList;
        private static Dictionary<string, List<Types>> _functionsList = new Dictionary<string, List<Types>>() 
        {
            { "RelMove", new List<Types>() { Types.Void, Types.Vector } },
            { "AbsMove", new List<Types>() { Types.Void, Types.Vector } },
        };
        
    }
}
