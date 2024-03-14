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
	public class ArrayVariableReferenceExpression : Expression
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
				ArrayVariableReferenceExpression arrayVariableReferenceExpression = null;
				if (arrayVariableReferenceExpression.Variable != null)
				{
					yield return arrayVariableReferenceExpression.Variable;
				}
				if (arrayVariableReferenceExpression.Dimensions != null)
				{
					foreach (ICodeNode dimension in arrayVariableReferenceExpression.Dimensions)
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
				return Telerik.JustDecompiler.Ast.CodeNodeType.ArrayAssignmentVariableReferenceExpression;
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
				return this.Variable.ExpressionType;
			}
			set
			{
				throw new NotSupportedException("Array assignment variable reference expression cannot change its type.");
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

		public VariableReferenceExpression Variable
		{
			get;
			set;
		}

		public ArrayVariableReferenceExpression(VariableReferenceExpression variable, TypeReference arrayType, ExpressionCollection dimensions, bool hasInitializer, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Variable = variable;
			this.ArrayType = arrayType;
			this.Dimensions = dimensions;
			this.HasInitializer = hasInitializer;
		}

		public override Expression Clone()
		{
			return new ArrayVariableReferenceExpression(this.Variable, this.ArrayType, this.Dimensions.CloneExpressionsOnly(), this.HasInitializer, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new ArrayVariableReferenceExpression(this.Variable, this.ArrayType, this.Dimensions.CloneExpressionsOnly(), this.HasInitializer, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is ArrayVariableReferenceExpression))
			{
				return false;
			}
			ArrayVariableReferenceExpression arrayVariableReferenceExpression = other as ArrayVariableReferenceExpression;
			if (!this.Variable.Equals(arrayVariableReferenceExpression.Variable))
			{
				return false;
			}
			if (this.ArrayType.get_FullName() != arrayVariableReferenceExpression.ArrayType.get_FullName())
			{
				return false;
			}
			if (!this.Dimensions.Equals(arrayVariableReferenceExpression.Dimensions))
			{
				return false;
			}
			if (this.HasInitializer != arrayVariableReferenceExpression.HasInitializer)
			{
				return false;
			}
			return true;
		}
	}
}