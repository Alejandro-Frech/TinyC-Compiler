﻿using System;
using ParserProject.Generation;
using ParserProject.Semantic;
using ParserProject.Semantic.CustomTypes;

namespace ParserProject.Nodes.ExpressionNodes.TypeProductionNodes
{
    public class VoidTypeNode:TypeExpressionNode
    {
        public VoidTypeNode()
        {
            @Type = "void";
        }

        public override CustomType EvaluateSemantic()
        {
            return CustomTypesTable.Instance.GetType("Void");
        }

		public override ExpressionCode GenerateCode()
		{
			return new ExpressionCode { Code = "void" };
		}
    }
}
