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
	public class UnaryExpression : Expression
	{
		private TypeReference expressionType;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				UnaryExpression unaryExpression = null;
				yield return unaryExpression.Operand;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.UnaryExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				if (this.expressionType == null)
				{
					this.DecideExpressionType();
				}
				return this.expressionType;
			}
			set
			{
				throw new NotSupportedException("Cannot change the type of Unary expression.");
			}
		}

		public override bool HasType
		{
			get
			{
				return this.Operand.HasType;
			}
		}

		public Expression Operand
		{
			get;
			set;
		}

		public UnaryOperator Operator
		{
			get;
			set;
		}

		public UnaryExpression(UnaryOperator @operator, Expression operand, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Operator = @operator;
			this.Operand = operand;
			if (operand is UnaryExpression && @operator == UnaryOperator.None)
			{
				this.Operand = (operand as UnaryExpression).Operand;
				this.Operator = (operand as UnaryExpression).Operator;
				this.instructions.AddRange((operand as UnaryExpression).instructions);
			}
		}

		public override Expression Clone()
		{
			return new UnaryExpression(this.Operator, this.Operand.Clone(), this.instructions)
			{
				expressionType = this.expressionType
			};
		}

		public override Expression CloneExpressionOnly()
		{
			return new UnaryExpression(this.Operator, this.Operand.CloneExpressionOnly(), new Instruction[0])
			{
				expressionType = this.expressionType
			};
		}

		public void DecideExpressionType()
		{
			if (this.Operand.ExpressionType == null)
			{
				this.expressionType = null;
				return;
			}
			if (this.Operator != UnaryOperator.AddressDereference)
			{
				if (this.Operator == UnaryOperator.AddressOf)
				{
					this.expressionType = new PointerType(this.Operand.ExpressionType);
					return;
				}
				if (this.Operator != UnaryOperator.AddressReference)
				{
					this.expressionType = this.Operand.ExpressionType;
					return;
				}
				this.expressionType = new ByReferenceType(this.Operand.ExpressionType);
				return;
			}
			TypeReference expressionType = this.Operand.ExpressionType;
			if (expressionType == null)
			{
				this.expressionType = null;
				return;
			}
			while (expressionType.get_IsOptionalModifier() || expressionType.get_IsRequiredModifier())
			{
				expressionType = (expressionType as TypeSpecification).get_ElementType();
			}
			if (expressionType.get_IsPointer())
			{
				this.expressionType = (expressionType as PointerType).get_ElementType();
				return;
			}
			if (expressionType.get_IsByReference())
			{
				this.expressionType = (expressionType as ByReferenceType).get_ElementType();
				return;
			}
			if (!expressionType.get_IsPinned())
			{
				this.expressionType = expressionType;
				return;
			}
			this.expressionType = ((expressionType as PinnedType).get_ElementType() as ByReferenceType).get_ElementType();
		}

		public override bool Equals(Expression other)
		{
			if (!(other is UnaryExpression))
			{
				return false;
			}
			UnaryExpression unaryExpression = other as UnaryExpression;
			if (this.Operator != unaryExpression.Operator)
			{
				return false;
			}
			return this.Operand.Equals(unaryExpression.Operand);
		}
	}
}