using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class ConditionCase : SwitchCase
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				ConditionCase conditionCase = null;
				yield return conditionCase.Condition;
				if (conditionCase.Body != null)
				{
					yield return conditionCase.Body;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.ConditionCase;
			}
		}

		public Expression Condition
		{
			get;
			set;
		}

		public ConditionCase()
		{
		}

		public ConditionCase(Expression condition, BlockStatement body) : base(body)
		{
			this.Condition = condition;
		}

		public override Statement Clone()
		{
			BlockStatement blockStatement;
			if (base.Body != null)
			{
				blockStatement = base.Body.Clone() as BlockStatement;
			}
			else
			{
				blockStatement = null;
			}
			ConditionCase conditionCase = new ConditionCase(this.Condition.Clone(), blockStatement);
			base.CopyParentAndLabel(conditionCase);
			return conditionCase;
		}

		public override Statement CloneStatementOnly()
		{
			BlockStatement blockStatement;
			if (base.Body != null)
			{
				blockStatement = base.Body.CloneStatementOnly() as BlockStatement;
			}
			else
			{
				blockStatement = null;
			}
			ConditionCase conditionCase = new ConditionCase(this.Condition.CloneExpressionOnly(), blockStatement);
			base.CopyParentAndLabel(conditionCase);
			return conditionCase;
		}
	}
}