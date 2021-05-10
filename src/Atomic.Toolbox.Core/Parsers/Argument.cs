using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Atomic.Toolbox.Core.Parsers
{
    public class Argument
    {
        public string Name { get; }
        public string Value { get; }
        public Location Location { get; }
        
        
        private readonly AttributeArgumentSyntax _attributeSyntax;
        private readonly SemanticModel _semanticModel;

        public Argument(AttributeArgumentSyntax attributeSyntax, SemanticModel semanticModel)
        {
            _attributeSyntax = attributeSyntax;
            _semanticModel = semanticModel;

            Location = attributeSyntax.GetLocation();
            Name = attributeSyntax.NameEquals?.Name.ToString() ?? string.Empty;
            Value = attributeSyntax.Expression.ToString();

            var typeofToken = attributeSyntax.DescendantNodes().OfType<TypeOfExpressionSyntax>().FirstOrDefault();
            if (typeofToken != null)
            {
                Value = semanticModel.GetTypeInfo(typeofToken.Type).Type?.ToString() ?? string.Empty; 
            }
        }
    }
}