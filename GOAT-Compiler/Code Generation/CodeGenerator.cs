﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using GOAT_Compiler.Code_Generation;
using GOAT_Compiler.Exceptions;
using GOATCode.node;

namespace GOAT_Compiler
{
    internal class CodeGenerator : SymbolTableVisitor
    {
        private Dictionary<Node, Types> typeMap;
        private RuntimeTable<Node> nodeMap;
        private dynamic CurrentReturnValue = null;
        private bool BreakFromFunction = false;

        internal RuntimeTable<Symbol> RT
        {
            get => CallStackRT.TryPeek(out RuntimeTable<Symbol> rt) ? rt : GlobalRT;
        }

        private List<dynamic> CurrentParams = new();

        internal RuntimeTable<Symbol> GlobalRT;
        internal Stack<RuntimeTable<Symbol>> CallStackRT;
        private Stack<bool> BuildStack;

        private CNCMachine _machine;

        private BuildInFunctionImplementations _buildInFunctions;


        public CodeGenerator(ISymbolTable symbolTable, Dictionary<Node, Types> typesDictionary, StreamWriter outputStream) : base(symbolTable)
        {
            _machine = new();
            _buildInFunctions = new BuildInFunctionImplementations(_machine, outputStream);
            _symbolTable = symbolTable;
            typeMap = typesDictionary;
            GlobalRT = new();
            CallStackRT = new();
            BuildStack = new Stack<bool>();
            nodeMap = new RuntimeTable<Node>();
        }

        void RTPutValue(Symbol symbol, dynamic value)
        {
            if (_symbolTable.IsGlobal(symbol))
            {
                GlobalRT.Put(symbol, value);
            }
            else
            {
                RT.Put(symbol, value);
            }
        }

        dynamic GetValue(Symbol symbol)
        {
            if (RT.Contains(symbol))
            {
                return RT.Get(symbol, symbol.type);
            }
            else
            {
                return GlobalRT.Get(symbol, symbol.type);
            }
        }

        dynamic GetValue(Node node)
        {
            return nodeMap.Get(node, typeMap[node]);
        }


