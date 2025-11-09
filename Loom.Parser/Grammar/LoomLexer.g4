lexer grammar LoomLexer;

fragment WS_CHAR: [ \t]+;

TITLE : 'title' ;
TAGS  : 'tags' ;

BLOCK_START : '-' '-' '-' '-'* ;
BLOCK_END   : '=' '=' '=' '='* ;

WORD        : ~[{}\r\n":$ ]+ ; 
    
COLON       : ':' ;

WS          : WS_CHAR ;
NL          : [\r\n]+ ;

// --- BRACES for JavaScript blocks ---
LBRACE       : '{' -> pushMode(JS_BLOCK) ;

mode JS_BLOCK;

// Capture everything until we find the matching closing brace
JS_CONTENT   : ( JS_STRING | JS_REGEX | JS_COMMENT | JS_NESTED_BRACES | ~[{}/"'`] )+ ;

fragment JS_STRING         : '"' ( '\\' . | ~[\\"\r\n] )* '"'
                           | '\'' ( '\\' . | ~[\\'\r\n] )* '\''
                           | '`' ( '\\' . | ~[\\`] )* '`'
                           ;

fragment JS_REGEX          : '/' ( '\\' . | ~[/\r\n\\] )+ '/' [gimsuvy]* ;

fragment JS_COMMENT        : '//' ~[\r\n]*
                           | '/*' .*? '*/'
                           ;

fragment JS_NESTED_BRACES  : '{' ( JS_STRING | JS_REGEX | JS_COMMENT | JS_NESTED_BRACES | ~[{}/"'`] )* '}'
                           ;

RBRACE    : '}' -> popMode ;