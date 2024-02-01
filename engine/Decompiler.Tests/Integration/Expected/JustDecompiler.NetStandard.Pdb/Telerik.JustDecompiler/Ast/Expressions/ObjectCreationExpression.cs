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
	public class ObjectCreationExpression : Expression
	{
		public ExpressionCollection Arguments
		{
			get;
			set;
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				ObjectCreationExpression objectCreationExpression = null;
				if (objectCreationExpression.Initializer != null)
				{
					yield return objectCreationExpression.Initializer;
				}
				foreach (ICodeNode argument in objectCreationExpression.Arguments)
				{
					yield return argument;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.ObjectCreationExpression;
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
				return this.Constructor.get_DeclaringType();
			}
			set
			{
				throw new NotSupportedException("Object creation expression cannot change it's type");
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

		public ObjectCreationExpression(MethodReference constructor, TypeReference type, InitializerExpression initializer, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Constructor = constructor;
			this.Type = type;
			this.Initializer = initializer;
			this.Arguments = new ExpressionCollection();
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
			return new ObjectCreationExpression(this.Constructor, this.Type, initializerExpression1, this.instructions)
			{
				Arguments = this.Arguments.Clone()
			};
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
			InitializerExpression initializerExpression1 = initializerExpression;
			return new ObjectCreationExpression(this.Constructor, this.Type, initializerExpression1, null)
			{
				Arguments = this.Arguments.CloneExpressionsOnly()
			};
		}

		public override bool Equals(Expression other)
		{
			if (!(other is ObjectCreationExpression))
			{
				return false;
			}
			ObjectCreationExpression objectCreationExpression = other as ObjectCreationExpression;
			if (this.Constructor == null)
			{
				if (objectCreationExpression.Constructor != null)
				{
					return false;
				}
			}
			else if (objectCreationExpression.Constructor == null || this.Constructor.get_FullName() != objectCreationExpression.Constructor.get_FullName())
			{
				return false;
			}
			if (this.Arguments == null)
			{
				if (objectCreationExpression.Arguments != null)
				{
					return false;
				}
			}
			else if (objectCreationExpression.Arguments == null || !this.Arguments.Equals(objectCreationExpression.Arguments))
			{
				return false;
			}
			if (this.Type == null)
			{
				if (objectCreationExpression.Type != null)
				{
					return false;
				}
			}
			else if (objectCreationExpression.Type == null || this.Type.get_FullName() != objectCreationExpression.Type.get_FullName())
			{
				return false;
			}
			if (this.Initializer == null)
			{
				if (objectCreationExpression.Initializer != null)
				{
					return false;
				}
			}
			else if (objectCreationExpression.Initializer == null || !this.Initializer.Equals(objectCreationExpression.Initializer))
			{
				return false;
			}
			return true;
		}
	}
}