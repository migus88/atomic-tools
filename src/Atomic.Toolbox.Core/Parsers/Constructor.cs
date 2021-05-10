using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Atomic.Toolbox.Core.Parsers
{
    public class Constructor
    {
        public List<Attribute> Attributes { get; }
        public List<Parameter> Parameters { get; }
        public Location Location { get; }
        public bool IsTriggeringBaseConstructor { get; }
        public bool IsTriggeringInternalConstructor { get; }

        private readonly ConstructorDeclarationSyntax _constructorDeclarationSyntax;
        private readonly SemanticModel _semanticModel;

        public Constructor(ConstructorDeclarationSyntax constructorDeclarationSyntax, SemanticModel semanticModel)
        {
            _constructorDeclarationSyntax = constructorDeclarationSyntax;
            _semanticModel = semanticModel;

            Location = _constructorDeclarationSyntax.GetLocation();
            IsTriggeringBaseConstructor = _constructorDeclarationSyntax.DescendantNodes().Any(n => n.IsKind(SyntaxKind.BaseConstructorInitializer));
            IsTriggeringInternalConstructor = _constructorDeclarationSyntax.DescendantNodes().Any(n => n.IsKind(SyntaxKind.ThisConstructorInitializer));

            Attributes = new List<Attribute>();
            InitAttributes();

            Parameters = new List<Parameter>();
            InitParameters();
        }

        public bool HasAttribute(Type attributeType)
        {
            return Attributes.Any(parser => parser.TypeName == attributeType.FullName);
        }

        public Attribute GetAttribute(Type attributeType)
        {
            return Attributes.FirstOrDefault(parser => parser.TypeName == attributeType.FullName);
        }

        private void InitParameters()
        {
            var parameters = _constructorDeclarationSyntax.ParameterList?.Parameters.ToList() ??
                             new List<ParameterSyntax>();

            foreach (var parameter in parameters)
            {
                Parameters.Add(new Parameter(parameter, _semanticModel));
            }
        }
        
        private void InitAttributes()
        {
            var attributeSyntaxes = _constructorDeclarationSyntax.AttributeLists.SelectMany(al => al.Attributes).ToList();
            foreach (var attributeSyntax in attributeSyntaxes)
            {
                Attributes.Add(new Attribute(attributeSyntax, _semanticModel));
            }
        }
    }
}