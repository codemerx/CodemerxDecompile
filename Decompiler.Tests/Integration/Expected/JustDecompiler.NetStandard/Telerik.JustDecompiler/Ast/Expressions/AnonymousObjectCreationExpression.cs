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
	public class AnonymousObjectCreationExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				AnonymousObjectCreationExpression anonymousObjectCreationExpression = null;
				if (anonymousObjectCreationExpression.Initializer != null)
				{
					yield return anonymousObjectCreationExpression.Initializer;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.AnonymousObjectCreationExpression;
			}
		}

		public MethodReference Constructor
		{
			get;
			set;
		}

		public override TypeReference ExpressionType
		{
			get
			{
				if (this.Type != null)
				{
					return this.Type;
				}
				return this.Constructor.DeclaringType;
			}
			set
			{
				throw new NotSupportedException("Anonymous object creation expression cannot change it's type");
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

		public TypeReference Type
		{
			get;
			set;
		}

		public AnonymousObjectCreationExpression(MethodReference constructor, TypeReference type, InitializerExpression initializer, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Constructor = constructor;
			this.Type = type;
			this.Initializer = initializer;
		}

		public override Expression Clone()
		{
			InitializerExpression initializerExpression;
			if (this.Initializer != null)
			{
				initializerExpression = this.Initializer.Clone() as InitializerExpression;
			}
			else
			{
				initializerExpression = null;
			}
			InitializerExpression initializerExpression1 = initializerExpression;
			return new AnonymousObjectCreationExpression(this.Constructor, this.Type, initializerExpression1, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			InitializerExpression initializerExpression;
			if (this.Initializer != null)
			{
				initializerExpression = this.Initializer.CloneExpressionOnly() as InitializerExpression;
			}
			else
			{
				initializerExpression = null;
			}
			return new AnonymousObjectCreationExpression(this.Constructor, this.Type, initializerExpression, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is AnonymousObjectCreationExpression))
			{
				return false;
			}
			AnonymousObjectCreationExpression anonymousObjectCreationExpression = other as AnonymousObjectCreationExpression;
			if (this.Constructor == null)
			{
				if (anonymousObjectCreationExpression.Constructor != null)
				{
					return false;
				}
			}
			else if (anonymousObjectCreationExpression.Constructor == null || this.Constructor.FullName != anonymousObjectCreationExpression.Constructor.FullName)
			{
				return false;
			}
			if (this.Type == null)
			{
				if (anonymousObjectCreationExpression.Type != null)
				{
					return false;
				}
			}
			else if (anonymousObjectCreationExpression.Type == null || this.Type.FullName != anonymousObjectCreationExpression.Type.FullName)
			{
				return false;
			}
			if (this.Initializer == null)
			{
				if (anonymousObjectCreationExpression.Initializer != null)
				{
					return false;
				}
			}
			else if (anonymousObjectCreationExpression.Initializer == null || !this.Initializer.Equals(anonymousObjectCreationExpression.Initializer))
			{
				return false;
			}
			return true;
		}
	}
}