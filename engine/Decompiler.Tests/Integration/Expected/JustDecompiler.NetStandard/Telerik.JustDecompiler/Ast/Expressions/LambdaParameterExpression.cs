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
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.LambdaParameterExpression;
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
				return this.Parameter.get_ParameterType();
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

		public LambdaParameterExpression(ParameterReference parameterRef, bool displayType, IEnumerable<Instruction> instructions) : base(instructions)
		{
			if (parameterRef == null)
			{
				throw new ArgumentNullException("parameterRef");
			}
			this.Parameter = parameterRef;
			this.DisplayType = displayType;
		}

		public override Expression Clone()
		{
			return new LambdaParameterExpression(this.Parameter, this.DisplayType, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new LambdaParameterExpression(this.Parameter, this.DisplayType, null);
		}

		public override bool Equals(Expression other)
		{
			return this == other;
		}
	}
}