﻿using System;
using System.Collections.Generic;
using System.Text;
using ParserProject.Generation;
using ParserProject.Nodes.ExpressionNodes.AccesorNodes;
using ParserProject.Nodes.ExpressionNodes.TypeProductionNodes;
using ParserProject.Semantic.CustomTypes;

namespace ParserProject.Nodes.ExpressionNodes.NewExpressionNodes.NewCreationNodes
{
    public class NewCreationExpressionNode:NewExpressionNode
    {
        public TypeProductionNode Type { get; set; }
        public NewExpressionNode NewCreationNode { get; set; }
        public AccesorExpressionNode Accessor { get; set; }

        public override CustomType EvaluateSemantic()
        {
            return null;
        }

        public override ExpressionCode GenerateCode()
        {
            if (NewCreationNode is NewObjectCreation)
            {
                var stringCode = "new " + Type.GenerateCode().Type + " ( ";

                if (((NewObjectCreation)NewCreationNode).ObjectArgumentsList != null)
                {
                    for (int i = 0; 0 < ((NewObjectCreation)NewCreationNode).ObjectArgumentsList.Count; i++)
                    {
                        if (((NewObjectCreation)NewCreationNode).ObjectArgumentsList[i] == ((NewObjectCreation)NewCreationNode).ObjectArgumentsList[((NewObjectCreation)NewCreationNode).ObjectArgumentsList.Count - 1])
                        {
                            stringCode += ((NewObjectCreation)NewCreationNode).ObjectArgumentsList[i].GenerateCode().Code;
                        }
                        else
                        {
                            stringCode += ((NewObjectCreation)NewCreationNode).ObjectArgumentsList[i].GenerateCode().Code + " , ";
                        }
                    }
					stringCode += " )" + Accessor.GenerateCode().Code + " ";

                    if(((NewObjectCreation)NewCreationNode).ObjectCollectionInitalizer != null){
                        
                    }					
                }
                stringCode += " )" + Accessor.GenerateCode().Code + " ";
                return new ExpressionCode { Code = stringCode };

            }else{
               var x= NewCreationNode as NewArrayCreation;
                return new ExpressionCode { Code = x.GenerateCode().Code };
            }
        }
    }
}
