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
tags  : 'tags' WS* COLON plainWords+ NL ;

plainLine : WS* (WORD | WS)+;
plainWords : op=(WS | WORD);

blockStart : BLOCK_START NL ;
blockEnd   : BLOCK_END NL* ;

// text

line : name? dialogLine NL ;

name : WORD+ COLON ;

dialogLine : WS*(WORD | WS )+ ;

// Lexer rules

BLOCK_START : '-' '-' '-' '-'* ;
BLOCK_END   : '=' '=' '=' '='* ;

WORD        : ~[@{}\r\n[\]: ]+ ; 

AT          : '@' ;
COLON       : ':' ;

WS    : [ \t]+  ;
NL    : [\r\n]+ ;
