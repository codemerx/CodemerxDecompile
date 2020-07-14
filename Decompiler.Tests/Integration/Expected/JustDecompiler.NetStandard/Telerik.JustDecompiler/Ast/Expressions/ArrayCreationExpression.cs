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
	public class ArrayCreationExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				ArrayCreationExpression arrayCreationExpression = null;
				if (arrayCreationExpression.Initializer != null)
				{
					yield return arrayCreationExpression.Initializer;
				}
				if (arrayCreationExpression.Dimensions != null)
				{
					foreach (ICodeNode dimension in arrayCreationExpression.Dimensions)
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
				return Telerik.JustDecompiler.Ast.CodeNodeType.ArrayCreationExpression;
			}
		}

		public ExpressionCollection Dimensions
		{
			get;
			set;
		}

		public TypeReference ElementType
		{
			get;
			set;
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return new ArrayType(this.ElementType, this.Dimensions.Count);
			}
			set
			{
				throw new NotSupportedException("Array creation expression cannot change its type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public InitializerExpression Initializer
		{
			get;
			set;
		}

		public ArrayCreationExpression(TypeReference type, InitializerExpression initializer, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.ElementType = type;
			this.Initializer = initializer;
			this.Dimensions = new ExpressionCollection();
		}

		public override Expression Clone()
		{
			InitializerExpression initializerExpression;
			if (this.Initializer != null)
			{
				initializerExpression = (InitializerExpression)this.Initializer.Clone();
			}
			else
			{
				initializerExpression = null;
			}
			return new ArrayCreationExpression(this.ElementType, initializerExpression, this.instructions)
			{
				Dimensions = this.Dimensions.Clone()
			};
		}

		public override Expression CloneExpressionOnly()
		{
			InitializerExpression initializerExpression;
			if (this.Initializer != null)
			{
				initializerExpression = (InitializerExpression)this.Initializer.CloneExpressionOnly();
			}
			else
			{
				initializerExpression = null;
			}
			return new ArrayCreationExpression(this.ElementType, initializerExpression, null)
			{
				Dimensions = this.Dimensions.CloneExpressionsOnly()
			};
		}

		public override bool Equals(Expression other)
		{
			if (!(other is ArrayCreationExpression))
			{
				return false;
			}
			ArrayCreationExpression arrayCreationExpression = other as ArrayCreationExpression;
			if (this.Initializer == null)
			{
				if (arrayCreationExpression.Initializer != null)
				{
					return false;
				}
			}
			else if (!this.Initializer.Equals(arrayCreationExpression.Initializer))
			{
				return false;
			}
			if (this.ElementType.get_FullName() != arrayCreationExpression.ElementType.get_FullName())
			{
				return false;
			}
			return this.Dimensions.Equals(arrayCreationExpression.Dimensions);
		}
	}
}