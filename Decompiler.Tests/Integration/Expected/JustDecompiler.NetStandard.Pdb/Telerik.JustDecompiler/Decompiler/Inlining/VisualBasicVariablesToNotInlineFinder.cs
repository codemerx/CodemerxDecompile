using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
	internal class VisualBasicVariablesToNotInlineFinder : BaseCodeVisitor, IVariablesToNotInlineFinder
	{
		private Dictionary<VariableReference, CodeNodeType> variableToAssignedCodeNodeTypeMap;

		private HashSet<VariableDefinition> variablesToNotInline;

		private ILanguage language;

		public VisualBasicVariablesToNotInlineFinder(ILanguage language)
		{
			base();
			this.language = language;
			return;
		}

		public HashSet<VariableDefinition> Find(Dictionary<int, IList<Expression>> blockExpressions)
		{
			this.ResetInternalState();
			V_0 = blockExpressions.get_Values().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = ((List<Expression>)V_0.get_Current()).GetEnumerator();
					try
					{
						while (V_1.MoveNext())
						{
							V_2 = V_1.get_Current();
							this.ProcessExpression(V_2);
						}
					}
					finally
					{
						((IDisposable)V_1).Dispose();
					}
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return this.variablesToNotInline;
		}

		public HashSet<VariableDefinition> Find(StatementCollection statements)
		{
			this.ResetInternalState();
			this.Visit(statements);
			return this.variablesToNotInline;
		}

		private void ProcessBinaryExpression(BinaryExpression binaryExpression)
		{
			if (!binaryExpression.get_IsAssignmentExpression())
			{
				return;
			}
			if (binaryExpression.get_Left().get_CodeNodeType() != 26)
			{
				return;
			}
			if (!binaryExpression.get_Right().IsArgumentReferenceToRefParameter())
			{
				V_0 = binaryExpression.get_Right().get_CodeNodeType();
			}
			else
			{
				V_0 = 25;
			}
			V_1 = (binaryExpression.get_Left() as VariableReferenceExpression).get_Variable();
			if (this.variableToAssignedCodeNodeTypeMap.ContainsKey(V_1))
			{
				this.variableToAssignedCodeNodeTypeMap.set_Item(V_1, V_0);
				return;
			}
			this.variableToAssignedCodeNodeTypeMap.Add(V_1, V_0);
			return;
		}

		private void ProcessDelegateInvokeExpression(DelegateInvokeExpression delegateInvokeExpression)
		{
			if (delegateInvokeExpression.get_Target() == null || delegateInvokeExpression.get_Target().get_CodeNodeType() != 26)
			{
				return;
			}
			this.ProcessVariableReferenceExpression(delegateInvokeExpression.get_Target() as VariableReferenceExpression);
			return;
		}

		private void ProcessExpression(Expression expression)
		{
			if (expression.get_CodeNodeType() == 24)
			{
				this.ProcessBinaryExpression(expression as BinaryExpression);
				return;
			}
			if (expression.get_CodeNodeType() == 19)
			{
				this.ProcessMethodInvocation(expression as MethodInvocationExpression);
				return;
			}
			if (expression.get_CodeNodeType() == 51)
			{
				this.ProcessDelegateInvokeExpression(expression as DelegateInvokeExpression);
			}
			return;
		}

		private void ProcessMethodInvocation(MethodInvocationExpression methodInvocationExpression)
		{
			V_0 = methodInvocationExpression.GetTarget();
			if (V_0 == null)
			{
				return;
			}
			if (V_0.get_CodeNodeType() != 26)
			{
				if (V_0.get_CodeNodeType() != 23)
				{
					return;
				}
				V_2 = V_0 as UnaryExpression;
				if (V_2.get_Operator() != 8 || V_2.get_Operand().get_CodeNodeType() != 23)
				{
					return;
				}
				V_3 = V_2.get_Operand() as UnaryExpression;
				if (V_3.get_Operator() != 7 || V_3.get_Operand().get_CodeNodeType() != 26)
				{
					return;
				}
				V_1 = V_3.get_Operand() as VariableReferenceExpression;
			}
			else
			{
				V_1 = V_0 as VariableReferenceExpression;
			}
			this.ProcessVariableReferenceExpression(V_1);
			return;
		}

		private void ProcessVariableReferenceExpression(VariableReferenceExpression variableReferenceExpression)
		{
			V_0 = variableReferenceExpression.get_Variable();
			if (this.variableToAssignedCodeNodeTypeMap.ContainsKey(V_0))
			{
				V_1 = this.variableToAssignedCodeNodeTypeMap.get_Item(V_0);
				V_2 = V_0.Resolve();
				if (!this.variablesToNotInline.Contains(V_2) && !this.language.IsValidLineStarter(V_1))
				{
					dummyVar0 = this.variablesToNotInline.Add(V_2);
				}
			}
			return;
		}

		private void ResetInternalState()
		{
			this.variableToAssignedCodeNodeTypeMap = new Dictionary<VariableReference, CodeNodeType>();
			this.variablesToNotInline = new HashSet<VariableDefinition>();
			return;
		}

		public override void VisitExpressionStatement(ExpressionStatement node)
		{
			this.ProcessExpression(node.get_Expression());
			return;
		}
	}
}