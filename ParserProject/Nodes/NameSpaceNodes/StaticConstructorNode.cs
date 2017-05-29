﻿using System;
using System.Collections.Generic;
using System.Text;
using ParserProject.Nodes.ExpressionNodes.TypeProductionNodes;
using ParserProject.Nodes.NameSpaceNodes.ClassDeclarationNodes;
using ParserProject.Nodes.PrivacyModifierNodes;
using ParserProject.Nodes.StatementNodes;

namespace ParserProject.Nodes.NameSpaceNodes
{
    public class StaticConstructorNode:ClassMemberDeclaration
    {
        public PrivacyModifierNode PrivacyModifie { get; set; }
        public IdTypeProductionNode Type { get; set; }
        public List<StatementNode> StatementList { get; set; }
        
    }
}