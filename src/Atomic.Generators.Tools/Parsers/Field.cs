using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Atomic.Generators.Tools.Parsers
{
    public class Field
    {
        public string Name { get; }
        public string TypeName { get; }
        public List<Attribute> Attributes { get; }

        private readonly FieldDeclarationSyntax _fieldDeclarationSyntax;
        private readonly SemanticModel _semanticModel;

        public Field(FieldDeclarationSyntax fieldDeclarationSyntax, SemanticModel semanticModel)
        {
            _fieldDeclarationSyntax = fieldDeclarationSyntax;
            _semanticModel = semanticModel;

            TypeName = semanticModel.GetTypeInfo(fieldDeclarationSyntax.Declaration.Type).Type?.ToString() ?? string.Empty;
            Name = _fieldDeclarationSyntax.Declaration.Variables.FirstOrDefault()?.ToString() ?? string.Empty;

            Attributes = new List<Attribute>();
            InitAttributes();
        }

        public bool HasAttribute(Type attributeType)
        {
            return Attributes.Any(attribute => attribute.TypeName == attributeType.FullName);
        }

        public bool HasAnyAttribute(params Type[] attributeTypes)
        {
            return Attributes.Any(attribute => attributeTypes.Any(at => at.FullName == attribute.TypeName));
        }

        public List<Attribute> GetAttributes(params Type[] attributeTypes)
        {
            return Attributes.Where(attribute => attributeTypes.Any(at => at.FullName == attribute.TypeName)).ToList();
        }

        private void InitAttributes()
        {
            var attributeSyntaxes = _fieldDeclarationSyntax.AttributeLists.SelectMany(al => al.Attributes).ToList();
            foreach (var attributeSyntax in attributeSyntaxes)
            {
                Attributes.Add(new Attribute(attributeSyntax, _semanticModel));
            }
        }
    }
}