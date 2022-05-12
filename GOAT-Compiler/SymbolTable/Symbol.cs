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
        private readonly List<Types> Types = new List<Types>();
        public string Name { get; private set; }
        public Types Type { get => Types[0]; private set => Types.Add(value); }

        public Symbol(string name, Types type, params Types[] paramTypes)
        {
            Name = name;
            Type = type;
            Types.AddRange(paramTypes);
        }

        public List<Types> ParamTypes => Types.GetRange(1, Types.Count - 1);

    }
}
