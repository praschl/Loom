grammar Loom; // Define a grammar called CSV

// Parser rules
file : block+ EOF ;

// blocks
block      : blockStart line+ blockEnd ;

blockStart : BLOCK_START NL ;
blockEnd   : BLOCK_END NL ;

// lines
line : name? text ;

name : WORD+ COLON ;

text : WS* (WORD | WS)+ NL;

// Lexer rules

BLOCK_START : '---' ;
BLOCK_END   : '===' ;

COLON : ':' ;
WORD : ~[@{}\r\n[\]: ]+ ; 

WS : [ \t]+ -> skip;
NL : [\r\n]+ ;
