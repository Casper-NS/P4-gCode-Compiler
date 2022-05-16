using GOAT_Compiler;
using GOATCode.lexer;
using GOATCode.node;
using GOATCode.parser;
using System;
using System.IO;

namespace P4_GCode_Compiler
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ReadArgs(args, out string fileIn, out string fileOut);

            try
            {
                ReadAndGenerateAST(fileIn, out Start AST, out ISymbolTable symTable, out TypeChecker typeChecker);

                GenerateCode(fileOut, AST, symTable, typeChecker);

                ShowSuccess("Compiled succesfully to " + fileOut + "!");
            }
            catch (CompilerException e)
            {
                ShowError(e.Message);
            }
            catch (CallBuildInWalkException e)
            {
                ShowError(e.Message);
            }
            catch (LexerException e)
            {
                ShowError(e.Message);
            }
            catch (ParserException e)
            {
                ShowError(e.Message);
            }
        }

        private static void ShowError(string error)
        {
            var tempCol = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ForegroundColor = tempCol;
        }

        private static void ShowSuccess(string success)
        {
            var tempCol = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(success);
            Console.ForegroundColor = tempCol;
        }

        private static void ReadArgs(string[] args, out string fileIn, out string fileOut)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("Wrong arguments. Usage: GOAT inputFile outputFile");
            }
            fileIn = args[0];
            fileOut = args[1];
        }

        private static void ReadAndGenerateAST(string file, out Start AST, out ISymbolTable symTable, out TypeChecker typeChecker)
        {
            symTable = new RecSymbolTable();
            StreamReader reader = new StreamReader(file);
            Lexer l = new Lexer(reader);
            Parser p = new Parser(l);
            AST = p.Parse();
            SymbolTableBuilder builder = new SymbolTableBuilder(symTable);
            AST.Apply(builder);
            ScopeChecker scopeChecker = new ScopeChecker(symTable);
            AST.Apply(scopeChecker);
            typeChecker = new TypeChecker(symTable);
            AST.Apply(typeChecker);
        }

        private static void GenerateCode(string file, Start AST, ISymbolTable symTable, TypeChecker typeChecker)
        {
            using (Stream stream = new FileStream(file, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    CodeGenerator codeGenerator = new CodeGenerator(symTable, typeChecker.TypeDictionary, writer);
                    AST.Apply(codeGenerator);
                }
            }
        }
    }
}