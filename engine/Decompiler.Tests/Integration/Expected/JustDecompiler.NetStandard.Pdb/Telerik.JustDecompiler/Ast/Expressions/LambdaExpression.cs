using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class LambdaExpression : Expression
	{
		public ExpressionCollection Arguments
		{
			get;
			set;
		}

		public BlockStatement Body
		{
			get;
			set;
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new LambdaExpression.u003cget_Childrenu003ed__10(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 50;
			}
		}

		public bool IsAsync
		{
			get;
			private set;
		}

		public bool IsExpressionTreeLambda
		{
			get;
			private set;
		}

		public bool IsFunction
		{
			get;
			private set;
		}

		public ParameterReference[] Parameters
		{
			get;
			private set;
		}

		public LambdaExpression(ExpressionCollection arguments, BlockStatement body, bool isAsync, bool isFunction, IEnumerable<ParameterReference> parameters, bool isExpressionTreeLambda, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Arguments(arguments);
			this.set_Body(body);
			this.set_IsAsync(isAsync);
			this.set_IsFunction(isFunction);
			this.set_Parameters(parameters.ToArray<ParameterReference>());
			this.set_IsExpressionTreeLambda(isExpressionTreeLambda);
			return;
		}

		public override Expression Clone()
		{
			stackVariable17 = new LambdaExpression(this.get_Arguments().Clone(), this.get_Body().Clone() as BlockStatement, this.get_IsAsync(), this.get_IsFunction(), this.get_Parameters(), this.get_IsExpressionTreeLambda(), this.instructions);
			stackVariable17.set_ExpressionType(this.get_ExpressionType());
			return stackVariable17;
		}

		public override Expression CloneExpressionOnly()
		{
			stackVariable16 = new LambdaExpression(this.get_Arguments().CloneExpressionsOnly(), this.get_Body().CloneStatementOnly() as BlockStatement, this.get_IsAsync(), this.get_IsFunction(), this.get_Parameters(), this.get_IsExpressionTreeLambda(), null);
			stackVariable16.set_ExpressionType(this.get_ExpressionType());
			return stackVariable16;
		}

		public override bool Equals(Expression other)
		{
			return this.Equals(other);
		}
	}
}