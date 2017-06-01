﻿using System;
using ParserProject.Nodes.ExpressionNodes.ArrayNodes;

namespace ParserProject.Nodes.ExpressionNodes.NewExpressionNodes
{
    public class NewArrayInitalizerNode:NewExpressionNode
    {
        public RankSpeciferNode RankSpecifer { get; set; }

        public ArrayInitalizerNode ArrayInitalizerNode { get; set; }

        public NewArrayInitalizerNode()
        {
        }
    }
}
