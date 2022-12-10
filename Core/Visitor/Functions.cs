using Antlr4.Runtime.Tree;

namespace WAL.Core.Visitor;

public partial class WALVisitor
{
    private bool _hasReturnStatement;
    private bool _insideFunction;
    public Dictionary<string, List<bool>> Functions = new();

    public override object VisitFunctionCallStatement(WALParser.FunctionCallStatementContext context)
    {
        var name = context.Identifier().GetText();
        if (!Functions.ContainsKey(name))
            throw new Exception();
        var arguments = context.expressionList() == null
            ? Array.Empty<IParseTree>()
            : context.expressionList().expression();
        if (!(arguments.Length >= Functions[name].Count(x => !x) && arguments.Length <= Functions[name].Count))
            throw new Exception();
        var callString = $"{name}(";
        foreach (var expression in arguments)
        {
            var result = Visit(expression);
            if (result == null) throw new Exception();
            callString += $"{result}, ";
        }

        if (callString.EndsWith(", "))
            callString = callString[..^2];
        callString += ");";
        return callString;
    }

    public override object VisitFunctionCallExpression(WALParser.FunctionCallExpressionContext context)
    {
        return (Visit(context.functionCallStatement()) as string)![..^1];
    }

    public override object VisitFunctionDeclarationStatement(WALParser.FunctionDeclarationStatementContext context)
    {
        var name = context.Identifier().GetText();
        if (Functions.ContainsKey(name))
            throw new Exception();
        var arguments = context.functionArgumentList() == null
            ? Array.Empty<WALParser.FunctionArgumentContext>()
            : context.functionArgumentList().functionArgument();
        if (arguments.DistinctBy(x => x.Identifier().GetText()).Count() != arguments.Length)
            throw new Exception();
        for (var i = 0; i < arguments.Length - 1; i++)
            if (arguments[i].Assignment() != null &&
                arguments[i + 1].Assignment() == null) // optional arguments must be at the end
                throw new Exception();

        var argumentString = arguments.Aggregate("",
            (current, arg) =>
                current +
                $"auto {arg.Identifier().GetText()}{(arg.Assignment() != null ? $" = {arg.constant().GetText()}" : "")},");
        if (!string.IsNullOrEmpty(argumentString))
            argumentString = argumentString[..^1];

        var functionString = "\t";
        _insideFunction = true;
        _hasReturnStatement = false;
        foreach (var line in context.block().line())
        {
            var result = Visit(line);
            if (result == null) continue;
            Output.MainCode = Output.MainCode.SkipLast(1).ToList();
            functionString += result + "\n\t";
        }

        functionString = functionString[..^1];
        functionString += "}\n";
        functionString = $"{(_hasReturnStatement ? "auto" : "void")} {name}({argumentString}) {{\n" + functionString;
        _insideFunction = false;
        var optional = arguments.Select(functionArgumentContext => functionArgumentContext.Assignment() != null)
            .ToList();
        Functions[name] = optional;
        Output.Add(functionString, context);
        return null;
    }

    public override object VisitReturnStatement(WALParser.ReturnStatementContext context)
    {
        if (!_insideFunction) throw new Exception();
        _hasReturnStatement = true;
        var result = Visit(context.expression());
        if (result == null) return new Exception();
        return $"return {result};";
    }
}