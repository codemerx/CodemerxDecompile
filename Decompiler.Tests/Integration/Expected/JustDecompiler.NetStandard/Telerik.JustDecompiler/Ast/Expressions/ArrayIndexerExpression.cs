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
	public class ArrayIndexerExpression : Expression, IIndexerExpression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				ArrayIndexerExpression arrayIndexerExpression = null;
				yield return arrayIndexerExpression.Target;
				foreach (ICodeNode index in arrayIndexerExpression.Indices)
				{
					yield return index;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.ArrayIndexerExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				TypeReference expressionType = this.Target.ExpressionType;
				if (expressionType == null)
				{
					return null;
				}
				if (this.Target is ArgumentReferenceExpression)
				{
					TypeReference typeReference = (this.Target as ArgumentReferenceExpression).ExpressionType;
					if (typeReference.IsByReference)
					{
						expressionType = (typeReference as ByReferenceType).ElementType;
					}
				}
				if (expressionType.IsOptionalModifier)
				{
					expressionType = (expressionType as OptionalModifierType).ElementType;
				}
				if (expressionType.IsRequiredModifier)
				{
					expressionType = (expressionType as RequiredModifierType).ElementType;
				}
				if (expressionType.IsArray)
				{
					return (expressionType as ArrayType).ElementType;
				}
				if (expressionType.FullName == "System.Array")
				{
					return expressionType.Module.TypeSystem.Object;
				}
				if (expressionType.FullName != "System.String")
				{
					throw new ArgumentOutOfRangeException("Target of array indexer expression is not an array.");
				}
				return expressionType.Module.TypeSystem.Char;
			}
			set
			{
				throw new NotSupportedException("Array indexer expression cannot change its type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return this.Target.HasType;
			}
		}

		public ExpressionCollection Indices
		{
			get;
			set;
		}

		internal bool IsSimpleStore
		{
			get;
			set;
		}

		public Expression Target
		{
			get;
			set;
		}

		public ArrayIndexerExpression(Expression target, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Target = target;
			this.Indices = new ExpressionCollection();
		}

		public override Expression Clone()
		{
			return new ArrayIndexerExpression(this.Target.Clone(), this.instructions)
			{
				Indices = this.Indices.Clone(),
				IsSimpleStore = this.IsSimpleStore
			};
		}

		public override Expression CloneExpressionOnly()
		{
			return new ArrayIndexerExpression(this.Target.CloneExpressionOnly(), null)
			{
				Indices = this.Indices.CloneExpressionsOnly(),
				IsSimpleStore = this.IsSimpleStore
			};
		}

		public override bool Equals(Expression other)
		{
			if (!(other is ArrayIndexerExpression))
			{
				return false;
			}
			ArrayIndexerExpression arrayIndexerExpression = other as ArrayIndexerExpression;
			if (!this.Target.Equals(arrayIndexerExpression.Target))
			{
				return false;
			}
			return this.Indices.Equals(arrayIndexerExpression.Indices);
		}
	}
}