using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Atomic.Toolbox.Core.Helpers;
using Atomic.Toolbox.Core.Enums;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Atomic.Toolbox.Core.Parsers
{
    public class Field
    {
        public string Name { get; }
        public string TypeName { get; }
        public List<Attribute> Attributes { get; }
        public Visibility Visibility { get; }
        public Location Location => _fieldDeclarationSyntax.Declaration.GetLocation();

        private readonly FieldDeclarationSyntax _fieldDeclarationSyntax;
        private readonly SemanticModel _semanticModel;

        public Field(FieldDeclarationSyntax fieldDeclarationSyntax, SemanticModel semanticModel)
        {
            _fieldDeclarationSyntax = fieldDeclarationSyntax;
            _semanticModel = semanticModel;

            TypeName = ModelExtensions.GetTypeInfo(semanticModel, fieldDeclarationSyntax.Declaration.Type).Type?.ToString() ?? string.Empty;
            Name = _fieldDeclarationSyntax.Declaration.Variables.FirstOrDefault()?.ToString() ?? string.Empty;
            Visibility = _fieldDeclarationSyntax.GetVisibility();

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

        public (Type type, int count)[] CountAttributes(params Type[] attributeTypes)
        {
            var result = new (Type type, int count)[attributeTypes.Length];

            for (int i = 0; i < attributeTypes.Length; i++)
            {
                var type = attributeTypes[i];
                var count = GetAttributes(type).Count;
                result[i] = (type, count);
            }

            return result;
        }

        public List<Attribute> GetAttributes(params Type[] attributeTypes)
        {
            return Attributes.Where(attribute => attributeTypes.Any(at => at.FullName == attribute.TypeName)).ToList();
        }

        public Attribute GetAttribute(Type attributeType)
        {
            return Attributes.FirstOrDefault(attribute => attribute.TypeName == attributeType.FullName);
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