using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Atomic.Generators.Tools.Parsers
{
    public class Parameter
    {
        public string Type { get; }
        public string Name { get; }
        public List<Attribute> Attributes { get; }
        
        
        private readonly ParameterSyntax _parameterSyntax;
        private readonly SemanticModel _semanticModel;

        public Parameter(ParameterSyntax parameterSyntax, SemanticModel semanticModel)
        {
            _parameterSyntax = parameterSyntax;
            _semanticModel = semanticModel;

            Name = _parameterSyntax.Identifier.Text;

            if (_parameterSyntax.Type == null)
            {
                Type = string.Empty;
            }
            else
            {
                Type = semanticModel.GetTypeInfo(_parameterSyntax.Type).Type?.ToString() ?? string.Empty;
            }

            Attributes = new List<Attribute>();
            InitAttributes();
        }

        private void InitAttributes()
        {
            var attributeSyntaxes = _parameterSyntax.AttributeLists.SelectMany(al => al.Attributes).ToList();
            foreach (var attributeSyntax in attributeSyntaxes)
            {
                Attributes.Add(new Attribute(attributeSyntax, _semanticModel));
            }
        }
    }
}