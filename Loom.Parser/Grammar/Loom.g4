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
tags  : 'tags' WS* COLON words+ NL ;

blockStart : BLOCK_START NL ;
blockEnd   : BLOCK_END NL* ;

// text

line : name? plainLine NL ;

name : WORD+ COLON ;

plainLine : WS* (WORD | WS)+;

words : (WS | Word=WORD);

// Lexer rules

BLOCK_START : '-' '-' '-' '-'* ;
BLOCK_END   : '=' '=' '=' '='* ;

WORD      : ~[@{}\r\n[\]: ]+ ; 

COLON : ':' ;
WS    : [ \t]+  ;
NL    : [\r\n]+ ;