        public override void CaseADeclProgram(ADeclProgram node)
        {
            InADeclProgram(node);
            {
                Object[] temp = new Object[node.GetDecl().Count];
                node.GetDecl().CopyTo(temp, 0);
                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp[i] is AVarDecl vardecl)
                    {
                        vardecl.Apply(this);
                    }
                }
                _symbolTable.GetFunctionNode(_symbolTable.GetFunctionSymbol("main")).Apply(this);
            }
            OutADeclProgram(node);
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
                        RT.Put(symbol, GetValue(nodeExpr));
                        break;
                    case Types.FloatingPoint:
                        RT.Put(symbol, (float)GetValue(nodeExpr));
                        break;
                    case Types.Boolean:
                        RT.Put(symbol, GetValue(nodeExpr));
                        break;
                    case Types.Vector:
                        RT.Put(symbol, GetValue(nodeExpr));
                        break;
                    default:
                        throw new Exception("This should never happen.");
                }
            }
        }

        public override void OutAIdExp(AIdExp node)
        {
            Symbol VarSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            nodeMap.Put(node, GetValue(VarSymbol));
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
                if (idSymbol.type == Types.FloatingPoint)
                {
                    RTPutValue(idSymbol, (float)GetValue(node.GetExp()));
                }
                else
                {
                    RTPutValue(idSymbol, GetValue(node.GetExp()));
                }
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
                    RTPutValue(idSymbol, resultVec);
                }
                else
                {
                    if (idSymbol.type == Types.FloatingPoint)
                    {
                        RTPutValue(idSymbol, (float)(GetValue(idSymbol) + GetValue(node.GetExp())));
                    }
                    else
                    {
                        RTPutValue(idSymbol, GetValue(idSymbol) + GetValue(node.GetExp()));
                    }
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
                    RTPutValue(idSymbol, resultVec);
                }
                else
                {
                    if (idSymbol.type == Types.FloatingPoint)
                    {
                        RTPutValue(idSymbol, (float)(GetValue(idSymbol) - GetValue(node.GetExp())));
                    }
                    else
                    {
                        RTPutValue(idSymbol, GetValue(idSymbol) - GetValue(node.GetExp()));
                    }

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
                    RTPutValue(idSymbol, resultVec);
                }
                else
                {
                    if (idSymbol.type == Types.FloatingPoint)
                    {
                        RTPutValue(idSymbol, (float)(GetValue(idSymbol) * GetValue(node.GetExp())));
                    }
                    else
                    {
                        RTPutValue(idSymbol, GetValue(idSymbol) * GetValue(node.GetExp()));
                    }
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
                if (idSymbol.type == Types.FloatingPoint)
                {
                    RTPutValue(idSymbol, (float)(GetValue(idSymbol) % GetValue(node.GetExp())));
                }
                else
                {
                    RTPutValue(idSymbol, GetValue(idSymbol) % GetValue(node.GetExp()));
                }
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
                    RTPutValue(idSymbol, resultVec);
                }
                else
                {
                    if (idSymbol.type == Types.FloatingPoint)
                    {
                        RTPutValue(idSymbol, (float)(GetValue(idSymbol) / GetValue(node.GetExp())));
                    }
                    else
                    {
                        RTPutValue(idSymbol, GetValue(idSymbol) / GetValue(node.GetExp()));
                    }
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


        public override void CaseAIfStmt(AIfStmt node)
        {
            InAIfStmt(node);
            if (node.GetExp() != null)
            {
                node.GetExp().Apply(this);
            }

            if (GetValue(node.GetExp()))
            {
                if (node.GetThen() != null)
                {
                    node.GetThen().Apply(this);
                }
            }
            else
            {
                if (node.GetElse() != null)
                {
                    node.GetElse().Apply(this);
                }
            }
            OutAIfStmt(node);
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


        public override void CaseARepeatStmt(ARepeatStmt node)
        {
            InARepeatStmt(node);
            if (node.GetExp() != null)
            {
                node.GetExp().Apply(this);
            }

            int i = 0;
            while (GetValue(node.GetExp()) > i)
            {
                if (node.GetBlock() != null)
                {
                    node.GetBlock().Apply(this);
                }

                i++;
                node.GetExp().Apply(this);
            }

            OutARepeatStmt(node);
        }


        public override void OutAReturnStmt(AReturnStmt node)
        {
            CurrentReturnValue = GetValue(node.GetExp());
            BreakFromFunction = true;
        }

        public override void CaseAStmtlistBlock(AStmtlistBlock node)
        {
            InAStmtlistBlock(node);
            {
                Object[] temp = new Object[node.GetStmt().Count];
                node.GetStmt().CopyTo(temp, 0);
                for (int i = 0; i < temp.Length; i++)
                {
                    ((PStmt)temp[i]).Apply(this);

                    if (BreakFromFunction)
                    {
                        break;
                    }
                }
            }
            OutAStmtlistBlock(node);
        }


        public override void InABuildStmt(ABuildStmt node)
        {
            BuildStack.Push(true);
        }

        public override void OutABuildStmt(ABuildStmt node)
        {
            BuildStack.Pop();
        }

        public override void InABuildExp(ABuildExp node)
        {
            BuildStack.Push(true);
        }

        public override void OutABuildExp(ABuildExp node)
        {
            BuildStack.Pop();
        }

        public override void InABuildBlock(ABuildBlock node)
        {
            BuildStack.Push(true);
        }

        public override void OutABuildBlock(ABuildBlock node)
        {
            BuildStack.Pop();
        }

        public override void InAWalkStmt(AWalkStmt node)
        {
            BuildStack.Push(false);
        }

        public override void OutAWalkStmt(AWalkStmt node)
        {
            BuildStack.Pop();
        }

        public override void InAWalkExp(AWalkExp node)
        {
            BuildStack.Push(false);
        }

        public override void OutAWalkExp(AWalkExp node)
        {
            BuildStack.Pop();
        }

        public override void InAWalkBlock(AWalkBlock node)
        {
            BuildStack.Push(false);
        }

        public override void OutAWalkBlock(AWalkBlock node)
        {
            BuildStack.Pop();
        }


        public override void OutAFunctionStmt(AFunctionStmt node)
        {
            node.GetExp().Apply(this);
        }

        public override void OutAFunctionExp(AFunctionExp node)
        {
            Symbol funcSymbol = _symbolTable.GetFunctionSymbol(node.GetName().Text);
            if (funcSymbol == null)
            {
                throw null;
            }

            Node[] args = new Node[node.GetArgs().Count];
            node.GetArgs().CopyTo(args, 0);

            foreach (var arg in args)
            {
                CurrentParams.Add(GetValue(arg));
            }

            if (BuiltInFunctions.FunctionsList.ContainsKey(node.GetName().Text))
            {
                if (BuildStack.TryPeek(out bool build))
                {
                    _machine.Build = build;
                }
                else
                {
                    throw new BuildWalkException(node, "cant execute command without a build or walk scope.");
                }
                dynamic returnValue = _buildInFunctions.CallBuildInFunctions(node.GetName().Text, CurrentParams);
                if (returnValue != null)
                {
                    nodeMap.Put(node, returnValue);
                }
                CurrentParams.Clear();
            }
            else
            {
                Node funcNode = _symbolTable.GetFunctionNode(_symbolTable.GetFunctionSymbol(node.GetName().Text));

                funcNode.Apply(this);

                if (funcNode is AFuncDecl)
                {
                    nodeMap.Put(node, CurrentReturnValue);
                }

                BreakFromFunction = false;
            }
        }


        public override void OutAParamDecl(AParamDecl node)
        {
            Symbol paramSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            RTPutValue(paramSymbol, CurrentParams[0]);
            CurrentParams.RemoveAt(0);
        }


        public override void CaseAFuncDecl(AFuncDecl node)
        {
            InAFuncDecl(node);
            CallStackRT.Push(new RuntimeTable<Symbol>());
            if (node.GetTypes() != null)
            {
                node.GetTypes().Apply(this);
            }
            if (node.GetId() != null)
            {
                node.GetId().Apply(this);
            }
            {
                Object[] temp = new Object[node.GetDecl().Count];
                node.GetDecl().CopyTo(temp, 0);
                for (int i = 0; i < temp.Length; i++)
                {
                    ((PDecl)temp[i]).Apply(this);
                }
            }
            if (node.GetBlock() != null)
            {
                node.GetBlock().Apply(this);
            }
            OutAFuncDecl(node);
        }

        public override void InsideScopeOutAFuncDecl(AFuncDecl node)
        {
            CallStackRT.Pop();
        }


        public override void CaseAProcDecl(AProcDecl node)
        {
            InAProcDecl(node);
            CallStackRT.Push(new RuntimeTable<Symbol>());
            if (node.GetId() != null)
            {
                node.GetId().Apply(this);
            }
            {
                Object[] temp = new Object[node.GetDecl().Count];
                node.GetDecl().CopyTo(temp, 0);
                for (int i = 0; i < temp.Length; i++)
                {
                    ((PDecl)temp[i]).Apply(this);
                }
            }
            if (node.GetBlock() != null)
            {
                node.GetBlock().Apply(this);
            }
            OutAProcDecl(node);
        }


        public override void InsideScopeOutAProcDecl(AProcDecl node)
        {
            CallStackRT.Pop();
        }



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
                if (IntMap.ContainsKey(key) ||
                    FloatMap.ContainsKey(key) ||
                    BoolMap.ContainsKey(key) ||
                    VecMap.ContainsKey(key))
                {
                    return true;
                }

                return false;
            }
        }
    }
}