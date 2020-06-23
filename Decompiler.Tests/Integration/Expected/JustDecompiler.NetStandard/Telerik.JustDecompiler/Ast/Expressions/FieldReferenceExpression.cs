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
	public class FieldReferenceExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				FieldReferenceExpression fieldReferenceExpression = null;
				if (fieldReferenceExpression.Target != null)
				{
					yield return fieldReferenceExpression.Target;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.FieldReferenceExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.Field.FieldType;
			}
			set
			{
				throw new NotSupportedException("Field expression cannot change its type.");
			}
		}

		public FieldReference Field
		{
			get;
			private set;
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		internal bool IsSimpleStore
		{
			get;
			set;
		}

		public bool IsStatic
		{
			get
			{
				return this.Target == null;
			}
		}

		public Expression Target
		{
			get;
			set;
		}

		public FieldReferenceExpression(Expression target, FieldReference field, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Target = target;
			this.Field = field;
		}

		public override Expression Clone()
		{
			Expression expression;
			if (this.Target != null)
			{
				expression = this.Target.Clone();
			}
			else
			{
				expression = null;
			}
			return new FieldReferenceExpression(expression, this.Field, this.instructions)
			{
				IsSimpleStore = this.IsSimpleStore
			};
		}

		public override Expression CloneExpressionOnly()
		{
			Expression expression;
			if (this.Target != null)
			{
				expression = this.Target.CloneExpressionOnly();
			}
			else
			{
				expression = null;
			}
			return new FieldReferenceExpression(expression, this.Field, null)
			{
				IsSimpleStore = this.IsSimpleStore
			};
		}

		public override bool Equals(Expression other)
		{
			if (!(other is FieldReferenceExpression))
			{
				return false;
			}
			FieldReferenceExpression fieldReferenceExpression = other as FieldReferenceExpression;
			if (this.Target == null)
			{
				if (fieldReferenceExpression.Target != null)
				{
					return false;
				}
			}
			else if (!this.Target.Equals(fieldReferenceExpression.Target))
			{
				return false;
			}
			return this.Field.FullName == fieldReferenceExpression.Field.FullName;
		}
	}
}