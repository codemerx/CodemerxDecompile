using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class InitializerExpression : Expression
	{
		public InitializerExpression(ExpressionCollection expressions, InitializerType initializerType)
			: this(new BlockExpression(expressions, null), initializerType, Enumerable.Empty<Instruction>())
		{
		}

		public InitializerExpression(BlockExpression expression, InitializerType initializerType)
			: this(expression, initializerType, Enumerable.Empty<Instruction>())
		{
		}

		public InitializerExpression(BlockExpression expression, InitializerType initializerType, IEnumerable<Instruction> instructions)
			: base(instructions)
		{
			this.Expression = expression;
			this.InitializerType = initializerType;
		}

		public BlockExpression Expression { get; set; }

		public ExpressionCollection Expressions {
			get { return this.Expression.Expressions; }
			set { this.Expression.Expressions = value; }
		}

		public InitializerType InitializerType { get; set; }

		public bool IsMultiLine { get; set; }

		public override bool Equals(Expression other)
		{
			InitializerExpression initializer = other as InitializerExpression;
			bool result = initializer != null && this.Expression.Equals(initializer.Expression) &&
				this.InitializerType == initializer.InitializerType;
				//this.IsMultiLine == initializer.IsMultiLine;
			return result;
		}

		public override Expression Clone()
		{
			InitializerExpression result = new InitializerExpression(
				Expression.Clone() as BlockExpression, this.InitializerType, this.instructions);
			result.IsMultiLine = this.IsMultiLine;
			return result;
		}

		public override Expression CloneExpressionOnly()
		{
			InitializerExpression result = new InitializerExpression(
				Expression.Clone() as BlockExpression, this.InitializerType, null);
			result.IsMultiLine = this.IsMultiLine;
			return result;
		}

		public override CodeNodeType CodeNodeType
		{
			get { return CodeNodeType.InitializerExpression; }
		}

		public override IEnumerable<ICodeNode> Children
		{
			get { yield return this.Expression; }
		}

		public override bool HasType
		{
			get { return false; }
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
	}

	public enum InitializerType
	{
		CollectionInitializer,
		ObjectInitializer,
		ArrayInitializer,
		AnonymousInitializer,
	}
}
