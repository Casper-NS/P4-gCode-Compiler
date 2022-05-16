using GOAT_Compiler.Code_Generation;
using GOAT_Compiler.Exceptions;
using GOATCode.node;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace GOAT_Compiler
{
    internal class CodeGenerator : SymbolTableVisitor
    {
        /// <summary>
        /// The max amount of iterations of a while-loop
        /// </summary>
        private const int _maxIterationLimit = 100000;

        /// <summary>
        /// Stores the types of nodes with types, e.g. id-expression-nodes.
        /// </summary>
        private readonly Dictionary<Node, Types> typeMap;

        /// <summary>
        /// Stores the values of all expression nodes.
        /// </summary>
        private readonly RuntimeTable<Node> valueMap;

        private dynamic CurrentReturnValue = null;
        private bool BreakFromFunction = false;

        /// <summary>
        /// The top-value of the stack indicates wheter we are in a build-scope(true) or not(false).
        /// </summary>
        private readonly Stack<bool> BuildStack;

        private readonly CNCMachine _machine;
        private readonly BuildInFunctionImplementations _buildInFunctions;

        /// <summary>
        /// The textwriter which writes g-code to the output file.
        /// </summary>
        private readonly TextWriter _textWriter;

        private readonly List<dynamic> CurrentParams = new();

        internal RuntimeTable<Symbol> GlobalRT;

        /// <summary>
        /// A callstack of runtime tables, a table is added for each function-call.
        /// </summary>
        internal Stack<RuntimeTable<Symbol>> CallStackRT;

        internal RuntimeTable<Symbol> RT
        {
            get => CallStackRT.TryPeek(out RuntimeTable<Symbol> rt) ? rt : GlobalRT;
        }

        public CodeGenerator(ISymbolTable symbolTable, Dictionary<Node, Types> typesDictionary, TextWriter outputStream) : base(symbolTable)
        {
            _machine = new();
            _buildInFunctions = new BuildInFunctionImplementations(_machine, outputStream);
            _textWriter = outputStream;
            _symbolTable = symbolTable;
            typeMap = typesDictionary;
            GlobalRT = new();
            CallStackRT = new();
            BuildStack = new Stack<bool>();
            valueMap = new RuntimeTable<Node>();
        }

        private void RTPutValue(Symbol symbol, dynamic value)
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

        private dynamic GetValue(Symbol symbol)
        {
            if (RT.Contains(symbol))
            {
                return RT.Get(symbol, symbol.Type);
            }
            else
            {
                return GlobalRT.Get(symbol, symbol.Type);
            }
        }

        private dynamic GetValue(Node node) => valueMap.Get(node, typeMap[node]);

        public override void CaseADeclProgram(ADeclProgram node)
        {
            InADeclProgram(node);
            {
                Object[] decls = new Object[node.GetDecl().Count];
                node.GetDecl().CopyTo(decls, 0);
                foreach (object decl in decls)
                {
                    if (decl is AVarDecl vardecl)
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
                        RT.Put(symbol, (double)GetValue(nodeExpr));
                        break;

                    case Types.Boolean:
                        RT.Put(symbol, GetValue(nodeExpr));
                        break;

                    case Types.Vector:
                        RT.Put(symbol, GetValue(nodeExpr));
                        break;
                }
            }
        }

        public override void OutAIdExp(AIdExp node)
        {
            Symbol VarSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (node.GetDot() != null)
            {
                switch (node.GetDot().Text)
                {
                    case ".x":
                        valueMap.Put(node, GetValue(VarSymbol).X);
                        break;

                    case ".y":
                        valueMap.Put(node, GetValue(VarSymbol).Y);
                        break;

                    case ".z":
                        valueMap.Put(node, GetValue(VarSymbol).Z);
                        break;
                }
            }
            else
            {
                valueMap.Put(node, GetValue(VarSymbol));
            }
        }

        public override void OutABoolvalExp(ABoolvalExp node) => valueMap.Put(node, bool.Parse(node.GetBoolValue().Text));

        public override void OutAVectorExp(AVectorExp node)
        {
            double x = GetValue(node.GetX());
            double y = GetValue(node.GetY());
            double z = GetValue(node.GetZ());
            valueMap.Put(node, new Vector(x, y, z));
        }

        public override void OutANumberExp(ANumberExp node)
        {
            switch (typeMap[node])
            {
                case Types.Integer:
                    valueMap.Put(node, int.Parse(node.GetNumber().Text));
                    break;

                case Types.FloatingPoint:
                    valueMap.Put(node, double.Parse(node.GetNumber().Text, CultureInfo.InvariantCulture));
                    break;
            }
        }

        public override void OutANegExp(ANegExp node) => valueMap.Put(node, -GetValue(node.GetExp()));

        public override void OutAAssignStmt(AAssignStmt node)
        {
            Symbol idSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);

            if (idSymbol.Type == Types.FloatingPoint)
            {
                RTPutValue(idSymbol, (double)GetValue(node.GetExp()));
            }
            else
            {
                RTPutValue(idSymbol, GetValue(node.GetExp()));
            }
        }

        public override void OutAAssignPlusStmt(AAssignPlusStmt node)
        {
            Symbol idSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (idSymbol.Type == Types.Vector)
            {
                Vector Vec1 = ((Vector)GetValue(node.GetExp()));
                Vector Vec2 = ((Vector)GetValue(idSymbol));

                Vector resultVec = new Vector(Vec1.X + Vec2.X, Vec1.Y + Vec2.Y, Vec1.Z + Vec2.Z);
                RTPutValue(idSymbol, resultVec);
            }
            else
            {
                if (idSymbol.Type == Types.FloatingPoint)
                {
                    RTPutValue(idSymbol, (double)(GetValue(idSymbol) + GetValue(node.GetExp())));
                }
                else
                {
                    RTPutValue(idSymbol, GetValue(idSymbol) + GetValue(node.GetExp()));
                }
            }
        }

        public override void OutAAssignMinusStmt(AAssignMinusStmt node)
        {
            Symbol idSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (idSymbol.Type == Types.Vector)
            {
                Vector Vec1 = ((Vector)GetValue(node.GetExp()));
                Vector Vec2 = ((Vector)GetValue(idSymbol));

                Vector resultVec = new Vector(Vec1.X - Vec2.X, Vec1.Y - Vec2.Y, Vec1.Z - Vec2.Z);
                RTPutValue(idSymbol, resultVec);
            }
            else
            {
                if (idSymbol.Type == Types.FloatingPoint)
                {
                    RTPutValue(idSymbol, (double)(GetValue(idSymbol) - GetValue(node.GetExp())));
                }
                else
                {
                    RTPutValue(idSymbol, GetValue(idSymbol) - GetValue(node.GetExp()));
                }
            }
        }

        public override void OutAAssignMultStmt(AAssignMultStmt node)
        {
            Symbol idSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (idSymbol.Type == Types.Vector)
            {
                Vector Vec = ((Vector)GetValue(idSymbol));
                dynamic scale = GetValue(node.GetExp());

                Vector resultVec = new Vector(Vec.X * scale, Vec.Y * scale, Vec.Z * scale);
                RTPutValue(idSymbol, resultVec);
            }
            else
            {
                if (idSymbol.Type == Types.FloatingPoint)
                {
                    RTPutValue(idSymbol, (double)(GetValue(idSymbol) * GetValue(node.GetExp())));
                }
                else
                {
                    RTPutValue(idSymbol, GetValue(idSymbol) * GetValue(node.GetExp()));
                }
            }
        }

        public override void OutAAssignModStmt(AAssignModStmt node)
        {
            Symbol idSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (idSymbol.Type == Types.FloatingPoint)
            {
                RTPutValue(idSymbol, (double)(GetValue(idSymbol) % GetValue(node.GetExp())));
            }
            else
            {
                RTPutValue(idSymbol, GetValue(idSymbol) % GetValue(node.GetExp()));
            }
        }

        public override void OutAAssignDivisionStmt(AAssignDivisionStmt node)
        {
            Symbol idSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (idSymbol.Type == Types.Vector)
            {
                Vector Vec1 = ((Vector)GetValue(idSymbol));
                dynamic value = GetValue(node.GetExp());

                Vector resultVec = new Vector(Vec1.X / value, Vec1.Y / value, Vec1.Z / value);
                RTPutValue(idSymbol, resultVec);
            }
            else
            {
                if (idSymbol.Type == Types.FloatingPoint)
                {
                    RTPutValue(idSymbol, (double)(GetValue(idSymbol) / GetValue(node.GetExp())));
                }
                else
                {
                    RTPutValue(idSymbol, GetValue(idSymbol) / GetValue(node.GetExp()));
                }
            }
        }

        public override void OutAPlusExp(APlusExp node)
        {
            Node left = node.GetL();
            Node right = node.GetR();
            if (typeMap[left] == Types.Vector && typeMap[right] == Types.Vector)
            {
                Vector Vec1 = ((Vector)GetValue(left));
                Vector Vec2 = ((Vector)GetValue(right));

                valueMap.Put(node, Vec1 + Vec2);
            }
            else
            {
                valueMap.Put(node, GetValue(left) + GetValue(right));
            }
        }

        public override void OutAMinusExp(AMinusExp node)
        {
            Node left = node.GetL();
            Node right = node.GetR();
            if (typeMap[left] == Types.Vector && typeMap[right] == Types.Vector)
            {
                Vector Vec1 = ((Vector)GetValue(left));
                Vector Vec2 = ((Vector)GetValue(right));

                valueMap.Put(node, (Vec1 - Vec2));
            }
            else
            {
                valueMap.Put(node, GetValue(left) - GetValue(right));
            }
        }

        public override void OutADivdExp(ADivdExp node)
        {
            Node left = node.GetL();
            Node right = node.GetR();
            if (typeMap[left] == Types.Vector && (typeMap[right] == Types.FloatingPoint || typeMap[right] == Types.Integer))
            {
                Vector Vec1 = ((Vector)GetValue(left));
                dynamic value = GetValue(right);

                Vector resultVec = new Vector(Vec1.X / value, Vec1.Y / value, Vec1.Z / value);
                valueMap.Put(node, resultVec);
            }
            else
            {
                valueMap.Put(node, GetValue(left) / GetValue(right));
            }
        }

        public override void OutAModuloExp(AModuloExp node)
        {
            Node left = node.GetL();
            Node right = node.GetR();
            valueMap.Put(node, GetValue(left) % GetValue(right));
        }

        public override void OutAMultExp(AMultExp node)
        {
            Node left = node.GetL();
            Node right = node.GetR();
            if (typeMap[left] == Types.Vector && (typeMap[right] == Types.FloatingPoint || typeMap[right] == Types.Integer))
            {
                Vector Vec1 = ((Vector)GetValue(left));
                dynamic value = GetValue(right);

                Vector resultVec = new Vector(Vec1.X * value, Vec1.Y * value, Vec1.Z * value);
                valueMap.Put(node, resultVec);
            }
            else
            {
                valueMap.Put(node, GetValue(left) * GetValue(right));
            }
        }

        public override void OutAAndExp(AAndExp node)
        {
            bool l = valueMap.Get(node.GetL(), typeMap[node.GetL()]);
            bool r = valueMap.Get(node.GetR(), typeMap[node.GetR()]);
            valueMap.Put(node, l && r);
        }

        public override void OutAOrExp(AOrExp node)
        {
            bool l = valueMap.Get(node.GetL(), typeMap[node.GetL()]);
            bool r = valueMap.Get(node.GetR(), typeMap[node.GetR()]);
            valueMap.Put(node, l || r);
        }

        public override void OutAEqExp(AEqExp node)
        {
            dynamic l = GetValue(node.GetL());
            dynamic r = GetValue(node.GetR());
            valueMap.Put(node, l == r);
        }

        public override void OutAGtExp(AGtExp node)
        {
            dynamic l = GetValue(node.GetL());
            dynamic r = GetValue(node.GetR());
            valueMap.Put(node, l > r);
        }

        public override void OutALtExp(ALtExp node)
        {
            dynamic l = GetValue(node.GetL());
            dynamic r = GetValue(node.GetR());
            valueMap.Put(node, l < r);
        }

        public override void OutANeqExp(ANeqExp node)
        {
            dynamic l = GetValue(node.GetL());
            dynamic r = GetValue(node.GetR());
            valueMap.Put(node, l != r);
        }

        public override void OutALeqExp(ALeqExp node)
        {
            dynamic l = GetValue(node.GetL());
            dynamic r = GetValue(node.GetR());
            valueMap.Put(node, l <= r);
        }

        public override void OutAGeqExp(AGeqExp node)
        {
            dynamic l = GetValue(node.GetL());
            dynamic r = GetValue(node.GetR());
            valueMap.Put(node, l >= r);
        }

        public override void OutANotExp(ANotExp node)
        {
            bool value = GetValue(node.GetExp());
            valueMap.Put(node, !value);
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

        public override void CaseAWhileStmt(AWhileStmt node)
        {
            int iterations = 0;
            InAWhileStmt(node);
            if (node.GetExp() != null && node.GetBlock() != null)
            {
                node.GetExp().Apply(this);

                while (GetValue(node.GetExp()) && iterations < _maxIterationLimit)
                {
                    node.GetBlock().Apply(this);
                    node.GetExp().Apply(this);
                    iterations++;
                }
                if (iterations >= _maxIterationLimit)
                {
                    throw new ExceededMaxIterationLimitException(node);
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
            while (GetValue(node.GetExp()) > i && i < _maxIterationLimit)
            {
                if (node.GetBlock() != null)
                {
                    node.GetBlock().Apply(this);
                }

                i++;
                node.GetExp().Apply(this);
            }
            if (i >= _maxIterationLimit)
            {
                throw new ExceededMaxIterationLimitException(node);
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
                Object[] stmts = new Object[node.GetStmt().Count];
                node.GetStmt().CopyTo(stmts, 0);
                foreach (object stmt in stmts)
                {
                    ((PStmt)stmt).Apply(this);

                    if (BreakFromFunction)
                    {
                        break;
                    }
                }
            }
            OutAStmtlistBlock(node);
        }

        public override void InABuildStmt(ABuildStmt node) => BuildStack.Push(true);

        public override void OutABuildStmt(ABuildStmt node) => BuildStack.Pop();

        public override void InABuildExp(ABuildExp node) => BuildStack.Push(true);

        public override void OutABuildExp(ABuildExp node) => BuildStack.Pop();

        public override void InABuildBlock(ABuildBlock node) => BuildStack.Push(true);

        public override void OutABuildBlock(ABuildBlock node) => BuildStack.Pop();

        public override void InAWalkStmt(AWalkStmt node) => BuildStack.Push(false);

        public override void OutAWalkStmt(AWalkStmt node) => BuildStack.Pop();

        public override void InAWalkExp(AWalkExp node) => BuildStack.Push(false);

        public override void OutAWalkExp(AWalkExp node) => BuildStack.Pop();

        public override void InAWalkBlock(AWalkBlock node) => BuildStack.Push(false);

        public override void OutAWalkBlock(AWalkBlock node) => BuildStack.Pop();

        public override void OutAFunctionExp(AFunctionExp node)
        {
            dynamic returnValue;
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
                    if (build)
                    {
                        _machine.ExtrusionMode = ExtrusionMode.build;
                    }
                    else
                    {
                        _machine.ExtrusionMode = ExtrusionMode.walk;
                    }
                }
                else
                {
                    _machine.ExtrusionMode = ExtrusionMode.none;
                }

                try
                {
                    returnValue = _buildInFunctions.CallBuildInFunctions(node.GetName().Text, CurrentParams);
                }
                catch (MoveWithoutScopeException e)
                {
                    throw new BuildInWalkException(node, e.Message);
                }

                valueMap.Put(node, returnValue);
                CurrentParams.Clear();
            }
            else
            {
                Node funcNode = _symbolTable.GetFunctionNode(_symbolTable.GetFunctionSymbol(node.GetName().Text));

                funcNode.Apply(this);

                if (funcNode is AFuncDecl)
                {
                    valueMap.Put(node, CurrentReturnValue);
                }

                BreakFromFunction = false;
            }
        }

        public override void OutAParamDecl(AParamDecl node)
        {
            Symbol paramSymbol = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (typeMap[node] == Types.FloatingPoint)
            {
                RTPutValue(paramSymbol, (double)CurrentParams[0]);
            }
            else
            {
                RTPutValue(paramSymbol, CurrentParams[0]);
            }
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
            Object[] decls = new Object[node.GetDecl().Count];
            node.GetDecl().CopyTo(decls, 0);
            foreach (object decl in decls)
            {
                ((PDecl)decl).Apply(this);
            }
            if (node.GetBlock() != null)
            {
                node.GetBlock().Apply(this);
            }
            OutAFuncDecl(node);
        }

        public override void InsideScopeOutAFuncDecl(AFuncDecl node) => CallStackRT.Pop();

        public override void CaseAProcDecl(AProcDecl node)
        {
            InAProcDecl(node);
            CallStackRT.Push(new RuntimeTable<Symbol>());
            if (node.GetId() != null)
            {
                node.GetId().Apply(this);
            }
            Object[] decls = new Object[node.GetDecl().Count];
            node.GetDecl().CopyTo(decls, 0);
            foreach (var decl in decls)
            {
                ((PDecl)decl).Apply(this);
            }
            if (node.GetBlock() != null)
            {
                node.GetBlock().Apply(this);
            }
            OutAProcDecl(node);
        }

        public override void InsideScopeOutAProcDecl(AProcDecl node) => CallStackRT.Pop();

        public override void CaseAGcodeStmt(AGcodeStmt node)
        {
            string literal = node.GetGcodeLiteral().Text;
            var linebyline = literal.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (string line in linebyline)
            {
                _textWriter.WriteLine(line.Trim());
            }
        }

        internal class RuntimeTable<TKey>
        {
            private readonly Dictionary<TKey, int> IntMap = new Dictionary<TKey, int>();
            private readonly Dictionary<TKey, double> FloatMap = new Dictionary<TKey, double>();
            private readonly Dictionary<TKey, bool> BoolMap = new Dictionary<TKey, bool>();
            private readonly Dictionary<TKey, Vector> VecMap = new Dictionary<TKey, Vector>();

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

            public void Put(TKey key, double value)
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
                return (IntMap.ContainsKey(key) ||
                    FloatMap.ContainsKey(key) ||
                    BoolMap.ContainsKey(key) ||
                    VecMap.ContainsKey(key));
            }
        }
    }
}