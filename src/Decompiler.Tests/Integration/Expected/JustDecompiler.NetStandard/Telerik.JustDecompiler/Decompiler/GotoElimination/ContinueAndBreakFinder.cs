using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Decompiler.GotoElimination
{
	internal class ContinueAndBreakFinder : BaseCodeVisitor
	{
		public ICollection<BreakStatement> Breaks
		{
			get;
			private set;
		}

		public ICollection<ContinueStatement> Continues
		{
			get;
			private set;
		}

		public ContinueAndBreakFinder()
		{
			this.Breaks = new List<BreakStatement>();
			this.Continues = new List<ContinueStatement>();
		}

		public override void VisitBreakStatement(BreakStatement node)
		{
			this.Breaks.Add(node);
		}

		public override void VisitContinueStatement(ContinueStatement node)
		{
			this.Continues.Add(node);
		}

		public override void VisitDoWhileStatement(DoWhileStatement node)
		{
		}

		public override void VisitForEachStatement(ForEachStatement node)
		{
		}

		public override void VisitForStatement(ForStatement node)
		{
		}

		public override void VisitSwitchStatement(SwitchStatement node)
		{
		}

		public override void VisitWhileStatement(WhileStatement node)
		{
		}
	}
}