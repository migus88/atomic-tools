using System;
using System.Collections.Generic;
using System.Linq;
using Atomic.Generators.Tools.Enums;
using Atomic.Generators.Tools.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Atomic.Generators.Tools.Parsers
{
    public class Class
    {
        public string Namespace { get; set; }
        public string ClassName { get; }
        public string FullClassName => HelperMethods.GetFullClassName(Namespace, ClassName);
        public List<Attribute> Attributes { get; }
        public List<Field> Fields { get; }
        public List<Constructor> Constructors { get; }
        public Visibility Visibility { get; set; }
        public Location Location { get; }

        private readonly INamedTypeSymbol _classSymbol;
        private readonly SemanticModel _semanticModel;
        private readonly ClassDeclarationSyntax _classDeclarationSyntax;

        public Class(SemanticModel semanticModel, ClassDeclarationSyntax classDeclarationSyntax, string @namespace)
        {
            _semanticModel = semanticModel;
            _classDeclarationSyntax = classDeclarationSyntax;

            _classSymbol = GetClassSymbol();

            Namespace = @namespace;
            Location = _classDeclarationSyntax.Identifier.GetLocation();
            ClassName = GetClassNameString();
            Visibility = _classDeclarationSyntax.GetVisibility();

            Attributes = new List<Attribute>();
            InitAttributes();

            Fields = new List<Field>();
            InitFields();

            Constructors = new List<Constructor>();
            InitConstructors();
        }

        public bool HasInterface(Type interfaceType) =>
            _classSymbol.AllInterfaces.Any(i => i.ToString() == interfaceType.FullName);

        public List<Constructor> GetConstructorsWithAttribute(Type attributeType)
        {
            var attributeFullName = attributeType.FullName;
            return Constructors
                .Where(ctor =>
                    ctor.Attributes.Any(a => a.TypeName == attributeFullName) ||
                    ctor.Parameters.Any(p => p.Attributes.Any(pa => pa.TypeName == attributeFullName))
                ).ToList();
        }

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

        public bool HasAnyConstructor()
        {
            return Constructors.Any();
        }

        public Constructor GetDefaultConstructor()
        {
            return Constructors.FirstOrDefault(ctor => ctor.Parameters.Count == 0);
        }

        private string GetClassNameString() =>
            _classDeclarationSyntax
                .Identifier
                .ToString();

        private INamedTypeSymbol GetClassSymbol() => _semanticModel.GetDeclaredSymbol(_classDeclarationSyntax);

        private void InitAttributes()
        {
            var attributeSyntaxes = _classDeclarationSyntax.AttributeLists.SelectMany(al => al.Attributes).ToList();
            foreach (var attributeSyntax in attributeSyntaxes)
            {
                Attributes.Add(new Attribute(attributeSyntax, _semanticModel));
            }
        }

        private void InitConstructors()
        {
            var constructorSyntaxes = _classDeclarationSyntax.DescendantNodes().OfType<ConstructorDeclarationSyntax>()
                .ToList();

            foreach (var syntax in constructorSyntaxes)
            {
                Constructors.Add(new Constructor(syntax, _semanticModel));
            }
        }

        private void InitFields()
        {
            var fieldSyntaxes = _classDeclarationSyntax.Members.OfType<FieldDeclarationSyntax>().ToList();
            foreach (var syntax in fieldSyntaxes)
            {
                Fields.Add(new Field(syntax, _semanticModel));
            }
        }
    }
}