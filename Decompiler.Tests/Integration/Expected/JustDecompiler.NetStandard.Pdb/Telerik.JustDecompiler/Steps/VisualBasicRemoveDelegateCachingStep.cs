using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps
{
	internal class VisualBasicRemoveDelegateCachingStep : RemoveDelegateCachingStep
	{
		private HashSet<VariableReference> variablesToNotInline;

		private Dictionary<VariableReference, Statement> initializationsToFix;

		public VisualBasicRemoveDelegateCachingStep()
		{
			base();
			this.variablesToNotInline = new HashSet<VariableReference>();
			this.initializationsToFix = new Dictionary<VariableReference, Statement>();
			return;
		}

		protected override ICodeNode GetIfSubstitution(IfStatement node)
		{
			V_0 = node.get_Condition() as BinaryExpression;
			if (V_0.get_Left().get_CodeNodeType() != 30)
			{
				return null;
			}
			V_1 = (V_0.get_Left() as FieldReferenceExpression).get_Field().Resolve();
			if (!this.fieldToReplacingExpressionMap.ContainsKey(V_1))
			{
				throw new Exception("Caching field not found.");
			}
			V_2 = new VariableDefinition(V_1.get_FieldType(), this.context.get_MethodContext().get_Method());
			V_3 = new VariableReferenceExpression(V_2, null);
			V_4 = new ExpressionStatement(new BinaryExpression(26, V_3, this.fieldToReplacingExpressionMap.get_Item(V_1), this.context.get_MethodContext().get_Method().get_Module().get_TypeSystem(), null, false));
			this.initializationsToRemove.Add(V_2, V_4);
			this.variableToReplacingExpressionMap.Add(V_2, this.fieldToReplacingExpressionMap.get_Item(V_1));
			this.fieldToReplacingExpressionMap.set_Item(V_1, V_3);
			this.context.get_MethodContext().get_Variables().Add(V_2);
			dummyVar0 = this.context.get_MethodContext().get_VariablesToRename().Add(V_2);
			return V_4;
		}

		protected override void ProcessInitializations()
		{
			V_0 = this.initializationsToFix.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!this.variableToReplacingExpressionMap.ContainsKey(V_1.get_Key()))
					{
						continue;
					}
					((V_1.get_Value() as ExpressionStatement).get_Expression() as BinaryExpression).set_Right(this.variableToReplacingExpressionMap.get_Item(V_1.get_Key()));
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			this.RemoveInitializations();
			return;
		}

		public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			V_0 = this.VisitFieldReferenceExpression(node);
			if (V_0.get_CodeNodeType() != 26)
			{
				return V_0;
			}
			return this.Visit(V_0);
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			V_0 = node.GetTarget();
			if (V_0 != null)
			{
				if (V_0.get_CodeNodeType() != 26)
				{
					if (V_0.get_CodeNodeType() == 30)
					{
						V_2 = (V_0 as FieldReferenceExpression).get_Field().Resolve();
						if (this.fieldToReplacingExpressionMap.ContainsKey(V_2))
						{
							V_3 = (this.fieldToReplacingExpressionMap.get_Item(V_2) as VariableReferenceExpression).get_Variable();
							dummyVar1 = this.variablesToNotInline.Add(V_3);
						}
					}
				}
				else
				{
					V_1 = (V_0 as VariableReferenceExpression).get_Variable();
					if (this.variableToReplacingExpressionMap.ContainsKey(V_1))
					{
						dummyVar0 = this.variablesToNotInline.Add(V_1);
					}
				}
			}
			return this.VisitMethodInvocationExpression(node);
		}

		public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if (!this.variableToReplacingExpressionMap.ContainsKey(node.get_Variable()) || !this.variablesToNotInline.Contains(node.get_Variable()))
			{
				return this.VisitVariableReferenceExpression(node);
			}
			this.initializationsToFix.Add(node.get_Variable(), this.initializationsToRemove.get_Item(node.get_Variable()));
			dummyVar0 = this.initializationsToRemove.Remove(node.get_Variable());
			return node;
		}
	}
}