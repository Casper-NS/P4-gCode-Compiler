using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using GOAT_Compiler;
using GOATCode.lexer;
using GOATCode.node;
using GOATCode.parser;
using VisitorTests;

namespace SymbolTableTest
{
    public class SymbolTableTest
    {

        [Theory]
        [InlineData("a", Types.Integer)]
        [InlineData("b", Types.Boolean)]
        [InlineData("v", Types.Vector)]
        public void Check_If_The_Type_Is_Correct_In_Symbol_Table(string value, Types type)
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope(new ANumberExp());
            symbolTable.AddVariableSymbol(value, type);
            Assert.Equal(type, symbolTable.GetVariableSymbol(value).Type);
            symbolTable.CloseScope();
        }

        [Theory]
        [InlineData("a", 2, Types.Integer)]
        [InlineData("b", 3, Types.Boolean)]
        public void Checks_Acces_After_Build(string symbolName, int depth, Types type)
        {
            List<ANumberExp> nodes = new();
            for (int i = 0; i <= depth; i++)
            {
                nodes.Add(new ANumberExp());
            }
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope(nodes[0]);
            for (int i = 1; i <= depth; i++)
            {
                symbolTable.OpenScope(nodes[i]);
            }
            symbolTable.AddVariableSymbol(symbolName, type);
            for (int i = 1; i <= depth; i++)
            {
                symbolTable.CloseScope();
            }
            symbolTable.CloseScope();
            symbolTable.OpenScope(nodes[0]);
            for (int i = 1; i <= depth; i++)
            {
                symbolTable.OpenScope(nodes[i]);
            }
            Assert.Contains(symbolName, symbolTable.GetVariableSymbol(symbolName).Name);
            for (int i = 1; i <= depth; i++)
            {
                symbolTable.CloseScope();
            }
            symbolTable.CloseScope();
        }

        [Theory]
        [InlineData("a", "a", Types.Boolean)]
        public void Check_Duplicate_Decl_In_SymbolTable(string firestSymbol, string secondSymbol, Types type)
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope(new ANumberExp());
            symbolTable.AddVariableSymbol(firestSymbol, type);
            Assert.Throws<ArgumentException>(() => symbolTable.AddVariableSymbol(secondSymbol, type));
            symbolTable.CloseScope();
        }

        [Fact]
        public void Check_If_The_Symbol_Is_In_SymbolTable()
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope(new ANumberExp());
            Assert.Null(symbolTable.GetVariableSymbol("a"));
            symbolTable.CloseScope();
        }

        [Fact]
        public void Check_Parameters_Types_in_func()
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope(new ANumberExp());
            symbolTable.AddFunctionSymbol(new ANumberExp(), "testfunc", Types.Void, Types.Boolean, Types.FloatingPoint, Types.Integer, Types.Vector);
            List<Types> paramTypes = symbolTable.GetFunctionSymbol("testfunc").ParamTypes;
            Assert.Equal(Types.Boolean, paramTypes[0]);
            Assert.Equal(Types.FloatingPoint, paramTypes[1]);
            Assert.Equal(Types.Integer, paramTypes[2]);
            Assert.Equal(Types.Vector, paramTypes[3]);
            symbolTable.CloseScope();
        }

        [Fact]
        public void Check_Outer_Scope_Access()
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope(new ANumberExp());
            symbolTable.AddVariableSymbol("a", Types.Integer);
            symbolTable.OpenScope(new ANumberExp());
            Assert.NotNull(symbolTable.GetVariableSymbol("a"));
            symbolTable.CloseScope();
            symbolTable.CloseScope();
        }

        [Fact]
        public void Check_If_GetSymbol_Returns_Null()
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope(new ANumberExp());
            symbolTable.OpenScope(new ANumberExp());
            symbolTable.AddVariableSymbol("a", Types.Integer);
            symbolTable.CloseScope();
            Assert.Null(symbolTable.GetVariableSymbol("a"));
            symbolTable.CloseScope();
        }

        [Fact]
        public void Checks_The_Right_Symbol_Gets_Returned_When_In_A_Scope()
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope(new ANumberExp());
            symbolTable.AddVariableSymbol("a", Types.Integer);
            symbolTable.OpenScope(new ANumberExp());
            symbolTable.AddVariableSymbol("a", Types.Boolean);
            Assert.Equal(Types.Boolean, symbolTable.GetVariableSymbol("a").Type);
            symbolTable.CloseScope();
            Assert.Equal(Types.Integer, symbolTable.GetVariableSymbol("a").Type);
            symbolTable.CloseScope();
        }
    }
}
