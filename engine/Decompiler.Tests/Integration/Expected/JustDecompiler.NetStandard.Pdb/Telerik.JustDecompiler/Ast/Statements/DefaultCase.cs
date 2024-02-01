using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class DefaultCase : SwitchCase
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				DefaultCase defaultCase = null;
				if (defaultCase.Body != null)
				{
					yield return defaultCase.Body;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.DefaultCase;
			}
		}

		public DefaultCase(BlockStatement body) : base(body)
		{
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
			DefaultCase defaultCase = new DefaultCase(blockStatement);
			base.CopyParentAndLabel(defaultCase);
			return defaultCase;
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
			DefaultCase defaultCase = new DefaultCase(blockStatement);
			base.CopyParentAndLabel(defaultCase);
			return defaultCase;
		}
	}
}