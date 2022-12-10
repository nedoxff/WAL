﻿using Antlr4.Runtime;

namespace WAL;

public class AdvancedOutput
{
    private static readonly Dictionary<Type, int> _contextToInt = new()
    {
        { typeof(WALParser.FunctionDeclarationStatementContext), 0 },
        { typeof(WALParser.AssignmentStatementContext), 1 },
        { typeof(WALParser.ConstantDeclarationStatementContext), 2 },
        { typeof(WALParser.ImportStatementContext), 3 },
        { typeof(WALParser.UnsafeStatementContext), 4 }
    };

    public List<string> ConstantDeclarations = new();
    public List<string> Functions = new();
    public List<string> GlobalUnsafeStatements = new();
    public List<string> GlobalVariableDeclarations = new();
    public List<string> ImportStatements = new();
    public List<string> MainCode = new();

    public void Add(string str, ParserRuleContext context)
    {
        if (context == null)
        {
            MainCode.Add(str);
            return;
        }

        switch (_contextToInt[context.GetType()])
        {
            case 0:
                Functions.Add(str);
                break;
            case 1:
                GlobalVariableDeclarations.Add(str);
                break;
            case 2:
                ConstantDeclarations.Add(str);
                break;
            case 3:
                ImportStatements.Add(str);
                break;
            case 4:
                GlobalUnsafeStatements.Add(str);
                break;
        }
    }

    public override string ToString()
    {
        return $@"/* This file was auto-generated by WAL. Please do not modify it. */

/* Imports */
using namespace std;
{string.Join('\n', ImportStatements)}

/* Constants */
{string.Join('\n', ConstantDeclarations)}

/* Functions */
{string.Join('\n', Functions)}

/* Global unsafe blocks (raw code) */
{string.Join('\n', GlobalUnsafeStatements)}

int main()
{{
    /* Main code */
{'\t' + string.Join("\n\t", MainCode)}
}}";
    }
}