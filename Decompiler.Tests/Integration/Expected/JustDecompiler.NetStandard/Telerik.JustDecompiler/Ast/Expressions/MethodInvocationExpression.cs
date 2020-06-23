using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class MethodInvocationExpression : Expression
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
				MethodInvocationExpression methodInvocationExpression = null;
				yield return methodInvocationExpression.MethodExpression;
				foreach (ICodeNode argument in methodInvocationExpression.Arguments)
				{
					yield return argument;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.MethodInvocationExpression;
			}
		}

		public TypeReference ConstraintType
		{
			get;
			set;
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.MethodExpression.ExpressionType;
			}
			set
			{
				throw new NotSupportedException("Expression type of method invocation expression can not be changed.");
			}
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public IList<Instruction> InvocationInstructions
		{
			get
			{
				List<Instruction> instructions = new List<Instruction>(this.instructions);
				instructions.AddRange(this.MethodExpression.MappedInstructions);
				return instructions;
			}
		}

		public bool IsByReference
		{
			get
			{
				return this.MethodExpression.Method.ReturnType.IsByReference;
			}
		}

		public bool IsConstrained
		{
			get
			{
				return this.ConstraintType != null;
			}
		}

		public MethodReferenceExpression MethodExpression
		{
			get;
			set;
		}

		public bool VirtualCall
		{
			get;
			set;
		}

		public MethodInvocationExpression(MethodReferenceExpression method, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.MethodExpression = method;
			this.Arguments = new ExpressionCollection();
		}

		public override Expression Clone()
		{
			return new MethodInvocationExpression(this.MethodExpression.Clone() as MethodReferenceExpression, this.instructions)
			{
				Arguments = this.Arguments.Clone(),
				VirtualCall = this.VirtualCall,
				ConstraintType = this.ConstraintType
			};
		}

		public override Expression CloneExpressionOnly()
		{
			return new MethodInvocationExpression(this.MethodExpression.CloneExpressionOnly() as MethodReferenceExpression, null)
			{
				Arguments = this.Arguments.CloneExpressionsOnly(),
				VirtualCall = this.VirtualCall,
				ConstraintType = this.ConstraintType
			};
		}

		public override bool Equals(Expression other)
		{
			if (!(other is MethodInvocationExpression))
			{
				return false;
			}
			MethodInvocationExpression methodInvocationExpression = other as MethodInvocationExpression;
			if (!this.MethodExpression.Equals(methodInvocationExpression.MethodExpression))
			{
				return false;
			}
			if (this.VirtualCall != methodInvocationExpression.VirtualCall || !this.Arguments.Equals(methodInvocationExpression.Arguments))
			{
				return false;
			}
			return this.ConstraintType == methodInvocationExpression.ConstraintType;
		}

		public Expression GetTarget()
		{
			if (this.MethodExpression.MethodDefinition == null || !this.MethodExpression.MethodDefinition.IsExtensionMethod)
			{
				return this.MethodExpression.Target;
			}
			if (this.Arguments.Count < 1)
			{
				throw new Exception("Extension methods invocations should have at least 1 argument.");
			}
			return this.Arguments[0];
		}
	}
}