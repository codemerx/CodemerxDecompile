using Mono.Cecil;
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

		private SwitchByStringMatcher matcher = new SwitchByStringMatcher();

		private SwitchByStringFixer fixer;

		public RebuildSwitchByString()
		{
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			this.context = context;
			this.fixer = new SwitchByStringFixer(context.MethodContext.Method.get_Module().get_TypeSystem());
			this.Visit(block);
			return block;
		}

		public override ICodeNode VisitIfStatement(IfStatement node)
		{
			if (!this.matcher.TryMatch(node))
			{
				return base.VisitIfStatement(node);
			}
			Statement @switch = this.fixer.FixToSwitch(node, this.matcher.StringVariable, this.matcher.IntVariable);
			return base.Visit(@switch);
		}
	}
}