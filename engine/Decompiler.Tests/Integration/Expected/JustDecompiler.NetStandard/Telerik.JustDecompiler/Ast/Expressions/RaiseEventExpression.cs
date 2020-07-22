using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class RaiseEventExpression : Expression
	{
		public ExpressionCollection Arguments
		{
			get;
			set;
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				return this.get_Arguments();
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 92;
			}
		}

		public EventReference Event
		{
			get;
			set;
		}

		public MethodReference InvokeMethodReference
		{
			get;
			set;
		}

		public RaiseEventExpression(EventReference event, MethodReference invokeMethodReference, ExpressionCollection arguments, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Event(event);
			this.set_InvokeMethodReference(invokeMethodReference);
			this.set_Arguments(arguments);
			return;
		}

		public override Expression Clone()
		{
			return new RaiseEventExpression(this.get_Event(), this.get_InvokeMethodReference(), this.get_Arguments().Clone(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new RaiseEventExpression(this.get_Event(), this.get_InvokeMethodReference(), this.get_Arguments().CloneExpressionsOnly(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other.get_CodeNodeType() != 92)
			{
				return false;
			}
			V_0 = other as RaiseEventExpression;
			if (!String.op_Equality(this.get_Event().get_FullName(), V_0.get_Event().get_FullName()) || !String.op_Equality(this.get_InvokeMethodReference().get_FullName(), V_0.get_InvokeMethodReference().get_FullName()))
			{
				return false;
			}
			return this.get_Arguments().Equals(V_0.get_Arguments());
		}
	}
}