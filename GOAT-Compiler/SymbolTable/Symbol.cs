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
        public Symbol(string name, Types type, params Types[] paramTypes)
        {
            Name = name;
            Type = type;
            ParamTypes = paramTypes.ToList();
        }

        /// <summary>
        /// The name of the symbol (the id).
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// If the symbol is a variable, this is the type of the variable. 
        /// If the symbol is a function then this is the returntype of the function.
        /// </summary>
        public Types Type { get; }

        /// <summary>
        /// If the symbol is a function, then this returns the types of the formal parameters.
        /// </summary>
        public List<Types> ParamTypes { get; } = new();
    }
}
