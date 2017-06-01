﻿using System;
using ParserProject.Nodes.ExpressionNodes;
using ParserProject.Nodes.ExpressionNodes.BinaryOperators;
using ParserProject.Nodes.ExpressionNodes.PrimitiveTypeNodes;

namespace ParserProject.BinaryOperators.ExpressionNodes.Nodes
{
    public class LeftShiftExpressionNode : BinaryOperatorNode
	{

        public LeftShiftExpressionNode(){
			OperatorRules.Add(new Tuple<PrimitiveTypeNode, PrimitiveTypeNode>(Integer, Integer), Integer); ;
			OperatorRules.Add(new Tuple<PrimitiveTypeNode, PrimitiveTypeNode>(Char, Integer), Integer);
			OperatorRules.Add(new Tuple<PrimitiveTypeNode, PrimitiveTypeNode>(Integer, Char), Integer);

			OperatorRules.Add(new Tuple<PrimitiveTypeNode, PrimitiveTypeNode>(Char, Char), Integer);
        }
	}

}
