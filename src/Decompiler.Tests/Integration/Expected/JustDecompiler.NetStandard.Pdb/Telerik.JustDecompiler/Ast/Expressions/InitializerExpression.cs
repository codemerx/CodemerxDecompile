using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class InitializerExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				InitializerExpression initializerExpression = null;
				yield return initializerExpression.Expression;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.InitializerExpression;
			}
		}

		public BlockExpression Expression
		{
			get;
			set;
		}

		public ExpressionCollection Expressions
		{
			get
			{
				return this.Expression.Expressions;
			}
			set
			{
				this.Expression.Expressions = value;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				throw new NotSupportedException("Initializer expressions have no type.");
			}
			set
			{
				throw new NotSupportedException("Initializer expressions cannot have type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return false;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.InitializerType InitializerType
		{
			get;
			set;
		}

		public bool IsMultiLine
		{
			get;
			set;
		}

		public InitializerExpression(ExpressionCollection expressions, Telerik.JustDecompiler.Ast.Expressions.InitializerType initializerType) : this(new BlockExpression(expressions, null), initializerType, Enumerable.Empty<Instruction>())
		{
		}

		public InitializerExpression(BlockExpression expression, Telerik.JustDecompiler.Ast.Expressions.InitializerType initializerType) : this(expression, initializerType, Enumerable.Empty<Instruction>())
		{
		}

		public InitializerExpression(BlockExpression expression, Telerik.JustDecompiler.Ast.Expressions.InitializerType initializerType, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Expression = expression;
			this.InitializerType = initializerType;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new InitializerExpression(this.Expression.Clone() as BlockExpression, this.InitializerType, this.instructions)
			{
				IsMultiLine = this.IsMultiLine
			};
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new InitializerExpression(this.Expression.Clone() as BlockExpression, this.InitializerType, null)
			{
				IsMultiLine = this.IsMultiLine
			};
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			InitializerExpression initializerExpression = other as InitializerExpression;
			if (initializerExpression == null || !this.Expression.Equals(initializerExpression.Expression))
			{
				return false;
			}
			return this.InitializerType == initializerExpression.InitializerType;
		}
	}
}