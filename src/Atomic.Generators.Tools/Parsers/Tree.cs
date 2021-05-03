using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Atomic.Generators.Tools.Parsers
{
    public class Tree
    {
        public string Namespace { get; }
        public string UsingString { get; }
        public List<Class> Classes { get; }
        

        private readonly GeneratorExecutionContext _context;
        private readonly SyntaxNode _root;
        private readonly SemanticModel _semanticModel;
        private readonly NamespaceDeclarationSyntax _namespaceDeclarationSyntax;

        public Tree(SyntaxTree tree, GeneratorExecutionContext context)
        {
            _context = context;
            _root = tree.GetRoot();

            _semanticModel = _context.Compilation.GetSemanticModel(tree);
            _namespaceDeclarationSyntax = GetNamespaceDeclarationSyntax();

            Classes = new List<Class>();
            
            var classDeclarationSyntaxes = GetClassDeclarationSyntaxes();
            foreach (var classDeclarationSyntax in classDeclarationSyntaxes)
            {
                Classes.Add(new Class(_semanticModel, classDeclarationSyntax));
            }

            Namespace = GetNamespaceString();
            UsingString = GetUsingString();
        }

        public bool HasClassWithInterface(Type interfaceType)
        {
            return Classes.Any(@class => @class.HasInterface(interfaceType));
        }

        public List<Class> GetClassesWithInterface(Type interfaceType)
        {
            return Classes.Where(@class => @class.HasInterface(interfaceType)).ToList();
        }

        private string GetUsingString()
        {
            return string.Join("\r\n",
                _root
                    .DescendantNodes()
                    .OfType<UsingDirectiveSyntax>()
                    .Select(usingDirective => usingDirective.ToString())
            );
        }

        private string GetNamespaceString()
        {
            if (_namespaceDeclarationSyntax == null)
            {
                return string.Empty;
            }
            
            var symbol = _semanticModel.GetDeclaredSymbol(_namespaceDeclarationSyntax);

            if (symbol == null)
            {
                return string.Empty;
            }

            return _context.Compilation.GetCompilationNamespace(symbol)?.ToString() ?? string.Empty;
        }

        private NamespaceDeclarationSyntax GetNamespaceDeclarationSyntax() =>
            _root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

        private List<ClassDeclarationSyntax> GetClassDeclarationSyntaxes() =>
            _root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
    }
}