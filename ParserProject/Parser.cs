﻿using System;
using LexerProject;
using LexerProject.Tokens;
using ParserProject.BinaryOperators.ExpressionNodes.Nodes;
using ParserProject.Exceptions;
using ParserProject.Extensions;
using ParserProject.Nodes.ExpressionNodes;
using ParserProject.Nodes.ExpressionNodes.BinaryOperators;
using ParserProject.Nodes.ExpressionNodes.UnaryNodes;

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

        public void Parse()
        {
            Code();
            if (_currentToken.Type != TokenType.Eof)
                throw new SintacticalException("Expected Eof");
        }

        private void Code()
        {
            NameSpaceList();
        }

        private void NameSpaceList()
        {
            if (_currentToken.Type.IsNameSpace())
            {
                NameSpace();
                NameSpaceList();
            }
            else
            {

            }

        }

        private void NameSpace()
        {
            UsingDirectives();
            NameSpaceDeclarations();
        }

        private void UsingDirectives()
        {
            if (_currentToken.Type == TokenType.RwUsing)
            {
                UsingDirective();
                UsingDirectives();
            }
            else
            {

            }
        }

        private void UsingDirective()
        {
            if (_currentToken.Type != TokenType.RwUsing)
                throw new SintacticalException("Expected using Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            TypeName();
            if (_currentToken.Type != TokenType.EndStatement)
                throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " + _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
        }

        private void TypeName()
        {
            if (_currentToken.Type != TokenType.Id)
                throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            TypeNamePrime();
        }

        private void TypeNamePrime()
        {
            if (_currentToken.Type == TokenType.Period)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                TypeNamePrime();
            }
            else
            {

            }
        }

        private void NameSpaceDeclarations()
        {
            if (_currentToken.Type.IsPrivacyModifier() || _currentToken.Type == TokenType.RwNamespace || _currentToken.Type.IsClassModifier() || _currentToken.Type == TokenType.RwClass
                || _currentToken.Type == TokenType.RwInterface || _currentToken.Type == TokenType.RwEnum)
            {
                NameSpaceDeclaration();
                NameSpaceDeclarations();
            }
            else
            {

            }
        }

        private void NameSpaceDeclaration()
        {
            if (_currentToken.Type == TokenType.RwNamespace)
            {
                NameSpaceStatement();
            }
            else
            {
                PrivacyModifier();
                ClassInterfaceEnum();
            }

        }

        private void PrivacyModifier()
        {
            if (_currentToken.Type.IsPrivacyModifier())
                _currentToken = _lexer.GetNextToken();
            else
            {

            }
        }

        private void NameSpaceStatement()
        {
            if (_currentToken.Type != TokenType.RwNamespace)
                throw new SintacticalException("Expected Namespace Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            TypeName();
            NameSpaceBody();

        }

        private void NameSpaceBody()
        {
            if (_currentToken.Type != TokenType.KeyOpen)
                throw new SintacticalException("Expected { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            NameSpace();
            if (_currentToken.Type != TokenType.KeyClose)
                throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
        }

        private void ClassInterfaceEnum()
        {
            if (_currentToken.Type == TokenType.RwInterface)
                InterfaceDeclaration();
            else if (_currentToken.Type == TokenType.RwEnum)
                EnumDeclaration();
            else if (_currentToken.Type == TokenType.RwClass || _currentToken.Type.IsClassModifier())
                ClassDeclaration();
            else
                throw new SintacticalException("Expected inteface,enum,class or class modifiers Line " + _currentToken.Line + " Col " +
                                                _currentToken.Column);
        }

        private void InterfaceDeclaration()
        {
            if (_currentToken.Type != TokenType.RwInterface)
                throw new SintacticalException("Expected interface Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            if (_currentToken.Type != TokenType.Id)
                throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            Heredance();
            InterfaceBody();
        }

        private void InterfaceBody()
        {
            if (_currentToken.Type != TokenType.KeyOpen)
                throw new SintacticalException("Expected { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            InterfaceMemberDeclarations();
            if (_currentToken.Type != TokenType.KeyClose)
                throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
        }

        private void InterfaceMemberDeclarations()
        {
            if (_currentToken.Type.IsType() || _currentToken.Type == TokenType.RwVoid)
            {
                InterfaceElement();
                InterfaceMemberDeclarations();
            }
            else
            {

            }
        }

        private void InterfaceElement()
        {
            if (_currentToken.Type.IsType())
            {
                TypeProduction();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                MethodProperty();
            }
            else if (_currentToken.Type == TokenType.RwVoid)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                MethodProperty();
            }
            else
            {
                throw new SintacticalException("Expected type or void Line " + _currentToken.Line + " Col " +
                                                _currentToken.Column);
            }
        }

        private void MethodProperty()
        {
            if (_currentToken.Type == TokenType.ParOpen)
            {
                MethodHeader();
                if (_currentToken.Type != TokenType.EndStatement)
                    throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
            }
            else
            {
                throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                                _currentToken.Column);
            }
        }


        private void MethodHeader()
        {
            if (_currentToken.Type != TokenType.ParOpen)
                throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            FormalParameterList();
            if (_currentToken.Type != TokenType.ParClose)
                throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
        }

        private void FormalParameterList()
        {
            if (_currentToken.Type.IsType())
            {
                Parameter();
                FormalParameterListPrime();
            }
            else
            {

            }
        }

        private void FormalParameterListPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                Parameter();
                FormalParameterListPrime();
            }
            else
            {

            }
        }

        private void Parameter()
        {
            TypeProduction();
            if (_currentToken.Type != TokenType.Id)
                throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
        }



        private void EnumDeclaration()
        {
            if (_currentToken.Type != TokenType.RwEnum)
                throw new SintacticalException("Expected enum Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            if (_currentToken.Type != TokenType.Id)
                throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            EnumBody();
        }

        private void EnumBody()
        {
            if (_currentToken.Type != TokenType.KeyOpen)
                throw new SintacticalException("Expected { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            EnumMemberDeclaration();
            if (_currentToken.Type != TokenType.KeyClose)
                throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
        }

        private void EnumMemberDeclaration()
        {
            if (_currentToken.Type == TokenType.Id)
            {
                EnumElement();
                EnumMemberDeclarationPrime();
            }
            else
            {

            }
        }

        private void EnumMemberDeclarationPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                EnumElement();
                EnumMemberDeclarationPrime();
            }
            else
            {

            }
        }

        private void EnumElement()
        {
            if (_currentToken.Type != TokenType.Id)
                throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            EnumElementBody();
        }

        private void EnumElementBody()
        {
            if (_currentToken.Type == TokenType.OpAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                Expresion();
            }
            else
            {

            }
        }


        private void ClassDeclaration()
        {
            ClassModifier();
            if (_currentToken.Type != TokenType.RwClass)
                throw new SintacticalException("Expected class Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            if (_currentToken.Type != TokenType.Id)
                throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            Heredance();
            ClassBody();

        }

        private void ClassBody()
        {
            if (_currentToken.Type != TokenType.KeyOpen)
                throw new SintacticalException("Expected { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            ClassMemberDeclarations();
            if (_currentToken.Type != TokenType.KeyClose)
                throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
        }

        private void ClassMemberDeclarations()
        {


            if (_currentToken.Type.IsPrivacyModifier() || _currentToken.Type == TokenType.RwStatic || _currentToken.Type.IsType() || _currentToken.Type == TokenType.RwVoid
              || _currentToken.Type.IsMethodModifiers())
            {

                ClassMemberDeclaration();

                ClassMemberDeclarations();
            }
            else
            {

            }
        }

        private void ClassMemberDeclaration()
        {
            PrivacyModifier();
            if (_currentToken.Type == TokenType.RwAbstract)
            {
                _currentToken = _lexer.GetNextToken();
                InterfaceElement();
            }
            else
            {
                FieldMethodPropertyConstructor();
            }
        }

        private void FieldMethodPropertyConstructor()
        {
            if (_currentToken.Type == TokenType.RwStatic)
            {
                _currentToken = _lexer.GetNextToken();
                StaticOptions();
            }
            else if (_currentToken.Type.IsPredifinedType() || _currentToken.Type == TokenType.RwEnum)
            {
                CustomTypeProduction();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                FieldMethodPropertyDeclaration();
            }
            else if (_currentToken.Type == TokenType.RwVoid)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                MethodDeclaration();
            }
            else if (_currentToken.Type.IsMethodModifiers())
            {
                MethodModifiers();
                MethodReturn();
            }
            else if (_currentToken.Type == TokenType.Id)
            {
                TypeName();
                TypeProductionPrime();
                Mierda3();
            }
            else
            {
                throw new SintacticalException("Expected Primitive type, enum , void, static , Id or method modifiers Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
            }
        }

        private void Mierda3()
        {
            if (_currentToken.Type == TokenType.Id)
            {
                _currentToken = _lexer.GetNextToken();
                FieldMethodPropertyDeclaration();
            }
            else if (_currentToken.Type == TokenType.ParOpen)
            {
                if (_currentToken.Type != TokenType.ParOpen)
                    throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                FormalParameterList();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                ConstructorInitializer();
                Block();
            }
            else
            {
                throw new SintacticalException("Expected Id or ( Line " + _currentToken.Line + " Col " +
                                                     _currentToken.Column);
            }

        }

        private void ConstructorInitializer()
        {
            if (_currentToken.Type == TokenType.Colon)
            {
                _currentToken = _lexer.GetNextToken();
                ConstructorInitializerPrime();
            }
            else
            {

            }
        }

        private void ConstructorInitializerPrime()
        {
            if (_currentToken.Type == TokenType.RwBase || _currentToken.Type == TokenType.RwThis)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.ParOpen)
                    throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                ArgumentList();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
            }
        }

        private void ArgumentList()
        {
            if (_currentToken.Type.IsExpression())
            {
                Expresion();
                ArgumentListPrime();
            }
            else
            {

            }
        }

        private void ArgumentListPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                Expresion();
                ArgumentListPrime();
            }
            else
            {

            }
        }

        private void MethodReturn()
        {
            if (_currentToken.Type == TokenType.RwVoid)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                MethodDeclaration();
            }
            else if (_currentToken.Type.IsType())
            {
                TypeProduction();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                MethodPropertyDeclaration();
            }
            else
            {
                throw new SintacticalException("Expected void or type Line " + _currentToken.Line + " Col " +
                                                  _currentToken.Column);
            }
        }

        private void MethodPropertyDeclaration()
        {
            if (_currentToken.Type == TokenType.ParOpen)
            {
                MethodDeclaration();
            }
            else
            {
                throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                                  _currentToken.Column);
            }
        }

        private void StaticOptions()
        {
            if (_currentToken.Type.IsPredifinedType() || _currentToken.Type == TokenType.RwEnum)
            {
                CustomTypeProduction();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                FieldMethodPropertyDeclaration();
            }
            else if (_currentToken.Type == TokenType.RwVoid)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                MethodDeclaration();
            }
            else if (_currentToken.Type == TokenType.Id)
            {
                TypeName();
                TypeProductionPrime();
                Mierda2();
            }
            else
            {
                throw new SintacticalException("Expected Primitive type ,enum,void or id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
            }
        }

        private void Mierda2()
        {
            if (_currentToken.Type == TokenType.ParOpen)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                Block();
            }
            else if (_currentToken.Type == TokenType.Id)
            {
                _currentToken = _lexer.GetNextToken();
                FieldMethodPropertyDeclaration();
            }
            else
            {
                throw new SintacticalException("Expected ( or Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
            }

        }

        private void CustomTypeProduction()
        {
            if (_currentToken.Type.IsPredifinedType())
            {
                PredifinedType();
                TypeProductionPrime();
            }
            else if (_currentToken.Type == TokenType.RwEnum)
            {
                _currentToken = _lexer.GetNextToken();
                TypeProductionPrime();
            }
            else
            {
                throw new SintacticalException("Expected Primitve type or Enum Line " + _currentToken.Line + " Col " +
                                                     _currentToken.Column);
            }

        }

        private void FieldMethodPropertyDeclaration()
        {
            if (_currentToken.Type == TokenType.ParOpen)
            {
                MethodDeclaration();
            }
            else
            {
                FieldDeclaration();
            }

        }

        private void FieldDeclaration()
        {
            FieldDeclarations();
            if (_currentToken.Type != TokenType.EndStatement)
                throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
        }

        private void FieldDeclarations()
        {
            FieldAssignation();
            FieldDeclarationsPrime();
        }

        private void FieldDeclarationsPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                FieldAssignation();
                FieldDeclarationsPrime();
            }
            else
            {

            }
        }

        private void FieldAssignation()
        {
            if (_currentToken.Type == TokenType.OpAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                VaraibleInitializer();
            }
            else
            {

            }

        }

        private void VaraibleInitializer()
        {
            if (_currentToken.Type == TokenType.KeyOpen)
            {
                ArrayInitalizer();
            }
            else if (_currentToken.Type.IsExpression())
            {
                Expresion();
            }
            else
            {
                throw new SintacticalException("Expected valid variable initalizer Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private void ArrayInitalizer()
        {
            if (_currentToken.Type != TokenType.KeyOpen)
                throw new SintacticalException("Expected { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            VaraibleInitializerListOpt();
            if (_currentToken.Type != TokenType.KeyClose)
                throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
        }



        private void MethodDeclaration()
        {
            MethodHeader();
            Block();
        }



        private void MethodModifiers()
        {
            if (_currentToken.Type.IsMethodModifiers())
                _currentToken = _lexer.GetNextToken();
        }


        private void Heredance()
        {
            if (_currentToken.Type == TokenType.Colon)
            {
                _currentToken = _lexer.GetNextToken();
                TypeName();
                Base();
            }
            else
            {

            }
        }

        private void Base()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                TypeName();
                Base();
            }
            else
            {

            }


        }

        private void ClassModifier()
        {
            if (_currentToken.Type.IsClassModifier())
                _currentToken = _lexer.GetNextToken();
            else
            {

            }
        }


        private void TypeProduction()
        {
            if (_currentToken.Type == TokenType.Id)
            {
                TypeName();
                TypeProductionPrime();
            }
            else if (_currentToken.Type.IsPredifinedType())
            {
                PredifinedType();
                TypeProductionPrime();

            }
            else if (_currentToken.Type == TokenType.RwEnum)
            {
                _currentToken = _lexer.GetNextToken();
                TypeProductionPrime();
            }
            else
            {
                throw new SintacticalException("Expected valid type Line " + _currentToken.Line + " Col " +
                                              _currentToken.Column);
            }
        }




        private void TypeProductionForArrayOrObject()
        {
            if (_currentToken.Type == TokenType.Id)
            {
                TypeName();
            }
            else if (_currentToken.Type.IsPredifinedType())
            {
                PredifinedType();

            }
            else if (_currentToken.Type == TokenType.RwEnum)
            {
                _currentToken = _lexer.GetNextToken();
            }
            else
            {
                throw new SintacticalException("Expected valid type Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private void TypeProductionPrime()
        {
            if (_currentToken.Type == TokenType.BraOpen)
            {
                RankSpecifiers();
                TypeProductionPrime();
            }
            else
            {

            }
        }

        private void RankSpecifiers()
        {
            RankSpecifier();
            RankSpecifiersPrime();
        }

        private void RankSpecifiersPrime()
        {
            if (_currentToken.Type == TokenType.BraOpen)
            {
                RankSpecifier();
                RankSpecifiersPrime();
            }
            else
            {

            }
        }

        private void RankSpecifier()
        {
            if (_currentToken.Type != TokenType.BraOpen)
                throw new SintacticalException("Expected [ Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            DimSpearetorsOpt();
            if (_currentToken.Type != TokenType.BraClose)
                throw new SintacticalException("Expected ] Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
        }

        private void DimSpearetorsOpt()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                DimSpearetors();
            }
            else
            {

            }
        }

        private void DimSpearetors()
        {
            if (_currentToken.Type != TokenType.Comma)
                throw new SintacticalException("Expected , Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            DimSpearetorsPrime();
        }

        private void DimSpearetorsPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                DimSpearetorsPrime();
            }
            else
            {

            }
        }

        private void PredifinedType()
        {
            if (_currentToken.Type.IsPredifinedType())
                _currentToken = _lexer.GetNextToken();
        }


        private void Block()
        {
            if (_currentToken.Type != TokenType.KeyOpen)
                throw new SintacticalException("Expected { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            StatementList();
            if (_currentToken.Type != TokenType.KeyClose)
                throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
        }



        private void VaraibleInitializerListOpt()
        {
            if (_currentToken.Type == TokenType.KeyOpen || _currentToken.Type.IsExpression())
            {
                VaraibleInitializerList();
            }
            else
            {

            }
        }

        private void VaraibleInitializerList()
        {
            VaraibleInitializer();
            VaraibleInitializerListPrime();
        }

        private void VaraibleInitializerListPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                VaraibleInitializer();
                VaraibleInitializerListPrime();
            }
            else
            {

            }
        }

        private void StatementList()
        {
            if (_currentToken.Type.IsStatements())
            {
                Statement();
                StatementList();
            }
            else
            {

            }
        }

        private void Statement()
        {
            if (_currentToken.Type == TokenType.RwIf)
            {
                IfStatement();
            }
            else if (_currentToken.Type == TokenType.RwWhile)
            {
                WhileStatement();
            }
            else if (_currentToken.Type == TokenType.RwDo)
            {
                DoStatement();
            }
            else if (_currentToken.Type == TokenType.RwSwitch)
            {
                SwitchStatement();
            }
            else if (_currentToken.Type == TokenType.RwReturn)
            {
                ReturnStatement();
            }
            else if (_currentToken.Type == TokenType.RwBreak)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.EndStatement)
                    throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
            }
            else if (_currentToken.Type == TokenType.RwContinue)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.EndStatement)
                    throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
            }
            else if (_currentToken.Type == TokenType.EndStatement)
            {
                _currentToken = _lexer.GetNextToken();
            }
            else if (_currentToken.Type == TokenType.RwForeach)
            {
                ForEachStatement();
            }
            else if (_currentToken.Type == TokenType.RwFor)
            {
                ForStatement();
            }
            else if (_currentToken.Type == TokenType.OpDec)
            {
                _currentToken = _lexer.GetNextToken();
                PrimaryExpression();
                if (_currentToken.Type != TokenType.EndStatement)
                    throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
            }
            else if (_currentToken.Type == TokenType.OpInc)
            {
                _currentToken = _lexer.GetNextToken();
                PrimaryExpression();
                if (_currentToken.Type != TokenType.EndStatement)
                    throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
            }
            else if (_currentToken.Type == TokenType.RwVar || _currentToken.Type.IsPredifinedType() || _currentToken.Type == TokenType.RwEnum
                || _currentToken.Type == TokenType.RwBase || _currentToken.Type == TokenType.RwThis || _currentToken.Type == TokenType.Id)
            {
                DeclarationAsignationStatement();
                if (_currentToken.Type != TokenType.EndStatement)
                    throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
            }
            else if (_currentToken.Type == TokenType.ParOpen)
            {
                _currentToken = _lexer.GetNextToken();
                CastStatement();
                if (_currentToken.Type != TokenType.Period)
                    throw new SintacticalException("Expected . Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                IdExpression();
                if (_currentToken.Type.IsAssignationOperator())
                {
                    _currentToken = _lexer.GetNextToken();
                    Expresion();
                    AssignmentStatementList();
                }
                if (_currentToken.Type != TokenType.EndStatement)
                    throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();

            }
            else if (_currentToken.Type == TokenType.KeyOpen)
            {
                Block();
            }
            else
            {
                throw new SintacticalException("Expected Statement Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private void CastStatement()
        {

            if (_currentToken.Type.IsType())
            {
                TypeProduction();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                PrimaryExpression();

            }
            else if (_currentToken.Type == TokenType.ParOpen)
            {
                CastStatementPrime();
            }
            else
            {
                throw new SintacticalException("Expected type or ( Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private void CastStatementPrime()
        {
            if (_currentToken.Type == TokenType.ParOpen)
            {
                _currentToken = _lexer.GetNextToken();
                CastStatement();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
            }
            else
            {

            }
        }

        private void DeclarationAsignationStatement()
        {
            if (_currentToken.Type == TokenType.RwVar)
            {
                _currentToken = _lexer.GetNextToken();
                DeclaratorsList();

            }
            else if (_currentToken.Type.IsPredifinedType() || _currentToken.Type == TokenType.RwEnum)
            {
                _currentToken = _lexer.GetNextToken();
                DeclaratorsListOrPrimitiveAccessor();
            }
            else if (_currentToken.Type == TokenType.RwBase || _currentToken.Type == TokenType.RwThis)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Period)
                    throw new SintacticalException("Expected . Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                IdExpression();
                if (_currentToken.Type.IsAssignationOperator())
                {

                    _currentToken = _lexer.GetNextToken();
                    Expresion();
                    AssignmentStatementList();
                }

            }
            else if (_currentToken.Type == TokenType.Id)
            {
                _currentToken = _lexer.GetNextToken();
                WWWWW();
            }
            else
            {
                throw new SintacticalException("Expected declaration asignation Statement Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private void DeclaratorsListOrPrimitiveAccessor()
        {
            if (_currentToken.Type == TokenType.Period)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                IdExpression();
            }
            else
            {
                TypeProductionPrime();
                DeclaratorsList();
            }


        }

        private void AssignmentStatementList()
        {
            if (_currentToken.Type.IsAssignationOperator())
            {
                _currentToken = _lexer.GetNextToken();
                Expresion();
                AssignmentStatementList();
            }
            else
            {

            }
        }

        private void WWWWW()
        {
            TypeNamePrime();
            YYYYY();
        }

        private void YYYYY()
        {
            if (_currentToken.Type == TokenType.BraOpen)
            {
                _currentToken = _lexer.GetNextToken();
                YYYYYPrime();
            }
            else if (_currentToken.Type == TokenType.Id)
            {
                DeclaratorsList();
            }
            else
            {
                IdExpressionWithoutOpenBra();
                XXXX();
            }
            //if (_currentToken.Type == TokenType.Period || _currentToken.Type == TokenType.ParOpen ||
            //    _currentToken.Type == TokenType.BraOpen || _currentToken.Type == TokenType.OpDec
            //    || _currentToken.Type == TokenType.OpInc || _currentToken.Type.IsAssignationOperator())
            //{
            //    IdExpression();
            //    XXXX();
            //}
            //else if (_currentToken.Type == TokenType.Id)
            //{
            //    DeclaratorsList();
            //}
        }

        private void IdExpressionWithoutOpenBra()
        {
            if (_currentToken.Type == TokenType.Period)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                IdExpression();
            }
            else if (_currentToken.Type == TokenType.ParOpen)
            {
                _currentToken = _lexer.GetNextToken();
                ArgumentList();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                ParentesisExpression();

            }
            else
            {

            }
        }

        private void YYYYYPrime()
        {
            if (_currentToken.Type.IsExpression())
            {
                Mierda4();
                XXXX();
            }
            else
            {
                RankSpecifierDec();
                TypeProductionPrime();
                DeclaratorsList();
            }
        }

        private void Mierda4()
        {
            ExpresionList();
            if (_currentToken.Type != TokenType.BraClose)
                throw new SintacticalException("Expected ] Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            IdExpression();
        }

        private void RankSpecifierDec()
        {
            DimSpearetorsOpt();
            if (_currentToken.Type != TokenType.BraClose)
                throw new SintacticalException("Expected ] Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();

        }

        private void XXXX()
        {
            if (_currentToken.Type == TokenType.OpDec
                || _currentToken.Type == TokenType.OpInc)
            {
                _currentToken = _lexer.GetNextToken();
            }
            else if (_currentToken.Type.IsAssignationOperator())
            {
                _currentToken = _lexer.GetNextToken();
                Expresion();
                AssignmentStatementList();
            }
        }

        private void DeclarationStatement()
        {
            LocalVariableType();
            DeclaratorsList();
        }

        private void DeclaratorsList()
        {
            Declarator();
            DeclaratorsListPrime();
        }

        private void DeclaratorsListPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                Declarator();
                DeclaratorsListPrime();
            }
            else if (_currentToken.Type == TokenType.OpAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                VaraibleInitializer();
                DeclaratorsListPrime();
            }
            else
            {

            }
        }

        private void Declarator()
        {
            if (_currentToken.Type != TokenType.Id)

                throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            DeclaratorPrime();
        }

        private void DeclaratorPrime()
        {
            if (_currentToken.Type == TokenType.OpAsgn)
            {
                _currentToken = _lexer.GetNextToken();
                VaraibleInitializer();
            }
            else
            {

            }
        }

        private void ForStatement()
        {
            if (_currentToken.Type != TokenType.RwFor)

                throw new SintacticalException("Expected for Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            if (_currentToken.Type != TokenType.ParOpen)
                throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            ForInitalizer();
            if (_currentToken.Type != TokenType.EndStatement)
                throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            if (_currentToken.Type.IsExpression())
                Expresion();
            if (_currentToken.Type != TokenType.EndStatement)
                throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            if (_currentToken.Type.IsExpression())
                ExpresionList();
            if (_currentToken.Type != TokenType.ParClose)
                throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            EmbededStatement();
        }

        private void ForInitalizer()
        {
            if (_currentToken.Type == TokenType.RwVar || _currentToken.Type.IsPredifinedType() || _currentToken.Type == TokenType.RwEnum
                || _currentToken.Type == TokenType.RwBase || _currentToken.Type == TokenType.RwThis || _currentToken.Type == TokenType.Id)
                DeclarationAsignationStatement();
            else
            {

            }
        }

        private void ExpresionList()
        {
            if (_currentToken.Type.IsExpression())
            {
                Expresion();
                ExpresionListPrime();
            }
            else
            {
                throw new SintacticalException("Expected expression Line " + _currentToken.Line + " Col " +
                                              _currentToken.Column);
            }
        }

        private void ExpresionListPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                Expresion();
                ExpresionListPrime();
            }
            else
            {

            }
        }

        private void ForEachStatement()
        {
            if (_currentToken.Type != TokenType.RwForeach)

                throw new SintacticalException("Expected foreach Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            if (_currentToken.Type != TokenType.ParOpen)
                throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            LocalVariableType();
            if (_currentToken.Type != TokenType.Id)
                throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            if (_currentToken.Type != TokenType.RwIn)
                throw new SintacticalException("Expected in Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            Expresion();
            if (_currentToken.Type != TokenType.ParClose)
                throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            EmbededStatement();
        }

        private void LocalVariableType()
        {
            if (_currentToken.Type == TokenType.RwVar)
            {
                _currentToken = _lexer.GetNextToken();
            }
            else if (_currentToken.Type.IsType())
            {
                TypeProduction();
            }
            else
            {
                throw new SintacticalException("Expected var or type Line " + _currentToken.Line + " Col " +
                                                _currentToken.Column);
            }
        }

        private void ReturnStatement()
        {
            if (_currentToken.Type != TokenType.RwReturn)
                throw new SintacticalException("Expected return Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            ReturnBody();
            if (_currentToken.Type != TokenType.EndStatement)
                throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
        }

        private void ReturnBody()
        {
            if (_currentToken.Type.IsExpression())
                Expresion();
            else
            {

            }
        }

        private void SwitchStatement()
        {
            if (_currentToken.Type != TokenType.RwSwitch)
                throw new SintacticalException("Expected switch Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            if (_currentToken.Type != TokenType.ParOpen)
                throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            Expresion();
            if (_currentToken.Type != TokenType.ParClose)
                throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            if (_currentToken.Type != TokenType.KeyOpen)
                throw new SintacticalException("Expected { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            SwitchSections();
            if (_currentToken.Type != TokenType.KeyClose)
                throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();

        }

        private void SwitchSections()
        {
            if (_currentToken.Type == TokenType.RwCase || _currentToken.Type == TokenType.RwDefault)
            {
                SwitchSection();
                SwitchSections();
            }
            else
            {

            }
        }

        private void SwitchSection()
        {
            SwitchLables();
            StatementList();
            BreakStatement();
        }

        private void BreakStatement()
        {
            if (_currentToken.Type == TokenType.RwBreak)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.EndStatement)
                    throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
            }
            else
            {

            }
        }

        private void SwitchLables()
        {
            if (_currentToken.Type == TokenType.RwCase)
            {
                _currentToken = _lexer.GetNextToken();
                Expresion();
                if (_currentToken.Type != TokenType.Colon)
                    throw new SintacticalException("Expected : Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
            }
            else if (_currentToken.Type == TokenType.RwDefault)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Colon)
                    throw new SintacticalException("Expected : Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
            }
            else
            {
                throw new SintacticalException("Expected Case or Default Line " + _currentToken.Line + " Col " +
                                                _currentToken.Column);
            }
        }

        private void DoStatement()
        {
            if (_currentToken.Type != TokenType.RwDo)
                throw new SintacticalException("Expected Do Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            EmbededStatement();
            if (_currentToken.Type != TokenType.RwWhile)
                throw new SintacticalException("Expected while Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            if (_currentToken.Type != TokenType.ParOpen)
                throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            Expresion();
            if (_currentToken.Type != TokenType.ParClose)
                throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();

            if (_currentToken.Type != TokenType.EndStatement)
                throw new SintacticalException("Expected ; Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();

        }

        private void WhileStatement()
        {
            if (_currentToken.Type != TokenType.RwWhile)
                throw new SintacticalException("Expected While Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();

            if (_currentToken.Type != TokenType.ParOpen)
                throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            Expresion();
            if (_currentToken.Type != TokenType.ParClose)
                throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            EmbededStatement();
        }

        private void IfStatement()
        {
            if (_currentToken.Type != TokenType.RwIf)
                throw new SintacticalException("Expected If Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();

            if (_currentToken.Type != TokenType.ParOpen)
                throw new SintacticalException("Expected ( Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            Expresion();
            if (_currentToken.Type != TokenType.ParClose)
                throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            EmbededStatement();
            ElseStatement();

        }

        private void ElseStatement()
        {
            if (_currentToken.Type == TokenType.RwElse)
            {
                _currentToken = _lexer.GetNextToken();
                EmbededStatement();
            }
            else
            {

            }
        }

        private void EmbededStatement()
        {
            if (_currentToken.Type == TokenType.KeyOpen)
            {
                _currentToken = _lexer.GetNextToken();
                StatementList();
                if (_currentToken.Type != TokenType.KeyClose)
                    throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
            }
            else if (_currentToken.Type.IsStatements())
            {
                Statement();
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
                return ConditionalOrExpressionPrime(new LogicalOrExpressionNode(param, right));
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
                return ConditionalAndExpressionPrime(new LogicalAndExpressionNode(param, right));
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
                return InclusiveOrExpressionPrime(new BitOrExpressionNode(param, right));
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
                return ExclusiveOrExpressionPrime(new BitXorExpressionNode(param, right));
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
                return AndExpressionPrime(new BitAndExpressionNode(param, right));
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
                return EqualityExpressionPrime(new EqualExpressionNode(param, right));
            }
            else if (_currentToken.Type == TokenType.OpNotEquals)
            {
                _currentToken = _lexer.GetNextToken();
                var right = RelationalExpresion();
                return EqualityExpressionPrime(new NotEqualExpressionNode(param, right));
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
                return RelationalExpresionPrime(new LessThanExpressionNode(param, right));
            }
            else if (_currentToken.Type == TokenType.OpGrtThan)
            {
                _currentToken = _lexer.GetNextToken();
                var right = ShiftExpression();
                return RelationalExpresionPrime(new GreaterThanExpressionNode(param, right));
            }
            else if (_currentToken.Type == TokenType.OpLessThanOrEqual)
            {
                _currentToken = _lexer.GetNextToken();
                var right = ShiftExpression();
                return RelationalExpresionPrime(new LessThanOrEqualExpressionNode(param, right));
            }
            else if (_currentToken.Type == TokenType.OpGrtThanOrEqual)
            {
                _currentToken = _lexer.GetNextToken();
                var right = ShiftExpression();
                return RelationalExpresionPrime(new GreaterThanOrEqualExpressionNode(param, right));
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
                return ShiftExpressionPrime(new RightShiftExpressionNode(param, right));
            }
            else if (_currentToken.Type == TokenType.OpLftShft)
            {
                _currentToken = _lexer.GetNextToken();
                var right = AdditiveExpression();
                return ShiftExpressionPrime(new LeftShiftExpressionNode(param, right));
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
                return AdditiveExpressionPrime(new SumExpressionNode(param, right));
            }
            else if (_currentToken.Type == TokenType.OpSub)
            {
                _currentToken = _lexer.GetNextToken();
                var right = MultiplicativeExpression();
                return AdditiveExpressionPrime(new SubExpressionNode(param, right));
            }
            else
            {
                return param;
            }
        }

        private ExpressionNode MultiplicativeExpression()
        {
            var unary = UnaryExpression();
            var left = PrimaryExpression();
            left.unaryNode = unary;
            return MultiplicativeExpressionPrime(left);
        }

        private ExpressionNode MultiplicativeExpressionPrime(ExpressionNode param)
        {
            if (_currentToken.Type == TokenType.OpMul)
            {
                _currentToken = _lexer.GetNextToken();
                var unary = UnaryExpression();
                var right = PrimaryExpression();
                right.unaryNode = unary;
                return MultiplicativeExpressionPrime(new MultExpressionNode(param, right));
            }
            else if (_currentToken.Type == TokenType.OpDiv)
            {
                _currentToken = _lexer.GetNextToken();
                var unary = UnaryExpression();
                var right = PrimaryExpression();
                right.unaryNode = unary;
                return MultiplicativeExpressionPrime(new DivExpressionNode(param, right));
            }
            else if (_currentToken.Type == TokenType.OpMod)
            {
                _currentToken = _lexer.GetNextToken();
                var unary = UnaryExpression();
                var right = PrimaryExpression();
                right.unaryNode = unary;
                return MultiplicativeExpressionPrime(new ModExpressionNode(param, right));
            }
            else
            {
                return param;
            }
        }

        private UnaryExpressionNode UnaryExpression()
        {
            if (_currentToken.Type == TokenType.OpSum)
            {
                var unary = _currentToken.Lexeme;
                _currentToken = _lexer.GetNextToken();
                return new SumUnaryExpressionNode(unary);
            }
            else if (_currentToken.Type == TokenType.OpSub)
            {
                var unary = _currentToken.Lexeme;
                _currentToken = _lexer.GetNextToken();
                return new SubUnaryExpressionNode(unary);
            }
            else if (_currentToken.Type == TokenType.OpLogicalNot)
            {
                var unary = _currentToken.Lexeme;
                _currentToken = _lexer.GetNextToken();
                return new NotUnaryExpressionNode(unary);
            }
            else if (_currentToken.Type == TokenType.OpBinaryComplement)
            {
                var unary = _currentToken.Lexeme;
                _currentToken = _lexer.GetNextToken();
                return new ComplementUnaryExpressionNode(unary);
            }
            else if (_currentToken.Type == TokenType.OpInc)
            {
                var unary = _currentToken.Lexeme;
                _currentToken = _lexer.GetNextToken();
                return new IncUnaryExpressionNode(unary);
            }
            else if (_currentToken.Type == TokenType.OpDec)
            {
                var unary = _currentToken.Lexeme;
                _currentToken = _lexer.GetNextToken();
                return new DecUnaryExpressionNode(unary);
            }
            else
            {
                return null;
            }
        }
        private PrimaryExpressionNode PrimaryExpression()
        {
            if (_currentToken.Type == TokenType.RwNew)
            {
                _currentToken = _lexer.GetNextToken();
                ArrayOrObject();
            }
            else if (_currentToken.Type.IsPrimaryNoArrayCreationExpression())
            {
                PrimaryNoArrayCreationExpression();
            }
            else if (_currentToken.Type == TokenType.ParOpen)
            {
                _currentToken = _lexer.GetNextToken();
                CastOrExpression();
            }
            else
            {
                throw new SintacticalException("Expected valid Primary Expression Line " + _currentToken.Line + " Col " +
                                                _currentToken.Column);
            }
        }


        private void IdExpression()
        {
            if (_currentToken.Type == TokenType.Period)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                IdExpression();
            }
            else if (_currentToken.Type == TokenType.BraOpen)
            {
                _currentToken = _lexer.GetNextToken();
                ExpresionList();
                if (_currentToken.Type != TokenType.BraClose)
                    throw new SintacticalException("Expected ] Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                IdExpression();
            }
            else if (_currentToken.Type == TokenType.ParOpen)
            {
                _currentToken = _lexer.GetNextToken();
                ArgumentList();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                ParentesisExpression();

            }
            else
            {

            }
        }



        private void ParentesisExpression()
        {
            if (_currentToken.Type == TokenType.Period)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                IdExpression();
            }
            else if (_currentToken.Type == TokenType.BraOpen)
            {
                _currentToken = _lexer.GetNextToken();
                ExpresionList();
                if (_currentToken.Type != TokenType.BraClose)
                    throw new SintacticalException("Expected ] Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                IdExpression();
            }
            else
            {

            }
        }





        private void ArrayOrObject()
        {
            if (_currentToken.Type.IsType())
            {
                TypeProductionForArrayOrObject();
                ArrayOrObjectPrime();
                IdExpression();
            }
            else if (_currentToken.Type == TokenType.ParOpen)
            {
                RankSpecifier();
                ArrayInitalizer();
            }
            else
            {
                throw new SintacticalException("Expected type or [  Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private void ArrayOrObjectPrime()
        {
            if (_currentToken.Type == TokenType.BraOpen)
            {
                ArrayCreationExpression();
            }
            else if (_currentToken.Type == TokenType.ParOpen || _currentToken.Type == TokenType.KeyOpen)
            {
                ObjectCreationExpression();
            }
            else
            {
                throw new SintacticalException("Expected (, { or [  Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private void ArrayCreationExpression()
        {
            if (_currentToken.Type != TokenType.BraOpen)
                throw new SintacticalException("Expected [ Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            PostArrayCreationExpression();
        }

        private void PostArrayCreationExpression()
        {
            if (_currentToken.Type.IsExpression())
            {
                ExpresionList();
                if (_currentToken.Type != TokenType.BraClose)
                    throw new SintacticalException("Expected ] Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type == TokenType.BraOpen)
                    RankSpecifiers();
                if (_currentToken.Type == TokenType.KeyOpen)
                    ArrayInitalizer();
            }
            else
            {
                DimSpearetorsOpt();
                if (_currentToken.Type != TokenType.BraClose)
                    throw new SintacticalException("Expected ] Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                RankSpecifiersPrime();
                ArrayInitalizer();
            }
        }

        private void ObjectCreationExpression()
        {
            if (_currentToken.Type == TokenType.ParOpen)
            {
                _currentToken = _lexer.GetNextToken();
                ArgumentList();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                ObjectCollectionInitalizerOpt();
            }
            else if (_currentToken.Type == TokenType.KeyOpen)
            {
                ObjectCollectionInitalizer();
            }
        }

        private void ObjectCollectionInitalizerOpt()
        {
            if (_currentToken.Type == TokenType.KeyOpen)
            {
                ObjectCollectionInitalizer();
            }
            else
            {

            }
        }

        private void ObjectCollectionInitalizer()
        {
            if (_currentToken.Type == TokenType.KeyOpen)
            {
                _currentToken = _lexer.GetNextToken();
                ObjectCollectionInitalizerBody();
            }
            else
            {
                throw new SintacticalException("Expected { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private void ObjectCollectionInitalizerBody()
        {
            if (_currentToken.Type == TokenType.Id)
            {
                MemberOrElement();
            }
            else
            {
                ElementInitalizerList();
                if (_currentToken.Type != TokenType.KeyClose)
                    throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                                       _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
            }
        }

        private void MemberOrElement()
        {
            if (_currentToken.Type == TokenType.Id)
            {
                _currentToken = _lexer.GetNextToken();
                MemberInitalizerList();
                if (_currentToken.Type != TokenType.KeyClose)
                    throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                                       _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
            }
            else if (_currentToken.Type.IsExpression() || _currentToken.Type == TokenType.ParOpen)
            {
                ElementInitalizerList();
                if (_currentToken.Type != TokenType.KeyClose)
                    throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                                       _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
            }
            else
            {
                if (_currentToken.Type != TokenType.KeyClose)
                    throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                                       _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
            }
        }

        private void MemberInitalizerListOpt()
        {
            if (_currentToken.Type == TokenType.OpAsgn)
            {
                MemberInitalizerList();
            }
            else
            {

            }
        }

        private void MemberInitalizerList()
        {
            MemberInitalizer();
            MemberInitalizerListPrime();
        }

        private void MemberInitalizerListPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                       _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                MemberInitalizer();
                MemberInitalizerListPrime();
            }
            else
            {

            }
        }

        private void MemberInitalizer()
        {
            if (_currentToken.Type != TokenType.OpAsgn)
                throw new SintacticalException("Expected = Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            _currentToken = _lexer.GetNextToken();
            InitalizerValue();
        }

        private void InitalizerValue()
        {
            if (_currentToken.Type.IsExpression())
            {
                Expresion();
            }
            else if (_currentToken.Type == TokenType.KeyOpen)
            {
                ObjectCollectionInitalizer();
            }
            else
            {
                throw new SintacticalException("Expected Expression or { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private void ElementInitalizerList()
        {
            ElementInitalizer();
            ElementInitalizerListPrime();
        }

        private void ElementInitalizerListPrime()
        {
            if (_currentToken.Type == TokenType.Comma)
            {
                _currentToken = _lexer.GetNextToken();
                ElementInitalizer();
                ElementInitalizerListPrime();
            }
            else
            {

            }
        }

        private void ElementInitalizer()
        {
            if (_currentToken.Type.IsExpression())
            {
                Expresion();
            }
            else if (_currentToken.Type == TokenType.KeyOpen)
            {
                _currentToken = _lexer.GetNextToken();
                ExpresionList();
                if (_currentToken.Type != TokenType.KeyClose)
                    throw new SintacticalException("Expected } Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();

            }
            else
            {
                throw new SintacticalException("Expected Expresion or { Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }


        private void CastOrExpression()
        {
            if (_currentToken.Type.IsPredifinedType() || _currentToken.Type == TokenType.RwEnum)
            {
                CustomTypeProduction();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                PrimaryExpression();
            }
            else if (_currentToken.Type.IsExpression())
            {
                Expresion();
                if (_currentToken.Type != TokenType.ParClose)
                    throw new SintacticalException("Expected ) Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                CastOrExpressionPrime();
            }
            else
            {
                throw new SintacticalException("Expected primitive type or Expression Line " + _currentToken.Line + " Col " +
                                               _currentToken.Column);
            }
        }

        private void CastOrExpressionPrime()
        {
            if (_currentToken.Type.IsPrimaryExpression())
            {
                PrimaryExpression();
            }
            else if (_currentToken.Type == TokenType.Period || _currentToken.Type == TokenType.ParOpen || _currentToken.Type == TokenType.BraOpen)
            {
                IdExpression();
            }
            else
            {

            }
        }

        private void PrimaryNoArrayCreationExpression()
        {
            if (_currentToken.Type.IsLiteral())
            {
                _currentToken = _lexer.GetNextToken();
            }
            else if (_currentToken.Type == TokenType.RwThis || _currentToken.Type == TokenType.RwBase || _currentToken.Type == TokenType.Id)
            {
                PreIdExpression();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                IdExpression();
                AssignmentInExpression();
                PostIncrementExpression();

            }
            else if (_currentToken.Type.IsPredifinedType() || _currentToken.Type == TokenType.RwEnum)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Period)
                    throw new SintacticalException("Expected . Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Id)
                    throw new SintacticalException("Expected Id Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
                IdExpression();
                PostIncrementExpression();
            }
        }

        private void AssignmentInExpression()
        {
            if (_currentToken.Type.IsAssignationOperator())
            {
                _currentToken = _lexer.GetNextToken();
                Expresion();
            }
            else
            {

            }
        }

        private void PreIdExpression()
        {
            if (_currentToken.Type == TokenType.RwThis || _currentToken.Type == TokenType.RwBase)
            {
                _currentToken = _lexer.GetNextToken();
                if (_currentToken.Type != TokenType.Period)
                    throw new SintacticalException("Expected . Line " + _currentToken.Line + " Col " +
                                                   _currentToken.Column);
                _currentToken = _lexer.GetNextToken();
            }
            else
            {

            }
        }

        private void PostIncrementExpression()
        {
            if (_currentToken.Type == TokenType.OpInc || _currentToken.Type == TokenType.OpDec)
            {
                _currentToken = _lexer.GetNextToken();
            }
            else
            {

            }
        }
    }
}