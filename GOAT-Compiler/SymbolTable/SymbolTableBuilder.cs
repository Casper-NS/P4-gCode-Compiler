using GOATCode.analysis;
using GOATCode.node;
using System;
using System.Collections.Generic;
using GOAT_Compiler.Exceptions;

namespace GOAT_Compiler
{
    internal class SymbolTableBuilder : SymbolTableVisitor
    {
        /// <summary>
        /// The dictionary that stores the name of the type, that a variable or function declaration, is stored.
        /// </summary>
        private readonly Dictionary<Node, string> _typeTable = new Dictionary<Node, string>();


        private readonly List<Types> paramTypesList = new List<Types>();

        /// <summary>
        /// The constructor for the SymbolTableBuilder
        /// </summary>
        /// <param name="IST">The instance of the symboltable</param>
        internal SymbolTableBuilder(ISymbolTable IST) : base(IST)
        {
        }

        /// <summary>
        /// Method used to find the right Type for decleration nodes, using strings with the same name.
        /// </summary>
        /// <param name="node">The Node used as key in typetable</param>
        /// <returns>Returns the right enum Type for the given node</returns>
        /// <exception cref="TypeAccessException">The exception that is thrown if an invalid string is inputted</exception>
        private Types ProcessTypeOfNode(Node node)
        {
            switch (_typeTable[node])
            {
                case "int":
                    return Types.Integer;
                case "float":
                    return Types.FloatingPoint;
                case "vector":
                    return Types.Vector;
                case "bool":
                    return Types.Boolean;
                case "void":
                    return Types.Void;
                default:
                    throw new TypeAccessException();
            }
        }

        public override void OutAVarDecl(AVarDecl node) => AddVariableSymbol(node, node.GetId().Text, ProcessTypeOfNode(node.GetTypes()));

        public override void OutAParamDecl(AParamDecl node)
        {
            Types type = ProcessTypeOfNode(node.GetTypes());
            AddVariableSymbol(node, node.GetId().Text, type);
            paramTypesList.Add(type);
        }
        private void AddVariableSymbol(Node node, string name, Types type)
        {
            try
            {
                _symbolTable.AddVariableSymbol(name, type);
            }
            catch (ArgumentException)
            {
                throw new VariableAlreadyDefinedException(node, name);
            }
        }

        public override void OutsideScopeOutAFuncDecl(AFuncDecl node)
        {
            AddFunctionSymbol(node, node.GetId().Text, ProcessTypeOfNode(node.GetTypes()), paramTypesList.ToArray());
            paramTypesList.Clear();
        }

        public override void OutsideScopeOutAProcDecl(AProcDecl node)
        {
            AddFunctionSymbol(node, node.GetId().Text, Types.Void, paramTypesList.ToArray());
            paramTypesList.Clear();
        }

        private void AddFunctionSymbol(Node node, string name, Types type, params Types[] types)
        {
            try
            {
                _symbolTable.AddFunctionSymbol(node, name, type, types);
            }
            catch (ArgumentException)
            {
                throw new FunctionAlreadyDefinedException(node, name);
            }
        }

        /// <summary>
        /// Adds a string with the name of the right type to typeTable.
        /// </summary>
        /// <param name="node">The given node being anlysed</param>
        public override void OutAIntTypes(AIntTypes node) => _typeTable.Add(node, "int");

        /// <summary>
        /// Adds a string with the name of the right type to typeTable.
        /// </summary>
        /// <param name="node">The given node being anlysed</param>
        public override void OutAFloatTypes(AFloatTypes node) => _typeTable.Add(node, "float");

        /// <summary>
        /// Adds a string with the name of the right type to typeTable.
        /// </summary>
        /// <param name="node">The given node being anlysed</param>
        public override void OutABoolTypes(ABoolTypes node) => _typeTable.Add(node, "bool");

        /// <summary>
        /// Adds a string with the name of the right type to typeTable. 
        /// </summary>
        /// <param name="node">The given node being anlysed</param>
        public override void OutAVectorTypes(AVectorTypes node) => _typeTable.Add(node, "vector");
    }
}