using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Steps
{
	public class CombinedTransformerStep : BaseCodeTransformer, IDecompilationStep
	{
		public static readonly IDecompilationStep Instance = new CombinedTransformerStep();

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
            return (BlockStatement)VisitBlockStatement(body);
        }

        public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
        {
			ICodeNode newNode = typeOfStep.VisitMethodInvocationExpression(node);
			if (newNode != null)
			{
				return newNode;
			}
			replaceThisWithBaseStep.VisitMethodInvocationExpression(node);

			newNode = operatorStep.VisitMethodInvocationExpression(node);

			if (newNode != null)
			{
				return base.Visit(newNode);
			}

			newNode = replaceDelegateInvokeStep.VisitMethodInvocationExpression(node);

			if (newNode != null)
			{
				return base.VisitDelegateInvokeExpression(newNode as DelegateInvokeExpression);
			}
			newNode = propertyRecognizer.VisitMethodInvocationExpression(node);

            if (newNode != null)
            {
                PropertyReferenceExpression propertyReference = newNode as PropertyReferenceExpression;

                if (propertyReference != null) // if it was a getter
                {
                    newNode = this.VisitPropertyReferenceExpression(propertyReference);
                }
				if (newNode is BinaryExpression) // if it was a setter
				{
					newNode = this.VisitBinaryExpression(newNode as BinaryExpression);
				}
                return newNode;
            }

			newNode = rebuildEventsStep.VisitMethodInvocationExpression(node);

			if (newNode != null)
			{
				if (newNode is BinaryExpression)
				{
					return VisitBinaryExpression(newNode as BinaryExpression);
				}
				return newNode;
			}
			return base.VisitMethodInvocationExpression(node);
		}
     
        public override ICodeNode VisitBinaryExpression(BinaryExpression node)
        {
            if (node.IsAssignmentExpression)
            {
                return VisitAssignExpression(node);
            }

            ICodeNode newNode = canCastStep.VisitBinaryExpression(node);
            if (newNode != null)
                return newNode;

            if (node.Operator == BinaryOperator.ValueEquality || node.Operator == BinaryOperator.ValueInequality)
            {
                return VisitCeqExpression(node);
            }

			return base.VisitBinaryExpression(node);
        }
  
        private ICodeNode VisitCeqExpression(BinaryExpression node)
        {
            //removes <boolean_expression> == true/false
            //and replaces them with (!)<boolean_expression> as appropriate
            if (node.Right is LiteralExpression && node.Left.CodeNodeType != CodeNodeType.LiteralExpression)
            {
				if (node.Left.HasType)
				{
					TypeReference leftType = node.Left.ExpressionType;
					if (leftType.FullName == "System.Boolean")
					{
						LiteralExpression rightSide = node.Right as LiteralExpression;
						bool shouldNegate = false;
						if (rightSide.Value.Equals(false) && node.Operator == BinaryOperator.ValueEquality ||
							rightSide.Value.Equals(true) && node.Operator == BinaryOperator.ValueInequality)
						{
							shouldNegate = true;
						}
						if (shouldNegate)
						{
							UnaryExpression result = new UnaryExpression(UnaryOperator.LogicalNot, node.Left, null);
							return Visit(result);
						}
						return Visit(node.Left);
					}
				}
            }
            else if (node.Left is LiteralExpression && node.Right.CodeNodeType != CodeNodeType.LiteralExpression)
            {
				if (node.Right.HasType)
				{
					TypeReference rightType = node.Right.ExpressionType;
					if (rightType.FullName == "System.Boolean")
					{
						{
							LiteralExpression leftSide = node.Left as LiteralExpression;
							bool shouldNegate = false;
							if (leftSide.Value.Equals(false) && node.Operator == BinaryOperator.ValueEquality ||
								leftSide.Value.Equals(true) && node.Operator == BinaryOperator.ValueInequality)
							{
								shouldNegate = true;
							}
							if (shouldNegate)
							{
								UnaryExpression result = new UnaryExpression(UnaryOperator.LogicalNot, node.Right, null);
								return Visit(result);
							}
							return Visit(node.Left);
						}
					}
				}
            }
            node.Left = (Expression)Visit(node.Left);
            node.Right = (Expression)Visit(node.Right);
            return node;
        }

        private ICodeNode VisitAssignExpression(BinaryExpression node)
        {
            BinaryExpression result = operatorStep.VisitAssignExpression(node);
            if(result != null)
            {
                return result;
            }

            return base.VisitBinaryExpression(node);
        }

        public override ICodeNode VisitArrayCreationExpression(ArrayCreationExpression node)
        {
            ICodeNode newNode = removePIDStep.VisitArrayCreationExpression(node);
            if (newNode != null)
            {
                return newNode;
            }
            return base.VisitArrayCreationExpression(node);
        }

        public override ICodeNode VisitSwitchStatement(SwitchStatement node)
        {
            fixSwitchCasesStep.FixCases(node);
            return base.VisitSwitchStatement(fixSwitchConditionStep.VisitSwitchStatement(node));
        }

        public override ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
        {
            ICodeNode result = rebuildAnonymousInitializersStep.VisitObjectCreationExpression(node);
            if (result != null)
            {
                return result;
            }
            return base.VisitObjectCreationExpression(node);
        }

        public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
        {
            if (this.context.MethodContext.Method.IsConstructor && node.Field.DeclaringType.FullName == this.context.MethodContext.Method.DeclaringType.FullName)
            {
                return propertyRecognizer.VisitFieldReferenceExpression(node);
            }

            return base.VisitFieldReferenceExpression(node);
        }
	}
}