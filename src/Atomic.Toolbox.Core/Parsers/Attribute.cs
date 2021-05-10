using System;
using System.Collections.Generic;
using System.Linq;
using Atomic.Toolbox.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Atomic.Toolbox.Core.Parsers
{
    public class Attribute
    {
        public string TypeName { get; }
        public List<Argument> Arguments { get; }
        public Location Location => _location;

        private readonly AttributeSyntax _attributeSyntax;
        private readonly SemanticModel _semanticModel;
        private readonly Location _location;

        public Attribute(AttributeSyntax attributeSyntax, SemanticModel semanticModel)
        {
            _attributeSyntax = attributeSyntax;
            _semanticModel = semanticModel;
            _location = attributeSyntax.GetLocation();

            TypeName = GetTypeName();
            
            Arguments = new List<Argument>();
            InitArguments();
        }

        public bool IsAnyType(params Type[] types)
        {
            return types.Any(t => t.FullName == TypeName);
        }

        public bool HasArgument(string name) => Arguments != null && Arguments.Any(a => a.Name == name);

        public Argument GetArgument(string name) => Arguments?.FirstOrDefault(a => a.Name == name);

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