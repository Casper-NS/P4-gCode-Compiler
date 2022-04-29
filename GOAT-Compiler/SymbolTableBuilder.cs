﻿using GOATCode.analysis;
using GOATCode.node;
using System;
using System.Collections.Generic;

namespace GOAT_Compiler
{
    internal class SymbolTableBuilder : SymbolTableVisitor
    {
        

        /// <summary>
        /// The dictionary that stores the name of the type, that a variable or function declaration, is stored.
        /// </summary>
        private Dictionary<Node, string> _typeTable = new Dictionary<Node, string>();


        private List<Types> paramTypesList = new List<Types>();

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
        private Types _processTypeOfNode(Node node)
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

        public override void OutAVarDecl(AVarDecl node)
        {
            _symbolTable.AddVariableSymbol(node.GetId().Text, _processTypeOfNode(node.GetTypes()));
        }

        public override void OutAParamDecl(AParamDecl node)
        {
            Types type = _processTypeOfNode(node.GetTypes());
            _symbolTable.AddVariableSymbol(node.GetId().Text, type);
            paramTypesList.Add(type);
        }


        public override void OutsideScopeOutAFuncDecl(AFuncDecl node)
        {
            _symbolTable.AddFunctionSymbol(node.GetId().Text, _processTypeOfNode(node.GetTypes()), paramTypesList.ToArray());
            paramTypesList.Clear();
        }

        public override void OutsideScopeOutAProcDecl(AProcDecl node)
        {
            _symbolTable.AddFunctionSymbol(node.GetId().Text, Types.Void, paramTypesList.ToArray());
            paramTypesList.Clear();
        }

        /// <summary>
        /// Adds a string with the name of the right type to typeTable.
        /// </summary>
        /// <param name="node">The given node being anlysed</param>
        public override void OutAIntTypes(AIntTypes node) 
        {
            _typeTable.Add(node, "int");
        }

        /// <summary>
        /// Adds a string with the name of the right type to typeTable.
        /// </summary>
        /// <param name="node">The given node being anlysed</param>
        public override void OutAFloatTypes(AFloatTypes node)
        {
            _typeTable.Add(node, "float");
        }

        /// <summary>
        /// Adds a string with the name of the right type to typeTable.
        /// </summary>
        /// <param name="node">The given node being anlysed</param>
        public override void OutABoolTypes(ABoolTypes node) 
        {
            _typeTable.Add(node, "bool");
        }

        /// <summary>
        /// Adds a string with the name of the right type to typeTable. 
        /// </summary>
        /// <param name="node">The given node being anlysed</param>
        public override void OutAVectorTypes(AVectorTypes node)
        {
            _typeTable.Add(node, "vector");
        }
    }
}