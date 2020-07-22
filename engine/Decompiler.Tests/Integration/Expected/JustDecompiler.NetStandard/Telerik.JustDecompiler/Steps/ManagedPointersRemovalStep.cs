using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class ManagedPointersRemovalStep : BaseCodeVisitor, IDecompilationStep
	{
		private DecompilationContext context;

		private readonly Dictionary<VariableDefinition, BinaryExpression> variableToAssignExpression;

		public ManagedPointersRemovalStep()
		{
			this.variableToAssignExpression = new Dictionary<VariableDefinition, BinaryExpression>();
			base();
			return;
		}

		private bool CheckForAssignment(BinaryExpression node)
		{
			if (node.get_Left().get_CodeNodeType() == 25 && (node.get_Left() as ArgumentReferenceExpression).get_Parameter().get_ParameterType().get_IsByReference())
			{
				throw new Exception("Managed pointer usage not in SSA");
			}
			if (node.get_Left().get_CodeNodeType() != 26 || !(node.get_Left() as VariableReferenceExpression).get_Variable().get_VariableType().get_IsByReference())
			{
				return false;
			}
			V_0 = (node.get_Left() as VariableReferenceExpression).get_Variable().Resolve();
			if (this.variableToAssignExpression.ContainsKey(V_0))
			{
				throw new Exception("Managed pointer usage not in SSA");
			}
			if (node.get_Right().get_CodeNodeType() != 26 && node.get_Right().get_CodeNodeType() != 25 && node.get_Right().get_CodeNodeType() != 23)
			{
				return false;
			}
			if (node.get_Right().get_CodeNodeType() == 23)
			{
				V_1 = node.get_Right() as UnaryExpression;
				if (V_1.get_Operator() != 7)
				{
					return false;
				}
				if (V_1.get_Operand().get_CodeNodeType() != 26 && V_1.get_Operand().get_CodeNodeType() != 25)
				{
					return false;
				}
			}
			this.variableToAssignExpression.Add(V_0, node);
			return true;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.VisitExpressions();
			this.TransformExpressions(new ManagedPointersRemovalStep.VariableReplacer(this.variableToAssignExpression));
			this.TransformExpressions(new ManagedPointersRemovalStep.ComplexDereferencer());
			this.RemoveVariablesFromContext();
			return body;
		}

		private void RemoveVariablesFromContext()
		{
			V_0 = this.variableToAssignExpression.get_Keys().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.context.get_MethodContext().RemoveVariable(V_1);
					dummyVar0 = this.context.get_MethodContext().get_VariablesToRename().Remove(V_1);
					dummyVar1 = this.context.get_MethodContext().get_VariableAssignmentData().Remove(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		public void TransformExpressions(BaseCodeTransformer transformer)
		{
			V_0 = this.context.get_MethodContext().get_Expressions().get_BlockExpressions().get_Values().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = 0;
					V_3 = 0;
					while (V_3 < V_1.get_Count())
					{
						V_4 = (Expression)transformer.Visit(V_1.get_Item(V_3));
						if (V_4 != null)
						{
							stackVariable27 = V_2;
							V_2 = stackVariable27 + 1;
							V_1.set_Item(stackVariable27, V_4);
						}
						V_3 = V_3 + 1;
					}
					V_5 = V_1.get_Count() - V_2;
					while (V_5 > 0)
					{
						V_1.RemoveAt(V_2 + V_5 - 1);
						V_5 = V_5 - 1;
					}
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			if (!node.get_IsAssignmentExpression() || !this.CheckForAssignment(node))
			{
				this.VisitBinaryExpression(node);
			}
			return;
		}

		public void VisitExpressions()
		{
			V_0 = this.context.get_MethodContext().get_Expressions().get_BlockExpressions().get_Values().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current().GetEnumerator();
					try
					{
						while (V_1.MoveNext())
						{
							V_2 = V_1.get_Current();
							this.Visit(V_2);
						}
					}
					finally
					{
						if (V_1 != null)
						{
							V_1.Dispose();
						}
					}
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		public override void VisitUnaryExpression(UnaryExpression node)
		{
			if (node.get_Operator() != 8 || node.get_Operand().get_CodeNodeType() != 26 || !this.variableToAssignExpression.ContainsKey((node.get_Operand() as VariableReferenceExpression).get_Variable().Resolve()))
			{
				this.VisitUnaryExpression(node);
			}
			return;
		}

		private class ComplexDereferencer : SimpleDereferencer
		{
			public ComplexDereferencer()
			{
				base();
				return;
			}

			public override ICodeNode VisitArrayIndexerExpression(ArrayIndexerExpression node)
			{
				node.set_Target((Expression)this.VisitTargetExpression(node.get_Target()));
				node.set_Indices((ExpressionCollection)this.Visit(node.get_Indices()));
				return node;
			}

			public override ICodeNode VisitArrayLengthExpression(ArrayLengthExpression node)
			{
				node.set_Target((Expression)this.VisitTargetExpression(node.get_Target()));
				return node;
			}

			public override ICodeNode VisitEventReferenceExpression(EventReferenceExpression node)
			{
				node.set_Target((Expression)this.VisitTargetExpression(node.get_Target()));
				return node;
			}

			public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
			{
				node.set_Target((Expression)this.VisitTargetExpression(node.get_Target()));
				return node;
			}

			public override ICodeNode VisitMethodReferenceExpression(MethodReferenceExpression node)
			{
				node.set_Target((Expression)this.VisitTargetExpression(node.get_Target()));
				return node;
			}

			public override ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node)
			{
				node.set_Target((Expression)this.VisitTargetExpression(node.get_Target()));
				node.set_Arguments((ExpressionCollection)this.Visit(node.get_Arguments()));
				return node;
			}

			private ICodeNode VisitTargetExpression(Expression target)
			{
				// 
				// Current member / type: Telerik.JustDecompiler.Ast.ICodeNode Telerik.JustDecompiler.Steps.ManagedPointersRemovalStep/ComplexDereferencer::VisitTargetExpression(Telerik.JustDecompiler.Ast.Expressions.Expression)
				// Exception in: Telerik.JustDecompiler.Ast.ICodeNode VisitTargetExpression(Telerik.JustDecompiler.Ast.Expressions.Expression)
				// Object reference not set to an instance of an object.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com

			}

			public override ICodeNode VisitUnaryExpression(UnaryExpression node)
			{
				if (node.get_Operator() == 8)
				{
					if (node.get_Operand().get_CodeNodeType() == 28)
					{
						return node.get_Operand();
					}
					V_0 = node.get_Operand() as ExplicitCastExpression;
					if (V_0 != null && V_0.get_TargetType().get_IsByReference())
					{
						V_1 = (V_0.get_TargetType() as ByReferenceType).get_ElementType();
						return new ExplicitCastExpression((Expression)this.Visit(V_0.get_Expression()), V_1, null);
					}
				}
				return this.VisitUnaryExpression(node);
			}
		}

		private class VariableReplacer : BaseCodeTransformer
		{
			private readonly Dictionary<VariableDefinition, BinaryExpression> variableToAssignExpression;

			private readonly HashSet<BinaryExpression> expressionsToSkip;

			public VariableReplacer(Dictionary<VariableDefinition, BinaryExpression> variableToAssignExpression)
			{
				this.expressionsToSkip = new HashSet<BinaryExpression>();
				base();
				this.variableToAssignExpression = variableToAssignExpression;
				this.expressionsToSkip = new HashSet<BinaryExpression>(variableToAssignExpression.get_Values());
				return;
			}

			private bool TryGetVariableValue(VariableDefinition variable, out Expression value)
			{
				if (!this.variableToAssignExpression.TryGetValue(variable, out V_0))
				{
					value = null;
					return false;
				}
				value = V_0.get_Right().CloneExpressionOnly();
				return true;
			}

			public override ICodeNode VisitBinaryExpression(BinaryExpression node)
			{
				if (!this.expressionsToSkip.Contains(node))
				{
					return this.VisitBinaryExpression(node);
				}
				dummyVar0 = this.VisitBinaryExpression(node);
				return null;
			}

			public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				if (!this.TryGetVariableValue(node.get_Variable().Resolve(), out V_0))
				{
					return node;
				}
				return V_0;
			}
		}
	}
}