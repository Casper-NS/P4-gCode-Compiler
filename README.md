# P4-gCode-Compiler
Compiler for g-code

To update the parser:
Open the "sableCC2 folder.
Open the "Generated" folder.
Replace the "example.sablecc" file with the updated version of the grammar.
Open the Command Prompt (press windows-button and type "cmd", then press enter)
If you havn't already, then download the c# version of sableCC from this link: http://www.mare.ee/indrek/sablecc/ (under the installation topic)

The format of the command:
java -jar absPathToSablecc.jar -t csharp,csharp-build  absPathToGrammar.sablecc

The absPathToSablecc.jar is specific to your machine (this is the jar file of the sablecc program), for me it is:
C:\Users\Henri\Downloads\sablecc-3-beta.3.altgen.20041114\sablecc-3-beta.3.altgen.20041114\lib\sablecc.jar

The same goes for absPathToGrammar (this is the grammar we have created and want sablecc to build a parser for), for me it is: C:\Users\Henri\OneDrive\Dokumenter\GithubClones\P4-gCode-Compiler\SableCC\Generated\example.sablecc

For me the total command is:
java -jar C:\Users\Henri\Downloads\sablecc-3-beta.3.altgen.20041114\sablecc-3-beta.3.altgen.20041114\lib\sablecc.jar -t csharp,csharp-build  C:\Users\Henri\OneDrive\Dokumenter\GithubClones\P4-gCode-Compiler\SableCC\Generated\example.sablecc
