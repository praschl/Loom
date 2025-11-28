parser grammar LoomParser;

options {
  tokenVocab = LoomLexer;
}

// Parser rules
file : jsBlock? NL*
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
plainWords : op=(WS | WORD); // we want individual words here

blockStart : BLOCK_START NL ;
blockEnd   : BLOCK_END NL* ;

// text

line       : dl=dialogLine NL* 
           | jsif=jsIfBlock NL* 
           ;

// Dialog line can contain text and inline script blocks
dialogLine : indent=WS* name? WS* lineContent+;

lineContent : Text=textFragment
            | Out=jsOutBlock
            | Script=jsBlock
            ;

name: lineContent+ COLON ;

// Standalone script block on its own line
jsIfBlock   : indent=WS* IF condition=JS_CONTENT* RBRACE NL*;

jsBlock : LBRACE script=JS_CONTENT* RBRACE ;

jsOutBlock  : OUT script=JS_CONTENT* RBRACE ;

textFragment : op=(WORD | WS)+ ;