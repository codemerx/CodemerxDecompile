using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class FixSwitchConditionStep
	{
		private int conditionOffset;

		private SwitchStatement theSwitch;

		private DecompilationContext context;

		public FixSwitchConditionStep(DecompilationContext context)
		{
			this.context = context;
		}

		private void ChangeCasesLabel()
		{
			foreach (SwitchCase @case in this.theSwitch.Cases)
			{
				if (@case.CodeNodeType != CodeNodeType.ConditionCase)
				{
					continue;
				}
				ConditionCase conditionCase = @case as ConditionCase;
				LiteralExpression condition = conditionCase.Condition as LiteralExpression;
				if (condition == null || condition.Value is String)
				{
					continue;
				}
				conditionCase.Condition = this.FixCaseLiteralValue(condition);
			}
		}

		private bool CheckAndModifyCondition()
		{
			this.conditionOffset = 0;
			if (this.theSwitch.Condition.CodeNodeType != CodeNodeType.BinaryExpression)
			{
				return false;
			}
			BinaryExpression condition = this.theSwitch.Condition as BinaryExpression;
			if (condition.Operator != BinaryOperator.Add && condition.Operator != BinaryOperator.Subtract || condition.Right.CodeNodeType != CodeNodeType.LiteralExpression)
			{
				return false;
			}
			LiteralExpression right = condition.Right as LiteralExpression;
			this.conditionOffset = Convert.ToInt32(right.Value);
			this.conditionOffset = (condition.Operator == BinaryOperator.Subtract ? this.conditionOffset : -this.conditionOffset);
			List<Instruction> instructions = new List<Instruction>(condition.Right.UnderlyingSameMethodInstructions);
			instructions.AddRange(condition.MappedInstructions);
			this.theSwitch.Condition = condition.Left.CloneAndAttachInstructions(instructions);
			return true;
		}

		private Expression FixCaseLiteralValue(LiteralExpression literalCondition)
		{
			TypeReference expressionType = this.theSwitch.Condition.ExpressionType;
			int num = Convert.ToInt32(literalCondition.Value) + this.conditionOffset;
			literalCondition.Value = num;
			TypeSystem typeSystem = this.context.MethodContext.Method.get_Module().get_TypeSystem();
			if (expressionType.get_Name() == "System.Nullable`1" && expressionType.get_HasGenericParameters())
			{
				expressionType = expressionType.get_GenericParameters().get_Item(0);
			}
			if (expressionType.get_FullName() == typeSystem.get_Char().get_FullName())
			{
				return new LiteralExpression((object)Convert.ToChar(num), typeSystem, null);
			}
			if (expressionType.get_FullName() == typeSystem.get_Boolean().get_FullName())
			{
				return new LiteralExpression((object)Convert.ToBoolean(num), typeSystem, null);
			}
			TypeDefinition typeDefinition = expressionType.Resolve();
			if (typeDefinition == null || !typeDefinition.get_IsEnum())
			{
				return literalCondition;
			}
			Expression enumExpression = EnumHelper.GetEnumExpression(typeDefinition, literalCondition, typeSystem);
			if (enumExpression is LiteralExpression)
			{
				enumExpression = new ExplicitCastExpression(enumExpression, typeDefinition, null);
			}
			return enumExpression;
		}

		public SwitchStatement VisitSwitchStatement(SwitchStatement node)
		{
			this.theSwitch = node;
			this.CheckAndModifyCondition();
			this.ChangeCasesLabel();
			return node;
		}
	}
}