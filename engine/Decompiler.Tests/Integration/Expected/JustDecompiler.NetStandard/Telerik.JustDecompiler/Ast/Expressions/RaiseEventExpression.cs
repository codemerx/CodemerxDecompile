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
				return this.Arguments;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.RaiseEventExpression;
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

		public RaiseEventExpression(EventReference @event, MethodReference invokeMethodReference, ExpressionCollection arguments, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Event = @event;
			this.InvokeMethodReference = invokeMethodReference;
			this.Arguments = arguments;
		}

		public override Expression Clone()
		{
			return new RaiseEventExpression(this.Event, this.InvokeMethodReference, this.Arguments.Clone(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new RaiseEventExpression(this.Event, this.InvokeMethodReference, this.Arguments.CloneExpressionsOnly(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other.CodeNodeType != Telerik.JustDecompiler.Ast.CodeNodeType.RaiseEventExpression)
			{
				return false;
			}
			RaiseEventExpression raiseEventExpression = other as RaiseEventExpression;
			if (!(this.Event.get_FullName() == raiseEventExpression.Event.get_FullName()) || !(this.InvokeMethodReference.get_FullName() == raiseEventExpression.InvokeMethodReference.get_FullName()))
			{
				return false;
			}
			return this.Arguments.Equals(raiseEventExpression.Arguments);
		}
	}
}