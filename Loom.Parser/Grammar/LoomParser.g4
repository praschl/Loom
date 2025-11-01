parser grammar LoomParser; // Define a grammar called CSV

options {
  tokenVocab = LoomLexer;
}

// Parser rules
file : block+ EOF ; 

// blocks
block      : Title=title
             Tags=tags?
             blockStart
             line+
             blockEnd
             ;

title : TITLE WS* COLON WS* Text=plainLine NL;
tags  : TAGS WS* COLON WS* plainWords+ NL ;

plainLine  : WS* textFragment;
plainWords : op=(WS | WORD); // we do not use sentence here, because we really want the words separated by spaces

blockStart : BLOCK_START NL ;
blockEnd   : BLOCK_END NL* ;

// text

line : WS* (dialogLine | statement) NL ;

dialogLine : (name=WORD COLON)? WS* (textFragment | expr )+ ;

statement  : LBRACE VAR_PREFIX TESTVAR EQUALS STRING_LITERAL RBRACE;
expr   : LBRACE VAR_PREFIX TESTVAR RBRACE ;

textFragment   : op=(WORD | WS)+ ;
