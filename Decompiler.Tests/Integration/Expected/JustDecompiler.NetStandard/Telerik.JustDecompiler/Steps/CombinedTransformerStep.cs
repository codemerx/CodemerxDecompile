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

		private readonly TypeOfStep typeOfStep = new TypeOfStep();

		private readonly ReplaceDelegateInvokeStep replaceDelegateInvokeStep;

		private PropertyRecognizer propertyRecognizer;

		private RebuildEventsStep rebuildEventsStep;

		private HandleVirtualMethodInvocations replaceThisWithBaseStep;

		private OperatorStep operatorStep;

		private readonly CanCastStep canCastStep;

		private RemovePrivateImplementationDetailsStep removePIDStep;

		private FixSwitchConditionStep fixSwitchConditionStep;

		private RebuildAnonymousTypesInitializersStep rebuildAnonymousInitializersStep;

		private FixSwitchCasesStep fixSwitchCasesStep = new FixSwitchCasesStep();

		static CombinedTransformerStep()
		{
			CombinedTransformerStep.Instance = new CombinedTransformerStep();
		}

		public CombinedTransformerStep()
		{
			this.canCastStep = new CanCastStep(this);
			this.replaceDelegateInvokeStep = new ReplaceDelegateInvokeStep(this);
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			TypeSystem typeSystem = context.MethodContext.Method.Module.TypeSystem;
			this.operatorStep = new OperatorStep(this, typeSystem);
			this.removePIDStep = new RemovePrivateImplementationDetailsStep(typeSystem);
			this.rebuildEventsStep = new RebuildEventsStep(typeSystem);
			this.propertyRecognizer = new PropertyRecognizer(typeSystem, context.TypeContext, context.Language);
			this.rebuildAnonymousInitializersStep = new RebuildAnonymousTypesInitializersStep(this, typeSystem);
			this.fixSwitchConditionStep = new FixSwitchConditionStep(context);
			this.replaceThisWithBaseStep = new HandleVirtualMethodInvocations(this.context.MethodContext.Method);
			return (BlockStatement)this.VisitBlockStatement(body);
		}

		public override ICodeNode VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			ICodeNode codeNode = this.removePIDStep.VisitArrayCreationExpression(node);
			if (codeNode != null)
			{
				return codeNode;
			}
			return base.VisitArrayCreationExpression(node);
		}

		private ICodeNode VisitAssignExpression(BinaryExpression node)
		{
			BinaryExpression binaryExpression = this.operatorStep.VisitAssignExpression(node);
			if (binaryExpression != null)
			{
				return binaryExpression;
			}
			return base.VisitBinaryExpression(node);
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			if (node.IsAssignmentExpression)
			{
				return this.VisitAssignExpression(node);
			}
			ICodeNode codeNode = this.canCastStep.VisitBinaryExpression(node);
			if (codeNode != null)
			{
				return codeNode;
			}
			if (node.Operator != BinaryOperator.ValueEquality && node.Operator != BinaryOperator.ValueInequality)
			{
				return base.VisitBinaryExpression(node);
			}
			return this.VisitCeqExpression(node);
		}

		private ICodeNode VisitCeqExpression(BinaryExpression node)
		{
			if (node.Right is LiteralExpression && node.Left.CodeNodeType != CodeNodeType.LiteralExpression)
			{
				if (node.Left.HasType && node.Left.ExpressionType.FullName == "System.Boolean")
				{
					LiteralExpression right = node.Right as LiteralExpression;
					bool flag = false;
					if (right.Value.Equals(false) && node.Operator == BinaryOperator.ValueEquality || right.Value.Equals(true) && node.Operator == BinaryOperator.ValueInequality)
					{
						flag = true;
					}
					if (!flag)
					{
						return this.Visit(node.Left);
					}
					UnaryExpression unaryExpression = new UnaryExpression(UnaryOperator.LogicalNot, node.Left, null);
					return this.Visit(unaryExpression);
				}
			}
			else if (node.Left is LiteralExpression && node.Right.CodeNodeType != CodeNodeType.LiteralExpression && node.Right.HasType && node.Right.ExpressionType.FullName == "System.Boolean")
			{
				LiteralExpression left = node.Left as LiteralExpression;
				bool flag1 = false;
				if (left.Value.Equals(false) && node.Operator == BinaryOperator.ValueEquality || left.Value.Equals(true) && node.Operator == BinaryOperator.ValueInequality)
				{
					flag1 = true;
				}
				if (!flag1)
				{
					return this.Visit(node.Left);
				}
				UnaryExpression unaryExpression1 = new UnaryExpression(UnaryOperator.LogicalNot, node.Right, null);
				return this.Visit(unaryExpression1);
			}
			node.Left = (Expression)this.Visit(node.Left);
			node.Right = (Expression)this.Visit(node.Right);
			return node;
		}

		public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			if (!this.context.MethodContext.Method.IsConstructor || !(node.Field.DeclaringType.FullName == this.context.MethodContext.Method.DeclaringType.FullName))
			{
				return base.VisitFieldReferenceExpression(node);
			}
			return this.propertyRecognizer.VisitFieldReferenceExpression(node);
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			ICodeNode codeNode = this.typeOfStep.VisitMethodInvocationExpression(node);
			if (codeNode != null)
			{
				return codeNode;
			}
			this.replaceThisWithBaseStep.VisitMethodInvocationExpression(node);
			codeNode = this.operatorStep.VisitMethodInvocationExpression(node);
			if (codeNode != null)
			{
				return base.Visit(codeNode);
			}
			codeNode = this.replaceDelegateInvokeStep.VisitMethodInvocationExpression(node);
			if (codeNode != null)
			{
				return base.VisitDelegateInvokeExpression(codeNode as DelegateInvokeExpression);
			}
			codeNode = this.propertyRecognizer.VisitMethodInvocationExpression(node);
			if (codeNode != null)
			{
				PropertyReferenceExpression propertyReferenceExpression = codeNode as PropertyReferenceExpression;
				if (propertyReferenceExpression != null)
				{
					codeNode = this.VisitPropertyReferenceExpression(propertyReferenceExpression);
				}
				if (codeNode is BinaryExpression)
				{
					codeNode = this.VisitBinaryExpression(codeNode as BinaryExpression);
				}
				return codeNode;
			}
			codeNode = this.rebuildEventsStep.VisitMethodInvocationExpression(node);
			if (codeNode == null)
			{
				return base.VisitMethodInvocationExpression(node);
			}
			if (!(codeNode is BinaryExpression))
			{
				return codeNode;
			}
			return this.VisitBinaryExpression(codeNode as BinaryExpression);
		}

		public override ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			ICodeNode codeNode = this.rebuildAnonymousInitializersStep.VisitObjectCreationExpression(node);
			if (codeNode != null)
			{
				return codeNode;
			}
			return base.VisitObjectCreationExpression(node);
		}

		public override ICodeNode VisitSwitchStatement(SwitchStatement node)
		{
			this.fixSwitchCasesStep.FixCases(node);
			return base.VisitSwitchStatement(this.fixSwitchConditionStep.VisitSwitchStatement(node));
		}
	}
}