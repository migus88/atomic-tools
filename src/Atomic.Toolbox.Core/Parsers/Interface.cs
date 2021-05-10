using System;
using System.Collections.Generic;
using System.Linq;
using Atomic.Toolbox.Core.Enums;
using Atomic.Toolbox.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Atomic.Toolbox.Core.Parsers
{
    public class Interface
    {
        public string Namespace { get; }
        public string InterfaceName { get; }
        public string FullInterfaceName => HelperMethods.GetFullClassName(Namespace, InterfaceName);
        public List<Attribute> Attributes { get; }
        public List<Field> Fields { get; }
        public Visibility Visibility { get; set; }
        public Location Location { get; }

        private readonly INamedTypeSymbol _interfaceSymbol;
        private readonly SemanticModel _semanticModel;
        private readonly InterfaceDeclarationSyntax _interfaceDeclarationSyntax;

        public Interface(SemanticModel semanticModel, InterfaceDeclarationSyntax interfaceDeclarationSyntax, string @namespace)
        {
            _semanticModel = semanticModel;
            _interfaceDeclarationSyntax = interfaceDeclarationSyntax;

            _interfaceSymbol = GetInterfaceSymbol();

            Namespace = @namespace;
            Location = _interfaceDeclarationSyntax.Identifier.GetLocation();
            InterfaceName = GetInterfaceNameString();
            Visibility = _interfaceDeclarationSyntax.GetVisibility();

            Attributes = new List<Attribute>();
            InitAttributes();

            Fields = new List<Field>();
            InitFields();
        }

        public bool HasInterface(Type interfaceType) =>
            _interfaceSymbol.AllInterfaces.Any(i => i.ToString() == interfaceType.FullName);

        
        public List<Field> GetFieldsWithAttribute(Type attributeType)
        {
            return Fields
                .Where(field => field.HasAttribute(attributeType))
                .ToList();
        }

        public List<Field> GetFieldsWithAttributes()
        {
            return Fields
                .Where(field => field.Attributes.Count > 0)
                .ToList();
        }

        public List<Field> GetFieldsWithAttributes(params Type[] attributeTypes)
        {
            return Fields
                .Where(field => attributeTypes.Any(field.HasAttribute))
                .ToList();
        }

        private string GetInterfaceNameString() =>
            _interfaceDeclarationSyntax
                .Identifier
                .ToString();

        private INamedTypeSymbol GetInterfaceSymbol() => _semanticModel.GetDeclaredSymbol(_interfaceDeclarationSyntax);

        private void InitAttributes()
        {
            var attributeSyntaxes = _interfaceDeclarationSyntax.AttributeLists.SelectMany(al => al.Attributes).ToList();
            foreach (var attributeSyntax in attributeSyntaxes)
            {
                Attributes.Add(new Attribute(attributeSyntax, _semanticModel));
            }
        }

        private void InitFields()
        {
            var fieldSyntaxes = _interfaceDeclarationSyntax.Members.OfType<FieldDeclarationSyntax>().ToList();
            foreach (var syntax in fieldSyntaxes)
            {
                Fields.Add(new Field(syntax, _semanticModel));
            }
        }
    }
}