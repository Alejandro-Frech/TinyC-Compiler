﻿using System;
using System.Collections.Generic;
using ParserProject.Generation;
using ParserProject.Nodes.ExpressionNodes;
using ParserProject.Semantic.CustomTypes;

namespace ParserProject.Nodes.StatementNodes
{
    public class DoWhileNodeStatement:StatementNode
    {
		public ExpressionNode Condition { get; set; }
		public List<StatementNode> TrueStatements { get; set; }

		public DoWhileNodeStatement(ExpressionNode condition, List<StatementNode> trueStatement)
		{
			Condition = condition;
			TrueStatements = trueStatement;
		}

        public DoWhileNodeStatement(){
            
        }

        public override void EvaluateSemantic()
        {
        }

        public override ExpressionCode GenerateCode()
        {
            var stringCode = "do { \n";
            foreach (var s in TrueStatements)
            {
                stringCode += s.GenerateCode().Code;
            }
            stringCode += "while (" + Condition.GenerateCode().Code + ") ;\n";
            return new ExpressionCode { Code = stringCode };
        }
    }
}
