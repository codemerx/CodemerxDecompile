using System;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
    class FieldUsageFinder : BaseUsageFinder
    {
        private readonly FieldDefinition theField;

        public FieldUsageFinder(FieldDefinition theField)
        {
            this.theField = theField;
        }

        public override void VisitFieldReferenceExpression(FieldReferenceExpression node)
        {
            if (node.Field.Resolve() == theField)
            {
                this.searchResult = UsageFinderSearchResult.Used;
            }
        }

        public override bool CheckExpression(Expression node)
        {
            return node.CodeNodeType == CodeNodeType.FieldReferenceExpression &&
                (node as FieldReferenceExpression).Field.Resolve() == theField;
        }
    }
}
