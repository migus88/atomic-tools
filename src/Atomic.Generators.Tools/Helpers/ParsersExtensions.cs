using System.Linq;
using Atomic.Generators.Tools.Enums;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Atomic.Generators.Tools.Helpers
{
    public static class ParsersExtensions
    {
        public static Visibility GetVisibility(this MemberDeclarationSyntax declarationSyntax)
        {
            var isProtected = declarationSyntax.DescendantTokens(t => t.IsKind(SyntaxKind.ProtectedKeyword)).Any();
            if (isProtected)
            {
                return Visibility.Protected;
            }
            
            var isPublic = declarationSyntax.DescendantTokens(t => t.IsKind(SyntaxKind.PublicKeyword)).Any();
            if (isPublic)
            {
                return Visibility.Public;
            }
            
            var isInternal = declarationSyntax.DescendantTokens(t => t.IsKind(SyntaxKind.InternalKeyword)).Any();
            if (isInternal)
            {
                return Visibility.Internal;
            }

            return Visibility.Private;
        }
    }
}