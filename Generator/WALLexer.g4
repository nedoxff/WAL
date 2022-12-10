lexer grammar WALLexer;
tokens {RAW_BLOCK}

fragment Digit: [0-9];
fragment HexDigit: [0-9A-F];
Whitespace: (' ' | '\t') -> channel(HIDDEN);
NewLine: ('\r'? '\n' | '\r' | '\n') -> skip;
Semicolon: ';';
LeftParen: '(';
RightParen: ')';
LeftBracket: '[';
RightBracket: ']';
LeftBrace: '{';
RightBrace: '}';
Assignment: '=';
Comma: ',';
Not: '!';
Arrow: '->';
Colon: ':';

FunctionDeclare: 'fn';

SingleLineCommentStart: '//';
MultiLineCommentStart: '/*';
MultiLineCommentEnd: '*/';

Comment
    :   ( SingleLineCommentStart (~[\r\n]|Whitespace)* 
        | MultiLineCommentStart .*? MultiLineCommentEnd
        )
    ;

At: '@';
Hashtag: '#';

Multiply: '*';
Plus: '+';
Minus: '-';
Divide: '/';
Power: '**';
Modulus: '%';

And: '&&';
Or: '||';
Xor: '^';

//<assoc=right>
Greater: '>';
Lesser: '<';
GreaterOrEqual: '>=';
LesserOrEqual: '<=';
Equal: '==';
NotEqual: '!=';

AdditionAsignment: '+=';
SubtractionAssignment: '-=';
MultiplicationAssignment: '*=';
DivisionAssignment: '/=';
ModulusAssignment: '%=';

Init: 'init';
Unsafe: 'unsafe' Whitespace+;
Global: 'global' Whitespace+;

/*
    Keywords
*/
If: 'if' Whitespace+;

/*Very important for newlines:

else
{
}
and
else if ...
{
}
and
else {
}

*/
Else: 'else';
While: 'while' Whitespace+;
True: 'true' Whitespace+;
False: 'false' Whitespace+;
Import: 'import' Whitespace+;
Return: 'return' Whitespace+;
Const: 'const' Whitespace+;

Boolean: True | False;
Number: Digit+ ([.] Digit+)?; 
Identifier: [a-zA-Z_][a-zA-Z0-9_]*;
String: ('"' ~'"'* '"') | ('\'' ~'\''* '\'');
Any: .;