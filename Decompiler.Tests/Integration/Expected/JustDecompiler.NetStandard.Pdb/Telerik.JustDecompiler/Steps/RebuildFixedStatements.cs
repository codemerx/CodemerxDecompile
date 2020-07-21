using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildFixedStatements : BaseCodeVisitor, IDecompilationStep
	{
		private MethodSpecificContext methodContext;

		private readonly IList<VariableReference> variables;

		public RebuildFixedStatements()
		{
			this.variables = new List<VariableReference>();
			base();
			return;
		}

		private FixedStatement GetFixedStatement(Statement statement, List<VariableReference> variableReferences)
		{
			if (statement.get_CodeNodeType() == 5)
			{
				V_0 = statement as ExpressionStatement;
				if (V_0.get_Expression() as BinaryExpression != null)
				{
					V_1 = V_0.get_Expression() as BinaryExpression;
					if (V_1.get_IsAssignmentExpression() && V_1.get_Left().get_ExpressionType().get_IsPinned() && V_1.get_Right().get_CodeNodeType() != 22)
					{
						variableReferences.Add((V_1.get_Left() as VariableReferenceExpression).get_Variable());
						return new FixedStatement(V_0.get_Expression(), new BlockStatement());
					}
				}
			}
			return null;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.get_MethodContext();
			this.Visit(body);
			return body;
		}

		private void ProcessBlock(BlockStatement node)
		{
			V_0 = 0;
			while (V_0 < node.get_Statements().get_Count() - 1)
			{
				V_1 = null;
				V_2 = new List<VariableReference>();
				V_3 = node.get_Statements().get_Item(V_0);
				V_1 = this.GetFixedStatement(V_3, V_2);
				if (V_1 != null)
				{
					V_7 = V_2.GetEnumerator();
					try
					{
						while (V_7.MoveNext())
						{
							V_8 = V_7.get_Current();
							this.methodContext.RemoveVariable(V_8);
						}
					}
					finally
					{
						((IDisposable)V_7).Dispose();
					}
					node.get_Statements().RemoveAt(V_0);
					node.AddStatementAt(V_0, V_1);
					V_4 = node.get_Statements().get_Item(V_0 + 1) as ExpressionStatement;
					V_5 = node.get_Statements().get_Count();
					V_6 = null;
					stackVariable47 = new RebuildFixedStatements.VariableReferenceVisitor();
					stackVariable47.Visit(V_1.get_Expression());
					V_6 = stackVariable47.get_Variable();
					V_9 = V_0 + 1;
					while (V_9 < V_5)
					{
						stackVariable56 = new RebuildFixedStatements.VariableReferenceVisitor();
						stackVariable56.Visit(node.get_Statements().get_Item(V_9));
						V_10 = stackVariable56.get_Variable();
						if (V_10 != null && !this.variables.Contains(V_10))
						{
							this.variables.Add(V_10);
						}
						if (node.get_Statements().get_Item(V_9).get_CodeNodeType() == 5)
						{
							V_4 = node.get_Statements().get_Item(V_9) as ExpressionStatement;
							if (V_10 != null && (object)V_10 == (object)V_6 && V_4.get_Expression().get_CodeNodeType() == 24 && (V_4.get_Expression() as BinaryExpression).get_IsAssignmentExpression() && (V_4.get_Expression() as BinaryExpression).get_Right().get_CodeNodeType() == 22 && ((V_4.get_Expression() as BinaryExpression).get_Right() as LiteralExpression).get_Value() == null)
							{
								node.get_Statements().RemoveAt(V_9);
								V_9 = V_9 - 1;
								V_5 = V_5 - 1;
								break;
							}
						}
						V_1.get_Body().AddStatement(node.get_Statements().get_Item(V_9));
						node.get_Statements().RemoveAt(V_9);
						V_9 = V_9 - 1;
						V_5 = V_5 - 1;
						V_9 = V_9 + 1;
					}
					this.ProcessBlock(V_1.get_Body());
					return;
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			this.ProcessBlock(node);
			V_0 = node.get_Statements().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.Visit(V_1);
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			return;
		}

		internal class VariableReferenceVisitor : BaseCodeVisitor
		{
			public VariableReference Variable
			{
				get;
				private set;
			}

			public VariableReferenceVisitor()
			{
				base();
				return;
			}

			public override void VisitBinaryExpression(BinaryExpression node)
			{
				if (!node.get_IsAssignmentExpression())
				{
					this.VisitBinaryExpression(node);
					return;
				}
				this.Visit(node.get_Left());
				return;
			}

			public override void VisitExpressionStatement(ExpressionStatement node)
			{
				this.Visit(node.get_Expression());
				return;
			}

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				this.set_Variable(node.get_Variable());
				return;
			}
		}
	}
}