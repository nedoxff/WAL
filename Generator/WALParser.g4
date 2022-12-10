parser grammar WALParser;
options { tokenVocab = WALLexer; }

program: line* EOF;
line: (statement | timedBlockStatement | unsafeStatement | ifStatement | functionDeclarationStatement | whileStatement | comment);
statement: (assignmentStatement | functionCallStatement | returnStatement | constantDeclarationStatement | importStatement) Semicolon;

functionCallStatement: Identifier LeftParen expressionList? RightParen;
functionDeclarationStatement: FunctionDeclare Identifier LeftParen functionArgumentList? RightParen block;

assignmentStatement: Global? Identifier assignmentOperators expression; 
ifStatement: If expression block (Else elseIfStatement)?;
whileStatement: While expression block;
elseIfStatement: block | ifStatement;
returnStatement: Return expression;
timedBlockStatement: (Number | timeBlockEvent | expression) Colon (block | statement);
unsafeStatement: unsafeBlock;
importStatement: Import String;
constantDeclarationStatement: Const Identifier Assignment constant;

expression
    : constant #constantExpression
    | Identifier #identifierExpression
    | functionCallStatement #functionCallExpression
    | LeftParen expression RightParen #parenthesizedExpression
    | Not expression #notExpression
    | addOperators expression #unaryAddExpression
    | expression multiplyOperators expression #binaryMultiplyExpression
    | expression addOperators expression #binaryAddExpression
    | expression compareOperators expression #binaryCompareExpression
    | expression booleanOperators expression #binaryBooleanExpression
    ;

multiplyOperators: Multiply | Divide | Power | Modulus;
addOperators: Plus | Minus;
compareOperators: Equal | NotEqual | Greater | GreaterOrEqual | Lesser | LesserOrEqual;
booleanOperators: And | Or | Xor;
assignmentOperators: Assignment | AdditionAsignment | SubtractionAssignment | MultiplicationAssignment | DivisionAssignment | ModulusAssignment;

block: LeftBrace line* RightBrace;

constant: Number | String | Boolean;
timeBlockEvent: Init;
comment: Comment;
unsafeBlock: (Global)? Unsafe genericBlock;
genericBlock: LeftBrace (. | genericBlock)*? RightBrace;
expressionList: expression (Comma expression)*?;
functionArgumentList: functionArgument (Comma functionArgument)*?;
functionArgument: Identifier (Assignment constant)?;