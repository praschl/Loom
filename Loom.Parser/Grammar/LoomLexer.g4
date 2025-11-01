lexer grammar LoomLexer;

fragment WS_CHAR: [ \t]+;

TITLE : 'title' ;
TAGS  : 'tags' ;

BLOCK_START : '-' '-' '-' '-'* ;
BLOCK_END   : '=' '=' '=' '='* ;

WORD        : ~[@{}\r\n[\]":$ ]+ ; 
    
AT          : '@' ;
COLON       : ':' ;

WS          : WS_CHAR ;
NL          : [\r\n]+ ;

// --- BRACES ---
LBRACE       : '{' -> pushMode(BRACES) ;

mode BRACES;
RBRACE       : '}' -> popMode ;
VAR_PREFIX   : '$' ;
EQUALS       : '=' ;

BRACES_WS    : WS_CHAR -> skip ;

STRING_LITERAL : '"' ~["]* '"' ;

TESTVAR  : 'var1' ;
