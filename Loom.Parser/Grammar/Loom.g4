grammar Loom; // Define a grammar called CSV

file : line+ ;

line : TEXT ;
    
TEXT   : ~[,\r\n"]+ ; // TEXT is any character other than ',', '\r' or '\n'
