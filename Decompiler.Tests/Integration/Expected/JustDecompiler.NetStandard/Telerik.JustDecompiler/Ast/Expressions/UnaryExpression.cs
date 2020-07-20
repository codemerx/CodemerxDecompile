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
				stackVariable1 = new UnaryExpression.u003cget_Childrenu003ed__11(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 23;
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
				return this.get_Operand().get_HasType();
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

		public UnaryExpression(UnaryOperator operator, Expression operand, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Operator(operator);
			this.set_Operand(operand);
			if (operand as UnaryExpression != null && operator == 11)
			{
				this.set_Operand((operand as UnaryExpression).get_Operand());
				this.set_Operator((operand as UnaryExpression).get_Operator());
				this.instructions.AddRange((operand as UnaryExpression).instructions);
			}
			return;
		}

		public override Expression Clone()
		{
			stackVariable7 = new UnaryExpression(this.get_Operator(), this.get_Operand().Clone(), this.instructions);
			stackVariable7.expressionType = this.expressionType;
			return stackVariable7;
		}

		public override Expression CloneExpressionOnly()
		{
			stackVariable7 = new UnaryExpression(this.get_Operator(), this.get_Operand().CloneExpressionOnly(), new Instruction[0]);
			stackVariable7.expressionType = this.expressionType;
			return stackVariable7;
		}

		public void DecideExpressionType()
		{
			if (this.get_Operand().get_ExpressionType() == null)
			{
				this.expressionType = null;
				return;
			}
			if (this.get_Operator() != 8)
			{
				if (this.get_Operator() == 9)
				{
					this.expressionType = new PointerType(this.get_Operand().get_ExpressionType());
					return;
				}
				if (this.get_Operator() != 7)
				{
					this.expressionType = this.get_Operand().get_ExpressionType();
					return;
				}
				this.expressionType = new ByReferenceType(this.get_Operand().get_ExpressionType());
				return;
			}
			V_0 = this.get_Operand().get_ExpressionType();
			if (V_0 == null)
			{
				this.expressionType = null;
				return;
			}
			while (V_0.get_IsOptionalModifier() || V_0.get_IsRequiredModifier())
			{
				V_0 = (V_0 as TypeSpecification).get_ElementType();
			}
			if (V_0.get_IsPointer())
			{
				this.expressionType = (V_0 as PointerType).get_ElementType();
				return;
			}
			if (V_0.get_IsByReference())
			{
				this.expressionType = (V_0 as ByReferenceType).get_ElementType();
				return;
			}
			if (!V_0.get_IsPinned())
			{
				this.expressionType = V_0;
				return;
			}
			this.expressionType = ((V_0 as PinnedType).get_ElementType() as ByReferenceType).get_ElementType();
			return;
		}

		public override bool Equals(Expression other)
		{
			if (other as UnaryExpression == null)
			{
				return false;
			}
			V_0 = other as UnaryExpression;
			if (this.get_Operator() != V_0.get_Operator())
			{
				return false;
			}
			return this.get_Operand().Equals(V_0.get_Operand());
		}
	}
}