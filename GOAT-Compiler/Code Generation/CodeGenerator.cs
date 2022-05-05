using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using GOATCode.node;

namespace GOAT_Compiler
{
    public enum GCommands
    {
        G00,  // Fast move
        G01,  // Linear interpolation
        G02,  // Circular Interpolation CW
        G03,  // Circular interpolation CCW
        G04,  // Dwell
        G20,  // Set English units
        G21,  // Set metric units
        G28,  // Machine zero return (point 1)
        G80,  // Fixed cycle cancel
        G90,  // Set the interpreter to absolute positions
        G91,  // Set the interpreter to relative positions
        G92,  // Set the current position of one or more axes.
        M00,  // Stop and wait for user
        M01,  // Is a deprecated alias for M0
        M80,  // Turn on the high-voltage power supply.
        M81,  // Turn off the high-voltage power supply.
        M104, // Set a new target hot end temperature and continue without waiting.
        M140, // Set a new target temperature for the heated bed and continue without waiting.
        F     // The speed of the 3D-printer
    }
    internal class CodeGenerator : SymbolTableVisitor
    {
        private Dictionary<Node, Types> typeMap;
        private RuntimeTable<Node> nodeMap;
        internal RuntimeTable<Symbol> RT;
        private Stream _outputFileStream;

        public CodeGenerator(ISymbolTable symbolTable, Dictionary<Node, Types> typesDictionary, Stream outputStream) : base(symbolTable)
        {
            _symbolTable = symbolTable;
            typeMap = typesDictionary;
            RT = new RuntimeTable<Symbol>();
            nodeMap = new RuntimeTable<Node>();
            _outputFileStream = outputStream;
        }
        public void CreateGCodeLine(GCommands gCommand, Vector vector)
        {
            string line = gCommand.ToString() + " X" + vector.X + " Y" + vector.Y + " Z" + vector.Z;
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
                        RT.Put(symbol, nodeMap.Get(nodeExpr, typeMap[node]));
                        break;
                    case Types.FloatingPoint:
                        RT.Put(symbol, (float)nodeMap.Get(nodeExpr, typeMap[nodeExpr]));
                        break;
                    case Types.Boolean:
                        RT.Put(symbol, nodeMap.Get(nodeExpr, typeMap[node]));
                        break;
                    case Types.Vector:
                        RT.Put(symbol, nodeMap.Get(nodeExpr, typeMap[node]));
                        break;
                    default:
                        throw new Exception("This should never happen.");
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
                    throw new Exception("Im litteraly crying right now");
            }
        }
        public override void OutAAssignStmt(AAssignStmt node)
        {
            Symbol idSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);

            if (idSymbol != null)
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
                    Vector Vec1 = ((Vector)GetValue(node.GetExp()));
                    Vector Vec2 = ((Vector)GetValue(idSymbol));

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
                throw new Exception("AssignPlusStmt did not work");
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
                throw new Exception("AssignMinusStmt did not work");
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
                throw new Exception("AssignMultStmt did not work");
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
                throw new Exception("AssignModStmt did not work");
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
                throw new Exception("AssignDivStmt did not work");
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
        public override void OutAOrExp(AOrExp node)
        {
            bool l = nodeMap.Get(node.GetL(), typeMap[node.GetL()]);
            bool r = nodeMap.Get(node.GetR(), typeMap[node.GetR()]);
            if (node.GetL() != null && node.GetR() != null)
            {
                nodeMap.Put(node, l || r);
            }
            else
            {
                throw new Exception("AndExp didnt not work");
            }
        }
        public override void OutAEqExp(AEqExp node)
        {
            dynamic l = GetValue(node.GetL());
            dynamic r = GetValue(node.GetR());
            if (node.GetL() != null && node.GetR() != null)
                nodeMap.Put(node, l == r);
            else
            {
                throw new Exception("EqExp did not work");
            }
        }
        public override void OutAGtExp(AGtExp node)
        {
            dynamic l = GetValue(node.GetL());
            dynamic r = GetValue(node.GetR());
            if (node.GetL() != null && node.GetR() != null)
                nodeMap.Put(node, l > r);
            else
            {
                throw new Exception("GtExp did not work");
            }
        }
        public override void OutALtExp(ALtExp node)
        {
            dynamic l = GetValue(node.GetL());
            dynamic r = GetValue(node.GetR());
            if (node.GetL() != null && node.GetR() != null)
                nodeMap.Put(node, l < r);
            else
            {
                throw new Exception("LtExp did not work");
            }
        }
        public override void OutANeqExp(ANeqExp node)
        {
            dynamic l = GetValue(node.GetL());
            dynamic r = GetValue(node.GetR());
            if (node.GetL() != null && node.GetR() != null)
                nodeMap.Put(node, l != r);
            else
            {
                throw new Exception("NeqExp did not work");
            }
        }
        public override void OutALeqExp(ALeqExp node)
        {
            dynamic l = GetValue(node.GetL());
            dynamic r = GetValue(node.GetR());
            if (node.GetL() != null && node.GetR() != null)
                nodeMap.Put(node, l <= r);
            else
            {
                throw new Exception("LeqExp did not work");
            }
        }
        public override void OutAGeqExp(AGeqExp node)
        {
            dynamic l = GetValue(node.GetL());
            dynamic r = GetValue(node.GetR());
            if (node.GetL() != null && node.GetR() != null)
                nodeMap.Put(node, l >= r);
            else
            {
                throw new Exception("GeqExp did not work");
            }
        }
        public override void OutANotExp(ANotExp node)
        {
            bool value = GetValue(node.GetExp());
            if (node != null)
            {
                nodeMap.Put(node, !value);
            }
            else
            {
                throw new Exception("NotExp did not work");
            }
        }
        public override void OutAIfStmt(AIfStmt node) //Needs fixing
        {
            bool value = GetValue(node.GetExp());
            if (value)
            {
                nodeMap.Put(node.GetThen(), GetValue(node.GetThen()));
            }
            else
            {
                nodeMap.Put(node.GetElse(), GetValue(node.GetElse()));
            }
        }

