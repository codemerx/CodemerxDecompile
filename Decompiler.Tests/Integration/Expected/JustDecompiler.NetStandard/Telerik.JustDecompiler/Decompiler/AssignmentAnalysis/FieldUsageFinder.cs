using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
	internal class FieldUsageFinder : BaseUsageFinder
	{
		private readonly FieldDefinition theField;

		public FieldUsageFinder(FieldDefinition theField)
		{
			this.theField = theField;
		}

		public override bool CheckExpression(Expression node)
		{
			if (node.CodeNodeType != CodeNodeType.FieldReferenceExpression)
			{
				return false;
			}
			return (node as FieldReferenceExpression).Field.Resolve() == this.theField;
		}

		public override void VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			if (node.Field.Resolve() == this.theField)
			{
				this.searchResult = UsageFinderSearchResult.Used;
			}
		}
	}
}