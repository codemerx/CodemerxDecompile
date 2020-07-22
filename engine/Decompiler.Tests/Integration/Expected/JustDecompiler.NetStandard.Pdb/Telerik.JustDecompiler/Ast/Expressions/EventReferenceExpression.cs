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
	public class EventReferenceExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new EventReferenceExpression.u003cget_Childrenu003ed__10(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 48;
			}
		}

		public EventReference Event
		{
			get;
			private set;
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_Event().get_EventType();
			}
			set
			{
				throw new InvalidOperationException("Cannot set the type of an event");
			}
		}

		public Expression Target
		{
			get;
			internal set;
		}

		public EventReferenceExpression(Expression target, EventReference event, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Target(target);
			this.set_Event(event);
			return;
		}

		public override Expression Clone()
		{
			if (this.get_Target() != null)
			{
				stackVariable4 = this.get_Target().Clone();
			}
			else
			{
				stackVariable4 = null;
			}
			return new EventReferenceExpression(stackVariable4, this.get_Event(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			if (this.get_Target() != null)
			{
				stackVariable4 = this.get_Target().CloneExpressionOnly();
			}
			else
			{
				stackVariable4 = null;
			}
			return new EventReferenceExpression(stackVariable4, this.get_Event(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other as EventReferenceExpression == null)
			{
				return false;
			}
			V_0 = other as EventReferenceExpression;
			if (this.get_Target() != null)
			{
				if (!this.get_Target().Equals(V_0.get_Target()))
				{
					return false;
				}
			}
			else
			{
				if (V_0.get_Target() != null)
				{
					return false;
				}
			}
			return String.op_Equality(this.get_Event().get_FullName(), V_0.get_Event().get_FullName());
		}
	}
}