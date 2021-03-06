﻿using System;
using ParserProject.Generation;
using ParserProject.Semantic.CustomTypes;

namespace ParserProject.Nodes.ExpressionNodes.AssignationNodes
{
    public class AssignationSumExpressionNode : AssignationExpressionNode
    {
        public AssignationSumExpressionNode(){

            OperatorRules.Add(new Tuple<CustomType, CustomType>(Integer, Integer), Integer); ;
            OperatorRules.Add(new Tuple<CustomType, CustomType>(Char, Char), Char);
            OperatorRules.Add(new Tuple<CustomType, CustomType>(Integer, Char), Integer);

            OperatorRules.Add(new Tuple<CustomType, CustomType>(Float, Float), Float);
            OperatorRules.Add(new Tuple<CustomType, CustomType>(Float, Integer), Float);
            OperatorRules.Add(new Tuple<CustomType, CustomType>(Float, Char), Float);

            
            OperatorRules.Add(new Tuple<CustomType, CustomType>(String, String), String);
            OperatorRules.Add(new Tuple<CustomType, CustomType>(String, Integer), String);
            OperatorRules.Add(new Tuple<CustomType, CustomType>(String, Float), String);
            OperatorRules.Add(new Tuple<CustomType, CustomType>(String, Boolean), String);
            OperatorRules.Add(new Tuple<CustomType, CustomType>(String, Char), String);
        }

		public override ExpressionCode GenerateCode()
		{
		    if (LeftValue.GenerateCode().Type == "string")
                return new ExpressionCode { Code = LeftValue.GenerateCode().Code + " += " + RightValue.GenerateCode().Code,Type = "string"};
		    if (LeftValue.GenerateCode().Type == "char")
		        return new ExpressionCode { Code = LeftValue.GenerateCode().Code + " += " + RightValue.GenerateCode().Code, Type = "char" };
		    if (LeftValue.GenerateCode().Type == "int")
		        return new ExpressionCode { Code = LeftValue.GenerateCode().Code + " += " + RightValue.GenerateCode().Code, Type = "int" };
		    return new ExpressionCode { Code = LeftValue.GenerateCode().Code + " += " + RightValue.GenerateCode().Code, Type = "float" };

        }
    }
}
