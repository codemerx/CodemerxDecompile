using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Steps.SwitchByString
{
	public class RebuildSwitchByString : BaseCodeTransformer, IDecompilationStep
	{
		private DecompilationContext context;

		private SwitchByStringMatcher matcher;

		private SwitchByStringFixer fixer;

		public RebuildSwitchByString()
		{
			this.matcher = new SwitchByStringMatcher();
			base();
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			this.context = context;
			this.fixer = new SwitchByStringFixer(context.get_MethodContext().get_Method().get_Module().get_TypeSystem());
			dummyVar0 = this.Visit(block);
			return block;
		}

		public override ICodeNode VisitIfStatement(IfStatement node)
		{
			if (!this.matcher.TryMatch(node))
			{
				return this.VisitIfStatement(node);
			}
			V_0 = this.fixer.FixToSwitch(node, this.matcher.get_StringVariable(), this.matcher.get_IntVariable());
			return this.Visit(V_0);
		}
	}
}