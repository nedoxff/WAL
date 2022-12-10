java -jar antlr-4.11.1-complete.jar -visitor -Dlanguage=CSharp -o ../Core/Parser WALLexer.g4
java -jar antlr-4.11.1-complete.jar -visitor -Dlanguage=CSharp -o ../Core/Parser WALParser.g4
git add ../Core/Parser