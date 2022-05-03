using System.Collections.Generic;
using System.IO;
using System.Linq;
using GOATCode.node;

namespace GOAT_Compiler
{
    internal class CodeGenerator : SymbolTableVisitor
    {
        FileStream gcodeFile;
        private ISymbolTable _symbolTable;
        private RuntimeTable RT;

        public CodeGenerator(ISymbolTable symbolTable, string outputName) : base(symbolTable)
        {
            _symbolTable = symbolTable;
            RT = new RuntimeTable(_symbolTable);
            if (outputName.Length == 0)
            {
                gcodeFile = File.Create("GOAT.gcode");
            }
            else
            {
                gcodeFile = File.Create(outputName + ".gcode");
            }
        }
    }

    internal class RuntimeTable
    {
        private Dictionary<Symbol, int> IntMap = new Dictionary<Symbol, int>();
        private Dictionary<Symbol, float> FloatMap = new Dictionary<Symbol, float>();
        private Dictionary<Symbol, bool> BoolMap = new Dictionary<Symbol, bool>();
        private Dictionary<Symbol, Vector> VecMap = new Dictionary<Symbol, Vector>();

        private ISymbolTable _symbolTable;

        public RuntimeTable(ISymbolTable symbolTable)
        {
            _symbolTable = symbolTable;
        }

        void Put(Symbol sym, int value)
        {
            IntMap.Add(sym, value);
        }

        void Put(Symbol sym, bool value)
        {
            BoolMap.Add(sym, value);
        }

        void Put(Symbol sym, float value)
        {  
            FloatMap.Add(sym, value);
        }

        void Put(Symbol sym, float x, float y, float z)
        {
            VecMap.Add(sym, new Vector(x, y, z));
        }

        dynamic Get(Symbol sym)
        {
            switch (sym.type)
            {
                case Types.Integer:
                    return IntMap[sym];
                case Types.FloatingPoint:
                    return FloatMap[sym];
                case Types.Boolean:
                    return BoolMap[sym];
                case Types.Vector:
                    return VecMap[sym];
                default:
                    return null;
            }
        }

        private class Vector
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }

            public Vector(float x, float y, float z)
            {
                X = x;
                Y = y;
                Z = z;
            }

        }
    }
}