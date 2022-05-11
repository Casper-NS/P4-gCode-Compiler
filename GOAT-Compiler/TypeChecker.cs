using GOAT_Compiler.Exceptions;
using GOATCode.node;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAT_Compiler
{
    /// <summary>
    /// The type checker class for the GOAT Code.
    /// </summary>
    internal class TypeChecker : SymbolTableVisitor
    {
        private Types currentFunctionType;
        private Dictionary<Node, Types> _typeDictionary = new Dictionary<Node, Types>();

        private HashSet<Node> guaranteeedToReturn = new HashSet<Node>();

        public Dictionary<Node, Types> GetTypeDictionary()
        {
            return _typeDictionary;
        }

        /// <summary>
        /// The constructor for the type checker.
        /// </summary>
        /// <param name="symbolTable">The symbol table</param>
        public TypeChecker(ISymbolTable symbolTable) : base(symbolTable)
        {
        }
        private void ArithmeticTypeChecker(Node left, Node right, Node current)
        {
            Types leftType = _typeDictionary[left];
            Types rightType = _typeDictionary[right];
            Types type = TypePromoter(leftType, rightType);
            if (type == Types.Void || type == Types.Boolean) 
            {
                throw new TypeMismatchException(current, "Type " + type + " is not valid for this arithmetic operations.");
            }
            else
            {
                _typeDictionary.Add(current, type);
            }            
        }
        private void EqlNotEqlTypeChecker(Node left, Node right, Node current)
        {
            Types leftType = _typeDictionary[left];
            Types rightType = _typeDictionary[right];

            if (leftType == Types.Integer && rightType == Types.FloatingPoint)
            {
                _typeDictionary.Add(current, Types.Boolean);
            }
            else if (leftType == Types.FloatingPoint && rightType == Types.Integer)
            {
                _typeDictionary.Add(current, Types.Boolean);
            }
            else if (leftType == rightType)
            {
                _typeDictionary.Add(current, Types.Boolean);
            }
            else
            {
                throw new TypeMismatchException(current, "Type " + leftType + " and " + rightType + " are not valid for this comparison operations.");
            }
        }

        private void AndAndOrTypeChecker(Node nodeleft, Node nodeRight, Node current)
        {
            Types leftType = _typeDictionary[nodeleft];
            Types rightType = _typeDictionary[nodeRight];
            if (leftType == Types.Boolean && rightType == Types.Boolean)
            {
                _typeDictionary.Add(current, Types.Boolean);
            }
            else
            {
                throw new TypeMismatchException(current, "Type " + leftType + " and " + rightType + " are not valid for this logical operation.");
            }
        }
        private void GreaterThanLessThanTypeChecker(Node nodeleft, Node nodeRight, Node current)
        {
            Types leftType = _typeDictionary[nodeleft];
            Types rightType = _typeDictionary[nodeRight];
            if ((leftType != Types.Boolean && rightType != Types.Boolean) && (leftType != Types.Vector && rightType != Types.Vector))
            {
                _typeDictionary.Add(current, Types.Boolean);
            }
            else
            {
                throw new TypeMismatchException(current, "Type " + leftType + " and " + rightType + " are not valid for this comparison operations.");
            }
        }
        private void MultDivModTypeChecker(Node left, Node right, Node current)
        {
            Types leftType = _typeDictionary[left];
            Types rightType = _typeDictionary[right];
            if ((leftType == Types.Integer || leftType == Types.FloatingPoint) && (rightType == Types.Integer || rightType == Types.FloatingPoint))
            {
                ArithmeticTypeChecker(left, right, current);
            }
            else
            {
                throw new TypeMismatchException(current, "Type " + leftType + " and " + rightType + " are not valid for this arithemtic operations.");
            }
        }
        private void DivMultTypeChecker(Symbol id, Types type, Node node, Node expr, TDot dot)
        {
            if (dot != null)
            {
                if (id.type != Types.Vector)
                {
                    throw new TypeMismatchException(node, "symbols with.extensions have to be vectors");
                }
                if (type == Types.FloatingPoint || type == Types.Integer)
                {
                    _typeDictionary.Add(node, Types.FloatingPoint);
                }
                else
                {
                    throw new TypeMismatchException(node, "Types not compatible with multiply or divide expression");
                }
            }
            else if (id.type == Types.Vector && (type == Types.FloatingPoint || type == Types.Integer))
            {
                _typeDictionary.Add(node, id.type);
            }
            else if (id.type != Types.Vector)
            {
                _typeDictionary.Add(node, Convert(expr, id.type));
            }
            else
            {
                throw new TypeMismatchException(node, "Types not compatible with multiply or divide expression");
            }
        }
        private Types TypePromoter(Types t1, Types t2)
        {
            if (t1 == Types.Integer && t2 == Types.FloatingPoint)
            {
                return Types.FloatingPoint;
            }
            else if (t1 == Types.FloatingPoint && t2 == Types.Integer)
            {
                return Types.FloatingPoint;
            }
            else if (t1 == t2)
            {
                return t1;
            }
            else
            {
                return Types.Void;
            }
        }
        private Types NumberType(string numberToken)
        {
            if (numberToken.Contains("."))
            {
                return Types.FloatingPoint;
            }
            else
            {
                return Types.Integer;
            }
        }
        private Types Convert(Node n, Types t)
        {
            if (_typeDictionary[n] == t)
            {
                return t;
            }
            else if (_typeDictionary[n] == Types.Integer && t == Types.FloatingPoint)
            {
                return Types.FloatingPoint;
            }
            else
            {
                throw new TypeMismatchException(n, "Type " + _typeDictionary[n] + " is not valid for the Convert method.");
            }
        }
        public override void OutAVectorExp(AVectorExp node)
        {
            Types x = _typeDictionary[node.GetX()];
            Types y = _typeDictionary[node.GetY()];
            Types z = _typeDictionary[node.GetZ()];
            if (Convert(node.GetX(), Types.FloatingPoint) == Types.FloatingPoint 
                && Convert(node.GetY(), Types.FloatingPoint) == Types.FloatingPoint 
                && Convert(node.GetZ(), Types.FloatingPoint) == Types.FloatingPoint)
            {
                _typeDictionary.Add(node, Types.Vector);
            }
        }
        public override void OutANumberExp(ANumberExp node)
        {
            _typeDictionary.Add(node, NumberType(node.GetNumber().Text));
        }

        public override void OutAPlusExp(APlusExp node)
        {
            ArithmeticTypeChecker(node.GetL(), node.GetR(), node);
        }

        public override void OutAMinusExp(AMinusExp node)
        {
            ArithmeticTypeChecker(node.GetL(), node.GetR(), node);
        }
        public override void OutANegExp(ANegExp node)
        {
            Types type = _typeDictionary[node.GetExp()];
            if (type == Types.Boolean)
            {
                throw new TypeMismatchException(node, "Tried to negate a boolean");
            }
            _typeDictionary.Add(node, type);
        }

        public override void OutABoolvalExp(ABoolvalExp node)
        {
            _typeDictionary.Add(node, Types.Boolean);
        }

        public override void OutAAndExp(AAndExp node)
        {
            AndAndOrTypeChecker(node.GetL(), node.GetR(), node);
        }
        public override void OutAOrExp(AOrExp node)
        {
            AndAndOrTypeChecker(node.GetL(), node.GetR(), node);
        }
        public override void OutAEqExp(AEqExp node)
        {
            EqlNotEqlTypeChecker(node.GetL(), node.GetR(), node);
        }
        public override void OutAModuloExp(AModuloExp node)
        {
            MultDivModTypeChecker(node.GetL(), node.GetR(), node);
        }

        public override void OutAMultExp(AMultExp node)
        {
            MultDivModTypeChecker(node.GetL(), node.GetR(), node);
        }

        public override void OutADivdExp(ADivdExp node)
        {
            MultDivModTypeChecker(node.GetL(), node.GetR(), node);
        }
        public override void OutAGeqExp(AGeqExp node)
        {
            GreaterThanLessThanTypeChecker(node.GetL(), node.GetR(), node);
        }

        public override void OutAGtExp(AGtExp node)
        {
            GreaterThanLessThanTypeChecker(node.GetL(), node.GetR(), node);
        }
        public override void OutALtExp(ALtExp node)
        {
            GreaterThanLessThanTypeChecker(node.GetL(), node.GetR(), node);
        }
        public override void OutALeqExp(ALeqExp node)
        {
            GreaterThanLessThanTypeChecker(node.GetL(), node.GetR(), node);
        }
        public override void OutANeqExp(ANeqExp node)
        {
            EqlNotEqlTypeChecker(node.GetL(), node.GetR(), node);
        }

        private void CheckDot(Node node, PExp expr, Symbol symbol, TDot dot)
        {
            if (dot != null)
            {
                if (symbol.type == Types.Vector)
                {
                    _typeDictionary.Add(node, Convert(expr, Types.FloatingPoint));
                }
                else
                {
                    throw new TypeMismatchException(node, "symbols with . extensions have to be vectors");
                }
            }
            else
            {
                _typeDictionary.Add(node, Convert(expr, symbol.type));
            }
        }

        public override void OutAAssignStmt(AAssignStmt node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            CheckDot(node, node.GetExp(), id, node.GetDot());
        }
        public override void OutAIfStmt(AIfStmt node)
        {
            if (_typeDictionary[node.GetExp()] != Types.Boolean)
            {
                throw new TypeMismatchException(node, "If statement must have a boolean expression");
            }
            else
            {
                _typeDictionary.Add(node, Types.Void);
            }

            if (guaranteeedToReturn.Contains(node.GetThen()) && (node.GetElse() == null  || guaranteeedToReturn.Contains(node.GetElse())))
            {
                guaranteeedToReturn.Add(node);
            }
        }
        public override void OutAWhileStmt(AWhileStmt node)
        {
            if (_typeDictionary[node.GetExp()] != Types.Boolean)
            {
                throw new TypeMismatchException(node);
            }
        }
        public override void OutARepeatStmt(ARepeatStmt node)
        {
            if (_typeDictionary[node.GetExp()] != Types.Integer)
            {
                throw new TypeMismatchException(node);
            }
        }
        public override void OutAAssignPlusStmt(AAssignPlusStmt node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            CheckDot(node, node.GetExp(), id, node.GetDot());
        }
        public override void OutAAssignMinusStmt(AAssignMinusStmt node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            CheckDot(node, node.GetExp(), id, node.GetDot());
        }
        public override void OutAAssignDivisionStmt(AAssignDivisionStmt node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            Types expType = _typeDictionary[node.GetExp()];
            DivMultTypeChecker(id, expType, node, node.GetExp(), node.GetDot());
        }
        public override void OutAAssignMultStmt(AAssignMultStmt node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            Types expType = _typeDictionary[node.GetExp()];
            DivMultTypeChecker(id, expType, node, node.GetExp(), node.GetDot());
        }
        public override void OutAAssignModStmt(AAssignModStmt node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            CheckDot(node, node.GetExp(), id, node.GetDot());
        }
        public override void OutANotExp(ANotExp node)
        {
            if (_typeDictionary[node.GetExp()] == Types.Boolean)
            {
                _typeDictionary.Add(node, Types.Boolean);
            }
            else
            {
                throw new TypeMismatchException(node, "Not operator can only be used on booleans");
            }
        }
        public override void OutAVarDecl(AVarDecl node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if(node.GetExp() != null)
            {
                if (Convert(node.GetExp(), id.type) == id.type)
                {
                    _typeDictionary.Add(node, id.type);
                }
            }
            else
            {
                _typeDictionary.Add(node, id.type);
            }
        }
        public override void OutAIdExp(AIdExp node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (node.GetDot() != null)
            {
                if (id.type == Types.Vector)
                {
                    _typeDictionary.Add(node, Types.FloatingPoint);
                }
                else
                {
                    throw new TypeMismatchException(node, "symbols with . extensions have to be vectors");
                }
            }
            else
            {
                _typeDictionary.Add(node, id.type);
            }
        }

        public override void OutAFunctionExp(AFunctionExp node)
        {
            IList list = node.GetArgs();

            Symbol id = _symbolTable.GetFunctionSymbol(node.GetName().Text);
            _typeDictionary.Add(node, id.type);
            List<Types> formelList = id.GetParamTypes();
            if(list.Count != formelList.Count)
            {
                throw new TypeMismatchException(node, "Wrong number of arguments");
            }
            for (int i = 0; i < formelList.Count; i++)
            {
                if(Convert((Node)list[i], formelList[i]) != formelList[i]) 
                {
                    throw new TypeMismatchException(node, "Wrong type of argument");
                }
            }
        }


        public override void OutAParamDecl(AParamDecl node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            _typeDictionary.Add(node, id.type);
        }


        public override void InsideScopeInAFuncDecl(AFuncDecl node)
        {
            currentFunctionType = _symbolTable.GetFunctionSymbol(node.GetId().Text).type;

        }
        public override void InsideScopeInAProcDecl(AProcDecl node)
        {
            currentFunctionType = Types.Void;

        }

        public override void OutAReturnStmt(AReturnStmt node)
        {
            if (currentFunctionType != Convert(node.GetExp(), currentFunctionType))
            {
                throw new TypeMismatchException(node, "Wrong type of return statement");
            }
            else
            {
                _typeDictionary.Add(node, currentFunctionType);
            }

            guaranteeedToReturn.Add(node);
        }
        public override void InsideScopeOutAFuncDecl(AFuncDecl node)
        {
            Symbol id = _symbolTable.GetFunctionSymbol(node.GetId().Text);
            _typeDictionary.Add(node, id.type);

            if (!guaranteeedToReturn.Contains(node.GetBlock()))
            {
                throw new NotAllPathsReturnException(node, id.name);
            }
        }
        public override void InsideScopeOutAProcDecl(AProcDecl node)
        {
            Symbol id = _symbolTable.GetFunctionSymbol(node.GetId().Text);
            _typeDictionary.Add(node, id.type);
        }

        // blocks
        public override void OutsideScopeOutAStmtlistBlock(AStmtlistBlock node)
        {
            Object[] nodes = new Object[node.GetStmt().Count];
            node.GetStmt().CopyTo(nodes, 0);
            foreach (Object stmtNode in nodes)
            {
                if (guaranteeedToReturn.Contains((Node)stmtNode))
                {
                    guaranteeedToReturn.Add(node);
                    break;
                }
            }
        }

        public override void OutAWalkBlock(AWalkBlock node)
        {
            OutWalkBuildNoneblock(node, node.GetBlock());
        }
        public override void OutABuildBlock(ABuildBlock node)
        {
            OutWalkBuildNoneblock(node, node.GetBlock());
        }
        public override void OutANoneBlock(ANoneBlock node)
        {
            OutWalkBuildNoneblock(node, node.GetBlock());
        }
        private void OutWalkBuildNoneblock(Node node, Node childBlock)
        {
            if (guaranteeedToReturn.Contains(childBlock))
            {
                guaranteeedToReturn.Add(node);
            }

        }

    }
}
