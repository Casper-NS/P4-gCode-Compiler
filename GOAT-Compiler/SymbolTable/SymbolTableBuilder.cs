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
        /// This contains the types of the formal parameters by position, the first element is the type of the first formal param..
        /// </summary>
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
        private Types ProcessTypeOfNode(PTypes node)
        {
            switch (node)
            {
                case AIntTypes:
                    return Types.Integer;
                case AFloatTypes:
                    return Types.FloatingPoint;
                case AVectorTypes:
                    return Types.Vector;
                case ABoolTypes:
                    return Types.Boolean;
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
        /// <summary>
        /// Adds a function symbol to the symboltable.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name">The id of the functions</param>
        /// <param name="type">The returntype of the function</param>
        /// <param name="types">The types of the formal parameters</param>
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
    }
}