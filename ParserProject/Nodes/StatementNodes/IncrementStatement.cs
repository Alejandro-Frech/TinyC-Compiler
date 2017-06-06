﻿using System;
using ParserProject.Nodes.ExpressionNodes;
using ParserProject.Semantic.CustomTypes;

namespace ParserProject.Nodes.StatementNodes
{
	public class IncrementStatement : StatementNode
	{
		public ExpressionNode ExpressionNode { get; set; }
		public IncrementStatement()
		{
		}

	    public override void EvaluateSemantic()
	    {
	        throw new NotImplementedException();
	    }
	}
}
