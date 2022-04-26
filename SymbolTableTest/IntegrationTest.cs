using System;
using Xunit;
using GOAT_Compiler;

namespace SymbolTableTest
{
    public class IntegrationTest
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
        public void Test()
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
    }
}
