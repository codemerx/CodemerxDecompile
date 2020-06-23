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
				ArrayVariableDeclarationExpression arrayVariableDeclarationExpression = null;
				if (arrayVariableDeclarationExpression.Variable != null)
				{
					yield return arrayVariableDeclarationExpression.Variable;
				}
				if (arrayVariableDeclarationExpression.Dimensions != null)
				{
					foreach (ICodeNode dimension in arrayVariableDeclarationExpression.Dimensions)
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
				return Telerik.JustDecompiler.Ast.CodeNodeType.ArrayVariableCreationExpression;
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

		public ArrayVariableDeclarationExpression(VariableDeclarationExpression variable, TypeReference arrayType, ExpressionCollection dimensions, bool hasInitializer, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Variable = variable;
			this.ArrayType = arrayType;
			this.Dimensions = dimensions;
			this.HasInitializer = hasInitializer;
		}

		public override Expression Clone()
		{
			return new ArrayVariableDeclarationExpression(this.Variable, this.ArrayType, this.Dimensions.CloneExpressionsOnly(), this.HasInitializer, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new ArrayVariableDeclarationExpression(this.Variable, this.ArrayType, this.Dimensions.CloneExpressionsOnly(), this.HasInitializer, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is ArrayVariableDeclarationExpression))
			{
				return false;
			}
			ArrayVariableDeclarationExpression arrayVariableDeclarationExpression = other as ArrayVariableDeclarationExpression;
			if (!this.Variable.Equals(arrayVariableDeclarationExpression.Variable))
			{
				return false;
			}
			if (this.ArrayType.FullName != arrayVariableDeclarationExpression.ArrayType.FullName)
			{
				return false;
			}
			if (!this.Dimensions.Equals(arrayVariableDeclarationExpression.Dimensions))
			{
				return false;
			}
			if (this.HasInitializer != arrayVariableDeclarationExpression.HasInitializer)
			{
				return false;
			}
			return true;
		}
	}
}