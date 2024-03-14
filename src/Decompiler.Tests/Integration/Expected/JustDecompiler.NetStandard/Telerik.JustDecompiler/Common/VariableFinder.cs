using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Common
{
	internal class VariableFinder : BaseCodeVisitor
	{
		private readonly VariableReference variable;

		private bool found;

		public VariableFinder(VariableReference variable)
		{
			this.variable = variable;
		}

		public bool FindVariable(ICodeNode node)
		{
			this.found = false;
			this.Visit(node);
			return this.found;
		}

		public override void Visit(ICodeNode node)
		{
			if (!this.found)
			{
				base.Visit(node);
			}
		}

		public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			if (node.Variable == this.variable)
			{
				this.found = true;
				return;
			}
			base.VisitVariableDeclarationExpression(node);
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if ((object)node.Variable == (object)this.variable)
			{
				this.found = true;
				return;
			}
			base.VisitVariableReferenceExpression(node);
		}
	}
}