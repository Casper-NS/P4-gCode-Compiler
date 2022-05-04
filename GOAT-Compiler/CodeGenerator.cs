using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using GOATCode.node;

namespace GOAT_Compiler
{
    internal class CodeGenerator : SymbolTableVisitor
    {
        FileStream gcodeFile;
        private Dictionary<Node, Types> typeMap;
        private RuntimeTable<Node> nodeMap;
        private RuntimeTable<Symbol> RT;

        public CodeGenerator(ISymbolTable symbolTable, Dictionary<Node, Types> typesDictionary, string outputName) : base(symbolTable)
        {
            _symbolTable = symbolTable;
            typeMap = typesDictionary;
            RT = new RuntimeTable<Symbol>();
            nodeMap = new RuntimeTable<Node>();
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

        dynamic GetValue(Symbol symbol)
        {
            return RT.Get(symbol, symbol.type);
        }

        dynamic GetValue(Node node)
        {
            return nodeMap.Get(node, typeMap[node]);
        }

        public override void OutAVarDecl(AVarDecl node)
        {
            Symbol symbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (node.GetExp() != null)
            {
                var nodeExpr = node.GetExp();
                switch (typeMap[node])
                {
                    case Types.Integer:
                        RT.Put(symbol, nodeMap.Get(nodeExpr, typeMap[nodeExpr]));
                        break;
                    case Types.FloatingPoint:
                        RT.Put(symbol, nodeMap.Get(nodeExpr, typeMap[nodeExpr]));
                        break;
                    case Types.Boolean:
                        RT.Put(symbol, nodeMap.Get(nodeExpr, typeMap[nodeExpr]));
                        break;
                    case Types.Vector:
                        RT.Put(symbol, nodeMap.Get(nodeExpr, typeMap[nodeExpr]));
                        break;
                    default:
                        throw new Exception("Everything is on fire!!!!!!!!!! \n Typechecker is broken");
                }
            }
        }

        public override void OutAIdExp(AIdExp node)
        {
            Symbol VarSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            nodeMap.Put(node, RT.Get(VarSymbol, VarSymbol.type));
        }

        public override void OutABoolvalExp(ABoolvalExp node)
        {
            nodeMap.Put(node, bool.Parse(node.GetBoolValue().Text));
        }

        public override void OutAVectorExp(AVectorExp node)
        {
            float x = GetValue(node.GetX());
            float y = GetValue(node.GetY());
            float z = GetValue(node.GetZ());
            nodeMap.Put(node, new Vector(x, y, z));
        }

        public override void OutANumberExp(ANumberExp node)
        {
            switch (typeMap[node])
            {
                case Types.Integer:
                    nodeMap.Put(node, int.Parse(node.GetNumber().Text));
                    break;
                case Types.FloatingPoint:
                    nodeMap.Put(node, float.Parse(node.GetNumber().Text, CultureInfo.InvariantCulture));
                    break;
                default:
                    throw new Exception("im litteraly crying right now");
            }
        }

        public override void OutAAssignStmt(AAssignStmt node)
        {
            Symbol idSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);

            if(idSymbol != null)
            {
                RT.Put(idSymbol, GetValue(node.GetExp()));
            }
            else
            {
                throw new Exception("AssignStmt did not work");
            }
        }