        //Todo: Make sure the compiler stops even if the while true is written.
        public override void CaseAWhileStmt(AWhileStmt node)
        {
            InAWhileStmt(node);
            if (node.GetExp() != null && node.GetBlock() != null)
            {
                node.GetExp().Apply(this);

                while (GetValue(node.GetExp()))
                {
                    node.GetBlock().Apply(this);
                    node.GetExp().Apply(this);
                }

            }
            OutAWhileStmt(node);
        }

        public override void OutAReturnStmt(AReturnStmt node)
        {
            nodeMap.Put(node, GetValue(node.GetExp()));
        }

        public override void CaseAStmtlistBlock(AStmtlistBlock node)
        {
            InAStmtlistBlock(node);
            {
                Object[] temp = new Object[node.GetStmt().Count];
                node.GetStmt().CopyTo(temp, 0);
                for (int i = temp.Length - 1; i >= 0; i--)
                {
                    ((PStmt)temp[i]).Apply(this);

                    if (nodeMap.Contains((PStmt)temp[i]))
                    {
                        break;
                    }
                }
            }
            OutAStmtlistBlock(node);
        }

        /*
        public override void OutARepeatStmt(ARepeatStmt node)
        {
        }
        public override void OutAFunctionExp(AFunctionExp node)
        {
        }
        public override void InsideScopeOutAFuncDecl(AFuncDecl node)
        {
        }
        public override void CaseAFunctionExp(AFunctionExp node)
        {
        }
        */
        internal class RuntimeTable<TKey>
        {
            private Dictionary<TKey, int> IntMap = new Dictionary<TKey, int>();
            private Dictionary<TKey, float> FloatMap = new Dictionary<TKey, float>();
            private Dictionary<TKey, bool> BoolMap = new Dictionary<TKey, bool>();
            private Dictionary<TKey, Vector> VecMap = new Dictionary<TKey, Vector>();

            public void Put(TKey key, int value)
            {
                if (IntMap.ContainsKey(key))
                {
                    IntMap[key] = value;
                }
                else
                {
                    IntMap.Add(key, value);
                }
            }

            public void Put(TKey key, bool value)
            {
                if (BoolMap.ContainsKey(key))
                {
                    BoolMap[key] = value;
                }
                else
                {
                    BoolMap.Add(key, value);
                }
            }

            public void Put(TKey key, float value)
            {
                if (FloatMap.ContainsKey(key))
                {
                    FloatMap[key] = value;
                }
                else
                {
                    FloatMap.Add(key, value);
                }
            }

            public void Put(TKey key, Vector vector)
            {
                if (VecMap.ContainsKey(key))
                {
                    VecMap[key] = vector;
                }
                else
                {
                    VecMap.Add(key, vector);
                }
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

            public bool Contains(TKey key)
            {
                if (IntMap.ContainsKey(key)   || 
                    FloatMap.ContainsKey(key) || 
                    BoolMap.ContainsKey(key)  ||
                    VecMap.ContainsKey(key))
                {
                    return true;
                }

                return false;
            }
        }
    }
}