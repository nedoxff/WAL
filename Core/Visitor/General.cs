using Antlr4.Runtime.Misc;

namespace WAL.Core.Visitor;

public partial class WALVisitor : WALParserBaseVisitor<object>
{
    public static Dictionary<string, string> ImportConverter = new()
    {
        { "sfml/graphics", "SFML/Graphics.hpp" },
        { "sfml/audio", "SFML/Audio.hpp" },
        { "sfml/system", "SFML/System.hpp" },
        { "windows", "Windows.h" }
    };

    public List<string> Imports = new();
    public AdvancedOutput Output;

    public override object VisitLine(WALParser.LineContext context)
    {
        var result = VisitChildren(context) as string;
        if (!string.IsNullOrEmpty(result))
            Output.Add(result, null);
        return result;
    }

    /*public override object VisitStatement(WALParser.StatementContext context)
    {
        var result = VisitChildren((IRuleNode)context.children.First());
        return result;
    }*/

    /*public override object VisitStatement(WALParser.StatementContext context)
    {
        object result = null;
        if (context.assignmentStatement() != null)
            result = Visit(context.assignmentStatement());
        if (context.returnStatement() != null)
            result = Visit(context.returnStatement());
        if (context.importStatement() != null)
            result = Visit(context.importStatement());
        return result;
    }*/

    public override object VisitConstant(WALParser.ConstantContext context)
    {
        if (context.Number() is { } n)
            return int.Parse(n.GetText());
        if (context.String() is { } s)
            return s.GetText();
        if (context.Boolean() is { } b)
            return b.GetText() == "true";

        return null;
    }

    //TODO
    public override object VisitIdentifierExpression(WALParser.IdentifierExpressionContext context)
    {
        //if (Variables.All(x => x != context.Identifier().GetText())) throw new Exception();
        return context.Identifier().GetText();
    }

    public override object VisitUnsafeStatement(WALParser.UnsafeStatementContext context)
    {
        var start = context.unsafeBlock().genericBlock().Start.StartIndex;
        var end = context.unsafeBlock().genericBlock().Stop.StopIndex;
        var code = context.Start.InputStream.GetText(new Interval(start, end))[1..^1];
        if (context.unsafeBlock().Global() == null)
            return string.Join("\r\n", code.Split("\r\n").Select(x => x.Trim()));
        Output.Add(code, context);
        return null;
    }

    public override object VisitImportStatement(WALParser.ImportStatementContext context)
    {
        var importString = $"#include <{GetIncludedFile(context.String().GetText()[1..^1])}>";
        if (Output.ImportStatements.Contains(importString)) return null;
        Output.Add(importString, context);
        return null;
    }

    private string GetIncludedFile(string name)
    {
        if (ImportConverter.ContainsKey(name))
            return ImportConverter[name];
        if (name.StartsWith("cpp/"))
            return name[4..];
        throw new Exception();
    }

    protected override object AggregateResult(object aggregate, object nextResult)
    {
        return $"{aggregate}{nextResult}";
    }
}