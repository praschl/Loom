grammar Loom; // Define a grammar called CSV

// Parser rules
file : line+ EOF ;

line
    : name text     # namedLine
    | text          # plainLine
    ;

name : WORD COLON ;

text : (WORD | WS)+ NEWLINE;

// Lexer rules
COLON : ':' ;

WORD : ([a-z] | [A-Z])+ ; 

WS : [ \t]+ -> skip ;
NEWLINE : [\r\n]+ ;
