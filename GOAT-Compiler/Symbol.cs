using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.Linq;

namespace GOAT_Compiler
{
    public enum Types
    {
        Integer,
        FloatingPoint,
        Vector,
        Boolean,
        Void
    };

    internal class Symbol
    {
        private List<Types> types = new List<Types>();
        public string name { get; private set; }
        public Types type { get => types[0]; private set => types.Add(value); }

        public Symbol(string Name, Types Type, params Types[] paramTypes)
        {
            name = Name;
            type = Type;
            types.AddRange(paramTypes);
        }

        public List<Types> GetParamTypes()
        {
            return types.GetRange(1, types.Count-1);
        }

    }
}
