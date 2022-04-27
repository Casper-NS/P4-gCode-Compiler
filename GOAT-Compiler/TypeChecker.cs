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
            if (Convert(left, rightType) != Convert(right, leftType))
            {
                throw new Exception("Type mismatch");
            }
            _typeDictionary.Add(current, leftType);
        }
        public Types numberType(string numberToken)
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

        private Types Convert(Node n, Types t)
        {
            if(_typeDictionary[n] == t)
            {
                return t;
            }
            else if (_typeDictionary[n] == Types.Integer && t == Types.FloatingPoint)
            {
                return Types.FloatingPoint;
            }
            else
            {
                throw new Exception("Type mismatch");
            }
        }
        
        public override void OutABoolvalExp(ABoolvalExp node)
        {
            Types type = _symbolTable.GetSymbol(node.GetBoolValue().Text).type;
            if (type != Types.Boolean)
            {
                throw new Exception("Type mismatch");
            }
            _typeDictionary.Add(node, type);
        }

        public override void OutAAndExp(AAndExp node)
        {
            ExpressionTypeChecker(node.GetL(), node.GetR(), node);
        }

        public override void OutAEqExp(AEqExp node)
        {
            ExpressionTypeChecker(node.GetL(), node.GetR(), node);
        }

        public override void OutAModuloExp(AModuloExp node)
        {
            ExpressionTypeChecker(node.GetL(), node.GetR(), node);
        }

        public override void OutAMultExp(AMultExp node)
        {
            ExpressionTypeChecker(node.GetL(), node.GetR(), node);
        }

        public override void OutADivdExp(ADivdExp node)
        {
            ExpressionTypeChecker(node.GetL(), node.GetR(), node);
        }

        public override void OutAOrExp(AOrExp node)
        {
            ExpressionTypeChecker(node.GetL(), node.GetR(), node);
        }

        public override void OutAGeqExp(AGeqExp node)
        {
            ExpressionTypeChecker(node.GetL(), node.GetR(), node);
        }

        public override void OutAGtExp(AGtExp node)
        {
            ExpressionTypeChecker(node.GetL(), node.GetR(), node);
        }
        public override void OutALtExp(ALtExp node)
        {
            ExpressionTypeChecker(node.GetL(), node.GetR(), node);
        }
        public override void OutALeqExp(ALeqExp node)
        {
            ExpressionTypeChecker(node.GetL(), node.GetR(), node);
        }
        public override void OutANeqExp(ANeqExp node)
        {
            ExpressionTypeChecker(node.GetL(), node.GetR(), node);
        }
    }
}
