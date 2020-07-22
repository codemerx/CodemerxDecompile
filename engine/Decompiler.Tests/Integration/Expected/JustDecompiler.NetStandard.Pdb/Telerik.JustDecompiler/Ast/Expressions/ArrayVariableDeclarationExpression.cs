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
	public class ArrayVariableDeclarationExpression : Expression
	{
		public TypeReference ArrayType
		{
			get;
			set;
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new ArrayVariableDeclarationExpression.u003cget_Childrenu003ed__18(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 82;
			}
		}

		public ExpressionCollection Dimensions
		{
			get;
			set;
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_Variable().get_ExpressionType();
			}
			set
			{
				throw new NotSupportedException("Array variable creation expression cannot change its type.");
			}
		}

		public bool HasInitializer
		{
			get;
			set;
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public VariableDeclarationExpression Variable
		{
			get;
			set;
		}

		public ArrayVariableDeclarationExpression(VariableDeclarationExpression variable, TypeReference arrayType, ExpressionCollection dimensions, bool hasInitializer, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Variable(variable);
			this.set_ArrayType(arrayType);
			this.set_Dimensions(dimensions);
			this.set_HasInitializer(hasInitializer);
			return;
		}

		public override Expression Clone()
		{
			return new ArrayVariableDeclarationExpression(this.get_Variable(), this.get_ArrayType(), this.get_Dimensions().CloneExpressionsOnly(), this.get_HasInitializer(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new ArrayVariableDeclarationExpression(this.get_Variable(), this.get_ArrayType(), this.get_Dimensions().CloneExpressionsOnly(), this.get_HasInitializer(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other as ArrayVariableDeclarationExpression == null)
			{
				return false;
			}
			V_0 = other as ArrayVariableDeclarationExpression;
			if (!this.get_Variable().Equals(V_0.get_Variable()))
			{
				return false;
			}
			if (String.op_Inequality(this.get_ArrayType().get_FullName(), V_0.get_ArrayType().get_FullName()))
			{
				return false;
			}
			if (!this.get_Dimensions().Equals(V_0.get_Dimensions()))
			{
				return false;
			}
			if (this.get_HasInitializer() != V_0.get_HasInitializer())
			{
				return false;
			}
			return true;
		}
	}
}