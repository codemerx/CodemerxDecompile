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
	public class AutoPropertyConstructorInitializerExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				AutoPropertyConstructorInitializerExpression autoPropertyConstructorInitializerExpression = null;
				if (autoPropertyConstructorInitializerExpression.Target != null)
				{
					yield return autoPropertyConstructorInitializerExpression.Target;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.AutoPropertyConstructorInitializerExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.Property.get_PropertyType();
			}
			set
			{
				throw new NotSupportedException("Auto-property constructor initializer cannot change its type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return (object)this.Property.get_PropertyType() != (object)null;
			}
		}

		public PropertyDefinition Property
		{
			get;
			private set;
		}

		public Expression Target
		{
			get;
			private set;
		}

		public AutoPropertyConstructorInitializerExpression(PropertyDefinition property, Expression target, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Property = property;
			this.Target = target;
		}

		public override Expression Clone()
		{
			Expression expression;
			PropertyDefinition property = this.Property;
			if (this.Target != null)
			{
				expression = this.Target.Clone();
			}
			else
			{
				expression = null;
			}
			return new AutoPropertyConstructorInitializerExpression(property, expression, base.MappedInstructions);
		}

		public override Expression CloneExpressionOnly()
		{
			Expression expression;
			PropertyDefinition property = this.Property;
			if (this.Target != null)
			{
				expression = this.Target.CloneExpressionOnly();
			}
			else
			{
				expression = null;
			}
			return new AutoPropertyConstructorInitializerExpression(property, expression, null);
		}

		public override bool Equals(Expression other)
		{
			AutoPropertyConstructorInitializerExpression autoPropertyConstructorInitializerExpression = other as AutoPropertyConstructorInitializerExpression;
			if (autoPropertyConstructorInitializerExpression == null)
			{
				return false;
			}
			if (this.Target == null)
			{
				if (autoPropertyConstructorInitializerExpression.Target != null)
				{
					return false;
				}
			}
			else if (!this.Target.Equals(autoPropertyConstructorInitializerExpression.Target))
			{
				return false;
			}
			return this.Property.get_FullName() == autoPropertyConstructorInitializerExpression.Property.get_FullName();
		}
	}
}