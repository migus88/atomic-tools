using System.Linq;
using Atomic.Toolbox.Core.Enums;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharpExtensions;

namespace Atomic.Toolbox.Core.Helpers
{
    public static class ParsersExtensions
    {
        public static Visibility GetVisibility(this MemberDeclarationSyntax declarationSyntax)
        {
            var tokens = declarationSyntax.DescendantTokens().ToArray();
            
            var isProtected = tokens.Any(t => t.IsKind(SyntaxKind.ProtectedKeyword));
            if (isProtected)
            {
                return Visibility.Protected;
            }
            
            var isPublic = tokens.Any(t => t.IsKind(SyntaxKind.PublicKeyword));
            if (isPublic)
            {
                return Visibility.Public;
            }
            
            var isInternal = tokens.Any(t => t.IsKind(SyntaxKind.InternalKeyword));
            if (isInternal)
            {
                return Visibility.Internal;
            }

            return Visibility.Private;
        }
    }
}