namespace WAL.Core.Visitor;

public partial class WALVisitor
{
    private readonly Dictionary<Type, string> _typeToCpp = new()
    {
        { typeof(int), "int" },
        { typeof(bool), "bool" },
        { typeof(string), "string" }
    };

    public List<string> Constants = new();
    public List<string> Variables = new();

    public override object VisitAssignmentStatement(WALParser.AssignmentStatementContext context)
    {
        var name = context.Identifier().GetText();
        var expression = Visit(context.expression());
        if (expression == null) throw new Exception();
        if (Variables.All(x => x != name))
        {
            Variables.Add(name);
            var definition = $"auto {name} = {expression};";
            if (context.Global() != null)
            {
                Output.Add(definition, context);
                return null;
            }

            return definition;
        }

        return $"{name} = {expression};";
    }

    public override object VisitConstantDeclarationStatement(WALParser.ConstantDeclarationStatementContext context)
    {
        var name = context.Identifier().GetText();
        var constant = Visit(context.constant());
        var type = _typeToCpp[constant.GetType()];
        Output.Add($"const {type} {name} = {constant};", context);
        return null;
    }
}