﻿﻿﻿﻿﻿using System;
using LexerProject;
using LexerProject.Tokens;
using ParserProject.Exceptions;
using ParserProject.Extensions;
using System.Collections.Generic;
using ParserProject.Nodes.ExtendsNodes;
using ParserProject.Nodes.NameSpaceNodes;
using ParserProject.Nodes.StatementNodes;
using ParserProject.Nodes.ExpressionNodes;
using ParserProject.Nodes.NameSpaceNodes.EnumNodes;
using ParserProject.Nodes.ExpressionNodes.ArrayNodes;
using ParserProject.Nodes.ExpressionNodes.AccesorNodes;
using ParserProject.Nodes.ExpressionNodes.LiteralNodes;
using ParserProject.Nodes.NameSpaceNodes.InterfaceNodes;
using ParserProject.BinaryOperators.ExpressionNodes.Nodes;
using ParserProject.Nodes.ExpressionNodes.AssignationNodes;
using ParserProject.Nodes.ExpressionNodes.BinaryOperators;
using ParserProject.Nodes.ExpressionNodes.CastExpresionNodes;
using ParserProject.Nodes.ExpressionNodes.PrimitiveTypeNodes;
using ParserProject.Nodes.ExpressionNodes.NewExpressionNodes;
using ParserProject.Nodes.ExpressionNodes.TypeProductionNodes;
using ParserProject.Nodes.NameSpaceNodes.ClassDeclarationNodes;
using ParserProject.Nodes.NameSpaceNodes.MethodModiferNodes;
using ParserProject.Nodes.ExpressionNodes.NewExpressionNodes.NewCreationNodes;
using ParserProject.Nodes.StatementNodes.DeclarationAsignationStatementNodes;
using ParserProject.Nodes.NameSpaceNodes.ClassDeclarationNodes.FieldMethodConstructorNodes;

namespace ParserProject
{
    public class Parser
    {
        private readonly Lexer _lexer;
        private Token _currentToken;

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            _currentToken = _lexer.GetNextToken();
        }

        public Parser(){
            
        }

        public List<CodeNode> Parse()
        {
             var codeList=Code();

            if (_currentToken.Type != TokenType.Eof)
                throw new SintacticalException("Expected Eof");
            return codeList;
        }

        private List<CodeNode> Code()
        {
            return NameSpaceList();
        }

        private List<CodeNode> NameSpaceList()
        {
            if (_currentToken.Type.IsNameSpace())
            {
                var namespaceNode=NameSpace();
                var list=NameSpaceList();
                list.Insert(0,namespaceNode);
                return list;
            }
            else
            {
                return new List<CodeNode>();
            }

        }

        private CodeNode NameSpace()
        {
            var usingDirectiveList=UsingDirectives();
            var namespaceDeclarationList=NameSpaceDeclarations();
            return new CodeNode{UsingDirectiveList=usingDirectiveList,NameSpaceDeclarationList=namespaceDeclarationList};
        }

        private List<UsingDirectiveNode> UsingDirectives()
        {
            if (_currentToken.Type == TokenType.RwUsing)
            {
                var usingdirective=UsingDirective();
                var list=UsingDirectives();
                list.Insert(0,usingdirective);
                return list;
            }
            else
            {
                return new List<UsingDirectiveNode>();
            }
        }

        private UsingDirectiveNode UsingDirective()
        {
            if (_currentToken.Type != TokenType.RwUsing)
                throw new SintacticalException("Expected using Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var idtype=TypeName();
            if (_currentToken.Type != TokenType.EndStatement)
                throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " + _currentToken.Column);
            _currentToken = _lexer.GetNextToken();

            return new UsingDirectiveNode(idtype);
        }

        private IdTypeNode TypeName()
        {
            var idlexeme = _currentToken;
            if (_currentToken.Type != TokenType.Id)
                throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var idnode=TypeNamePrime();
            return new IdTypeNode(idlexeme, idnode);
        }

        private IdTypeNode TypeNamePrime()
        {
            if (_currentToken.Type == TokenType.Period)
            {
                var idlexeme = _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var idnode = TypeNamePrime();
                return new IdTypeNode(idlexeme, idnode);
            }
            else
            {
                return null;
            }
        }

        private List<NameSpaceDeclarationNode> NameSpaceDeclarations()
        {
            if (_currentToken.Type.IsPrivacyModifier() || _currentToken.Type == TokenType.RwNamespace || _currentToken.Type.IsClassModifier() || _currentToken.Type == TokenType.RwClass
                || _currentToken.Type == TokenType.RwInterface || _currentToken.Type == TokenType.RwEnum)
            {
                var nameSpaceNode=NameSpaceDeclaration();
                var list=NameSpaceDeclarations();
                list.Insert(0,nameSpaceNode);
                return list;
            }
            else
            {
                return new List<NameSpaceDeclarationNode>();
            }
        }

        private NameSpaceDeclarationNode NameSpaceDeclaration()
        {
            if (_currentToken.Type == TokenType.RwNamespace)
            {
                return NameSpaceStatement();
            }
            else
            {
                var privacyNode=PrivacyModifier();
                return ClassInterfaceEnum(privacyNode);
            }

        }

        private NameSpaceNode NameSpaceStatement()
		{
			if (_currentToken.Type != TokenType.RwNamespace)
				throw new SintacticalException("Expected Namespace Line " + _currentToken.Line + " Col " +
											   _currentToken.Column);
			_currentToken = _lexer.GetNextToken();
			var id = TypeName();
			var code = NameSpaceBody();
            return new NameSpaceNode { IdNode = id ,Code=code };

		}

        private CodeNode NameSpaceBody()
		{
			if (_currentToken.Type != TokenType.KeyOpen)
				throw new SintacticalException("Expected { Line " + _currentToken.Line + " Col " +
											   _currentToken.Column);
			_currentToken = _lexer.GetNextToken();
			var codeNode=NameSpace();
			if (_currentToken.Type != TokenType.KeyClose)
				throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
											   _currentToken.Column);
			_currentToken = _lexer.GetNextToken();
            return codeNode;
		}

        private string PrivacyModifier()
        {
            if (_currentToken.Type == TokenType.RwPublic)
            {
                _currentToken = _lexer.GetNextToken();
                return "Public";
            }
            else if (_currentToken.Type == TokenType.RwPrivate)
            {
                _currentToken = _lexer.GetNextToken();
                return "Private";
            }
            else if (_currentToken.Type == TokenType.RwProtected)
            {
                _currentToken = _lexer.GetNextToken();
                return  "Protected";
            }
            else
            {
                return "";
            }
        }

        private NameSpaceDeclarationNode ClassInterfaceEnum(string privacyNode)
        {
            if (_currentToken.Type == TokenType.RwInterface)
            {
                var interfaceStructure = InterfaceDeclaration();
               
                return new InterfaceNode
                {
                    PrivacyModifier = privacyNode,
                    NameToken = interfaceStructure.Name,
                    HeritageList = interfaceStructure.ExtendsNode.ListIdNodes,
                    InterfaceMethodList = interfaceStructure.InterfaceBody.InterfaceMethodList
                };
            }
            else if (_currentToken.Type == TokenType.RwEnum)
            {
                var enumstructure=EnumDeclaration();
                return new EnumNode { PrivacyModifier = privacyNode,
                    NameToken =enumstructure.Name,
                    EnumElementList= enumstructure.Body.EnumElementList
                };
            }
            else if (_currentToken.Type == TokenType.RwClass || _currentToken.Type.IsClassModifier())
            {
                var classStructure = ClassDeclaration();
                return new ClassNode
                {
                    PrivacyModifier = privacyNode,
                    ClassModifier = classStructure.ClassModifierNode,
                    NameToken=classStructure.Name,
                    HeritageList=classStructure.ExtendsNode.ListIdNodes,
                    FieldMethodConstructorList = classStructure.Body.ClassMemberDeclarationList
                };
            }
            else
                throw new SintacticalException("Expected inteface,enum,class or class modifiers Line " +
                                               _currentToken.Line + " Col " +
                                               _currentToken.Column);
        }

