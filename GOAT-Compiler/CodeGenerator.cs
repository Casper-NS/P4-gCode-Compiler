using System.Collections.Generic;
using System.IO;
using System.Linq;
using GOATCode.node;

namespace GOAT_Compiler
{
    internal class CodeGenerator : SymbolTableVisitor
    {
        FileStream gcodeFile;
        private ISymbolTable _symbolTable;
        private Dictionary<Symbol, (object, Types)> RunTimeTable = new Dictionary<Symbol, (object, Types)>();

        public CodeGenerator(ISymbolTable symbolTable, string outputName) : base(symbolTable)
        {
            _symbolTable = symbolTable;
            if (outputName.Length == 0)
            {
                gcodeFile = File.Create("GOAT.gcode");
            }
            else
            {
                gcodeFile = File.Create(outputName + ".gcode");
            }
        }
    }
}