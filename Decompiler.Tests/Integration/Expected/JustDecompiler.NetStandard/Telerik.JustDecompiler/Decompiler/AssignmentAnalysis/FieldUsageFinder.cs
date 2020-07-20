using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.AssignmentAnalysis
{
	internal class FieldUsageFinder : BaseUsageFinder
	{
		private readonly FieldDefinition theField;

		public FieldUsageFinder(FieldDefinition theField)
		{
			base();
			this.theField = theField;
			return;
		}

		public override bool CheckExpression(Expression node)
		{
			if (node.get_CodeNodeType() != 30)
			{
				return false;
			}
			return (object)(node as FieldReferenceExpression).get_Field().Resolve() == (object)this.theField;
		}

		public override void VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			if ((object)node.get_Field().Resolve() == (object)this.theField)
			{
				this.searchResult = 2;
			}
			return;
		}
	}
}