        public override void OutAAssignPlusStmt(AAssignPlusStmt node)
        {
            Symbol idSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (idSymbol != null)
            {
                if (idSymbol.type == Types.Vector)
                {
                    Vector Vec1 = ((Vector) GetValue(node.GetExp()));
                    Vector Vec2 = ((Vector) GetValue(idSymbol));

                    Vector resultVec = new Vector(Vec1.X + Vec2.X, Vec1.Y + Vec2.Y, Vec1.Z + Vec2.Z);
                    RT.Put(idSymbol, resultVec);
                }
                else
                {
                    RT.Put(idSymbol, GetValue(node.GetExp()) + GetValue(idSymbol));
                }
            }
            else
            {
                throw new Exception("AssignStmt did not work");
            }
        }
        public override void OutAAssignMinusStmt(AAssignMinusStmt node)
        {
            Symbol idSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (idSymbol != null)
            {
                if (idSymbol.type == Types.Vector)
                {
                    Vector Vec1 = ((Vector)GetValue(node.GetExp()));
                    Vector Vec2 = ((Vector)GetValue(idSymbol));

                    Vector resultVec = new Vector(Vec1.X - Vec2.X, Vec1.Y - Vec2.Y, Vec1.Z - Vec2.Z);
                    RT.Put(idSymbol, resultVec);
                }
                else
                {
                    RT.Put(idSymbol, GetValue(node.GetExp()) - GetValue(idSymbol));
                }
            }
            else
            {
                throw new Exception("AssignStmt did not work");
            }
        }
        public override void OutAAssignMultStmt(AAssignMultStmt node)
        {
            Symbol idSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (idSymbol != null)
            {
                if (idSymbol.type == Types.Vector)
                {
                    Vector Vec = ((Vector)GetValue(idSymbol));
                    dynamic scale = GetValue(node.GetExp());

                    Vector resultVec = new Vector(Vec.X * scale, Vec.Y * scale, Vec.Z * scale);
                    RT.Put(idSymbol, resultVec);
                }
                else
                {
                    RT.Put(idSymbol, GetValue(node.GetExp()) * GetValue(idSymbol));
                }
            }
            else
            {
                throw new Exception("MultStmt did not work");
            }
        }
        //Check the modulo statements
        public override void OutAAssignModStmt(AAssignModStmt node)
        {
            Symbol idSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (idSymbol != null)
            {
                RT.Put(idSymbol, GetValue(node.GetExp()) % GetValue(idSymbol));
            }
            else
            {
                throw new Exception("ModStmt did not work");
            }
        }
        public override void OutAAssignDivisionStmt(AAssignDivisionStmt node)
        {
            Symbol idSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (idSymbol != null)
            {
                if (idSymbol.type == Types.Vector)
                {
                    Vector Vec1 = ((Vector)GetValue(idSymbol));
                    dynamic value = GetValue(node.GetExp());

                    Vector resultVec = new Vector(Vec1.X / value, Vec1.Y / value, Vec1.Z / value);
                    RT.Put(idSymbol, resultVec);
                }
                else
                {
                    RT.Put(idSymbol, GetValue(node.GetExp()) / GetValue(idSymbol));
                }
            }
            else
            {
                throw new Exception("DivStmt did not work");
            }
        }
        public override void OutAPlusExp(APlusExp node)
        {
            Node left = node.GetL();
            Node right = node.GetR();
            if (left != null && right != null)
            {
                if (typeMap[left] == Types.Vector && typeMap[right] == Types.Vector)
                {
                    Vector Vec1 = ((Vector)GetValue(left));
                    Vector Vec2 = ((Vector)GetValue(right));

                    Vector resultVec = new Vector(Vec1.X + Vec2.X, Vec1.Y + Vec2.Y, Vec1.Z + Vec2.Z);
                    nodeMap.Put(node, resultVec);
                }
                else
                {
                    nodeMap.Put(node, GetValue(left) + GetValue(right));
                }
            }
            else
            {
                throw new Exception("PlusExp did not work");
            }
        }
        public override void OutAMinusExp(AMinusExp node)
        {
            Node left = node.GetL();
            Node right = node.GetR();
            if (left != null && right != null)
            {
                if (typeMap[left] == Types.Vector && typeMap[right] == Types.Vector)
                {
                    Vector Vec1 = ((Vector)GetValue(left));
                    Vector Vec2 = ((Vector)GetValue(right));

                    Vector resultVec = new Vector(Vec1.X - Vec2.X, Vec1.Y - Vec2.Y, Vec1.Z - Vec2.Z);
                    nodeMap.Put(node, resultVec);
                }
                else
                {
                    nodeMap.Put(node, GetValue(left) - GetValue(right));
                }
            }
            else
            {
                throw new Exception("MinusExp did not work");
            }
        }
        public override void OutADivdExp(ADivdExp node)
        {
            Node left = node.GetL();
            Node right = node.GetR();
            if (left != null && right != null)
            {
                if (typeMap[left] == Types.Vector && (typeMap[right] == Types.FloatingPoint || typeMap[right] == Types.Integer))
                {
                    Vector Vec1 = ((Vector)GetValue(left));
                    dynamic value = GetValue(right);

                    Vector resultVec = new Vector(Vec1.X / value, Vec1.Y / value, Vec1.Z / value);
                    nodeMap.Put(node, resultVec);
                }
                else
                {
                    nodeMap.Put(node, GetValue(left) / GetValue(right));
                }
            }
            else
            {
                throw new Exception("MinusExp did not work");
            }
        }

