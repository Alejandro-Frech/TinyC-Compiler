﻿using System;
using ParserProject.Generation;
using ParserProject.Semantic.CustomTypes;

namespace ParserProject.Nodes.StatementNodes
{
    public class ContinueNodeStatement:StatementNode
    {
        public ContinueNodeStatement()
        {
        }

        public override void EvaluateSemantic()
        {
        }

        public override ExpressionCode GenerateCode()
        {
            return new ExpressionCode { Code = "continue ; \n" };
        }
    }
}
