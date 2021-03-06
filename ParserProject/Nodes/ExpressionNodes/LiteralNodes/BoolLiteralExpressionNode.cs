﻿using System;
using LexerProject.Tokens;
using ParserProject.Generation;
using ParserProject.Semantic;
using ParserProject.Semantic.CustomTypes;

namespace ParserProject.Nodes.ExpressionNodes.LiteralNodes
{
    public class BoolLiteralExpressionNode:LiteralNodeExpression
    {
        public bool Value { get; set; }

        public BoolLiteralExpressionNode(Token lit)
        {
            literal = lit;
            Value = bool.Parse(literal.Lexeme);
        }

        public BoolLiteralExpressionNode(){
            
        }

        public override CustomType EvaluateSemantic()
        {
            return CustomTypesTable.Instance.GetType("Bool");
        }

        public override ExpressionCode GenerateCode()
        {
            return new ExpressionCode { Code = Value.ToString(),Type = "bool"};
        }
    }
}
