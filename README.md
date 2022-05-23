# P4-gCode-Compiler
Compiler for g-code

## How to update the parser:
- Open the "sableCC2 folder.
- Open the "Generated" folder.
- Replace the ".sablecc" file with the updated version of the grammar.
- Open the Command Prompt (press windows-button and type "cmd", then press enter)
- If you havn't already, then download the c# version of sableCC from this link: [sablecc-download](http://www.mare.ee/indrek/sablecc/) (the download link can be found in the installation topic)
- Insert the following command:

### The update parser command:
java -jar _absPathToSablecc.jar_ -t csharp,csharp-build  _absPathToGrammar.sablecc_


#### How to use the compiler for GOAT-code:
Open a terminal and execude the program with two parameters. 
The first parameter is path of the file which you want to compile to G-code and the second parameter is the absolute path for the compiled file should have, this includes the name of the file and the ".gcode".
Example: 
P4/gCode-Compiler RelativePath/text.goat AbsolutePath/NameOfFile.gcode
