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

        public override void OutAPlusExp(APlusExp node)
        {
            Types left = _typeDictionary[node.GetL()];
            Types right = _typeDictionary[node.GetR()];
            if (left != right)
            {
                throw new Exception("Type mismatch");
            }
        }
        public override void OutANumberExp(ANumberExp node)
        {
            _typeDictionary.Add(node, numberType(node.GetNumber().Text));
        }

        public override void OutAMinusExp(AMinusExp node)
        {
            Types left = _typeDictionary[node.GetL()];
            Types right = _typeDictionary[node.GetR()];
            if (Convert(node.GetL(), right) != Convert(node.GetR(), left))
            {
                throw new Exception("Type mismatch");
            }
            _typeDictionary.Add(node, left);
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
    }
}
