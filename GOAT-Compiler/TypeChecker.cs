using GOAT_Compiler.Exceptions;
using GOATCode.node;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GOAT_Compiler
{
    /// <summary>
    /// The type checker class for the GOAT Code.
    /// </summary>
    internal class TypeChecker : SymbolTableVisitor
    {
        private Types currentFunctionType;
        public Dictionary<Node, Types> TypeDictionary { get; } = new Dictionary<Node, Types>();

        /// <summary>
        /// Contains the nodes that are guaranteed to return. Like and if where both the then and the else blocks return.
        /// </summary>
        private readonly HashSet<Node> guaranteedToReturn = new HashSet<Node>();

        /// <summary>
        /// The constructor for the type checker.
        /// </summary>
        /// <param name="symbolTable">The symbol table</param>
        public TypeChecker(ISymbolTable symbolTable) : base(symbolTable)
        {
        }

        private void ArithmeticTypeChecker(Node left, Node right, Node current)
        {
            Types leftType = TypeDictionary[left];
            Types rightType = TypeDictionary[right];
            Types type = TypePromoter(leftType, rightType);
            if (type == Types.Void || type == Types.Boolean)
            {
                throw new TypeMismatchException(current, "Type " + type + " is not valid for this arithmetic operations.");
            }
            else
            {
                TypeDictionary.Add(current, type);
            }
        }

        /// <summary>
        /// Typechecker for != and == operators.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="current"></param>
        private void EqualAndNotEqualTypeChecker(Node left, Node right, Node current)
        {
            Types leftType = TypeDictionary[left];
            Types rightType = TypeDictionary[right];

            if (leftType == Types.Integer && rightType == Types.FloatingPoint)
            {
                TypeDictionary.Add(current, Types.Boolean);
            }
            else if (leftType == Types.FloatingPoint && rightType == Types.Integer)
            {
                TypeDictionary.Add(current, Types.Boolean);
            }
            else if ((leftType == rightType) && (leftType != Types.Void) && (rightType != Types.Void))
            {
                TypeDictionary.Add(current, Types.Boolean);
            }
            else
            {
                throw new TypeMismatchException(current, "Type " + leftType + " and " + rightType + " are not valid for this comparison operations.");
            }
        }

        /// <summary>
        /// Typechecker for && and || operators.
        /// </summary>
        /// <param name="nodeleft"></param>
        /// <param name="nodeRight"></param>
        /// <param name="current"></param>
        private void AndAndOrTypeChecker(Node nodeleft, Node nodeRight, Node current)
        {
            Types leftType = TypeDictionary[nodeleft];
            Types rightType = TypeDictionary[nodeRight];
            if (leftType == Types.Boolean && rightType == Types.Boolean)
            {
                TypeDictionary.Add(current, Types.Boolean);
            }
            else
            {
                throw new TypeMismatchException(current, "Type " + leftType + " and " + rightType + " are not valid for this logical operation.");
            }
        }

        private void GreaterThanLessThanTypeChecker(Node nodeleft, Node nodeRight, Node current)
        {
            Types leftType = TypeDictionary[nodeleft];
            Types rightType = TypeDictionary[nodeRight];
            if (TypePromoter(leftType, rightType) == Types.Integer || TypePromoter(leftType, rightType) == Types.FloatingPoint)
            {
                TypeDictionary.Add(current, Types.Boolean);
            }
            else
            {
                throw new TypeMismatchException(current, "Type " + leftType + " and " + rightType + " are not valid for this comparison operations.");
            }
        }

        private void ModuloTypeChecker(Node left, Node right, Node current)
        {
            Types leftType = TypeDictionary[left];
            Types rightType = TypeDictionary[right];
            Types type = TypePromoter(leftType, rightType);
            if (type == Types.Integer || type == Types.FloatingPoint)
            {
                TypeDictionary.Add(current, type);
            }
            else
            {
                throw new TypeMismatchException(current, "Type " + leftType + " and " + rightType + " are not valid for this arithemtic operations.");
            }
        }

        private void DivTypeChecker(Node left, Node right, Node current)
        {
            Types leftType = TypeDictionary[left];
            Types rightType = TypeDictionary[right];
            Types type = TypePromoter(leftType, rightType);
            if (type == Types.FloatingPoint)
            {
                TypeDictionary.Add(current, Types.FloatingPoint);
            }
            else if (type == Types.Integer)
            {
                TypeDictionary.Add(current, Types.Integer);
            }
            else if (leftType == Types.Vector && (rightType == Types.Integer || rightType == Types.FloatingPoint))
            {
                TypeDictionary.Add(current, Types.Vector);
            }
            else
            {
                throw new TypeMismatchException(current, "Type " + leftType + " and " + rightType + " are not valid for this arithemtic operations.");
            }
        }

        private void MultTypeChecker(Node left, Node right, Node current)
        {
            Types leftType = TypeDictionary[left];
            Types rightType = TypeDictionary[right];
            Types type = TypePromoter(leftType, rightType);
            if (type == Types.Integer)
            {
                TypeDictionary.Add(current, Types.Integer);
            }
            else if (type == Types.FloatingPoint)
            {
                TypeDictionary.Add(current, Types.FloatingPoint);
            }
            // Both sides cannot be combinations of int or float, since they have been caught in the previous cases
            // So it is guaranteed that a vector is involved.
            else if ((leftType == Types.Integer || leftType == Types.FloatingPoint || leftType == Types.Vector)
                   && (rightType == Types.Integer || rightType == Types.FloatingPoint || rightType == Types.Vector))
            {
                TypeDictionary.Add(current, Types.Vector);
            }
            else
            {
                throw new TypeMismatchException(current, "Type " + leftType + " and " + rightType + " are not valid for this arithemtic operations.");
            }
        }

        private void CompoundAssignmentDivAndMultTypeChecker(Symbol id, Types type, Node node, Node expr, TDot dot)
        {
            if (dot != null)
            {
                if (id.Type != Types.Vector)
                {
                    throw new TypeMismatchException(node, "symbols with.extensions have to be vectors");
                }
                if (type == Types.FloatingPoint || type == Types.Integer)
                {
                    TypeDictionary.Add(node, Types.FloatingPoint);
                }
                else
                {
                    throw new TypeMismatchException(node, "Types not compatible with multiply or divide expression");
                }
            }
            else if (id.Type == Types.Vector && (type == Types.FloatingPoint || type == Types.Integer))
            {
                TypeDictionary.Add(node, id.Type);
            }
            else if (id.Type != Types.Vector && id.Type != Types.Boolean && id.Type != Types.Void)
            {
                TypeDictionary.Add(node, Convert(expr, id.Type));
            }
            else
            {
                throw new TypeMismatchException(node, "Types not compatible with multiply or divide expression");
            }
        }

        private static Types TypePromoter(Types t1, Types t2)
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

        private static Types NumberType(string numberToken)
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
            if (TypeDictionary[n] == t)
            {
                return t;
            }
            else if (TypeDictionary[n] == Types.Integer && t == Types.FloatingPoint)
            {
                return Types.FloatingPoint;
            }
            else
            {
                throw new TypeMismatchException(n, "Type " + TypeDictionary[n] + " cannot be converted to the type " + t);
            }
        }

        public override void OutAVectorExp(AVectorExp node)
        {
            if (Convert(node.GetX(), Types.FloatingPoint) == Types.FloatingPoint
                && Convert(node.GetY(), Types.FloatingPoint) == Types.FloatingPoint
                && Convert(node.GetZ(), Types.FloatingPoint) == Types.FloatingPoint)
            {
                TypeDictionary.Add(node, Types.Vector);
            }
        }

        public override void OutANumberExp(ANumberExp node) => TypeDictionary.Add(node, NumberType(node.GetNumber().Text));

        public override void OutAPlusExp(APlusExp node) => ArithmeticTypeChecker(node.GetL(), node.GetR(), node);

        public override void OutAMinusExp(AMinusExp node) => ArithmeticTypeChecker(node.GetL(), node.GetR(), node);

        public override void OutANegExp(ANegExp node)
        {
            Types type = TypeDictionary[node.GetExp()];
            if (type == Types.Boolean)
            {
                throw new TypeMismatchException(node, "Tried to negate a boolean");
            }
            TypeDictionary.Add(node, type);
        }

        public override void OutABoolvalExp(ABoolvalExp node) => TypeDictionary.Add(node, Types.Boolean);

        public override void OutAAndExp(AAndExp node) => AndAndOrTypeChecker(node.GetL(), node.GetR(), node);

        public override void OutAOrExp(AOrExp node) => AndAndOrTypeChecker(node.GetL(), node.GetR(), node);

        public override void OutAEqExp(AEqExp node) => EqualAndNotEqualTypeChecker(node.GetL(), node.GetR(), node);

        public override void OutAModuloExp(AModuloExp node) => ModuloTypeChecker(node.GetL(), node.GetR(), node);

        public override void OutAMultExp(AMultExp node) => MultTypeChecker(node.GetL(), node.GetR(), node);

        public override void OutADivdExp(ADivdExp node) => DivTypeChecker(node.GetL(), node.GetR(), node);

        public override void OutAGeqExp(AGeqExp node) => GreaterThanLessThanTypeChecker(node.GetL(), node.GetR(), node);

        public override void OutAGtExp(AGtExp node) => GreaterThanLessThanTypeChecker(node.GetL(), node.GetR(), node);

        public override void OutALtExp(ALtExp node) => GreaterThanLessThanTypeChecker(node.GetL(), node.GetR(), node);

        public override void OutALeqExp(ALeqExp node) => GreaterThanLessThanTypeChecker(node.GetL(), node.GetR(), node);

        public override void OutANeqExp(ANeqExp node) => EqualAndNotEqualTypeChecker(node.GetL(), node.GetR(), node);

        private void CheckDot(Node node, PExp expr, Symbol symbol, TDot dot)
        {
            if (dot != null)
            {
                if (symbol.Type == Types.Vector)
                {
                    TypeDictionary.Add(node, Convert(expr, Types.FloatingPoint));
                }
                else
                {
                    throw new TypeMismatchException(node, "symbols with . extensions have to be vectors");
                }
            }
            else
            {
                TypeDictionary.Add(node, Convert(expr, symbol.Type));
            }
        }

        public override void OutAAssignStmt(AAssignStmt node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            CheckDot(node, node.GetExp(), id, node.GetDot());
        }

        public override void OutAIfStmt(AIfStmt node)
        {
            if (TypeDictionary[node.GetExp()] != Types.Boolean)
            {
                throw new TypeMismatchException(node, "If statement must have a boolean expression");
            }
            else
            {
                TypeDictionary.Add(node, Types.Void);
            }

            if (guaranteedToReturn.Contains(node.GetThen()) && (node.GetElse() == null || guaranteedToReturn.Contains(node.GetElse())))
            {
                guaranteedToReturn.Add(node);
            }
        }

        public override void OutAWhileStmt(AWhileStmt node)
        {
            if (TypeDictionary[node.GetExp()] != Types.Boolean)
            {
                throw new TypeMismatchException(node);
            }
        }

        public override void OutARepeatStmt(ARepeatStmt node)
        {
            if (TypeDictionary[node.GetExp()] != Types.Integer)
            {
                throw new TypeMismatchException(node);
            }
        }

        public override void OutAAssignPlusStmt(AAssignPlusStmt node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (node.GetDot() != null)
            {
                if (id.Type == Types.Vector)
                {
                    TypeDictionary.Add(node, Convert(node.GetExp(), Types.FloatingPoint));
                }
                else
                {
                    throw new TypeMismatchException(node, "symbols with . extensions have to be vectors");
                }
            }
            else if (id.Type != Types.Boolean && id.Type != Types.Void)
            {
                TypeDictionary.Add(node, Convert(node.GetExp(), id.Type));
            }
            else
            {
                throw new TypeMismatchException(node, "Cannot assign to a boolean or void");
            }
        }

        public override void OutAAssignMinusStmt(AAssignMinusStmt node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (node.GetDot() != null)
            {
                if (id.Type == Types.Vector)
                {
                    TypeDictionary.Add(node, Convert(node.GetExp(), Types.FloatingPoint));
                }
                else
                {
                    throw new TypeMismatchException(node, "symbols with . extensions have to be vectors");
                }
            }
            else if (id.Type != Types.Boolean && id.Type != Types.Void)
            {
                TypeDictionary.Add(node, Convert(node.GetExp(), id.Type));
            }
            else
            {
                throw new TypeMismatchException(node, "Cannot assign to a boolean or void");
            }
        }

        public override void OutAAssignDivisionStmt(AAssignDivisionStmt node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            Types expType = TypeDictionary[node.GetExp()];
            CompoundAssignmentDivAndMultTypeChecker(id, expType, node, node.GetExp(), node.GetDot());
        }

        public override void OutAAssignMultStmt(AAssignMultStmt node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            Types expType = TypeDictionary[node.GetExp()];
            CompoundAssignmentDivAndMultTypeChecker(id, expType, node, node.GetExp(), node.GetDot());
        }

        public override void OutAAssignModStmt(AAssignModStmt node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (node.GetDot() != null)
            {
                if (id.Type == Types.Vector)
                {
                    TypeDictionary.Add(node, Convert(node.GetExp(), Types.FloatingPoint));
                }
                else
                {
                    throw new TypeMismatchException(node, "symbols with . extensions have to be vectors");
                }
            }
            else if (id.Type == Types.Integer && TypeDictionary[node.GetExp()] == Types.Integer)
            {
                TypeDictionary.Add(node, Types.Integer);
            }
            else if (id.Type == Types.FloatingPoint && TypeDictionary[node.GetExp()] == Types.FloatingPoint)
            {
                TypeDictionary.Add(node, Types.FloatingPoint);
            }
            else if (id.Type == Types.FloatingPoint && TypeDictionary[node.GetExp()] == Types.Integer)
            {
                TypeDictionary.Add(node, Types.FloatingPoint);
            }
            else
            {
                throw new TypeMismatchException(node, "Types not compatible with multiply or divide expression");
            }
        }

        public override void OutANotExp(ANotExp node)
        {
            if (TypeDictionary[node.GetExp()] == Types.Boolean)
            {
                TypeDictionary.Add(node, Types.Boolean);
            }
            else
            {
                throw new TypeMismatchException(node, "Not operator can only be used on booleans");
            }
        }

        public override void OutAVarDecl(AVarDecl node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (node.GetExp() != null)
            {
                if (Convert(node.GetExp(), id.Type) == id.Type)
                {
                    TypeDictionary.Add(node, id.Type);
                }
            }
            else
            {
                TypeDictionary.Add(node, id.Type);
            }
        }

        public override void OutAIdExp(AIdExp node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            if (node.GetDot() != null)
            {
                if (id.Type == Types.Vector)
                {
                    TypeDictionary.Add(node, Types.FloatingPoint);
                }
                else
                {
                    throw new TypeMismatchException(node, "symbols with . extensions have to be vectors");
                }
            }
            else
            {
                TypeDictionary.Add(node, id.Type);
            }
        }

        public override void OutAFunctionExp(AFunctionExp node)
        {
            IList list = node.GetArgs();

            Symbol id = _symbolTable.GetFunctionSymbol(node.GetName().Text);
            TypeDictionary.Add(node, id.Type);
            List<Types> formelList = id.ParamTypes;
            if (list.Count != formelList.Count)
            {
                throw new TypeMismatchException(node, "Wrong number of arguments");
            }
            for (int i = 0; i < formelList.Count; i++)
            {
                if (Convert((Node)list[i], formelList[i]) != formelList[i])
                {
                    throw new TypeMismatchException(node, "Wrong type of argument");
                }
            }
        }

        public override void OutAParamDecl(AParamDecl node)
        {
            Symbol id = _symbolTable.GetVariableSymbol(node.GetId().Text);
            TypeDictionary.Add(node, id.Type);
        }

        public override void InsideScopeInAFuncDecl(AFuncDecl node) => currentFunctionType = _symbolTable.GetFunctionSymbol(node.GetId().Text).Type;

        public override void InsideScopeInAProcDecl(AProcDecl node) => currentFunctionType = Types.Void;

        public override void OutAReturnStmt(AReturnStmt node)
        {
            if (currentFunctionType != Convert(node.GetExp(), currentFunctionType))
            {
                throw new TypeMismatchException(node, "Wrong type of return statement");
            }
            else
            {
                TypeDictionary.Add(node, currentFunctionType);
            }

            guaranteedToReturn.Add(node);
        }

        public override void InsideScopeOutAFuncDecl(AFuncDecl node)
        {
            Symbol id = _symbolTable.GetFunctionSymbol(node.GetId().Text);
            TypeDictionary.Add(node, id.Type);

            if (!guaranteedToReturn.Contains(node.GetBlock()))
            {
                throw new NotAllPathsReturnException(node, id.Name);
            }
        }

        public override void InsideScopeOutAProcDecl(AProcDecl node)
        {
            Symbol id = _symbolTable.GetFunctionSymbol(node.GetId().Text);
            TypeDictionary.Add(node, id.Type);
        }

        // blocks
        public override void OutsideScopeOutAStmtlistBlock(AStmtlistBlock node)
        {
            Object[] nodes = new Object[node.GetStmt().Count];
            node.GetStmt().CopyTo(nodes, 0);
            foreach (Object stmtNode in nodes)
            {
                if (guaranteedToReturn.Contains((Node)stmtNode))
                {
                    guaranteedToReturn.Add(node);
                    break;
                }
            }
        }

        public override void OutAWalkBlock(AWalkBlock node) => OutWalkBuildNoneblock(node, node.GetBlock());

        public override void OutABuildBlock(ABuildBlock node) => OutWalkBuildNoneblock(node, node.GetBlock());

        public override void OutANoneBlock(ANoneBlock node) => OutWalkBuildNoneblock(node, node.GetBlock());

        private void OutWalkBuildNoneblock(Node node, Node childBlock)
        {
            if (guaranteedToReturn.Contains(childBlock))
            {
                guaranteedToReturn.Add(node);
            }
        }
    }
}