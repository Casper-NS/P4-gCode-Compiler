using System.IO;
using System.Linq;

namespace GOAT_Compiler
{
    internal class CodeGenerator : SymbolTableVisitor
    {
        FileStream gcodeFile;

        public CodeGenerator(ISymbolTable symbolTable, string outputName) : base(symbolTable)
        {
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