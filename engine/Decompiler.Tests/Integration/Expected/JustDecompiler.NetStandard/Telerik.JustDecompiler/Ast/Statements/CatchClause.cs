using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class CatchClause : BasePdbStatement
	{
		public BlockStatement Body
		{
			get;
			set;
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new CatchClause.u003cget_Childrenu003ed__19(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 16;
			}
		}

		public Statement Filter
		{
			get;
			set;
		}

		public TypeReference Type
		{
			get;
			set;
		}

		public VariableDeclarationExpression Variable
		{
			get;
			set;
		}

		public CatchClause()
		{
			base();
			return;
		}

		public CatchClause(BlockStatement body, TypeReference type, VariableDeclarationExpression variable, Statement filter = null)
		{
			base();
			this.set_Body(body);
			this.set_Type(type);
			this.set_Variable(variable);
			this.set_Filter(filter);
			return;
		}

		public override Statement Clone()
		{
			if (this.get_Body() != null)
			{
				stackVariable5 = (BlockStatement)this.get_Body().Clone();
			}
			else
			{
				stackVariable5 = null;
			}
			if (this.get_Variable() != null)
			{
				stackVariable11 = (VariableDeclarationExpression)this.get_Variable().Clone();
			}
			else
			{
				stackVariable11 = null;
			}
			V_0 = stackVariable11;
			if (this.get_Filter() != null)
			{
				stackVariable16 = this.get_Filter().Clone();
			}
			else
			{
				stackVariable16 = null;
			}
			return new CatchClause(stackVariable5, this.get_Type(), V_0, stackVariable16);
		}

		public override Statement CloneStatementOnly()
		{
			if (this.get_Body() != null)
			{
				stackVariable5 = (BlockStatement)this.get_Body().CloneStatementOnly();
			}
			else
			{
				stackVariable5 = null;
			}
			if (this.get_Variable() != null)
			{
				stackVariable11 = (VariableDeclarationExpression)this.get_Variable().CloneExpressionOnly();
			}
			else
			{
				stackVariable11 = null;
			}
			V_0 = stackVariable11;
			if (this.get_Filter() != null)
			{
				stackVariable16 = this.get_Filter().CloneStatementOnly();
			}
			else
			{
				stackVariable16 = null;
			}
			return new CatchClause(stackVariable5, this.get_Type(), V_0, stackVariable16);
		}
	}
}