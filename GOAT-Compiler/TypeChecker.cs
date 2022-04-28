using GOATCode.node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOAT_Compiler
{
    internal class TypeChecker : SymbolTableVisitor
    {
        private Dictionary<Node, Types> _typeDictionary = new Dictionary<Node, Types>();
        public TypeChecker(ISymbolTable symbolTable) : base(symbolTable)
        {
        }
        private void ExpressionTypeChecker(Node left, Node right, Node current)
        {
            Types leftType = _typeDictionary[left];
            Types rightType = _typeDictionary[right];
            _typeDictionary.Add(current, TypePromoter(leftType, rightType));
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
                throw new TypeMismatchException("Type mismatch");
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
                throw new TypeMismatchException("Type mismatch");
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
                throw new TypeMismatchException("Type mismatch");
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
                throw new TypeMismatchException("Type mismatch");
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
                throw new TypeMismatchException("Type mismatch");
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

        public override void OutAModuloExp(AModuloExp node)
        {
            Types left = _typeDictionary[node.GetL()];
            Types right = _typeDictionary[node.GetR()];
            if ((left == Types.Integer || left == Types.FloatingPoint) && (right == Types.Integer || right == Types.FloatingPoint))
            {
                ExpressionTypeChecker(node.GetL(), node.GetR(), node);
            }
            else
            {
                throw new TypeMismatchException("Type mismatch");
            }
        }

        public override void OutAMultExp(AMultExp node)
        {
            Types left = _typeDictionary[node.GetL()];
            Types right = _typeDictionary[node.GetR()];
            if ((left == Types.Integer || left == Types.FloatingPoint) && (right == Types.Integer || right == Types.FloatingPoint))
            {
                ExpressionTypeChecker(node.GetL(), node.GetR(), node);
            }
            else
            {
                throw new TypeMismatchException("Type mismatch");
            }
        }

        public override void OutADivdExp(ADivdExp node)
        {
            Types left = _typeDictionary[node.GetL()];
            Types right = _typeDictionary[node.GetR()];
            if ((left == Types.Integer || left == Types.FloatingPoint) && (right == Types.Integer || right == Types.FloatingPoint))
            {
                ExpressionTypeChecker(node.GetL(), node.GetR(), node);
            }
            else
            {
                throw new TypeMismatchException("Type mismatch");
            }
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
                throw new TypeMismatchException("Type mismatch");
            }
        }

        public override void OutAIfStmt(AIfStmt node)
        {
            if (_typeDictionary[node.GetExp()] != Types.Boolean)
            {
                throw new TypeMismatchException("Type mismatch");
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
                throw new TypeMismatchException("Type mismatch");
            }
            else
            {
                _typeDictionary.Add(node, Types.Void);
            }
        }

        public override void OutAAssignPlusStmt(AAssignPlusStmt node)
        {
            Symbol id = _symbolTable.GetSymbol(node.GetId().Text);
            _typeDictionary.Add(node, TypePromoter(id.type, _typeDictionary[node.GetExp()]));
        }
        public override void OutAAssignMinusStmt(AAssignMinusStmt node)
        {
            Symbol id = _symbolTable.GetSymbol(node.GetId().Text);
            _typeDictionary.Add(node, TypePromoter(id.type, _typeDictionary[node.GetExp()]));
        }

        public override void OutAAssignDivisionStmt(AAssignDivisionStmt node)
        {
            Symbol id = _symbolTable.GetSymbol(node.GetId().Text);
            _typeDictionary.Add(node, TypePromoter(id.type, _typeDictionary[node.GetExp()]));
        }

        public override void OutAAssignModStmt(AAssignModStmt node)
        {
            Symbol id = _symbolTable.GetSymbol(node.GetId().Text);
            _typeDictionary.Add(node, TypePromoter(id.type, _typeDictionary[node.GetExp()]));
        }

        public override void OutAAssignMultStmt(AAssignMultStmt node)
        {
            Symbol id = _symbolTable.GetSymbol(node.GetId().Text);
            _typeDictionary.Add(node, TypePromoter(id.type, _typeDictionary[node.GetExp()]));
        }
    }
}

        
