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
				EventReferenceExpression eventReferenceExpression = null;
				if (eventReferenceExpression.Target != null)
				{
					yield return eventReferenceExpression.Target;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.EventReferenceExpression;
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
				return this.Event.EventType;
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

		public EventReferenceExpression(Expression target, EventReference @event, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Target = target;
			this.Event = @event;
		}

		public override Expression Clone()
		{
			Expression expression;
			if (this.Target != null)
			{
				expression = this.Target.Clone();
			}
			else
			{
				expression = null;
			}
			return new EventReferenceExpression(expression, this.Event, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			Expression expression;
			if (this.Target != null)
			{
				expression = this.Target.CloneExpressionOnly();
			}
			else
			{
				expression = null;
			}
			return new EventReferenceExpression(expression, this.Event, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is EventReferenceExpression))
			{
				return false;
			}
			EventReferenceExpression eventReferenceExpression = other as EventReferenceExpression;
			if (this.Target == null)
			{
				if (eventReferenceExpression.Target != null)
				{
					return false;
				}
			}
			else if (!this.Target.Equals(eventReferenceExpression.Target))
			{
				return false;
			}
			return this.Event.FullName == eventReferenceExpression.Event.FullName;
		}
	}
}