        private InterfaceStructureNode InterfaceDeclaration()
        {
            if (_currentToken.Type != TokenType.RwInterface)
                throw new SintacticalException("Expected interface Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            var idlexeme=_currentToken = _lexer.GetNextToken();
            if (_currentToken.Type != TokenType.Id)
                throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var extendsNode=Heredance();
            var body=InterfaceBody();
            return new InterfaceStructureNode(idlexeme, extendsNode,body);
        }

        private InterfaceBodyNode InterfaceBody()
        {
            if (_currentToken.Type != TokenType.KeyOpen)
                throw new SintacticalException("Expected { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var list=InterfaceMemberDeclarations();
            if (_currentToken.Type != TokenType.KeyClose)
                throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            return new InterfaceBodyNode{ InterfaceMethodList = list};
        }

        private List<InterfaceMethodNode> InterfaceMemberDeclarations()
        {
            if (_currentToken.Type.IsType() || _currentToken.Type == TokenType.RwVoid)
            {
                var interfaceMethod = InterfaceElement();
                var list=InterfaceMemberDeclarations();
                list.Insert(0,interfaceMethod);
                return list;
            }
            else
            {
                return new List<InterfaceMethodNode>();
            }
        }

        private InterfaceMethodNode InterfaceElement()
        {
            if (_currentToken.Type.IsType())
            {
                var typeNode=TypeProduction();

                var idLexeme = _currentToken;
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var parameterLst=MethodProperty();

                return new InterfaceMethodNode { TypeNode = typeNode, Name = idLexeme ,  ParameterList=parameterLst };
            }
            else if (_currentToken.Type == TokenType.RwVoid)
            {
                var voidNode = new VoidTypeNode();
                var idLexeme=_currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var list=MethodProperty();

                return new InterfaceMethodNode { TypeNode = voidNode, Name = idLexeme, ParameterList = list };
            }
            else
            {
                throw new SintacticalException("Expected type or void Line " + _currentToken.Line + " Col " +
                                                _currentToken.Column);
            }
        }

        private List<ParameterNode> MethodProperty()
        {
            if (_currentToken.Type == TokenType.ParOpen)
            {
                var list=MethodHeader();
                if (_currentToken.Type != TokenType.EndStatement)
                    throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return list;
            }
            else
            {
                throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                                _currentToken.Column);
            }
        }

        private List<ParameterNode> MethodHeader()
        {
            if (_currentToken.Type != TokenType.ParOpen)
                throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var parameterlist=FormalParameterList();
            if (_currentToken.Type != TokenType.ParClose)
                throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            return parameterlist;
        }

        private List<ParameterNode> FormalParameterList()
        {
            if (_currentToken.Type.IsType())
            {
                var p=Parameter();
                var list=FormalParameterListPrime();
                list.Insert(0,p);
                return list;
            }
            else
            {
                return new List<ParameterNode>();
            }
        }

        private List<ParameterNode> FormalParameterListPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                var parameter=Parameter();
                var list=FormalParameterListPrime();
                list.Insert(0,parameter);
                return list;
            }
            else
            {
                return new List<ParameterNode>();
            }
        }

        private ParameterNode Parameter()
        {
            var type=TypeProduction();
            var idlexeme = _currentToken;
            if (_currentToken.Type != TokenType.Id)
                throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            return new ParameterNode { typeNode = type, Name = idlexeme };
        }

        private EnumStructureNode EnumDeclaration()
        {
            if (_currentToken.Type != TokenType.RwEnum)
                throw new SintacticalException("Expected enum Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            
            _currentToken = _lexer.GetNextToken();
            var idlexeme = _currentToken;
            if (_currentToken.Type != TokenType.Id)
                throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var enumbody=EnumBody();
            return new EnumStructureNode { Name=idlexeme , Body=enumbody };
        }

        private EnumBodyNode EnumBody()
        {
            if (_currentToken.Type != TokenType.KeyOpen)
                throw new SintacticalException("Expected { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var list=EnumMemberDeclaration();
            if (_currentToken.Type != TokenType.KeyClose)
                throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            return new EnumBodyNode {  EnumElementList = list };
        }

        private List<EnumElementNode> EnumMemberDeclaration()
        {
            if (_currentToken.Type == TokenType.Id)
            {
				var element = EnumElement();
				var list = EnumMemberDeclarationPrime();
				list.Insert(0, element);
				return list;
            }
            else
            {
                return new List<EnumElementNode>();
            }
        }

        private List<EnumElementNode> EnumMemberDeclarationPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                var element=EnumElement();
                var list=EnumMemberDeclarationPrime();
                list.Insert(0,element);
                return list;
            }
            else
            {
                return new List<EnumElementNode>();
            }
        }

        private EnumElementNode EnumElement()
        {
            var idlexeme = _currentToken;
            if (_currentToken.Type != TokenType.Id)
                throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var expressionNode=EnumElementBody();
            return new EnumElementNode { Name = idlexeme, Expression = expressionNode };
        }

        private ExpressionNode EnumElementBody()
        {
            if (_currentToken.Type == TokenType.OpAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                return Expresion();
            }
            else
            {
                return null;
            }
        }

        private ClassStructureNode ClassDeclaration()
        {
            var classModifier=ClassModifier();
            if (_currentToken.Type != TokenType.RwClass)
                throw new SintacticalException("Expected class Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            var idlexeme=_currentToken = _lexer.GetNextToken();
            if (_currentToken.Type != TokenType.Id && _currentToken.Type != TokenType.RwFloat && _currentToken.Type != TokenType.RwInt && _currentToken.Type != TokenType.RwChar && _currentToken.Type != TokenType.RwString && _currentToken.Type != TokenType.RwBool)
                throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var extendsNode=Heredance();
            var body= ClassBody();
            return  new ClassStructureNode{ClassModifierNode = classModifier,Name = idlexeme,ExtendsNode = extendsNode,Body = body};
        }

        private ClassBodyNode ClassBody()
        {
            if (_currentToken.Type != TokenType.KeyOpen)
                throw new SintacticalException("Expected { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var list=ClassMemberDeclarations();
            if (_currentToken.Type != TokenType.KeyClose)
                throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            return new ClassBodyNode{ClassMemberDeclarationList = list };
        }

        private List<FieldMethodConstructor> ClassMemberDeclarations()
        {

            if (_currentToken.Type.IsPrivacyModifier() || _currentToken.Type == TokenType.RwStatic || _currentToken.Type.IsType() || _currentToken.Type == TokenType.RwVoid
              || _currentToken.Type.IsMethodModifiers())
            {

                var x=ClassMemberDeclaration();
                var list=ClassMemberDeclarations();
                list.Insert(0,x);
                return list;
            }
            else
            {
                return new List<FieldMethodConstructor>();
            }
        }

        private FieldMethodConstructor ClassMemberDeclaration()
        {
            var privacyNode=PrivacyModifier();
            if (_currentToken.Type == TokenType.RwAbstract)
            {
                _currentToken = _lexer.GetNextToken();
                var x=InterfaceElement();

				return new FieldMethodConstructor
                {
                    IsStatic = false,
                    IsMethod = true,
                    IsField = false,
                    IsAbstract=true,
					IsConstructor = false,
                    Type = x.TypeNode,
					PrivacyModifier = privacyNode,
                    FieldList = new List<FieldNode>(),
                    Method = new MethodDeclarationNode{Name=x.Name,ParameterList=x.ParameterList}

				};
            }
            else
            {
                return FieldMethodConstructor(privacyNode);
            }
        }

        private FieldMethodConstructor FieldMethodConstructor(string privacyNode)
        {
            if (_currentToken.Type == TokenType.RwStatic)
            {
                _currentToken = _lexer.GetNextToken();
                return StaticOptions(privacyNode);
            }
            else if (_currentToken.Type.IsPredifinedType() || _currentToken.Type == TokenType.RwEnum)
            {
                var type=CustomTypeProduction();
                var id = _currentToken;
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var fieldmethodMetaData =FieldMethodDeclaration(id);
				return new FieldMethodConstructor
                {
                    IsStatic = false,
					IsMethod = fieldmethodMetaData.IsMethod,
					IsField = fieldmethodMetaData.IsField,
					IsConstructor = false,
					Type = type,
					PrivacyModifier = privacyNode,
					FieldList = fieldmethodMetaData.FieldList,
					Method = fieldmethodMetaData.Method

				};

            }
            else if (_currentToken.Type == TokenType.RwVoid)
            {
                _currentToken = _lexer.GetNextToken();
                var id = _currentToken;
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var method=MethodDeclaration(id);
                return new FieldMethodConstructor
                {
                    IsStatic = false,
                    IsMethod = true,
                    IsField = false,
					IsConstructor = false,
					Type = new VoidTypeNode(),
					PrivacyModifier = privacyNode,
                    FieldList = new List<FieldNode>(),
					Method = method

				};
            }
            else if (_currentToken.Type.IsMethodModifiers())
            {
                var method =MethodModifiers();
                return MethodReturn(privacyNode,method);
            }
            else if (_currentToken.Type == TokenType.Id)
            {
                var typeNode = TypeName();
                var type = new IdTypeProductionNode();
                type.IdType = typeNode;
                    
                return Mierda3(privacyNode,type);
            }
            else
            {
                throw new SintacticalException("Expected Primitive type, enum , void, static , Id or method modifiers Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
            }
        }

        private FieldMethodConstructor Mierda3(string privacyNode, IdTypeProductionNode type)
        {
            if (_currentToken.Type == TokenType.Id || _currentToken.Type == TokenType.BraOpen)
            {
                var rankspecList = TypeProductionPrime();
                type.RankSpecifiers = rankspecList;
                var idlexeme = _currentToken;
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var fieldmethodMetaData =FieldMethodDeclaration(idlexeme);
				return new FieldMethodConstructor
				{
					IsStatic = false,
					IsMethod = fieldmethodMetaData.IsMethod,
					IsField = fieldmethodMetaData.IsField,
					IsConstructor = false,
					Type = type,
					PrivacyModifier = privacyNode,
					FieldList = fieldmethodMetaData.FieldList,
					Method = fieldmethodMetaData.Method

				};

            }
            else if (_currentToken.Type == TokenType.ParOpen)
            {
                if (_currentToken.Type != TokenType.ParOpen)
                    throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var paremeterList=FormalParameterList();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var constructInit=ConstructorInitializer();
                var statementList=Block();
                return new FieldMethodConstructor
                {
                    IsStatic = false,
                    IsMethod = false,
                    IsField = false,
                    IsConstructor = true,
                    Type = type,
                    PrivacyModifier = privacyNode,
                    FieldList = new List<FieldNode>(),
					Method = null,
                    ConstructorParameterList=paremeterList,
                    BaseNode=constructInit,
                    ConstructorStatementList=statementList,

				};
            }
            else
            {
                throw new SintacticalException("Expected Id or ( Line " + _currentToken.Line + " Col " +
                                                     _currentToken.Column);
            }

        }

        private FieldMethodConstructor Mierda2(IdTypeProductionNode type, string privacyNode)
        {
            if (_currentToken.Type == TokenType.ParOpen)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var statementList = Block();
				return new FieldMethodConstructor
                {
                    IsStatic = true,
                    IsMethod = false,
                    IsField = false,
					IsConstructor = true,
					Type = type,
					PrivacyModifier = privacyNode,
					FieldList = new List<FieldNode>(),
					Method = null,
                    ConstructorStatementList=statementList

				};
            }
            else if (_currentToken.Type == TokenType.Id || _currentToken.Type == TokenType.BraOpen)
            {
                var rankspecList = TypeProductionPrime();
                type.RankSpecifiers = rankspecList;
                var idlexeme = _currentToken;
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                          
				var fieldmethodMetaData = FieldMethodDeclaration(idlexeme);
				return new FieldMethodConstructor
				{
					IsStatic = true,
					IsMethod = fieldmethodMetaData.IsMethod,
					IsField = fieldmethodMetaData.IsField,
					IsConstructor = false,
					Type = type,
					PrivacyModifier = privacyNode,
					FieldList = fieldmethodMetaData.FieldList,
					Method = fieldmethodMetaData.Method

				};

            }
            else
            {
                throw new SintacticalException("Expected ( or Id Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }

        }

        private BaseConstructorNode ConstructorInitializer()
        {
            if (_currentToken.Type == TokenType.Colon)
            {
                _currentToken = _lexer.GetNextToken();
                return ConstructorInitializerPrime();
            }
            else
            {
                return null;
            }
        }

        private BaseConstructorNode ConstructorInitializerPrime()
        {
            if (_currentToken.Type == TokenType.RwBase || _currentToken.Type == TokenType.RwThis)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.ParOpen)
                    throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var expList=ArgumentList();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return new BaseConstructorNode { ArgumeList = expList, BaseOrThis = "base" };

            }else if (_currentToken.Type == TokenType.RwThis)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.ParOpen)
                    throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var expList = ArgumentList();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return new BaseConstructorNode { ArgumeList = expList, BaseOrThis = "this" };
            }
            else
            {
                throw new SintacticalException("Expected this or base constructor Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private List<ExpressionNode> ArgumentList()
        {
            if (_currentToken.Type.IsExpression())
            {
                var exp=Expresion();
                var list=ArgumentListPrime();
                list.Insert(0,exp);
                return list;
            }
            else
            {
				return new List<ExpressionNode>();
            }
        }

        private List<ExpressionNode> ArgumentListPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                var exp=Expresion();
                var list=ArgumentListPrime();
                list.Insert(0,exp);
                return list;
            }
            else
            {
                return new List<ExpressionNode>();
            }
        }

        private FieldMethodConstructor MethodReturn(string privacyNode, MethodModifierNode method)
        {
            if (_currentToken.Type == TokenType.RwVoid)
            {
                var id=_currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var fieldmethodMetaData =MethodDeclaration(id);

				return new FieldMethodConstructor
                {
                    IsStatic = false,
                    IsMethod = true,
                    IsField = false,
					IsConstructor = false,
					Type = new VoidTypeNode(),
					PrivacyModifier = privacyNode,
                    MethodModifier=method.Value,
					FieldList = fieldmethodMetaData.FieldList,
					Method = fieldmethodMetaData

				};

            }
            else if (_currentToken.Type.IsType())
            {
                var type=TypeProduction();
                var id = _currentToken;
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var methodDec=MethodDeclaration(id);
				return new FieldMethodConstructor
                {
                    IsStatic = false,
                    IsMethod = true,
                    IsField = false,
					IsConstructor = false,
					Type = type,
					PrivacyModifier = privacyNode,
					MethodModifier = method.Value,
                    FieldList = new List<FieldNode>(),
					Method = methodDec

				};
            }
            else
            {
                throw new SintacticalException("Expected void or type Line " + _currentToken.Line + " Col " +
                                                  _currentToken.Column);
            }
        }

        private FieldMethodConstructor StaticOptions(string privacyNode)
        {
            if (_currentToken.Type.IsPredifinedType() || _currentToken.Type == TokenType.RwEnum)
            {
                var type=CustomTypeProduction();
                var idlexme = _currentToken;
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var fieldmethodMetaData=FieldMethodDeclaration(idlexme);
                return new FieldMethodConstructor
                {
                    IsStatic = true,
                    IsMethod = fieldmethodMetaData.IsMethod,
                    IsField = fieldmethodMetaData.IsField,
                    IsConstructor = false,
                    Type = type,
                    PrivacyModifier = privacyNode,
					FieldList = fieldmethodMetaData.FieldList,
					Method = fieldmethodMetaData.Method

                };
            }
            else if (_currentToken.Type == TokenType.RwVoid)
            {
                _currentToken = _lexer.GetNextToken();
                var idlexme = _currentToken;
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var method=MethodDeclaration(idlexme);
				return new FieldMethodConstructor
                {
                    IsStatic = true,
                    IsMethod = true,
                    IsField = false,
					IsConstructor = false,
					Type = new VoidTypeNode(),
					PrivacyModifier = privacyNode,
                    FieldList = new List<FieldNode>(),
					Method = method
				};
            }
            else if (_currentToken.Type == TokenType.Id)
            {
                var typeNode=TypeName();
                var type= new IdTypeProductionNode();
                type.IdType = typeNode;
                return Mierda2(type,privacyNode);
            }
            else
            {
                throw new SintacticalException("Expected Primitive type ,enum,void or id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
            }
        }

        private FieldMethodDeclarationNode FieldMethodDeclaration(Token idlexme)
        {
            var method = false;
            var field = false;
            List<FieldNode> list = new List<FieldNode>();
            MethodDeclarationNode methodNode = null;
            if (_currentToken.Type == TokenType.ParOpen)
            {
                method = true;
                methodNode=MethodDeclaration(idlexme);
            }
            else
            {
                field = true;
                list=FieldDeclaration(idlexme);

            }

            return new FieldMethodDeclarationNode { IsField=field, IsMethod=method,FieldList=list ,Method=methodNode };

        }

        private List<FieldNode> FieldDeclaration(Token idlexme)
        {
            var list=FieldDeclarations(idlexme);
            if (_currentToken.Type != TokenType.EndStatement)
                throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            return list;
        }

        private List<FieldNode> FieldDeclarations(Token idlexme)
        {
            var exp=FieldAssignation();
            var list=FieldDeclarationsPrime();
            var f = new FieldNode {Name = idlexme, ExpressionNode = exp};
            list.Insert(0,f);
            return list;
        }

        private List<FieldNode> FieldDeclarationsPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                var id=_currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var exp=FieldAssignation();
                var field = new FieldNode{Name = id,ExpressionNode = exp};
                var list=FieldDeclarationsPrime();
                list.Insert(0,field);
                return list;
            }
            else
            {
                return  new List<FieldNode>();
            }
        }

        private ExpressionNode FieldAssignation()
        {
            if (_currentToken.Type == TokenType.OpAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                return VaraibleInitializer();
            }
            else
            {
                return null;
            }

        }

        private ExpressionNode VaraibleInitializer()
        {
            if (_currentToken.Type == TokenType.KeyOpen)
            {
                return ArrayInitalizer();
            }
            else if (_currentToken.Type.IsExpression())
            {
                return Expresion();
            }
            else
            {
                throw new SintacticalException("Expected valid variable initalizer Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private ArrayInitalizerNode ArrayInitalizer()
        {
            if (_currentToken.Type != TokenType.KeyOpen)
                throw new SintacticalException("Expected { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var list=VaraibleInitializerListOpt();
            if (_currentToken.Type != TokenType.KeyClose)
                throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            return new ArrayInitalizerNode { ExpressionList = list};
        }

        private MethodDeclarationNode MethodDeclaration(Token idlexme)
        {
            var parameterlist=MethodHeader();
            var statementlist=Block();
            return new MethodDeclarationNode {Name= idlexme, ParameterList =parameterlist,StatementList = statementlist};
        }

        private MethodModifierNode MethodModifiers()
        {
            if (_currentToken.Type == TokenType.RwAbstract)
            {
                _currentToken = _lexer.GetNextToken();
                return new AbstractMethodModifer();
            }else if (_currentToken.Type == TokenType.RwOverride)
            {
                _currentToken = _lexer.GetNextToken();
                return new OverrideMethodModifer();
            }
            else if (_currentToken.Type == TokenType.RwVirtual)
            {
                _currentToken = _lexer.GetNextToken();
                return new VirtualMethodModifer();
            }else
            {
                throw new SintacticalException("Expected abstract or override Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private ExtendsNode Heredance()
        {
            if (_currentToken.Type == TokenType.Colon)
            {
                _currentToken = _lexer.GetNextToken();
                var idnode=TypeName();
                var list=Base();
                list.Insert(0,idnode);
                return new ExtendsNode(list);
            }
            else
            {
                return new ExtendsNode();
            }
        }

        private List<IdTypeNode> Base()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                var idNode = TypeName();
                var list=Base();
                list.Insert(0,idNode);
                return list;
            }
            else
            {
                return new List<IdTypeNode>();
            }
        }

        private string ClassModifier()
        {
            if (_currentToken.Type == TokenType.RwStatic)
            {
                _currentToken = _lexer.GetNextToken();
                return "static";
            }else if (_currentToken.Type==TokenType.RwAbstract)
            {
                _currentToken = _lexer.GetNextToken();
                return "abstract";
            }
            else
            {
                return "";
            }
        }

        private TypeProductionNode TypeProduction()
        {
            if (_currentToken.Type == TokenType.Id)
            {
                var idNode=TypeName();
                var rankspecfiers=TypeProductionPrime();
                return new IdTypeProductionNode { IdType  = idNode,RankSpecifiers = rankspecfiers };
            }
            else if (_currentToken.Type.IsPredifinedType())
            {
                var primitivetypeNode=PredifinedType();
                var rankspecifiers=TypeProductionPrime();
                return new PrimitiveTypeProductionNode { PrimitiveType = primitivetypeNode , RankSpecifiers=rankspecifiers };

            }
            else if (_currentToken.Type == TokenType.RwEnum)
            {
                var primitivetypeNode = new PrimitiveEnumNode();
                _currentToken = _lexer.GetNextToken();
               var rankspecifiers= TypeProductionPrime();
                return new PrimitiveTypeProductionNode { PrimitiveType = primitivetypeNode, RankSpecifiers = rankspecifiers };
            }
            else
            {
                throw new SintacticalException("Expected valid type Line " + _currentToken.Line + " Col " +
                                              _currentToken.Column);
            }
        }

        private TypeProductionNode CustomTypeProduction()
        {
            if (_currentToken.Type.IsPredifinedType())
            {
                var primitivetypeNode = PredifinedType();
                var rankspecifiers = TypeProductionPrime();
                return new PrimitiveTypeProductionNode { PrimitiveType = primitivetypeNode, RankSpecifiers = rankspecifiers };
            }
            else if (_currentToken.Type == TokenType.RwEnum)
            {
                var primitivetypeNode = new PrimitiveEnumNode();
                _currentToken = _lexer.GetNextToken();
                var rankspecifiers = TypeProductionPrime();
                return new PrimitiveTypeProductionNode { PrimitiveType = primitivetypeNode, RankSpecifiers = rankspecifiers };
            }
            else
            {
                throw new SintacticalException("Expected Primitve type or Enum Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }

        }

        private TypeProductionNode TypeProductionForArrayOrObject()
        {
            if (_currentToken.Type == TokenType.Id)
            {
                var idNode=TypeName();
                return new IdTypeProductionNode { IdType = idNode };
            }
            else if (_currentToken.Type.IsPredifinedType())
            {
                var primitivetypeNode = PredifinedType();
                return new PrimitiveTypeProductionNode { PrimitiveType = primitivetypeNode};

            }
            else if (_currentToken.Type == TokenType.RwEnum)
            {
                var primitivetypeNode = new PrimitiveEnumNode();
                _currentToken = _lexer.GetNextToken();
                return new PrimitiveTypeProductionNode { PrimitiveType = primitivetypeNode};
            }
            else
            {
                throw new SintacticalException("Expected valid type Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private List<RankSpeciferNode> TypeProductionPrime()
        {
            if (_currentToken.Type == TokenType.BraOpen)
            {
                return RankSpecifiers();
                //TypeProductionPrime();
            }
            else
            {
                return new List<RankSpeciferNode>();
            }
        }

        private List<RankSpeciferNode> RankSpecifiers()
        {
            var r=RankSpecifier();
            var list=RankSpecifiersPrime();
            list.Insert(0,r);
            return list;
        }

        private List<RankSpeciferNode> RankSpecifiersPrime()
        {
            if (_currentToken.Type == TokenType.BraOpen)
            {
                var r=RankSpecifier();
                var list=RankSpecifiersPrime();
                list.Insert(0,r);
                return list;
            }
            else
            {
                return new List<RankSpeciferNode>();
            }
        }

        private RankSpeciferNode  RankSpecifier()
        {
            if (_currentToken.Type != TokenType.BraOpen)
                throw new SintacticalException("Expected [ Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var list= DimSpearetorsOpt();
            if (_currentToken.Type != TokenType.BraClose)
                throw new SintacticalException("Expected ] Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            return new RankSpeciferNode { DimSeparatorList = list };

        }

        private List<string> DimSpearetorsOpt()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                return DimSpearetors();
            }
            else
            {
                return new List<string>();
            }
        }

        private List<string> DimSpearetors()
        {
            if (_currentToken.Type != TokenType.Comma)
                throw new SintacticalException("Expected , Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var dim = ",";
            var list=DimSpearetorsPrime();
            list.Insert(0,dim);
            return list;
        }

        private List<string> DimSpearetorsPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                var dim = ",";
                _currentToken = _lexer.GetNextToken();
                var list=DimSpearetorsPrime();
                list.Insert(0,dim);
                return list;

            }
            else
            {
                return new List<string>();
            }
        }

        private PrimitiveTypeNode PredifinedType()
        {
				if (_currentToken.Type == TokenType.RwBool)
				{
                    _currentToken = _lexer.GetNextToken();
					return new PrimitiveBoolNode();
				}
				if (_currentToken.Type == TokenType.RwChar)
				{
                    _currentToken = _lexer.GetNextToken();
					return new PrimitiveCharNode();
				}
				if (_currentToken.Type == TokenType.RwInt)
				{
                    _currentToken = _lexer.GetNextToken();
					return new PrimitiveIntNode();
				}
				if (_currentToken.Type == TokenType.RwFloat)
				{
                    _currentToken = _lexer.GetNextToken();
					return new PrimitiveFloatNode();
				}
				if (_currentToken.Type == TokenType.RwString)
				{
                    _currentToken = _lexer.GetNextToken();
					return new PrimitiveStringNode();
				}
				if (_currentToken.Type == TokenType.RwEnum)
				{
                    _currentToken = _lexer.GetNextToken();
					return new PrimitiveEnumNode();
				}
            else{
				throw new SintacticalException("Expected primitive type Line " + _currentToken.Line + " Col " +
											   _currentToken.Column);
            }
                 
        }

        private List<StatementNode> Block()
        {
            if (_currentToken.Type != TokenType.KeyOpen)
                throw new SintacticalException("Expected { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var list = StatementList();
            if (_currentToken.Type != TokenType.KeyClose)
                throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            return list;
        }

        private List<ExpressionNode> VaraibleInitializerListOpt()
        {
            if (_currentToken.Type == TokenType.KeyOpen || _currentToken.Type.IsExpression())
            {
                return VaraibleInitializerList();
            }
            else
            {
                return new List<ExpressionNode>();
            }
        }

        private List<ExpressionNode> VaraibleInitializerList()
        {
            var expression=VaraibleInitializer();
            var list=VaraibleInitializerListPrime();
            list.Insert(0,expression);
            return list;
        }

        private List<ExpressionNode> VaraibleInitializerListPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                var exp=VaraibleInitializer();
                var list=VaraibleInitializerListPrime();
                list.Insert(0,exp);
                return list;
            }
            else
            {
                return new List<ExpressionNode>();
            }
        }

        private List<StatementNode> StatementList()
        {
            if (_currentToken.Type.IsStatements())
            {
                var statement = Statement();
                var list = StatementList();
                list.Insert(0, statement);
                return list;
            }
            else
            {
                return new List<StatementNode>();
            }
        }

        private StatementNode Statement()
        {
            if (_currentToken.Type == TokenType.RwIf)
            {
                return IfStatement();
            }
            else if (_currentToken.Type == TokenType.RwWhile)
            {
                return WhileStatement();
            }
            else if (_currentToken.Type == TokenType.RwDo)
            {
                return DoStatement();
            }
            else if (_currentToken.Type == TokenType.RwSwitch)
            {
                return SwitchStatement();
            }
            else if (_currentToken.Type == TokenType.RwReturn)
            {
                return ReturnStatement();
            }
            else if (_currentToken.Type == TokenType.RwBreak)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.EndStatement)
                    throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return new BreakNodeStatement();
            }
            else if (_currentToken.Type == TokenType.RwContinue)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.EndStatement)
                    throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return new ContinueNodeStatement();
            }
            else if (_currentToken.Type == TokenType.EndStatement)
            {
                _currentToken = _lexer.GetNextToken();
                return new EndNodeStatement();
            }
            else if (_currentToken.Type == TokenType.RwForeach)
            {
                return ForEachStatement();
            }
            else if (_currentToken.Type == TokenType.RwFor)
            {
                return ForStatement();
            }
            else if (_currentToken.Type == TokenType.OpDec)
            {
                _currentToken = _lexer.GetNextToken();
                var primary=PrimaryExpression();
                if (_currentToken.Type != TokenType.EndStatement)
                    throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return new DecrementStatement { ExpressionNode = primary };
            }
            else if (_currentToken.Type == TokenType.OpInc)
            {
                _currentToken = _lexer.GetNextToken();
                var primary = PrimaryExpression();
                if (_currentToken.Type != TokenType.EndStatement)
                    throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return new IncrementStatement { ExpressionNode = primary };
            }
            else if (_currentToken.Type == TokenType.RwVar || _currentToken.Type.IsPredifinedType() || _currentToken.Type == TokenType.RwEnum
                || _currentToken.Type == TokenType.RwBase || _currentToken.Type == TokenType.RwThis || _currentToken.Type == TokenType.Id)
            {
                var x=DeclarationAsignationStatement();
                if (_currentToken.Type != TokenType.EndStatement)
                    throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return x;
            }
            else if (_currentToken.Type == TokenType.ParOpen)
            {
                _currentToken = _lexer.GetNextToken();
                var castExp=CastStatement();
                if (_currentToken.Type != TokenType.Period)
                    throw new SintacticalException("Expected . Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                var idLexeme = _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var y= new  ParentesisExpresionNode{ExpresioNode = castExp,AccesorExpression = new PeriodAccessor { ParentId = idLexeme, Accessor = null } };
                var accessor=IdExpression();
                var z =  new ParentesisExpresionNode{ExpresioNode = y,AccesorExpression = accessor};
                if (_currentToken.Type == TokenType.OpAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();            
                    return new AssignationEqualStatementNode{LeftValue = z, RightValue = right};
                }
                if (_currentToken.Type == TokenType.OpAddAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationSumStatementNode { LeftValue = z, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpSubAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationSubStatementNode { LeftValue = z, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpMulAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationMultStatemntNode { LeftValue = z, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpDivAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationDivStatementNode { LeftValue = z, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpModAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationModStatementNode { LeftValue = z, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpAndAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationAndStatementNode { LeftValue = z, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpOrAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationOrStatementNode { LeftValue = z, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpXorAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationXorStatementNode { LeftValue = z, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpLftShftAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationLftShftStatementNode { LeftValue = z, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpRghtShftAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationRghtShftStatementNode { LeftValue = z, RightValue = right };
                }
                return new AssignationNodeStatement { LeftValue = z };
            }
            else if (_currentToken.Type == TokenType.KeyOpen)
            {
                var list = Block();
                return new BlockNodeStatement(list);
            }
            else
            {
                throw new SintacticalException("Expected Statement Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
            }
        }

        private ExpressionNode CastStatement()
        {

            if (_currentToken.Type.IsPredifinedType() || _currentToken.Type == TokenType.RwEnum)
            {
                var t = CustomTypeProduction();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var p = PrimaryExpression();
                return new CastExpressionNode { Left = t, Right = p };
            }
            else if (_currentToken.Type.IsExpression())
            {
                var exp = Expresion();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return CastStatementPrime(exp);
            }
            else
            {
                throw new SintacticalException("Expected primitive type or Expression Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }

        }

        private ExpressionNode CastStatementPrime(ExpressionNode exp)
        {
            if (_currentToken.Type.IsPrimaryExpression())
            {
                var p = PrimaryExpression();
                return new CastExpressionNode { Left = exp, Right = p };
            }
            else
            {
                return exp;
            }

        }
        private StatementNode DeclarationAsignationStatement()
        {
            if (_currentToken.Type == TokenType.RwVar)
            {
                var type= new VarTypeNode();
                _currentToken = _lexer.GetNextToken();
                var declarationList=DeclaratorsList();
                return new DeclarationNodeStatement{Type = type , DeclarationList = declarationList};

            }
            else if (_currentToken.Type.IsPredifinedType() || _currentToken.Type == TokenType.RwEnum)
            {
                var primitive = PredifinedType();
                return DeclaratorsListOrPrimitiveAccessor(primitive);
            }
            else if (_currentToken.Type == TokenType.RwBase || _currentToken.Type == TokenType.RwThis)
            {
                string preId="";
                if(_currentToken.Type == TokenType.RwBase)
                    preId= "base.";
                else
                    preId = "this.";

               _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Period)
                    throw new SintacticalException("Expected . Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                var idlexeme=_currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var accessorNode=IdExpression();
                var left = new IdLeftExpressionNode();
                left.Accessor = accessorNode;
                left.PreId = preId;
                left.Id = new IdTypeNode(idlexeme, null);
                if (_currentToken.Type == TokenType.OpAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                   
                    return new AssignationEqualStatementNode{LeftValue = left,RightValue = right};
                }
                if (_currentToken.Type == TokenType.OpAddAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationSumStatementNode { LeftValue = left, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpSubAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationSubStatementNode { LeftValue = left, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpMulAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationMultStatemntNode { LeftValue = left, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpDivAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationDivStatementNode { LeftValue = left, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpModAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationModStatementNode { LeftValue = left, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpAndAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationAndStatementNode { LeftValue = left, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpOrAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationOrStatementNode { LeftValue = left, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpXorAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationXorStatementNode { LeftValue = left, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpLftShftAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationLftShftStatementNode { LeftValue = left, RightValue = right };
                }
                if (_currentToken.Type == TokenType.OpRghtShftAsgn)
                {
                    _currentToken = _lexer.GetNextToken();
                    var right = Expresion();
                    
                    return new AssignationRghtShftStatementNode { LeftValue = left, RightValue = right };
                }
                return new AssignationNodeStatement {LeftValue = left};
            }
            else if (_currentToken.Type == TokenType.Id)
            {
                var idlexeme = _currentToken;
                _currentToken = _lexer.GetNextToken();
                return WWWWW(idlexeme);
            }
            else
            {
                throw new SintacticalException("Expected declaration asignation Statement Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private StatementNode DeclaratorsListOrPrimitiveAccessor(PrimitiveTypeNode primitive)
        {
            if (_currentToken.Type == TokenType.Period)
            {
                var idlexeme=_currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var accessor=IdExpression();
                var postId = PostIncrementExpression();
                return new PrimitiveTypeAccessorStatement{Type = primitive,Name = idlexeme,Accesor = accessor,PostId=postId};
            }
            else
            {
                var rank=TypeProductionPrime();
                var typeProduction = new PrimitiveTypeProductionNode {PrimitiveType = primitive ,RankSpecifiers = rank};
                var list=DeclaratorsList();
                return new DeclarationNodeStatement {Type = typeProduction, DeclarationList = list};
            }
        }

        private StatementNode WWWWW(Token idName)
        {
            var x=TypeNamePrime();
            return YYYYY(idName,x);
        }

        private StatementNode YYYYY(Token idName,IdTypeNode idNode)
        {
            if (_currentToken.Type == TokenType.BraOpen)
            {
                _currentToken = _lexer.GetNextToken();
                IdTypeNode idTypeNode = new IdTypeNode(idName, idNode);
                return YYYYYPrime(idTypeNode);
            }
            else if (_currentToken.Type == TokenType.Id)
            {
                var listDeclarations = DeclaratorsList();
                IdTypeNode type = new IdTypeNode(idName,idNode);
               return new DeclarationNodeStatement { Type = type, DeclarationList = listDeclarations };
            }
            else
            {
                var accesorNode=IdExpressionWithoutOpenBra();
                var idleft = new IdLeftExpressionNode();
                idleft.Accessor = accesorNode;
                idleft.PreId = "";
                IdTypeNode type = new IdTypeNode(idName, idNode);
                idleft.Id = type;
                return XXXX(idleft);
            }
        }

        private StatementNode XXXX(ExpressionNode idleft)
        {
            if (_currentToken.Type == TokenType.OpDec)
            {
                _currentToken = _lexer.GetNextToken();
                return new AssignationNodeStatement {LeftValue = idleft,IncOrDec="--"};
            }
            else if (_currentToken.Type == TokenType.OpInc)
            {
                _currentToken = _lexer.GetNextToken();
                return new AssignationNodeStatement { LeftValue = idleft, IncOrDec="++" };
            }
            else if(_currentToken.Type == TokenType.OpAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                return new AssignationEqualStatementNode{LeftValue = idleft,RightValue = right};
            }
            else if(_currentToken.Type == TokenType.OpAddAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                return new AssignationSumStatementNode { LeftValue = idleft, RightValue = right };
            }
            else if(_currentToken.Type == TokenType.OpSubAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();          
                return new AssignationSubStatementNode { LeftValue = idleft, RightValue = right };
            }
            else if(_currentToken.Type == TokenType.OpMulAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                
                return new AssignationMultStatemntNode { LeftValue = idleft, RightValue = right };
            }
            else if(_currentToken.Type == TokenType.OpDivAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                
                return new AssignationDivStatementNode { LeftValue = idleft, RightValue = right };
            }
            else if(_currentToken.Type == TokenType.OpModAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                
                return new AssignationModStatementNode { LeftValue = idleft, RightValue = right };
            }
            else if(_currentToken.Type == TokenType.OpAndAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                
                return new AssignationAndStatementNode { LeftValue = idleft, RightValue = right };
            }
            else if(_currentToken.Type == TokenType.OpOrAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                
                return new AssignationOrStatementNode { LeftValue = idleft, RightValue = right };
            }
            else if(_currentToken.Type == TokenType.OpXorAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
               
                return new AssignationXorStatementNode { LeftValue = idleft, RightValue = right };
            }
            else if(_currentToken.Type == TokenType.OpLftShftAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                
                return new AssignationLftShftStatementNode { LeftValue = idleft, RightValue = right };
            }
            else if(_currentToken.Type == TokenType.OpRghtShftAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
               
                return new AssignationRghtShftStatementNode { LeftValue = idleft, RightValue = right };
            }
            return new AssignationNodeStatement { LeftValue = idleft };
        }

        private AccesorExpressionNode IdExpressionWithoutOpenBra()
        {
            if (_currentToken.Type == TokenType.Period)
            {
                var idlexeme = _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var accessor = IdExpression();
                if (accessor != null)
                    accessor.ParentId = idlexeme;
                return new PeriodAccessor { Id = idlexeme, Accessor = accessor };
            }
            else if (_currentToken.Type == TokenType.ParOpen)
            {
                _currentToken = _lexer.GetNextToken();
                var expresionl = ArgumentList();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var accessor = ParentesisExpression();
                return new ParentesisAccessor { Accessor = accessor, expresionList = expresionl };

            }
            else
            {
                return null;
            }
        }

        private StatementNode YYYYYPrime(IdTypeNode idTypeNode)
        {
            if (_currentToken.Type.IsExpression())
            {
                var accessor = Mierda4();
                var idleft = new IdLeftExpressionNode();
                idleft.Id = idTypeNode;
                idleft.PreId = "";
                idleft.Accessor = accessor;      
                return XXXX(idleft);
            }
            else
            {
                var rankspecifier=RankSpecifierDec();
                var listRankSpecifiers=TypeProductionPrime();
                listRankSpecifiers.Insert(0,rankspecifier);
                var listdeclartion=DeclaratorsList();

                var id = new IdTypeProductionNode();
                id.RankSpecifiers = listRankSpecifiers;
                id.IdType = idTypeNode;

                return new DeclarationNodeStatement {DeclarationList = listdeclartion, Type = id};
            }
        }

        private AccesorExpressionNode Mierda4()
        {
           var expresionList= ExpresionList();
            if (_currentToken.Type != TokenType.BraClose)
                throw new SintacticalException("Expected ] Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var accessor=IdExpression();
            return new BracketAccessor { expresionList = expresionList, Accessor = accessor };
        }

        private RankSpeciferNode RankSpecifierDec()
        {
            var list=DimSpearetorsOpt();
            if (_currentToken.Type != TokenType.BraClose)
                throw new SintacticalException("Expected ] Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            return new RankSpeciferNode { DimSeparatorList = list };

        }

        private List<DeclaratorNode> DeclaratorsList()
        {
            var declarator=Declarator();
            var list=DeclaratorsListPrime();
            list.Insert(0,declarator);
            return list;
        }

        private List<DeclaratorNode> DeclaratorsListPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                var declarator=Declarator();
                var list=DeclaratorsListPrime();
                list.Insert(0,declarator);
                return list;
            }
            else if (_currentToken.Type == TokenType.OpAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var expression=VaraibleInitializer();
                var d= new DeclaratorNode{Expression = expression};
                var list=DeclaratorsListPrime();
                list.Insert(0,d);
                return list;
            }
            else
            {
                return new List<DeclaratorNode>();
            }
        }

        private DeclaratorNode Declarator()
        {
            var idlexeme = _currentToken;
            if (_currentToken.Type != TokenType.Id)

                throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var expression =DeclaratorPrime();
            return new DeclaratorNode{Name = idlexeme,Expression =expression };
        }

        private ExpressionNode DeclaratorPrime()
        {
            if (_currentToken.Type == TokenType.OpAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                return VaraibleInitializer();
            }
            else
            {
                return null;
            }
        }

        private ForNodeStatement ForStatement()
        {  
            if (_currentToken.Type != TokenType.RwFor)
                throw new SintacticalException("Expected for Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            if (_currentToken.Type != TokenType.ParOpen)
                throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var declarationAsignation=ForInitalizer();
            if (_currentToken.Type != TokenType.EndStatement)
                throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var exp=ExpresionOpt();
            if (_currentToken.Type != TokenType.EndStatement)
                throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var  explist = ExpresionListOpt();
            if (_currentToken.Type != TokenType.ParClose)
                throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var statementlist=EmbededStatement();

            return new ForNodeStatement(declarationAsignation,exp,explist,statementlist);
        }

        private List<ExpressionNode> ExpresionListOpt()
        {
            if(_currentToken.Type.IsExpression()){
                return ExpresionList();
            }else{
                return new List<ExpressionNode>();
            }
        }

        private ExpressionNode ExpresionOpt()
        {
            if (_currentToken.Type.IsExpression())
            {
                return Expresion();
            }
            else{
                return null;
            }
        }

        private StatementNode ForInitalizer()
        {
            if (_currentToken.Type == TokenType.RwVar || _currentToken.Type.IsPredifinedType() || _currentToken.Type == TokenType.RwEnum
                || _currentToken.Type == TokenType.RwBase || _currentToken.Type == TokenType.RwThis || _currentToken.Type == TokenType.Id)
                return DeclarationAsignationStatement();
            else
            {
                return null;
            }
        }

        private List<ExpressionNode> ExpresionList()
        {
            if (_currentToken.Type.IsExpression())
            {
                var exp=Expresion();
                var list=ExpresionListPrime();
                list.Insert(0, exp);
                return list;

            }
            else
            {
                throw new SintacticalException("Expected expression Line " + _currentToken.Line + " Col " +
                                              _currentToken.Column);
            }
        }

        private List<ExpressionNode> ExpresionListPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                var exp=Expresion();
                var list=ExpresionListPrime();
                list.Insert(0,exp);
                return list;
            }
            else
            {
                return new List<ExpressionNode>();
            }
        }

        private ForEachNodeStatement ForEachStatement()
        {
            if (_currentToken.Type != TokenType.RwForeach)

                throw new SintacticalException("Expected foreach Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            if (_currentToken.Type != TokenType.ParOpen)
                throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var typenode = LocalVariableType();
            var idlexeme = _currentToken;
            if (_currentToken.Type != TokenType.Id)
                throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            if (_currentToken.Type != TokenType.RwIn)
                throw new SintacticalException("Expected in Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var exp = Expresion();
            if (_currentToken.Type != TokenType.ParClose)
                throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var list=EmbededStatement();

            return new ForEachNodeStatement(typenode, idlexeme, exp, list);
        }

        private TypeExpressionNode LocalVariableType()
        {
            if (_currentToken.Type == TokenType.RwVar)
            {
                _currentToken = _lexer.GetNextToken();
                return new VarTypeNode();
            }
            else if (_currentToken.Type.IsType())
            {
                return TypeProduction();
            }
            else
            {
                throw new SintacticalException("Expected var or type Line " + _currentToken.Line + " Col " +
                                                _currentToken.Column);
            }
        }

        private ReturnNodeStatement ReturnStatement()
        {
            if (_currentToken.Type != TokenType.RwReturn)
                throw new SintacticalException("Expected return Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var exp = ReturnBody();
            if (_currentToken.Type != TokenType.EndStatement)
                throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            return new ReturnNodeStatement(exp);
        }

        private ExpressionNode ReturnBody()
        {
            if (_currentToken.Type.IsExpression())
                return Expresion();
            else
            {
                return null;
            }
        }

        private StatementNode SwitchStatement()
        {
            if (_currentToken.Type != TokenType.RwSwitch)
                throw new SintacticalException("Expected switch Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            if (_currentToken.Type != TokenType.ParOpen)
                throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var expToEvaluate = Expresion();
            if (_currentToken.Type != TokenType.ParClose)
                throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            if (_currentToken.Type != TokenType.KeyOpen)
                throw new SintacticalException("Expected { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var cases = SwitchSections();
            var @default = DefaultStatement();
            if (_currentToken.Type != TokenType.KeyClose)
                throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            return new SwitchStatementNode(expToEvaluate, cases, @default);
        }

        private DefaultCaseNodeStatement DefaultStatement()
        {
            if (_currentToken.Type == TokenType.RwDefault)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Colon)
                    throw new SintacticalException("Expected : Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var list = StatementList();
                var @break = BreakStatement();
                return new DefaultCaseNodeStatement(list, @break);
            }
            else
            {
                return null;
            }
        }

        private List<CaseNodeStatement> SwitchSections()
        {
            if (_currentToken.Type == TokenType.RwCase)
            {
                var @case = SwitchSection();
                var caseList = SwitchSections();
                caseList.Insert(0, @case);
                return caseList;
            }
            else
            {
                return new List<CaseNodeStatement>();
            }
        }

        private CaseNodeStatement SwitchSection()
        {
            var exp = SwitchLables();
            var body = StatementList();
            var @break = BreakStatement();
            return new CaseNodeStatement(exp, body, @break);
        }

        private string BreakStatement()
        {
            if (_currentToken.Type == TokenType.RwBreak)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.EndStatement)
                    throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return "break;";
            }
            else
            {
                return "";
            }
        }

        private ExpressionNode SwitchLables()
        {
            if (_currentToken.Type == TokenType.RwCase)
            {
                _currentToken = _lexer.GetNextToken();
                var exp = Expresion();
                if (_currentToken.Type != TokenType.Colon)
                    throw new SintacticalException("Expected : Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return exp;
            }
            else
            {
                throw new SintacticalException("Expected Case or Default Line " + _currentToken.Line + " Col " +
                                                _currentToken.Column);
            }
        }

        private StatementNode DoStatement()
        {
            if (_currentToken.Type != TokenType.RwDo)
                throw new SintacticalException("Expected Do Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var list = EmbededStatement();
            if (_currentToken.Type != TokenType.RwWhile)
                throw new SintacticalException("Expected while Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            if (_currentToken.Type != TokenType.ParOpen)
                throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var condition = Expresion();
            if (_currentToken.Type != TokenType.ParClose)
                throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();

            if (_currentToken.Type != TokenType.EndStatement)
                throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            return new DoWhileNodeStatement(condition, list);

        }

        private StatementNode WhileStatement()
        {
            if (_currentToken.Type != TokenType.RwWhile)
                throw new SintacticalException("Expected While Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();

            if (_currentToken.Type != TokenType.ParOpen)
                throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var condition = Expresion();
            if (_currentToken.Type != TokenType.ParClose)
                throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var list = EmbededStatement();
            return new WhileNodeStatement(condition, list);
        }

        private IfNodeStatement IfStatement()
        {
            if (_currentToken.Type != TokenType.RwIf)
                throw new SintacticalException("Expected If Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();

            if (_currentToken.Type != TokenType.ParOpen)
                throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var condition = Expresion();
            if (_currentToken.Type != TokenType.ParClose)
                throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            var trueStatements = EmbededStatement();

            if (_currentToken.Type == TokenType.RwElse)
            {
                _currentToken = _lexer.GetNextToken();
                var falseStatements = EmbededStatement();
                return new IfNodeStatement(condition, trueStatements, falseStatements);
            }
            else
            {
                return new IfNodeStatement(condition, trueStatements);
            }

        }

        private List<StatementNode> EmbededStatement()
        {
            if (_currentToken.Type == TokenType.KeyOpen)
            {
                _currentToken = _lexer.GetNextToken();
                var list = StatementList();
                if (_currentToken.Type != TokenType.KeyClose)
                    throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return list;
            }
            else if (_currentToken.Type.IsStatements())
            {
                var x = Statement();
                var list = new List<StatementNode>();
                list.Add(x);
                return list;
            }
            else
            {
                throw new SintacticalException("Expected { or Statement Line " + _currentToken.Line + " Col " +
                                                  _currentToken.Column);
            }
        }

        private ExpressionNode Expresion()
        {
            return ConditionalExpression();
        }

        private ExpressionNode ConditionalExpression()
        {
            var condition = NullCoalescingExpression();
            return Ternary(condition);
        }

        private ExpressionNode Ternary(ExpressionNode condition)
        {
            if (_currentToken.Type == TokenType.OpTernario)
            {
                _currentToken = _lexer.GetNextToken();
                var @true = Expresion();
                if (_currentToken.Type != TokenType.Colon)
                    throw new SintacticalException("Expected : Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var @false = Expresion();
                return new TeranryExpressionNode { Condition = condition, TrueExpression = @true, FalseExpression = @false };
            }
            else
            {
                return condition;
            }
        }

        private ExpressionNode NullCoalescingExpression()
        {
            var left = ConditionalOrExpression();
            return NullCoalescingExpressionPrime(left);
        }

        private ExpressionNode NullCoalescingExpressionPrime(ExpressionNode param)
        {
            if (_currentToken.Type == TokenType.OpCoalescing)
            {
                _currentToken = _lexer.GetNextToken();
                var right = NullCoalescingExpression();
                return new CoalescingExpressionNode(param, right);
            }
            else
            {
                return param;
            }
        }

        private ExpressionNode ConditionalOrExpression()
        {
            var left = ConditionalAndExpression();
            return ConditionalOrExpressionPrime(left);
        }

        private ExpressionNode ConditionalOrExpressionPrime(ExpressionNode param)
        {
            if (_currentToken.Type == TokenType.OpLogicalOr)
            {
                _currentToken = _lexer.GetNextToken();
                var right = ConditionalAndExpression();
                return ConditionalOrExpressionPrime(new LogicalOrExpressionNode{ LeftOperand = param, RightOperand = right });
            }
            else
            {
                return param;
            }
        }

        private ExpressionNode ConditionalAndExpression()
        {
            var left = InclusiveOrExpression();
            return ConditionalAndExpressionPrime(left);
        }

        private ExpressionNode ConditionalAndExpressionPrime(ExpressionNode param)
        {
            if (_currentToken.Type == TokenType.OpLogicalAnd)
            {
                _currentToken = _lexer.GetNextToken();
                var right = InclusiveOrExpression();
                return ConditionalAndExpressionPrime(new LogicalAndExpressionNode{ LeftOperand = param, RightOperand = right });
            }
            else
            {
                return param;
            }
        }

        private ExpressionNode InclusiveOrExpression()
        {
            var left = ExclusiveOrExpression();
            return InclusiveOrExpressionPrime(left);
        }

        private ExpressionNode InclusiveOrExpressionPrime(ExpressionNode param)
        {
            if (_currentToken.Type == TokenType.OpBinaryOr)
            {
                _currentToken = _lexer.GetNextToken();
                var right = ExclusiveOrExpression();
                return InclusiveOrExpressionPrime(new BitOrExpressionNode { LeftOperand = param, RightOperand = right });
            }
            else
            {
                return param;
            }
        }

        private ExpressionNode ExclusiveOrExpression()
        {
            var left = AndExpression();
            return ExclusiveOrExpressionPrime(left);
        }

        private ExpressionNode ExclusiveOrExpressionPrime(ExpressionNode param)
        {
            if (_currentToken.Type == TokenType.OpBinaryXor)
            {
                _currentToken = _lexer.GetNextToken();
                var right = AndExpression();
                return ExclusiveOrExpressionPrime(new BitXorExpressionNode { LeftOperand = param, RightOperand = right });
            }
            else
            {
                return param;
            }
        }

        private ExpressionNode AndExpression()
        {
            var left = EqualityExpression();
            return AndExpressionPrime(left);
        }

        private ExpressionNode AndExpressionPrime(ExpressionNode param)
        {
            if (_currentToken.Type == TokenType.OpLogicalAnd)
            {
                _currentToken = _lexer.GetNextToken();
                var right = EqualityExpression();
                return AndExpressionPrime(new BitAndExpressionNode { LeftOperand = param,RightOperand = right });
            }
            else
            {
                return param;
            }
        }

        private ExpressionNode EqualityExpression()
        {
            var left = RelationalExpresion();
            return EqualityExpressionPrime(left);
        }

        private ExpressionNode EqualityExpressionPrime(ExpressionNode param)
        {
            if (_currentToken.Type == TokenType.OpEquals)
            {
                _currentToken = _lexer.GetNextToken();
                var right = RelationalExpresion();
                return EqualityExpressionPrime(new EqualExpressionNode{ LeftOperand = param, RightOperand = right });
            }
            else if (_currentToken.Type == TokenType.OpNotEquals)
            {
                _currentToken = _lexer.GetNextToken();
                var right = RelationalExpresion();
                return EqualityExpressionPrime(new NotEqualExpressionNode{ LeftOperand = param, RightOperand = right });
            }
            else
            {
                return param;
            }
        }

        private ExpressionNode RelationalExpresion()
        {
            var left = ShiftExpression();
            return RelationalExpresionPrime(left);
        }

        private ExpressionNode RelationalExpresionPrime(ExpressionNode param)
        {
            if (_currentToken.Type == TokenType.OpLessThan)
            {
                _currentToken = _lexer.GetNextToken();
                var right = ShiftExpression();
                return RelationalExpresionPrime(new LessThanExpressionNode{ LeftOperand = param, RightOperand = right });
            }
            else if (_currentToken.Type == TokenType.OpGrtThan)
            {
                _currentToken = _lexer.GetNextToken();
                var right = ShiftExpression();
                return RelationalExpresionPrime(new GreaterThanExpressionNode{ LeftOperand = param, RightOperand = right });
            }
            else if (_currentToken.Type == TokenType.OpLessThanOrEqual)
            {
                _currentToken = _lexer.GetNextToken();
                var right = ShiftExpression();
                return RelationalExpresionPrime(new LessThanOrEqualExpressionNode{ LeftOperand = param, RightOperand = right });
            }
            else if (_currentToken.Type == TokenType.OpGrtThanOrEqual)
            {
                _currentToken = _lexer.GetNextToken();
                var right = ShiftExpression();
                return RelationalExpresionPrime(new GreaterThanOrEqualExpressionNode{ LeftOperand = param, RightOperand = right });
            }
            else if (_currentToken.Type == TokenType.RwAs)
            {
                _currentToken = _lexer.GetNextToken();
                var right = TypeProduction();
                return RelationalExpresionPrime(new AsExpressionNode(param, right));
            }
            else if (_currentToken.Type == TokenType.RwIs)
            {
                _currentToken = _lexer.GetNextToken();
                var right = TypeProduction();
                return RelationalExpresionPrime(new IsExpressionNode(param, right));
            }
            else
            {
                return param;
            }
        }

        private ExpressionNode ShiftExpression()
        {
            var left = AdditiveExpression();
            return ShiftExpressionPrime(left);
        }

        private ExpressionNode ShiftExpressionPrime(ExpressionNode param)
        {
            if (_currentToken.Type == TokenType.OpRghtShft)
            {
                _currentToken = _lexer.GetNextToken();
                var right = AdditiveExpression();
                return ShiftExpressionPrime(new RightShiftExpressionNode{ LeftOperand = param, RightOperand = right });
            }
            else if (_currentToken.Type == TokenType.OpLftShft)
            {
                _currentToken = _lexer.GetNextToken();
                var right = AdditiveExpression();
                return ShiftExpressionPrime(new LeftShiftExpressionNode{ LeftOperand = param, RightOperand = right });
            }
            else
            {
                return param;
            }
        }

        private ExpressionNode AdditiveExpression()
        {
            var left = MultiplicativeExpression();
            return AdditiveExpressionPrime(left);
        }

        private ExpressionNode AdditiveExpressionPrime(ExpressionNode param)
        {
            if (_currentToken.Type == TokenType.OpSum)
            {
                _currentToken = _lexer.GetNextToken();
                var right = MultiplicativeExpression();
                return AdditiveExpressionPrime(new SumExpressionNode { LeftOperand = param, RightOperand = right });
            }
            else if (_currentToken.Type == TokenType.OpSub)
            {
                _currentToken = _lexer.GetNextToken();
                var right = MultiplicativeExpression();
                return AdditiveExpressionPrime(new SubExpressionNode { LeftOperand = param, RightOperand = right });
            }
            else
            {
                return param;
            }
        }

        private ExpressionNode MultiplicativeExpression()
        {
            var unary = UnaryExpression();
            if(unary==""){
				var left = PrimaryExpression();
				return MultiplicativeExpressionPrime(left);
            }else{
				var left = PrimaryExpression();
				var exp =MultiplicativeExpressionPrime(left);
                return new UnaryExpressionNode{ Unary=unary,Expression=exp};
            }

        }

        private ExpressionNode MultiplicativeExpressionPrime(ExpressionNode param)
        {
            if (_currentToken.Type == TokenType.OpMul)
            {
                _currentToken = _lexer.GetNextToken();
                var unary = UnaryExpression();
                if(unary==""){
					var right = PrimaryExpression();
					return MultiplicativeExpressionPrime(new MultExpressionNode { LeftOperand = param, RightOperand = right });
                }else{
					var right = PrimaryExpression();
                    return new UnaryExpressionNode { Unary = unary, Expression = MultiplicativeExpressionPrime(new MultExpressionNode { LeftOperand = param, RightOperand = right }) };
                }
              
            }
            else if (_currentToken.Type == TokenType.OpDiv)
            {
                _currentToken = _lexer.GetNextToken();
                var unary = UnaryExpression();
                if(unary==""){
					var right = PrimaryExpression();
					return MultiplicativeExpressionPrime(new DivExpressionNode { LeftOperand = param, RightOperand = right });
                }else{
                    var right = PrimaryExpression();
                    return new UnaryExpressionNode { Unary = unary, Expression = MultiplicativeExpressionPrime(new DivExpressionNode { LeftOperand = param, RightOperand = right }) };
                }
               
            }
            else if (_currentToken.Type == TokenType.OpMod)
            {
                _currentToken = _lexer.GetNextToken();
                var unary = UnaryExpression();
                if(unary==""){
					var right = PrimaryExpression();
					return MultiplicativeExpressionPrime(new ModExpressionNode { LeftOperand = param, RightOperand = right });
                }else{
					var right = PrimaryExpression();
                    return new UnaryExpressionNode { Unary = unary, Expression = MultiplicativeExpressionPrime(new ModExpressionNode { LeftOperand = param, RightOperand = right }) };
                }

            }
            else
            {
                return param;
            }
        }

        private string UnaryExpression()
        {
            if (_currentToken.Type == TokenType.OpSum)
            {
                var unary = _currentToken;
                _currentToken = _lexer.GetNextToken();
                return unary.Lexeme;
            }
            else if (_currentToken.Type == TokenType.OpSub)
            {
                var unary = _currentToken;
                _currentToken = _lexer.GetNextToken();
                return unary.Lexeme;
            }
            else if (_currentToken.Type == TokenType.OpLogicalNot)
            {
                var unary = _currentToken;
                _currentToken = _lexer.GetNextToken();
                return unary.Lexeme;
            }
            else if (_currentToken.Type == TokenType.OpBinaryComplement)
            {
                var unary = _currentToken;
                _currentToken = _lexer.GetNextToken();
                return unary.Lexeme;
            }
            else if (_currentToken.Type == TokenType.OpInc)
            {
                var unary = _currentToken;
                _currentToken = _lexer.GetNextToken();
                return unary.Lexeme;
            }
            else if (_currentToken.Type == TokenType.OpDec)
            {
                var unary = _currentToken;
                _currentToken = _lexer.GetNextToken();
                return unary.Lexeme;
            }
            else
            {
                return "";
            }
        }
        private ExpressionNode PrimaryExpression()
        {
            if (_currentToken.Type == TokenType.RwNew)
            {
                _currentToken = _lexer.GetNextToken();
                 return ArrayOrObject();
            }
            else if (_currentToken.Type.IsPrimaryNoArrayCreationExpression())
            {
                return PrimaryNoArrayCreationExpression();
            }
            else if (_currentToken.Type == TokenType.ParOpen)
            {
                _currentToken = _lexer.GetNextToken();
                return CastOrExpression();
            }
            else
            {
                throw new SintacticalException("Expected valid Primary Expression Line " + _currentToken.Line +
                                               " Col " +
                                               _currentToken.Column);
            }
        }

        private AccesorExpressionNode IdExpression()
        {
            if (_currentToken.Type == TokenType.Period)
            {
                var idlexeme =_currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var accessor=IdExpression();
                if (accessor != null)
                    accessor.ParentId = idlexeme;
                return new PeriodAccessor { Id=idlexeme, Accessor= accessor };
            }
            else if (_currentToken.Type == TokenType.BraOpen)
            {
                _currentToken = _lexer.GetNextToken();
                var expresionl=ExpresionList();
                if (_currentToken.Type != TokenType.BraClose)
                    throw new SintacticalException("Expected ] Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var accessor=IdExpression();
                return new BracketAccessor { expresionList = expresionl, Accessor = accessor };
            }
            else if (_currentToken.Type == TokenType.ParOpen)
            {
                _currentToken = _lexer.GetNextToken();
                var expresionl=ArgumentList();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var accessor=ParentesisExpression();
                return new ParentesisAccessor { Accessor = accessor, expresionList = expresionl };

            }
            else
            {
                return null;
            }
        }

        private AccesorExpressionNode ParentesisExpression()
        {
            if (_currentToken.Type == TokenType.Period)
            {
				var idlexeme = _currentToken = _lexer.GetNextToken();
				if (_currentToken.Type != TokenType.Id)
					throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
												   _currentToken.Column);
				_currentToken = _lexer.GetNextToken();
				var accessor = IdExpression();
				if (accessor != null)
					accessor.ParentId = idlexeme;
				return new PeriodAccessor { Id = idlexeme, Accessor = accessor };
            }
            else if (_currentToken.Type == TokenType.BraOpen)
            {
				_currentToken = _lexer.GetNextToken();
				var expresionl = ExpresionList();
				if (_currentToken.Type != TokenType.BraClose)
					throw new SintacticalException("Expected ] Line " + _currentToken.Line + " Col " +
												   _currentToken.Column);
				_currentToken = _lexer.GetNextToken();
				var accessor = IdExpression();
				return new BracketAccessor { expresionList = expresionl, Accessor = accessor };
            }
            else
            {
                return null;
            }
        }

        private NewExpressionNode ArrayOrObject()
        {
            if (_currentToken.Type.IsType())
            {
                var type=TypeProductionForArrayOrObject();
                var x=ArrayOrObjectPrime();
                var accesor=IdExpression();
                return new NewCreationExpressionNode { Type=type ,NewCreationNode=x,Accessor=accesor};
            }
            else if (_currentToken.Type == TokenType.BraOpen)
            {
                var rankspecifier=RankSpecifier();
                var arraynode=ArrayInitalizer();
                return new NewArrayInitalizerNode { RankSpecifer = rankspecifier,ArrayInitalizerNode  = arraynode};
            }
            else
            {
                throw new SintacticalException("Expected type or [  Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private NewExpressionNode ArrayOrObjectPrime()
        {
            if (_currentToken.Type == TokenType.BraOpen)
            {
                return ArrayCreationExpression();
            }
            else if (_currentToken.Type == TokenType.ParOpen || _currentToken.Type == TokenType.KeyOpen)
            {
                return ObjectCreationExpression();
            }
            else
            {
                throw new SintacticalException("Expected (, { or [  Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private NewExpressionNode ArrayCreationExpression()
        {
            if (_currentToken.Type != TokenType.BraOpen)
                throw new SintacticalException("Expected [ Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            return PostArrayCreationExpression();
        }

        private NewExpressionNode PostArrayCreationExpression()
        {
            if (_currentToken.Type.IsExpression())
            {
                var explist = ExpresionList();
                var bracketAccessor = new BracketAccessor { expresionList = explist };
                if (_currentToken.Type != TokenType.BraClose)
                    throw new SintacticalException("Expected ] Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                List<RankSpeciferNode> rankSpeciferNodes = new List<RankSpeciferNode>();
				ArrayInitalizerNode arrayNode = null;
                if (_currentToken.Type == TokenType.BraOpen)
                    rankSpeciferNodes=RankSpecifiers();
                if (_currentToken.Type == TokenType.KeyOpen)
                    arrayNode=ArrayInitalizer();

                var r = new RankSpeciferNode();
                rankSpeciferNodes.Insert(0,r);
                return new NewArrayCreation
                {
                    Bracket=bracketAccessor,
                    RankSpecifiers = rankSpeciferNodes,
                    ArrayInitalizer = arrayNode
                };
            }
            else
            {
                var dimlist=DimSpearetorsOpt();
                if (_currentToken.Type != TokenType.BraClose)
                    throw new SintacticalException("Expected ] Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var rankspec = new RankSpeciferNode { DimSeparatorList=dimlist};
                var ranklist=RankSpecifiersPrime();
                ranklist.Insert(0,rankspec);
                var arrayNode=ArrayInitalizer();

                return new NewArrayCreation
                {
                    RankSpecifiers = ranklist,
                    ArrayInitalizer = arrayNode
                };

            }
        }

        private NewObjectCreation ObjectCreationExpression()
        {
            if (_currentToken.Type == TokenType.ParOpen)
            {
                _currentToken = _lexer.GetNextToken();
                var expresionlist=ArgumentList();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var objectCollection=ObjectCollectionInitalizerOpt();
                return new NewObjectCreation{ObjectArgumentsList = expresionlist,ObjectCollectionInitalizer = objectCollection};
            }
            else if (_currentToken.Type == TokenType.KeyOpen)
            {
                var list = ObjectCollectionInitalizer();
                return new NewObjectCreation {ObjectCollectionInitalizer = list};

            }
            else
            {
                throw new SintacticalException("Expected (, {  Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private List<ExpressionNode> ObjectCollectionInitalizerOpt()
        {
            if (_currentToken.Type == TokenType.KeyOpen)
            {
                return ObjectCollectionInitalizer();
            }
            else
            {
                return new List<ExpressionNode>();
            }
        }

        private List<ExpressionNode> ObjectCollectionInitalizer()
        {
            if (_currentToken.Type == TokenType.KeyOpen)
            {
                _currentToken = _lexer.GetNextToken();
                return ObjectCollectionInitalizerBody();
            }
            else
            {
                throw new SintacticalException("Expected { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private List<ExpressionNode> ObjectCollectionInitalizerBody()
        {
            if (_currentToken.Type == TokenType.Id)
            {
                return MemberOrElement();
            }
            else
            {
                var e=ElementInitalizerList();
                if (_currentToken.Type != TokenType.KeyClose)
                    throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                                       _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return e;
            }
        }

        private List<ExpressionNode> MemberOrElement()
        {
            if (_currentToken.Type == TokenType.Id)
            {
                var id = _currentToken;
                _currentToken = _lexer.GetNextToken();
                var memberList=MemberInitalizerList(id);
                if (_currentToken.Type != TokenType.KeyClose)
                    throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                                       _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return memberList;
            }
            else if (_currentToken.Type.IsExpression() || _currentToken.Type == TokenType.ParOpen)
            {
                var elementlist=ElementInitalizerList();
                if (_currentToken.Type != TokenType.KeyClose)
                    throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                                       _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return elementlist;
            }
            else
            {
                if (_currentToken.Type != TokenType.KeyClose)
                    throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                                       _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return new List<ExpressionNode>();
            }
        }

        private List<ExpressionNode>  MemberInitalizerList(Token id)
        {
            var member=MemberInitalizer(id);
            var list=MemberInitalizerListPrime();
            list.Insert(0,member);
            return list;
        }

        private List<ExpressionNode> MemberInitalizerListPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                var id=_currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                       _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var member=MemberInitalizer(id);
                var list=MemberInitalizerListPrime();
                list.Insert(0,member);
                return list;
            }
            else
            {
                return new List<ExpressionNode>();
            }
        }

        private MemberNode MemberInitalizer(Token id)
        {
            if (_currentToken.Type == TokenType.OpAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var exp = InitalizerValue();
                return new MemberNode {Name = id, Expression = exp};
            }
            else
            {
                return new MemberNode {Name = id};
            }                         
        }

        private ExpressionNode InitalizerValue()
        {
            if (_currentToken.Type.IsExpression())
            {
                return Expresion();
            }
            else if (_currentToken.Type == TokenType.KeyOpen)
            {
                return new InitalizerValueNode {ExpressionList=ObjectCollectionInitalizer()};
            }
            else
            {
                throw new SintacticalException("Expected Expression or { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private List<ExpressionNode> ElementInitalizerList()
        {
            var exp=ElementInitalizer();
            var l2=ElementInitalizerListPrime();
            l2.Insert(0,exp);
            return l2;
        }

        private List<ExpressionNode> ElementInitalizerListPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                var exp=ElementInitalizer();
                var listb=ElementInitalizerListPrime();
                listb.Insert(0,exp);
                return listb;
            }
            else
            {
                return new List<ExpressionNode>();
            }
        }

        private ExpressionNode ElementInitalizer()
        {
            if (_currentToken.Type.IsExpression())
            {
                return Expresion();

            }
            else if (_currentToken.Type == TokenType.KeyOpen)
            {
                _currentToken = _lexer.GetNextToken();
                var expList=ExpresionList();
                if (_currentToken.Type != TokenType.KeyClose)
                    throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return new ElementLinitalizerNode{ExpressionList=expList};
            }
            else
            {
                throw new SintacticalException("Expected Expresion or { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private ExpressionNode CastOrExpression()
        {
            if (_currentToken.Type.IsPredifinedType() || _currentToken.Type == TokenType.RwEnum)
            {
                var t=CustomTypeProduction();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var p=PrimaryExpression();
                return new CastExpressionNode {Left = t,Right = p};

            }
            else if (_currentToken.Type.IsExpression())
            {
                var exp=Expresion();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return CastOrExpressionPrime(exp);
            }
            else
            {
                throw new SintacticalException("Expected primitive type or Expression Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private ExpressionNode CastOrExpressionPrime(ExpressionNode exp)
        {
            if (_currentToken.Type.IsPrimaryExpression())
            {
                var p= PrimaryExpression();
                return new CastExpressionNode {Left = exp, Right = exp};
            }
            else if (_currentToken.Type == TokenType.Period || _currentToken.Type == TokenType.ParOpen || _currentToken.Type == TokenType.BraOpen)
            {

                var accessor= IdExpression();
                return new ParentesisExpresionNode {ExpresioNode = exp,AccesorExpression = accessor};

            }
            else
            {
                return exp;
            }
        }

        private ExpressionNode PrimaryNoArrayCreationExpression()
        {

            if (_currentToken.Type == TokenType.LitBool)
            {
                var lit = _currentToken;
                _currentToken = _lexer.GetNextToken();
                return new BoolLiteralExpressionNode(lit);
            }
            if (_currentToken.Type == TokenType.LitChar)
            {
                var lit = _currentToken;
                _currentToken = _lexer.GetNextToken();
                return new CharLiteralExpressionNode(lit);
            }
            if (_currentToken.Type == TokenType.LitNum)
            {
                var lit = _currentToken;
                _currentToken = _lexer.GetNextToken();
                return new IntLiteralExpressionNode(lit);
            }
            if (_currentToken.Type == TokenType.LitFloat)
            {
                var lit = _currentToken;
                _currentToken = _lexer.GetNextToken();
                return new FloatLiteralExpressionNode(lit);
            }
            if (_currentToken.Type == TokenType.LitString)
            {
                var lit = _currentToken;
                _currentToken = _lexer.GetNextToken();
                return new StringLiteralExpressionNode(lit);
            }
            if (_currentToken.Type == TokenType.RwThis || _currentToken.Type == TokenType.RwBase || _currentToken.Type == TokenType.Id)
            {
                var preIdNode = PreIdExpression();
                var name = _currentToken;
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var accessor = IdExpression();
                var idLeft= new IdLeftExpressionNode(preIdNode,new IdTypeNode(name,null), accessor);
                var assgnExp = AssignmentInExpression(idLeft);
                var postId = PostIncrementExpression();
                return new IdExpressionNode(idLeft, assgnExp, postId);

            }
            if (_currentToken.Type.IsPredifinedType() || _currentToken.Type == TokenType.RwEnum)
            {
                var temp = _currentToken.Type;
                PrimitiveTypeNode primitiveNode = null;
                if (temp == TokenType.RwBool)
                {
                    primitiveNode = new PrimitiveBoolNode();
                }
                if (temp == TokenType.RwChar)
                {
                    primitiveNode = new PrimitiveCharNode();
                }
                if (temp == TokenType.RwInt)
                {
                    primitiveNode = new PrimitiveIntNode();
                }
                if (temp == TokenType.RwFloat)
                {
                    primitiveNode = new PrimitiveFloatNode();
                }
                if (temp == TokenType.RwString)
                {
                    primitiveNode = new PrimitiveStringNode();
                }
                if (temp == TokenType.RwEnum)
                {
                    primitiveNode = new PrimitiveEnumNode();
                }
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Period)
                    throw new SintacticalException("Expected . Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                var name=_currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                var accessor = IdExpression();
                var post = PostIncrementExpression();
                return new PrimitiveTypeExpressionNode(primitiveNode, name, accessor, post);
            }
            else
            {
                throw new SintacticalException("Expected Primary No Array Creation Expression Line " +
                                               _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private AssignationExpressionNode AssignmentInExpression(IdLeftExpressionNode left)
        {

            if (_currentToken.Type == TokenType.OpAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                
                return new AssignationEqualExpressionNode{LeftValue = left,RightValue = right};
            }
            if (_currentToken.Type == TokenType.OpAddAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
               
                return new AssignationSumExpressionNode { LeftValue = left, RightValue = right };
            }
            if (_currentToken.Type == TokenType.OpSubAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                
                return new AssignationSubExpressionNode { LeftValue = left, RightValue = right };
            }
            if (_currentToken.Type == TokenType.OpMulAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                
                return new AssignationMultExpressionNode { LeftValue = left, RightValue = right };
            }
            if (_currentToken.Type == TokenType.OpDivAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                
                return new AssignationDivExpressionNode { LeftValue = left, RightValue = right };
            }
            if (_currentToken.Type == TokenType.OpModAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                
                return new AssignationModExpressionNode { LeftValue = left, RightValue = right };
            }
            if (_currentToken.Type == TokenType.OpAndAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                
                return new AssignationAndExpressionNode { LeftValue = left, RightValue = right };
            }
            if (_currentToken.Type == TokenType.OpOrAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                
                return new AssignationOrExpressionNode { LeftValue = left, RightValue = right };
            }
            if (_currentToken.Type == TokenType.OpXorAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                
                return new AssignationXorExpressionNode { LeftValue = left, RightValue = right };
            }
            if (_currentToken.Type == TokenType.OpLftShftAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                
                return new AssignationLftShftExpressionNode { LeftValue = left, RightValue = right };
            }
            if (_currentToken.Type == TokenType.OpRghtShftAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                var right = Expresion();
                
                return new AssignationRghtShftExpressionNode { LeftValue = left, RightValue = right };
            }
            else
            {
                return null;
            }
        }

        private string PreIdExpression()
        {
            if (_currentToken.Type == TokenType.RwThis)
            {
                var @this = _currentToken;
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Period)
                    throw new SintacticalException("Expected . Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return "this.";
            }
            else if (_currentToken.Type == TokenType.RwBase)
            {
                var @base = _currentToken;
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Period)
                    throw new SintacticalException("Expected . Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                return "base.";
            }
            else
            {
                return "";
            }
        }

        private string PostIncrementExpression()
        {
            if (_currentToken.Type == TokenType.OpInc)
            {
                _currentToken = _lexer.GetNextToken();
                return "++";
            }
            else if (_currentToken.Type == TokenType.OpDec)
            {
                _currentToken = _lexer.GetNextToken();
                return "--";
            }
            else
            {
                return "";
            }
        }
    }
}