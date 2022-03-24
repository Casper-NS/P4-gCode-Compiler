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
