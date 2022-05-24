# GOAT-code compiler
GOAT-code (G-code Offering Abstraction and Turtling) is a programming language that offers higher level abstraction over G-code.
This project is a GOAT-code to G-code compiler.

This compiler generates G-code that should be supported by:
Marlin, RepRapFirmware, Repetier, Smoothie, Klipper, Prusa, MK4Duo, Sprinter, Machinekit and Redeem.

However, it has only been tested on Marlin

## How to use the compiler for GOAT-code:
Step by step guide to using the GOAT-code compiler:

1: Open the P4-GCode-Compiler.sln solution file in Visual Studio or equivalent IDE.

2: Build the project.

3: Open up the newly built P4-GCode-Compiler.

4: Inside the built P4-GCode-Compiler is GOAT.exe, which is the compiler. This can be run from a command line.

6: Insert or create a GOAT-code file in the same folder as GOAT.exe

7: Open up the folder containing GOAT.exe in a command line

8: Run the command "GOAT sourcefile targetfile", which will create a new file. For example, "GOAT Vase.goat Vase.gcode". This will create Vase.gcode, if the compilation is succesful.

Example files can be found in the source code under the VisitorTests folder. Especially CubeClean.goat and Vase.goat are good examples of goat code files.

Open a terminal and execude the program with two parameters. 
The first parameter is path of the file which you want to compile to G-code and the second parameter is the absolute path for the compiled file should have, this includes the name of the file and the ".gcode".
Example: 
P4/gCode-Compiler RelativePath/text.goat AbsolutePath/NameOfFile.gcode

Before using any compiled G-code, please run it through a G-code viewer such as https://ncviewer.com/ or https://nraynaud.github.io/webgcode/.

## How to update the parser:
- Open the "sableCC2 folder.
- Open the "Generated" folder.
- Replace the ".sablecc" file with the updated version of the grammar.
- Open the Command Prompt (press windows-button and type "cmd", then press enter)
- If you havn't already, then download the c# version of sableCC from this link: [sablecc-download](http://www.mare.ee/indrek/sablecc/) (the download link can be found in the installation topic)
- Insert the following command:

### The update parser command:
java -jar _absPathToSablecc.jar_ -t csharp,csharp-build  _absPathToGrammar.sablecc_


