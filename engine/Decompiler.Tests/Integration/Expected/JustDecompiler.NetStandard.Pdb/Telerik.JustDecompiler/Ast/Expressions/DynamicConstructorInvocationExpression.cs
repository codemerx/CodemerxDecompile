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
	public class DynamicConstructorInvocationExpression : Expression
	{
		public ExpressionCollection Arguments
		{
			get;
			internal set;
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new DynamicConstructorInvocationExpression.u003cget_Childrenu003ed__10(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 60;
			}
		}

		public TypeReference ConstructorType
		{
			get;
			private set;
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_ConstructorType();
			}
		}

		public DynamicConstructorInvocationExpression(TypeReference constructorType, IEnumerable<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_ConstructorType(constructorType);
			this.set_Arguments(new ExpressionCollection(arguments));
			return;
		}

		public override Expression Clone()
		{
			return new DynamicConstructorInvocationExpression(this.get_ConstructorType(), this.get_Arguments().Clone(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new DynamicConstructorInvocationExpression(this.get_ConstructorType(), this.get_Arguments().CloneExpressionsOnly(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other.get_CodeNodeType() != 60)
			{
				return false;
			}
			V_0 = other as DynamicConstructorInvocationExpression;
			if (!String.op_Equality(this.get_ConstructorType().get_FullName(), V_0.get_ConstructorType().get_FullName()))
			{
				return false;
			}
			return this.get_Arguments().Equals(V_0.get_Arguments());
		}
	}
}