using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using GOATCode.node;

namespace GOAT_Compiler
{
    internal class CodeGenerator : SymbolTableVisitor
    {
        FileStream gcodeFile;
        private ISymbolTable _symbolTable;
        private Dictionary<Node, Types> typeMap;
        private RuntimeTable RT;

        public CodeGenerator(ISymbolTable symbolTable, Dictionary<Node, Types> typesDictionary, string outputName) : base(symbolTable)
        {
            _symbolTable = symbolTable;
            typeMap = typesDictionary;
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

        public void CreateGCodeLine(string gCommand, Vector vector)
        {
            string line = gCommand + " X" + vector.X + " Y" + vector.Y + " Z" + vector.Z;
        }

        private Vector ToVector(string input)
        {
            float x, y, z;
            string[] variables = input.Split(" ");

            x = float.Parse(variables[0], CultureInfo.InvariantCulture);
            y = float.Parse(variables[1], CultureInfo.InvariantCulture);
            z = float.Parse(variables[2], CultureInfo.InvariantCulture);

            return new Vector(x, y, z);
        }

        public override void OutAVarDecl(AVarDecl node)
        {
            //Console.WriteLine(node.GetExp());
            Symbol symbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (node.GetExp() != null)
            {
                string nodeExpr = node.GetExp().ToString();
                Console.WriteLine(nodeExpr);
                switch (typeMap[node])
                {
                    case Types.Integer:
                        RT.Put(symbol, int.Parse(nodeExpr!));
                        break;
                    case Types.FloatingPoint:
                        RT.Put(symbol, float.Parse(nodeExpr!, CultureInfo.InvariantCulture));
                        break;
                    case Types.Boolean:
                        RT.Put(symbol, bool.Parse(nodeExpr!));
                        break;
                    case Types.Vector:
                        RT.Put(symbol, ToVector(nodeExpr!));
                        break;
                    default:
                        throw new Exception("Everything is on fire!!!!!!!!!! \n Typechecker is broken");
                        break;
                }
            }
        }

        public void CloseFile()
        {
            gcodeFile.Close();
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

        public void Put(Symbol sym, int value)
        {
            IntMap.Add(sym, value);
        }

        public void Put(Symbol sym, bool value)
        {
            BoolMap.Add(sym, value);
        }

        public void Put(Symbol sym, float value)
        {  
            FloatMap.Add(sym, value);
        }

        public void Put(Symbol sym, Vector vector)
        {
            VecMap.Add(sym, vector);
        }

        public dynamic Get(Symbol sym)
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
    }
}