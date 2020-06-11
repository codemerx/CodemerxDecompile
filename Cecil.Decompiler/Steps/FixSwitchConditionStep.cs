using System;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Steps
{
	class FixSwitchConditionStep
	{
        private int conditionOffset;
        private SwitchStatement theSwitch;
		private DecompilationContext context;

		public FixSwitchConditionStep(DecompilationContext context)
		{
			this.context = context;
		}

        public SwitchStatement VisitSwitchStatement(SwitchStatement node)
        {
            theSwitch = node;
			CheckAndModifyCondition();
            ChangeCasesLabel();
            return node;
        }

        private bool CheckAndModifyCondition()
        {
            this.conditionOffset = 0;
            if(theSwitch.Condition.CodeNodeType != CodeNodeType.BinaryExpression)
            {
                return false;
            }

            BinaryExpression theBinaryExpression = theSwitch.Condition as BinaryExpression;

            if(theBinaryExpression.Operator != BinaryOperator.Add && theBinaryExpression.Operator != BinaryOperator.Subtract ||
                theBinaryExpression.Right.CodeNodeType != CodeNodeType.LiteralExpression)
            {
                return false;
            }

            LiteralExpression theOffsetExpression = theBinaryExpression.Right as LiteralExpression;
            this.conditionOffset = Convert.ToInt32(theOffsetExpression.Value);

            this.conditionOffset = theBinaryExpression.Operator == BinaryOperator.Subtract ? this.conditionOffset : -this.conditionOffset;
            List<Instruction> instructions = new List<Instruction>(theBinaryExpression.Right.UnderlyingSameMethodInstructions);
            instructions.AddRange(theBinaryExpression.MappedInstructions);
            theSwitch.Condition = theBinaryExpression.Left.CloneAndAttachInstructions(instructions);

            return true;
        }

		private void ChangeCasesLabel()
		{
			foreach (SwitchCase @case in theSwitch.Cases)
			{
                if(@case.CodeNodeType != CodeNodeType.ConditionCase)
                {
                    continue;
                }

                ConditionCase conditionCase = @case as ConditionCase;
                LiteralExpression literalCondition = conditionCase.Condition as LiteralExpression;
				if (literalCondition == null || literalCondition.Value is string)
				{
					continue;
				}
				conditionCase.Condition = FixCaseLiteralValue(literalCondition);
			}
		}
  
		private Expression FixCaseLiteralValue(LiteralExpression literalCondition)
		{
			TypeReference conditionType = theSwitch.Condition.ExpressionType;
			int integerRepresentation = Convert.ToInt32(literalCondition.Value) + this.conditionOffset;
			literalCondition.Value = integerRepresentation;
			var theTypeSystem = context.MethodContext.Method.Module.TypeSystem;

			if (conditionType.Name == "System.Nullable`1" && conditionType.HasGenericParameters)
			{
				conditionType = conditionType.GenericParameters[0];
			}
			if (conditionType.FullName == theTypeSystem.Char.FullName)
			{
				//switch cases should have char labels;
				return new LiteralExpression(Convert.ToChar(integerRepresentation),theTypeSystem, null);
			}
			else if (conditionType.FullName == theTypeSystem.Boolean.FullName)
			{
				return new LiteralExpression(Convert.ToBoolean(integerRepresentation),theTypeSystem,null);
			}
			else
			{
				TypeDefinition resolvedConditionType = conditionType.Resolve();
				if (resolvedConditionType != null && resolvedConditionType.IsEnum)
				{
					Expression result = EnumHelper.GetEnumExpression(resolvedConditionType,literalCondition,theTypeSystem);
					if (result is LiteralExpression)
					{
						result = new ExplicitCastExpression(result, resolvedConditionType, null);
					}
					return result;
				}
			}
			return literalCondition;
		}
	}
}
