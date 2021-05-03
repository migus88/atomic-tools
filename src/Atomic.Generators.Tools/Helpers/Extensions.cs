using System;
using System.Collections.Generic;
using System.Linq;
using Atomic.Generators.Tools.Exceptions;
using Atomic.Generators.Tools.Parsers;
using Microsoft.CodeAnalysis;

namespace Atomic.Generators.Tools.Helpers
{
    public static class Extensions
    {
        public static void ShowError(this GeneratorExecutionContext context, GeneratorException ex)
        {
            context.ShowError(ex.Sender, ex.Message, ex.ID, ex.Category);
        }
        
        public static void ShowError(this GeneratorExecutionContext context, string sender, Exception ex,
            string id = GeneratorException.DefaultErrorID, string category = GeneratorException.DefaultErrorCategory)
        {
            context.ShowError(sender, ex.Message, id, category);
        }

        public static void ShowError(this GeneratorExecutionContext context, string sender, string message,
            string id = GeneratorException.DefaultErrorID, string category = GeneratorException.DefaultErrorCategory)
        {
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor(id, $"Error occured in '{sender}'", message, category,
                    DiagnosticSeverity.Error, true),
                null);
            context.ReportDiagnostic(diagnostic);
        }

        public static List<Field> GetFieldsOfType(this List<Field> fields, string fieldType)
        {
            return fields.Where(parser => parser.TypeName == fieldType).ToList();
        }
    }
}