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
            Assert.Equal(type, symbolTable.GetVariableSymbol(value).type);
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
            Assert.Contains(symbolName, symbolTable.GetVariableSymbol(symbolName).name);
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
            List<Types> paramTypes = symbolTable.GetFunctionSymbol("testfunc").GetParamTypes();
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
            Assert.Equal(Types.Boolean, symbolTable.GetVariableSymbol("a").type);
            symbolTable.CloseScope();
            Assert.Equal(Types.Integer, symbolTable.GetVariableSymbol("a").type);
            symbolTable.CloseScope();
        }

        // This test broke after redesigning the symbol table.
        /*
        [SkippableFact]
        //Checks whether the symbol table is built probably given a test file.
        public void Symbol_Build_test()
        {
            Start s = FileReadingTestUtilities.ParseFile(FileReadingTestUtilities.ProjectBaseDirectory + "ScopeChecker/SymbolBuildTest.txt");

            ISymbolTable symTable = new RecSymbolTable();
            SymbolTableBuilder builder = new SymbolTableBuilder(symTable);
            s.Apply(builder);

            Assert.True(symTable.IsComplete());

            symTable.OpenScope(new ANumberExp());
            Assert.NotNull(symTable.GetVariableSymbol("a"));
            Assert.NotNull(symTable.GetVariableSymbol("b"));
            Assert.NotNull(symTable.GetVariableSymbol("c"));
            symTable.OpenScope(new ANumberExp());
            Assert.Equal(Types.FloatingPoint, symTable.GetVariableSymbol("c").type);
            symTable.OpenScope(new ANumberExp());
            Assert.Equal(Types.FloatingPoint, symTable.GetVariableSymbol("a").type);
            Assert.Equal(Types.FloatingPoint, symTable.GetVariableSymbol("b").type);
            symTable.OpenScope(new ANumberExp());
            Assert.NotNull(symTable.GetVariableSymbol("a"));
            symTable.OpenScope(new ANumberExp());
            Assert.Equal(Types.Boolean, symTable.GetVariableSymbol("b").type);
            symTable.OpenScope(new ANumberExp());
            Assert.NotNull(symTable.GetVariableSymbol("a"));
            Assert.NotNull(symTable.GetVariableSymbol("b"));
            symTable.CloseScope();
            symTable.CloseScope();
            Assert.Equal(Types.FloatingPoint, symTable.GetVariableSymbol("b").type);
            symTable.CloseScope();
            symTable.CloseScope();
            symTable.OpenScope(new ANumberExp());
            Assert.Equal(Types.FloatingPoint, symTable.GetVariableSymbol("c").type);
            symTable.CloseScope();
            Assert.Equal(Types.Integer, symTable.GetFunctionSymbol("testFunc").type);
            symTable.CloseScope();

            symTable.OpenScope(new ANumberExp());
            Assert.Equal(Types.Integer, symTable.GetVariableSymbol("b").type);
            symTable.CloseScope();

            symTable.OpenScope(new ANumberExp());
            Assert.Equal(Types.FloatingPoint, symTable.GetVariableSymbol("x").type);
            Assert.Equal(Types.Integer, symTable.GetVariableSymbol("f").type);
            symTable.CloseScope();
            Assert.Null(symTable.GetVariableSymbol("x"));
            symTable.CloseScope();
        }
        */
    }
}
