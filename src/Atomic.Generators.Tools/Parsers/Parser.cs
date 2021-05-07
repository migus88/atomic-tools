using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Atomic.Generators.Tools.Helpers.HelperMethods;

namespace Atomic.Generators.Tools.Parsers
{
    public class Parser
    {
        public List<Tree> Trees { get; }
        
        private readonly Compilation _compilation;

        public Parser(Compilation compilation)
        {
            _compilation = compilation;
            Trees = GetTreesWithClassDeclaration();
        }
        
        public List<Tree> GetTreesWithInterface(Type interfaceType)
        {
            return Trees
                .Where(tree => tree.HasClassWithInterface(interfaceType))
                .ToList();
        }
        
        private List<Tree> GetTreesWithClassDeclaration()
        {
            return _compilation.SyntaxTrees.Select(syntaxTree => new Tree(syntaxTree, _compilation)).ToList();
        }
        
        public Class GetClass(string typeFullName)
        {
            return Trees
                .Where(tree => tree.Classes.Any(c => GetFullClassName(tree.Namespace, c.ClassName) == typeFullName))
                .Select(tree => tree.Classes.FirstOrDefault(c => GetFullClassName(tree.Namespace, c.ClassName) == typeFullName))
                .FirstOrDefault();
        }
    }
}