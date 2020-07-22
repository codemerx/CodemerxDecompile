using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class ArgumentReferenceExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				return new ArgumentReferenceExpression.u003cget_Childrenu003ed__5(-2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 25;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_Parameter().get_ParameterType();
			}
			set
			{
				throw new NotSupportedException("Type of Argument reference expression cannot be changed.");
			}
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public ParameterReference Parameter
		{
			get;
			set;
		}

		public ArgumentReferenceExpression(ParameterReference parameter, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Parameter(parameter);
			return;
		}

		public override Expression Clone()
		{
			return new ArgumentReferenceExpression(this.get_Parameter(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new ArgumentReferenceExpression(this.get_Parameter(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other as ArgumentReferenceExpression == null)
			{
				return false;
			}
			return this.get_Parameter().get_Index() == (other as ArgumentReferenceExpression).get_Parameter().get_Index();
		}
	}
}