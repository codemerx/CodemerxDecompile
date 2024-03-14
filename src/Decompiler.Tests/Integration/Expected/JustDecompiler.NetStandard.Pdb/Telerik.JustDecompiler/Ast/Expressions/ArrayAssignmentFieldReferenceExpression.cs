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
				ArrayAssignmentFieldReferenceExpression arrayAssignmentFieldReferenceExpression = null;
				if (arrayAssignmentFieldReferenceExpression.Field != null)
				{
					yield return arrayAssignmentFieldReferenceExpression.Field;
				}
				if (arrayAssignmentFieldReferenceExpression.Dimensions != null)
				{
					foreach (ICodeNode dimension in arrayAssignmentFieldReferenceExpression.Dimensions)
					{
						yield return dimension;
					}
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.ArrayAssignmentFieldReferenceExpression;
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
				return this.Field.ExpressionType;
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

		public ArrayAssignmentFieldReferenceExpression(FieldReferenceExpression field, TypeReference arrayType, ExpressionCollection dimensions, bool hasInitializer, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Field = field;
			this.ArrayType = arrayType;
			this.Dimensions = dimensions;
			this.HasInitializer = hasInitializer;
		}

		public override Expression Clone()
		{
			return new ArrayAssignmentFieldReferenceExpression(this.Field, this.ArrayType, this.Dimensions.CloneExpressionsOnly(), this.HasInitializer, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new ArrayAssignmentFieldReferenceExpression(this.Field, this.ArrayType, this.Dimensions.CloneExpressionsOnly(), this.HasInitializer, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is ArrayAssignmentFieldReferenceExpression))
			{
				return false;
			}
			ArrayAssignmentFieldReferenceExpression arrayAssignmentFieldReferenceExpression = other as ArrayAssignmentFieldReferenceExpression;
			if (!this.Field.Equals(arrayAssignmentFieldReferenceExpression.Field))
			{
				return false;
			}
			if (this.ArrayType.get_FullName() != arrayAssignmentFieldReferenceExpression.ArrayType.get_FullName())
			{
				return false;
			}
			if (!this.Dimensions.Equals(arrayAssignmentFieldReferenceExpression.Dimensions))
			{
				return false;
			}
			if (this.HasInitializer != arrayAssignmentFieldReferenceExpression.HasInitializer)
			{
				return false;
			}
			return true;
		}
	}
}