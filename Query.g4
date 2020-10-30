grammar Query;

/*
 * Parser Rules
 */

query                   : orExpression EOF;
orExpression            : andExpression (OR andExpression)*;
andExpression           : expressionPart (AND expressionPart)*;
expressionPart          : ATTRIBUTE COMPARISON QUOTEDVALUE;

/*
 * Lexer Rules
 */

OR      : 'OR';
AND     : 'AND';

COMPARISON     : GREATER | 
                 LESSER | 
                 EQUAL | 
                 GREATER EQUAL | 
                 LESSER EQUAL |
                 NOT EQUAL;
QUOTEDVALUE    : QUOTE ('\\\'' | ()? ~'\'')* QUOTE;
ATTRIBUTE      : (LETTER)+;
WHITESPACE     : (' ' | '\t' | '\n' | '\r')+ -> skip;

fragment DIGIT         : [0-9];
fragment LETTER        : [a-zA-Z];
fragment SPECIAL       : '_' | '-' | '.';
fragment GREATER       : '>';
fragment LESSER        : '<';
fragment EQUAL         : '=';
fragment NOT           : '!';
fragment QUOTE         : '\'';