using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
				LambdaExpression lambdaExpression = null;
				foreach (ICodeNode argument in lambdaExpression.Arguments)
				{
					yield return argument;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.LambdaExpression;
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

		public LambdaExpression(ExpressionCollection arguments, BlockStatement body, bool isAsync, bool isFunction, IEnumerable<ParameterReference> parameters, bool isExpressionTreeLambda, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Arguments = arguments;
			this.Body = body;
			this.IsAsync = isAsync;
			this.IsFunction = isFunction;
			this.Parameters = parameters.ToArray<ParameterReference>();
			this.IsExpressionTreeLambda = isExpressionTreeLambda;
		}

		public override Expression Clone()
		{
			return new LambdaExpression(this.Arguments.Clone(), this.Body.Clone() as BlockStatement, this.IsAsync, this.IsFunction, this.Parameters, this.IsExpressionTreeLambda, this.instructions)
			{
				ExpressionType = this.ExpressionType
			};
		}

		public override Expression CloneExpressionOnly()
		{
			return new LambdaExpression(this.Arguments.CloneExpressionsOnly(), this.Body.CloneStatementOnly() as BlockStatement, this.IsAsync, this.IsFunction, this.Parameters, this.IsExpressionTreeLambda, null)
			{
				ExpressionType = this.ExpressionType
			};
		}

		public override bool Equals(Expression other)
		{
			return this.Equals(other);
		}
	}
}