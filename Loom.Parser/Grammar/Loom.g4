grammar Loom; // Define a grammar called CSV

// Parser rules
file : line+ EOF ;

line
    : name text     # namedLine
    | text          # plainLine
    ;

name : WORD+ COLON ;

text : WS* (WORD | WS)+ NEWLINE;

// Lexer rules

COLON : ':' ;
WORD : ~[@{}\r\n[\]: ]+ ; 

WS : [ \t]+ -> skip;
NEWLINE : [\r\n]+ ;
