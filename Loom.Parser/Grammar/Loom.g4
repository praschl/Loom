grammar Loom; // Define a grammar called CSV

// Parser rules
file : block+ EOF ;

// blocks
block      : Title=title
             Tags=tags?
             blockStart
             line+
             blockEnd
             ;

title : 'title' WS* COLON WS* Text=plainLine NL;
tags  : 'tags' WS* COLON WS* plainWords+ NL ;

plainLine  : WS* textFragment;
plainWords : op=(WS | WORD); // we do not use sentence here, because we really want the words separated by spaces

blockStart : BLOCK_START NL ;
blockEnd   : BLOCK_END NL* ;

// text

line : (name=WORD COLON)? (dialogLine | statement) NL ;

dialogLine : WS* (textFragment | template )+ ;

statement  : '{$' 'var1' WS* '=' WS* '"' textFragment* '"' '}';
template   : '{$' 'var1' '}' ;

textFragment   : op=(WORD | WS)+ ;

// Lexer rules

BLOCK_START : '-' '-' '-' '-'* ;
BLOCK_END   : '=' '=' '=' '='* ;

WORD        : ~[@{}\r\n[\]": ]+ ; 
    
AT          : '@' ;
COLON       : ':' ;

WS    : [ \t]+  ;
NL    : [\r\n]+ ;
