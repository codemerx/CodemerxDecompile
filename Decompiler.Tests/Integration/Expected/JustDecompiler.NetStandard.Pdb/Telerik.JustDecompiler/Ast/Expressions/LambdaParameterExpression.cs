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
	public class LambdaParameterExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				return new LambdaParameterExpression.u003cget_Childrenu003ed__15(-2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 64;
			}
		}

		public bool DisplayType
		{
			get;
			private set;
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_Parameter().get_ParameterType();
			}
			set
			{
				throw new Exception("Cannot set the type of lambda parameter.");
			}
		}

		public ParameterReference Parameter
		{
			get;
			private set;
		}

		public LambdaParameterExpression(ParameterReference parameterRef, bool displayType, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			if (parameterRef == null)
			{
				throw new ArgumentNullException("parameterRef");
			}
			this.set_Parameter(parameterRef);
			this.set_DisplayType(displayType);
			return;
		}

		public override Expression Clone()
		{
			return new LambdaParameterExpression(this.get_Parameter(), this.get_DisplayType(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new LambdaParameterExpression(this.get_Parameter(), this.get_DisplayType(), null);
		}

		public override bool Equals(Expression other)
		{
			return this == other;
		}
	}
}