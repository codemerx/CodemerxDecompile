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
	public class DynamicConstructorInvocationExpression : Expression
	{
		public ExpressionCollection Arguments
		{
			get;
			internal set;
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				DynamicConstructorInvocationExpression dynamicConstructorInvocationExpression = null;
				foreach (ICodeNode argument in dynamicConstructorInvocationExpression.Arguments)
				{
					yield return argument;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.DynamicConstructorInvocationExpression;
			}
		}

		public TypeReference ConstructorType
		{
			get;
			private set;
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.ConstructorType;
			}
		}

		public DynamicConstructorInvocationExpression(TypeReference constructorType, IEnumerable<Expression> arguments, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.ConstructorType = constructorType;
			this.Arguments = new ExpressionCollection(arguments);
		}

		public override Expression Clone()
		{
			return new DynamicConstructorInvocationExpression(this.ConstructorType, this.Arguments.Clone(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new DynamicConstructorInvocationExpression(this.ConstructorType, this.Arguments.CloneExpressionsOnly(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other.CodeNodeType != Telerik.JustDecompiler.Ast.CodeNodeType.DynamicConstructorInvocationExpression)
			{
				return false;
			}
			DynamicConstructorInvocationExpression dynamicConstructorInvocationExpression = other as DynamicConstructorInvocationExpression;
			if (this.ConstructorType.FullName != dynamicConstructorInvocationExpression.ConstructorType.FullName)
			{
				return false;
			}
			return this.Arguments.Equals(dynamicConstructorInvocationExpression.Arguments);
		}
	}
}