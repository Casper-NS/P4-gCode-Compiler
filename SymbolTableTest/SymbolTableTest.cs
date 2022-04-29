using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using GOAT_Compiler;
using GOATCode.lexer;
using GOATCode.node;
using GOATCode.parser;

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
            symbolTable.OpenScope();
            symbolTable.AddSymbol(value, type);
            Assert.Equal(type, symbolTable.GetSymbol(value).type);
            symbolTable.CloseScope();
        }

        [Theory]
        [InlineData("a", 2, Types.Integer)]
        [InlineData("b", 3, Types.Boolean)]
        public void Checks_Acces_After_Build(string symbolName, int depth, Types type)
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope();
            for (int i = 1; i <= depth; i++)
            {
                symbolTable.OpenScope();
            }
            symbolTable.AddSymbol(symbolName, type);
            for (int i = 1; i <= depth; i++)
            {
                symbolTable.CloseScope();
            }
            symbolTable.CloseScope();
            symbolTable.OpenScope();
            for (int i = 1; i <= depth; i++)
            {
                symbolTable.OpenScope();
            }
            Assert.Contains(symbolName, symbolTable.GetSymbol(symbolName).name);
            for (int i = 1; i <= depth; i++)
            {
                symbolTable.CloseScope();
            }
            symbolTable.CloseScope();
        }

        [Theory]
        [InlineData("a", "a", Types.Boolean)]
        public void Check_Doublicate_Decl_In_SymbolTable(string firestSymbol, string secondSymbol, Types type)
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope();
            symbolTable.AddSymbol(firestSymbol, type);
            Assert.Throws<ArgumentException>(() => symbolTable.AddSymbol(secondSymbol, type));
            symbolTable.CloseScope();
        }

        [Fact]
        public void Check_If_The_Symbol_Is_In_SymbolTable()
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope();
            Assert.Null(symbolTable.GetSymbol("a"));
            symbolTable.CloseScope();
        }

        [Fact]
        public void Check_Parameters_Types_in_func()
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope();
            symbolTable.AddSymbol("testfunc", Types.Void, Types.Boolean, Types.FloatingPoint, Types.Integer, Types.Vector);
            List<Types> paramTypes = symbolTable.GetSymbol("testfunc").GetParamTypes();
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
            symbolTable.OpenScope();
            symbolTable.AddSymbol("a", Types.Integer);
            symbolTable.OpenScope();
            Assert.NotNull(symbolTable.GetSymbol("a"));
            symbolTable.CloseScope();
            symbolTable.CloseScope();
        }

        [Fact]
        public void Check_If_GetSymbol_Returns_Null()
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope();
            symbolTable.OpenScope();
            symbolTable.AddSymbol("a", Types.Integer);
            symbolTable.CloseScope();
            Assert.Null(symbolTable.GetSymbol("a"));
            symbolTable.CloseScope();
        }

        [Fact]
        public void Checks_The_Right_Symbol_Gets_Returned_When_In_A_Scope()
        {
            ISymbolTable symbolTable = new RecSymbolTable();
            symbolTable.OpenScope();
            symbolTable.AddSymbol("a", Types.Integer);
            symbolTable.OpenScope();
            symbolTable.AddSymbol("a", Types.Boolean);
            Assert.Equal(Types.Boolean, symbolTable.GetSymbol("a").type);
            symbolTable.CloseScope();
            Assert.Equal(Types.Integer, symbolTable.GetSymbol("a").type);
            symbolTable.CloseScope();
        }

        [Fact]
        //Checks whether the symbol table is built probably given a test file.
        public void Symbol_Build_test()
        {
            ISymbolTable symTable = new RecSymbolTable();
            StreamReader reader = new StreamReader("../../../../SymbolTableTest/SymbolBuildTest.txt");
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            SymbolTableBuilder builder = new SymbolTableBuilder(symTable);
            Start s = p.Parse();
            s.Apply(builder);

            Assert.True(symTable.IsComplete());

            symTable.OpenScope();
            Assert.NotNull(symTable.GetSymbol("a"));
            Assert.NotNull(symTable.GetSymbol("b"));
            Assert.NotNull(symTable.GetSymbol("c"));
            symTable.OpenScope();
            Assert.Equal(Types.FloatingPoint, symTable.GetSymbol("c").type);
            symTable.OpenScope();
            Assert.Equal(Types.FloatingPoint, symTable.GetSymbol("a").type);
            Assert.Equal(Types.FloatingPoint, symTable.GetSymbol("b").type);
            symTable.OpenScope();
            Assert.NotNull(symTable.GetSymbol("a"));
            symTable.OpenScope();
            Assert.Equal(Types.Boolean, symTable.GetSymbol("b").type);
            symTable.OpenScope();
            Assert.NotNull(symTable.GetSymbol("a"));
            Assert.NotNull(symTable.GetSymbol("b"));
            symTable.CloseScope();
            symTable.CloseScope();
            Assert.Equal(Types.FloatingPoint, symTable.GetSymbol("b").type);
            symTable.CloseScope();
            symTable.CloseScope();
            symTable.OpenScope();
            Assert.Equal(Types.FloatingPoint, symTable.GetSymbol("c").type);
            symTable.CloseScope();
            Assert.Equal(Types.Integer, symTable.GetSymbol("testFunc").type);
            symTable.CloseScope();

            symTable.OpenScope();
            Assert.Equal(Types.Integer, symTable.GetSymbol("b").type);
            symTable.CloseScope();

            symTable.OpenScope();
            Assert.Equal(Types.FloatingPoint, symTable.GetSymbol("x").type);
            Assert.Equal(Types.Integer, symTable.GetSymbol("f").type);
            symTable.CloseScope();
            Assert.Null(symTable.GetSymbol("x"));
            symTable.CloseScope();

        }

    }
}
