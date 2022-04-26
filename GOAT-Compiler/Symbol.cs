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
        public string name { get; private set; }
        public Types type { get; private set; }

        public Symbol(string Name, Types Type)
        {
            name = Name;
            type = Type;
        }

    }
}
