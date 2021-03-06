﻿using System;
using System.Collections.Generic;
using System.Text;
using ParserProject.Generation;
using ParserProject.Nodes.ExpressionNodes.AccesorNodes;
using ParserProject.Semantic.CustomTypes;

namespace ParserProject.Nodes.ExpressionNodes
{
    public class ParentesisExpresionNode : ExpressionNode
    {
        public  ExpressionNode ExpresioNode { get; set; }

        public AccesorExpressionNode AccesorExpression { get; set; }

        public override CustomType EvaluateSemantic()
        {
            return null;
        }

        public override ExpressionCode GenerateCode()
        {
            var stringCode = "( ";
            stringCode += ExpresioNode.GenerateCode().Code;
            stringCode += " )";
            if(AccesorExpression!=null)
                stringCode += AccesorExpression.GenerateCode().Code;
            return new ExpressionCode{Code = stringCode};
        }
    }
}
