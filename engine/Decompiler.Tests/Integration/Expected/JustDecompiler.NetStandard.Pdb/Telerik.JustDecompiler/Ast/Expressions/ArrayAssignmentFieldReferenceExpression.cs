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
	public class ArrayAssignmentFieldReferenceExpression : Expression
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
				stackVariable1 = new ArrayAssignmentFieldReferenceExpression.u003cget_Childrenu003ed__18(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 84;
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
				return this.get_Field().get_ExpressionType();
			}
			set
			{
				throw new NotSupportedException("Array assignment field reference expression cannot change its type.");
			}
		}

		public FieldReferenceExpression Field
		{
			get;
			set;
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

		public ArrayAssignmentFieldReferenceExpression(FieldReferenceExpression field, TypeReference arrayType, ExpressionCollection dimensions, bool hasInitializer, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Field(field);
			this.set_ArrayType(arrayType);
			this.set_Dimensions(dimensions);
			this.set_HasInitializer(hasInitializer);
			return;
		}

		public override Expression Clone()
		{
			return new ArrayAssignmentFieldReferenceExpression(this.get_Field(), this.get_ArrayType(), this.get_Dimensions().CloneExpressionsOnly(), this.get_HasInitializer(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new ArrayAssignmentFieldReferenceExpression(this.get_Field(), this.get_ArrayType(), this.get_Dimensions().CloneExpressionsOnly(), this.get_HasInitializer(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other as ArrayAssignmentFieldReferenceExpression == null)
			{
				return false;
			}
			V_0 = other as ArrayAssignmentFieldReferenceExpression;
			if (!this.get_Field().Equals(V_0.get_Field()))
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