        //C# should handel the modulo expression
        public override void OutAModuloExp(AModuloExp node)
        {
            Node left = node.GetL();
            Node right = node.GetR();
            if (left != null && right != null)
            {
                nodeMap.Put(node, GetValue(left) % GetValue(right));
            }
            else
            {
                throw new Exception("ModuloExp did not work");
            }
        }
        public override void OutAMultExp(AMultExp node)
        {
            Node left = node.GetL();
            Node right = node.GetR();
            if (left != null && right != null)
            {
                if (typeMap[left] == Types.Vector && (typeMap[right] == Types.FloatingPoint || typeMap[right] == Types.Integer))
                {
                    Vector Vec1 = ((Vector)GetValue(left));
                    dynamic value = GetValue(right);

                    Vector resultVec = new Vector(Vec1.X * value, Vec1.Y * value, Vec1.Z * value);
                    nodeMap.Put(node, resultVec);
                }
                else
                {
                    nodeMap.Put(node, GetValue(left) * GetValue(right));
                }
            }
            else
            {
                throw new Exception("MultExp did not work");
            }
        }


        public override void OutAAndExp(AAndExp node)
        {
            bool l = nodeMap.Get(node.GetL(), typeMap[node.GetL()]);
            bool r = nodeMap.Get(node.GetR(), typeMap[node.GetR()]);
            if (node.GetL() != null && node.GetR() != null)
            {
                nodeMap.Put(node, l && r);
            }
            else
            {
                throw new Exception("AndExp didnt not work");
            }
        }


        public override void OutAEqExp(AEqExp node)
        {
            string l = node.GetL().ToString();
            string r = node.GetR().ToString();
            if (node.GetL() != null && node.GetR() != null)
                if (l == r)
                {
                    //nodeMap.Put(node, "true");
                }
                else
                {
                    //nodeMap.Put(node, "false");
                }
            else
            {
                throw new Exception("EqExp did not work");
            }
        }

        public override void OutAGtExp(AGtExp node)
        {
            Symbol symbol = _symbolTable.GetVariableSymbol(node.ToString());
            float l = float.Parse(node.GetL().ToString(), CultureInfo.InvariantCulture);
            float r = float.Parse(node.GetR().ToString(), CultureInfo.InvariantCulture);
            if (node != null)
                if (l>r)
                {
                    RT.Put(symbol, true);
                }
                else
                {
                    RT.Put(symbol, false);
                }
            else
            {
                throw new Exception("GtExp didnt not work");
            }
        }
        public override void OutALtExp(ALtExp node)
        {
            Symbol symbol = _symbolTable.GetVariableSymbol(node.ToString());
            float l = float.Parse(node.GetL().ToString(), CultureInfo.InvariantCulture);
            float r = float.Parse(node.GetR().ToString(), CultureInfo.InvariantCulture);
            if (node != null)
                if (l < r)
                {
                    RT.Put(symbol, true);
                }
                else
                {
                    RT.Put(symbol, false);
                }
            else
            {
                throw new Exception("LtExp didnt not work");
            }
        }


        public void CloseFile()
        {
            gcodeFile.Close();
        }
    }

    internal class RuntimeTable<TKey>
    {
        private Dictionary<TKey, int> IntMap = new Dictionary<TKey, int>();
        private Dictionary<TKey, float> FloatMap = new Dictionary<TKey, float>();
        private Dictionary<TKey, bool> BoolMap = new Dictionary<TKey, bool>();
        private Dictionary<TKey, Vector> VecMap = new Dictionary<TKey, Vector>();

        public void Put(TKey key, int value)
        {
            IntMap.Add(key, value);
        }

        public void Put(TKey key, bool value)
        {
            BoolMap.Add(key, value);
        }

        public void Put(TKey key, float value)
        {  
            FloatMap.Add(key, value);
        }

        public void Put(TKey key, Vector vector)
        {
            VecMap.Add(key, vector);
        }

        public dynamic Get(TKey key, Types type)
        {
            switch (type)
            {
                case Types.Integer:
                    return IntMap[key];
                case Types.FloatingPoint:
                    return FloatMap[key];
                case Types.Boolean:
                    return BoolMap[key];
                case Types.Vector:
                    return VecMap[key];
                default:
                    return null;
            }
        }
    }
}