using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Atomic.Toolbox.Core.Parsers
{
    public class Tree
    {
        public string Namespace { get; }
        public string UsingString { get; }
        public List<Class> Classes { get; }
        public List<Interface> Interfaces { get; }
        public SyntaxTree SyntaxTree => _tree;


        private readonly SyntaxTree _tree;
        private readonly SyntaxNode _root;
        private readonly SemanticModel _semanticModel;
        private readonly NamespaceDeclarationSyntax _namespaceDeclarationSyntax;

        
        public Tree(SemanticModel semanticModel)
        {
            _tree = semanticModel.SyntaxTree;
            _root = _tree.GetRoot();

            _semanticModel = semanticModel;
            _namespaceDeclarationSyntax = GetNamespaceDeclarationSyntax();
            
            Namespace = GetNamespaceString();

            Classes = new List<Class>();
            Interfaces = new List<Interface>();
            
            var classDeclarationSyntaxes = GetClassDeclarationSyntaxes();
            foreach (var classDeclarationSyntax in classDeclarationSyntaxes)
            {
                Classes.Add(new Class(_semanticModel, classDeclarationSyntax, Namespace));
            }
            
            var interfaceDeclarationSyntaxes = GetInterfaceDeclarationSyntaxes();
            foreach (var interfaceDeclarationSyntax in interfaceDeclarationSyntaxes)
            {
                Interfaces.Add(new Interface(_semanticModel, interfaceDeclarationSyntax, Namespace));
            }

            UsingString = GetUsingString();
        }
        
        public Tree(SyntaxTree tree, Compilation compilation)
        {
            _tree = tree;
            _root = tree.GetRoot();

            _semanticModel = compilation.GetSemanticModel(tree);
            _namespaceDeclarationSyntax = GetNamespaceDeclarationSyntax();
            
            Namespace = GetNamespaceString();

            Classes = new List<Class>();
            Interfaces = new List<Interface>();
            
            var classDeclarationSyntaxes = GetClassDeclarationSyntaxes();
            foreach (var classDeclarationSyntax in classDeclarationSyntaxes)
            {
                Classes.Add(new Class(_semanticModel, classDeclarationSyntax, Namespace));
            }
            
            var interfaceDeclarationSyntaxes = GetInterfaceDeclarationSyntaxes();
            foreach (var interfaceDeclarationSyntax in interfaceDeclarationSyntaxes)
            {
                Interfaces.Add(new Interface(_semanticModel, interfaceDeclarationSyntax, Namespace));
            }

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

            return _semanticModel.Compilation.GetCompilationNamespace(symbol)?.ToString() ?? string.Empty;
        }

        private NamespaceDeclarationSyntax GetNamespaceDeclarationSyntax() =>
            _root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

        private List<ClassDeclarationSyntax> GetClassDeclarationSyntaxes() =>
            _root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
        private List<InterfaceDeclarationSyntax> GetInterfaceDeclarationSyntaxes() =>
            _root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().ToList();
    }
}