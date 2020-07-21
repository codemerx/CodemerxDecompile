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
			base();
			this.variable = variable;
			return;
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
				this.Visit(node);
			}
			return;
		}

		public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			if (node.get_Variable() == this.variable)
			{
				this.found = true;
				return;
			}
			this.VisitVariableDeclarationExpression(node);
			return;
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if ((object)node.get_Variable() == (object)this.variable)
			{
				this.found = true;
				return;
			}
			this.VisitVariableReferenceExpression(node);
			return;
		}
	}
}