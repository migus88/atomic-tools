using System;
using System.Collections.Generic;
using System.Linq;
using Atomic.Generators.Tools.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Atomic.Generators.Tools.Parsers
{
    public class Attribute
    {
        public string TypeName { get; }
        public List<Argument> Arguments { get; }

        private readonly AttributeSyntax _attributeSyntax;
        private readonly SemanticModel _semanticModel;

        public Attribute(AttributeSyntax attributeSyntax, SemanticModel semanticModel)
        {
            _attributeSyntax = attributeSyntax;
            _semanticModel = semanticModel;

            TypeName = GetTypeName();
            
            Arguments = new List<Argument>();
            InitArguments();
        }

        public bool IsAnyType(params Type[] types)
        {
            return types.Any(t => t.FullName == TypeName);
        }

        public Argument GetArgument(string name) => Arguments.FirstOrDefault(a => a.Name == name);

        private string GetTypeName()
        {
            var typeSymbol = _semanticModel.GetTypeInfo(_attributeSyntax).Type;
            return typeSymbol == null ? string.Empty : HelperMethods.GetFullClassName(typeSymbol.ContainingNamespace.ToString(), typeSymbol.Name);
        }

        private void InitArguments()
        {
            var syntaxes = _attributeSyntax
                               .ArgumentList
                               ?.Arguments
                               .Where(a => a.NameEquals != null)
                               .ToList()
                           ?? new List<AttributeArgumentSyntax>();

            foreach (var syntax in syntaxes)
            {
                Arguments.Add(new Argument(syntax, _semanticModel));
            }
        }
    }
}