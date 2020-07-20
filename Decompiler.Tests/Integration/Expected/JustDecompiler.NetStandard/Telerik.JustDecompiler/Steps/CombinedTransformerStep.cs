using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class CombinedTransformerStep : BaseCodeTransformer, IDecompilationStep
	{
		public readonly static IDecompilationStep Instance;

		private DecompilationContext context;

		private readonly TypeOfStep typeOfStep;

		private readonly ReplaceDelegateInvokeStep replaceDelegateInvokeStep;

		private PropertyRecognizer propertyRecognizer;

		private RebuildEventsStep rebuildEventsStep;

		private HandleVirtualMethodInvocations replaceThisWithBaseStep;

		private OperatorStep operatorStep;

		private readonly CanCastStep canCastStep;

		private RemovePrivateImplementationDetailsStep removePIDStep;

		private FixSwitchConditionStep fixSwitchConditionStep;

		private RebuildAnonymousTypesInitializersStep rebuildAnonymousInitializersStep;

		private FixSwitchCasesStep fixSwitchCasesStep;

		static CombinedTransformerStep()
		{
			CombinedTransformerStep.Instance = new CombinedTransformerStep();
			return;
		}

		public CombinedTransformerStep()
		{
			this.typeOfStep = new TypeOfStep();
			this.fixSwitchCasesStep = new FixSwitchCasesStep();
			base();
			this.canCastStep = new CanCastStep(this);
			this.replaceDelegateInvokeStep = new ReplaceDelegateInvokeStep(this);
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			V_0 = context.get_MethodContext().get_Method().get_Module().get_TypeSystem();
			this.operatorStep = new OperatorStep(this, V_0);
			this.removePIDStep = new RemovePrivateImplementationDetailsStep(V_0);
			this.rebuildEventsStep = new RebuildEventsStep(V_0);
			this.propertyRecognizer = new PropertyRecognizer(V_0, context.get_TypeContext(), context.get_Language());
			this.rebuildAnonymousInitializersStep = new RebuildAnonymousTypesInitializersStep(this, V_0);
			this.fixSwitchConditionStep = new FixSwitchConditionStep(context);
			this.replaceThisWithBaseStep = new HandleVirtualMethodInvocations(this.context.get_MethodContext().get_Method());
			return (BlockStatement)this.VisitBlockStatement(body);
		}

		public override ICodeNode VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			V_0 = this.removePIDStep.VisitArrayCreationExpression(node);
			if (V_0 != null)
			{
				return V_0;
			}
			return this.VisitArrayCreationExpression(node);
		}

		private ICodeNode VisitAssignExpression(BinaryExpression node)
		{
			V_0 = this.operatorStep.VisitAssignExpression(node);
			if (V_0 != null)
			{
				return V_0;
			}
			return this.VisitBinaryExpression(node);
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			if (node.get_IsAssignmentExpression())
			{
				return this.VisitAssignExpression(node);
			}
			V_0 = this.canCastStep.VisitBinaryExpression(node);
			if (V_0 != null)
			{
				return V_0;
			}
			if (node.get_Operator() != 9 && node.get_Operator() != 10)
			{
				return this.VisitBinaryExpression(node);
			}
			return this.VisitCeqExpression(node);
		}

		private ICodeNode VisitCeqExpression(BinaryExpression node)
		{
			if (node.get_Right() as LiteralExpression == null || node.get_Left().get_CodeNodeType() == 22)
			{
				if (node.get_Left() as LiteralExpression != null && node.get_Right().get_CodeNodeType() != 22 && node.get_Right().get_HasType() && String.op_Equality(node.get_Right().get_ExpressionType().get_FullName(), "System.Boolean"))
				{
					V_3 = node.get_Left() as LiteralExpression;
					V_4 = false;
					if (V_3.get_Value().Equals(false) && node.get_Operator() == 9 || V_3.get_Value().Equals(true) && node.get_Operator() == 10)
					{
						V_4 = true;
					}
					if (!V_4)
					{
						return this.Visit(node.get_Left());
					}
					V_5 = new UnaryExpression(1, node.get_Right(), null);
					return this.Visit(V_5);
				}
			}
			else
			{
				if (node.get_Left().get_HasType() && String.op_Equality(node.get_Left().get_ExpressionType().get_FullName(), "System.Boolean"))
				{
					V_0 = node.get_Right() as LiteralExpression;
					V_1 = false;
					if (V_0.get_Value().Equals(false) && node.get_Operator() == 9 || V_0.get_Value().Equals(true) && node.get_Operator() == 10)
					{
						V_1 = true;
					}
					if (!V_1)
					{
						return this.Visit(node.get_Left());
					}
					V_2 = new UnaryExpression(1, node.get_Left(), null);
					return this.Visit(V_2);
				}
			}
			node.set_Left((Expression)this.Visit(node.get_Left()));
			node.set_Right((Expression)this.Visit(node.get_Right()));
			return node;
		}

		public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			if (!this.context.get_MethodContext().get_Method().get_IsConstructor() || !String.op_Equality(node.get_Field().get_DeclaringType().get_FullName(), this.context.get_MethodContext().get_Method().get_DeclaringType().get_FullName()))
			{
				return this.VisitFieldReferenceExpression(node);
			}
			return this.propertyRecognizer.VisitFieldReferenceExpression(node);
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			V_0 = this.typeOfStep.VisitMethodInvocationExpression(node);
			if (V_0 != null)
			{
				return V_0;
			}
			this.replaceThisWithBaseStep.VisitMethodInvocationExpression(node);
			V_0 = this.operatorStep.VisitMethodInvocationExpression(node);
			if (V_0 != null)
			{
				return this.Visit(V_0);
			}
			V_0 = this.replaceDelegateInvokeStep.VisitMethodInvocationExpression(node);
			if (V_0 != null)
			{
				return this.VisitDelegateInvokeExpression(V_0 as DelegateInvokeExpression);
			}
			V_0 = this.propertyRecognizer.VisitMethodInvocationExpression(node);
			if (V_0 != null)
			{
				V_1 = V_0 as PropertyReferenceExpression;
				if (V_1 != null)
				{
					V_0 = this.VisitPropertyReferenceExpression(V_1);
				}
				if (V_0 as BinaryExpression != null)
				{
					V_0 = this.VisitBinaryExpression(V_0 as BinaryExpression);
				}
				return V_0;
			}
			V_0 = this.rebuildEventsStep.VisitMethodInvocationExpression(node);
			if (V_0 == null)
			{
				return this.VisitMethodInvocationExpression(node);
			}
			if (V_0 as BinaryExpression == null)
			{
				return V_0;
			}
			return this.VisitBinaryExpression(V_0 as BinaryExpression);
		}

		public override ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			V_0 = this.rebuildAnonymousInitializersStep.VisitObjectCreationExpression(node);
			if (V_0 != null)
			{
				return V_0;
			}
			return this.VisitObjectCreationExpression(node);
		}

		public override ICodeNode VisitSwitchStatement(SwitchStatement node)
		{
			this.fixSwitchCasesStep.FixCases(node);
			return this.VisitSwitchStatement(this.fixSwitchConditionStep.VisitSwitchStatement(node));
		}
	}
}