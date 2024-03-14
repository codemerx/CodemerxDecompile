using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class ArrayVariableReferenceExpression : Expression
	{
		public ArrayVariableReferenceExpression(VariableReferenceExpression variable, TypeReference arrayType, ExpressionCollection dimensions, bool hasInitializer, IEnumerable<Instruction> instructions) 
			: base(instructions)
		{
			this.Variable = variable;
			this.ArrayType = arrayType;
			this.Dimensions = dimensions;
			this.HasInitializer = hasInitializer;
		}

		public VariableReferenceExpression Variable { get; set; }

		public TypeReference ArrayType { get; set; }

		public ExpressionCollection Dimensions { get; set; }

		public bool HasInitializer { get; set; }

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				if (Variable != null)
				{
					yield return Variable;
				}

				if (Dimensions != null)
				{
					foreach (ICodeNode dimension in Dimensions)
					{
						yield return dimension;
					}
				}
			}
		}

		public override bool Equals(Expression other)
		{
			if (!(other is ArrayVariableReferenceExpression))
			{
				return false;
			}

			ArrayVariableReferenceExpression otherVariableDeclarationExpression = other as ArrayVariableReferenceExpression;

			if (!this.Variable.Equals(otherVariableDeclarationExpression.Variable))
			{
				return false;
			}

			if (this.ArrayType.FullName != otherVariableDeclarationExpression.ArrayType.FullName)
			{
				return false;
			}

			if (!this.Dimensions.Equals(otherVariableDeclarationExpression.Dimensions))
			{
				return false;
			}

			if (this.HasInitializer != otherVariableDeclarationExpression.HasInitializer)
			{
				return false;
			}

			return true;
		}

		public override Expression Clone()
		{
			return new ArrayVariableReferenceExpression(Variable, ArrayType, Dimensions.CloneExpressionsOnly(), HasInitializer, instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new ArrayVariableReferenceExpression(Variable, ArrayType, Dimensions.CloneExpressionsOnly(), HasInitializer, null);
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.Variable.ExpressionType;
			}
			set
			{
				throw new System.NotSupportedException("Array assignment variable reference expression cannot change its type.");
			}
		}

		public override CodeNodeType CodeNodeType
		{
			get { return CodeNodeType.ArrayAssignmentVariableReferenceExpression; }
		}
	}
}
