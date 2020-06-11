using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class ArrayAssignmentFieldReferenceExpression : Expression
	{
		public ArrayAssignmentFieldReferenceExpression(FieldReferenceExpression field, TypeReference arrayType, ExpressionCollection dimensions, bool hasInitializer, IEnumerable<Instruction> instructions) 
			: base(instructions)
		{
			this.Field = field;
			this.ArrayType = arrayType;
			this.Dimensions = dimensions;
			this.HasInitializer = hasInitializer;
		}

		public FieldReferenceExpression Field { get; set; }

		public TypeReference ArrayType { get; set; }

		public ExpressionCollection Dimensions { get; set; }

		public bool HasInitializer { get; set; }

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				if (Field != null)
				{
					yield return Field;
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
			if (!(other is ArrayAssignmentFieldReferenceExpression))
			{
				return false;
			}

			ArrayAssignmentFieldReferenceExpression otherVariableDeclarationExpression = other as ArrayAssignmentFieldReferenceExpression;

			if (!this.Field.Equals(otherVariableDeclarationExpression.Field))
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
			return new ArrayAssignmentFieldReferenceExpression(Field, ArrayType, Dimensions.CloneExpressionsOnly(), HasInitializer, instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new ArrayAssignmentFieldReferenceExpression(Field, ArrayType, Dimensions.CloneExpressionsOnly(), HasInitializer, null);
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
				return this.Field.ExpressionType;
			}
			set
			{
				throw new System.NotSupportedException("Array assignment field reference expression cannot change its type.");
			}
		}

		public override CodeNodeType CodeNodeType
		{
			get { return CodeNodeType.ArrayAssignmentFieldReferenceExpression; }
		}
	}
}
