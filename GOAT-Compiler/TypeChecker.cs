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
        
        /// <summary>
        /// The constructor for the type checker.
        /// </summary>
        /// <param name="symbolTable">The symbol table</param>
        public TypeChecker(ISymbolTable symbolTable) : base(symbolTable)
        {
        }
        private void ExpressionTypeChecker(Node left, Node right, Node current)
        {
            Types leftType = _typeDictionary[left];
            Types rightType = _typeDictionary[right];
            Types type = TypePromoter(leftType, rightType);
            if (type == Types.Void) 
            {
                throw new TypeMismatchException(current);
            }
            else
            {
                _typeDictionary.Add(current, type);
            }            
        }
        private void EqelNotEqel(Node left, Node right, Node current)
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
                throw new TypeMismatchException(current);
            }
        }

        private void AndAndOr(Node nodeleft, Node nodeRight, Node current)
        {
            Types leftType = _typeDictionary[nodeleft];
            Types rightType = _typeDictionary[nodeRight];
            if (leftType == Types.Boolean && rightType == Types.Boolean)
            {
                _typeDictionary.Add(current, Types.Boolean);
            }
            else
            {
                throw new TypeMismatchException(current);
            }
        }
        private void GreaterThanLessThan(Node nodeleft, Node nodeRight, Node current)
        {
            Types leftType = _typeDictionary[nodeleft];
            Types rightType = _typeDictionary[nodeRight];
            if ((leftType != Types.Boolean && rightType != Types.Boolean) && (leftType != Types.Vector && rightType != Types.Vector))
            {
                _typeDictionary.Add(current, Types.Boolean);
            }
            else
            {
                throw new TypeMismatchException(current);
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
        private Types numberType(string numberToken)
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
                throw new TypeMismatchException(n);
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
            else
            {
                throw new TypeMismatchException(node);
            }
        }


        public override void OutANumberExp(ANumberExp node)
        {
            _typeDictionary.Add(node, numberType(node.GetNumber().Text));
        }

        public override void OutAPlusExp(APlusExp node)
        {
            ExpressionTypeChecker(node.GetL(), node.GetR(), node);
        }

        public override void OutAMinusExp(AMinusExp node)
        {
            ExpressionTypeChecker(node.GetL(), node.GetR(), node);
        }

        public override void OutABoolvalExp(ABoolvalExp node)
        {
            _typeDictionary.Add(node, Types.Boolean);
        }

        public override void OutAAndExp(AAndExp node)
        {
            AndAndOr(node.GetL(), node.GetR(), node);
        }
        public override void OutAOrExp(AOrExp node)
        {
            AndAndOr(node.GetL(), node.GetR(), node);
        }
        public override void OutAEqExp(AEqExp node)
        {
            EqelNotEqel(node.GetL(), node.GetR(), node);
        }

        private void MultDivMod(Node left, Node right, Node current)
        {
            Types leftType = _typeDictionary[left];
            Types rightType = _typeDictionary[right];
            if ((leftType == Types.Integer || leftType == Types.FloatingPoint) && (rightType == Types.Integer || rightType == Types.FloatingPoint))
            {
                ExpressionTypeChecker(left, right, current);
            }
            else
            {
                throw new TypeMismatchException(current);
            }
        }
        public override void OutAModuloExp(AModuloExp node)
        {
            MultDivMod(node.GetL(), node.GetL(), node);
        }

        public override void OutAMultExp(AMultExp node)
        {
            MultDivMod(node.GetL(), node.GetL(), node);
        }

        public override void OutADivdExp(ADivdExp node)
        {
            MultDivMod(node.GetL(), node.GetL(), node);
        }
        public override void OutAGeqExp(AGeqExp node)
        {
            GreaterThanLessThan(node.GetL(), node.GetR(), node);
        }

        public override void OutAGtExp(AGtExp node)
        {
            GreaterThanLessThan(node.GetL(), node.GetR(), node);
        }
        public override void OutALtExp(ALtExp node)
        {
            GreaterThanLessThan(node.GetL(), node.GetR(), node);
        }
        public override void OutALeqExp(ALeqExp node)
        {
            GreaterThanLessThan(node.GetL(), node.GetR(), node);
        }
        public override void OutANeqExp(ANeqExp node)
        {
            EqelNotEqel(node.GetL(), node.GetR(), node);
        }

        public override void OutAAssignStmt(AAssignStmt node)
        {
            Symbol id = _symbolTable.GetSymbol(node.GetId().Text);
            if (Convert(node.GetExp(), id.type) == id.type)
            {
                _typeDictionary.Add(node, id.type);
            }
            else
            {
                throw new TypeMismatchException(node);
            }
        }

        public override void OutAIfStmt(AIfStmt node)
        {
            if (_typeDictionary[node.GetExp()] != Types.Boolean)
            {
                throw new TypeMismatchException(node);
            }
            else
            {
                _typeDictionary.Add(node, Types.Void);
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
            Symbol id = _symbolTable.GetSymbol(node.GetId().Text);
            _typeDictionary.Add(node, Convert(node.GetExp(), id.type));
        }
        public override void OutAAssignMinusStmt(AAssignMinusStmt node)
        {
            Symbol id = _symbolTable.GetSymbol(node.GetId().Text);

            _typeDictionary.Add(node, Convert(node.GetExp(), id.type));
        }

        private void DivMult(Symbol id, Types type, Node node, Node expr)
        {
            if (id.type == Types.Vector && (type == Types.FloatingPoint || type == Types.Integer))
            {
                _typeDictionary.Add(node, id.type);
            }
            else
            {
                _typeDictionary.Add(node, Convert(expr, id.type));
            }
        }
        public override void OutAAssignDivisionStmt(AAssignDivisionStmt node)
        {
            Symbol id = _symbolTable.GetSymbol(node.GetId().Text);
            Types expType = _typeDictionary[node.GetExp()];
            DivMult(id, expType, node, node.GetExp());
        }
        public override void OutAAssignMultStmt(AAssignMultStmt node)
        {
            Symbol id = _symbolTable.GetSymbol(node.GetId().Text);
            Types expType = _typeDictionary[node.GetExp()];
            DivMult(id, expType, node, node.GetExp());
        }
        public override void OutAAssignModStmt(AAssignModStmt node)
        {
            Symbol id = _symbolTable.GetSymbol(node.GetId().Text);
            _typeDictionary.Add(node, Convert(node.GetExp(), id.type));
        }

        public override void OutANotExp(ANotExp node)
        {
            if (_typeDictionary[node.GetExp()] == Types.Boolean)
            {
                _typeDictionary.Add(node, Types.Boolean);
            }
            else
            {
                throw new TypeMismatchException(node);
            }
        }

        public override void OutAVarDecl(AVarDecl node)
        {
            Symbol id = _symbolTable.GetSymbol(node.GetId().Text);
            if(node.GetExp() != null)
            {
                if (Convert(node.GetExp(), id.type) == id.type)
                {
                    _typeDictionary.Add(node, id.type);
                }
                else
                {
                    throw new TypeMismatchException(node);
                }
            }
            else
            {
                _typeDictionary.Add(node, id.type);
            }
        }
        public override void OutAIdExp(AIdExp node)
        {
            Symbol id = _symbolTable.GetSymbol(node.GetId().Text);
            _typeDictionary.Add(node, id.type);
        }

        public override void OutAFunctionExp(AFunctionExp node)
        {
            IList list = node.GetArgs();

            Symbol id = _symbolTable.GetSymbol(node.GetName().Text);
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
                    throw new TypeMismatchException(node);
                }
            }
        }
        public override void InsideScopeInAFuncDecl(AFuncDecl node)
        {
            System.Collections.IList list = node.GetDecl();
            currentFunctionType = _symbolTable.GetSymbol(node.GetId().Text).type;

        }
        public override void OutAReturnStmt(AReturnStmt node)
        {
            if (currentFunctionType != Convert(node.GetExp(), currentFunctionType))
            {
                throw new TypeMismatchException(node);
            }
            else
            {
                _typeDictionary.Add(node, currentFunctionType);
            }
        }






        public override void InsideScopeOutAFuncDecl(AFuncDecl node)
        {
            Symbol id = _symbolTable.GetSymbol(node.GetId().Text);
            _typeDictionary.Add(node, id.type);
        }
        public override void InsideScopeOutAProcDecl(AProcDecl node)
        {
            Symbol id = _symbolTable.GetSymbol(node.GetId().Text);
            _typeDictionary.Add(node, id.type);
        }
    }
}
