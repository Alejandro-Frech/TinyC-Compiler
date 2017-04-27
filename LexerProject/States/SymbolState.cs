﻿using System;
using System.Text;
using LexerProject.Tokens;
using LexerProject.Tables;
using LexerProject.Exceptions;
namespace LexerProject.States
{
    public class SymbolState
    {
        private readonly ReserverdSymbols _reserverdSymbols;

        public SymbolState()
        {
            _reserverdSymbols = new ReserverdSymbols();
        }
        public Token GetSymbol(ref Symbol currentSymbol,InputString inputString)
        {
			var line = currentSymbol.Line;
			var col = currentSymbol.Column;
			var lexeme = new StringBuilder();
            while (IsValid(currentSymbol.Character.ToString()))
            {
                lexeme.Append(currentSymbol.Character);
                currentSymbol = inputString.GetNextSymbol();
            }
            var exists = _reserverdSymbols.Collection.ContainsKey(lexeme.ToString().ToLower());
            if(exists)
	            return new Token
	            {
	                Type = _reserverdSymbols.Collection[lexeme.ToString().ToLower()],
	                Lexeme = lexeme.ToString(),
	                Column = col,
	                Line = line
	            };
            else
                throw new LexicalException("Cannot resolve symbol  " + lexeme.ToString() + "  Line: " + line +
                                           " Column: " + col);
        }

        public bool IsValid(string symbol)
        {
            return _reserverdSymbols.Collection.ContainsKey(symbol);
        }
    }
}
