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
			base();
			this.set_Breaks(new List<BreakStatement>());
			this.set_Continues(new List<ContinueStatement>());
			return;
		}

		public override void VisitBreakStatement(BreakStatement node)
		{
			this.get_Breaks().Add(node);
			return;
		}

		public override void VisitContinueStatement(ContinueStatement node)
		{
			this.get_Continues().Add(node);
			return;
		}

		public override void VisitDoWhileStatement(DoWhileStatement node)
		{
			return;
		}

		public override void VisitForEachStatement(ForEachStatement node)
		{
			return;
		}

		public override void VisitForStatement(ForStatement node)
		{
			return;
		}

		public override void VisitSwitchStatement(SwitchStatement node)
		{
			return;
		}

		public override void VisitWhileStatement(WhileStatement node)
		{
			return;
		}
	}
}