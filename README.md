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

**The absPathToSablecc.jar is specific to your machine (this is the jar file of the sablecc program), for me it is:**

C:\Users\Henri\Downloads\sablecc-3-beta.3.altgen.20041114\sablecc-3-beta.3.altgen.20041114\lib\sablecc.jar

**The same goes for absPathToGrammar (this is the grammar we have created and want sablecc to build a parser for), for me it is:**

C:\Users\Henri\OneDrive\Dokumenter\GithubClones\P4-gCode-Compiler\SableCC\Generated\GGcodeGrammar.sablecc

**For me the total command is:**

java -jar _C:\Users\Henri\Downloads\sablecc-3-beta.3.altgen.20041114\sablecc-3-beta.3.altgen.20041114\lib\sablecc.jar_ -t csharp,csharp-build  _C:\Users\Henri\OneDrive\Dokumenter\GithubClones\P4-gCode-Compiler\SableCC\Generated\GGcodeGrammar.sablecc_
