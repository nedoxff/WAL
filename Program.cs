using Antlr4.Runtime;
using WAL;
using WAL.Core.Visitor;
using WAL.Helpers;

var file = args[0];
var environment = new BuildEnvironment(file);

var advancedOutput = new AdvancedOutput();
var visitor = new WALVisitor
{
    Output = advancedOutput
};

void Compile(string code)
{
    var inputStream = new AntlrInputStream(code);
    var lexer = new WALLexer(inputStream);
    var tokenStream = new CommonTokenStream(lexer);
    var parser = new WALParser(tokenStream);
    visitor.Visit(parser.program());
}

Compile(File.ReadAllText(Path.Join("External", "global.wal")));
Compile(File.ReadAllText(file));

File.WriteAllText(environment.CppFile, advancedOutput.ToString());

#if DEBUG
environment.SaveCppOutput();
#endif

environment.Build();