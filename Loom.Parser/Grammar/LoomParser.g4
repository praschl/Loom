parser grammar LoomParser;

options {
  tokenVocab = LoomLexer;
}

// Parser rules
file : scriptBlock? NL*
       block+ EOF ; 

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

// Dialog line can contain text and inline script blocks
line : indent=WS* name? WS* lineContent+ NL ;

lineContent : Text=textFragment
            | Script=scriptBlock
            ;

name: lineContent+ COLON ;

// Standalone script block on its own line
scriptBlock : LBRACE script=JS_CONTENT* RBRACE ;

textFragment : op=(WORD | WS)+ ;