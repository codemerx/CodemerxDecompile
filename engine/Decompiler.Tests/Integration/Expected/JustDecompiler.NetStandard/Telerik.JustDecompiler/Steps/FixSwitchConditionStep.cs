using Mono.Cecil;
using System;
using System.Collections.Generic;
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
			base();
			this.context = context;
			return;
		}

		private void ChangeCasesLabel()
		{
			V_0 = this.theSwitch.get_Cases().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1.get_CodeNodeType() != 13)
					{
						continue;
					}
					V_2 = V_1 as ConditionCase;
					V_3 = V_2.get_Condition() as LiteralExpression;
					if (V_3 == null || V_3.get_Value() as String != null)
					{
						continue;
					}
					V_2.set_Condition(this.FixCaseLiteralValue(V_3));
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

		private bool CheckAndModifyCondition()
		{
			this.conditionOffset = 0;
			if (this.theSwitch.get_Condition().get_CodeNodeType() != 24)
			{
				return false;
			}
			V_0 = this.theSwitch.get_Condition() as BinaryExpression;
			if (V_0.get_Operator() != 1 && V_0.get_Operator() != 3 || V_0.get_Right().get_CodeNodeType() != 22)
			{
				return false;
			}
			V_1 = V_0.get_Right() as LiteralExpression;
			this.conditionOffset = Convert.ToInt32(V_1.get_Value());
			if (V_0.get_Operator() == 3)
			{
				stackVariable30 = this.conditionOffset;
			}
			else
			{
				stackVariable30 = -this.conditionOffset;
			}
			this.conditionOffset = stackVariable30;
			V_2 = new List<Instruction>(V_0.get_Right().get_UnderlyingSameMethodInstructions());
			V_2.AddRange(V_0.get_MappedInstructions());
			this.theSwitch.set_Condition(V_0.get_Left().CloneAndAttachInstructions(V_2));
			return true;
		}

		private Expression FixCaseLiteralValue(LiteralExpression literalCondition)
		{
			V_0 = this.theSwitch.get_Condition().get_ExpressionType();
			V_1 = Convert.ToInt32(literalCondition.get_Value()) + this.conditionOffset;
			literalCondition.set_Value(V_1);
			V_2 = this.context.get_MethodContext().get_Method().get_Module().get_TypeSystem();
			if (String.op_Equality(V_0.get_Name(), "System.Nullable`1") && V_0.get_HasGenericParameters())
			{
				V_0 = V_0.get_GenericParameters().get_Item(0);
			}
			if (String.op_Equality(V_0.get_FullName(), V_2.get_Char().get_FullName()))
			{
				return new LiteralExpression((object)Convert.ToChar(V_1), V_2, null);
			}
			if (String.op_Equality(V_0.get_FullName(), V_2.get_Boolean().get_FullName()))
			{
				return new LiteralExpression((object)Convert.ToBoolean(V_1), V_2, null);
			}
			V_3 = V_0.Resolve();
			if (V_3 == null || !V_3.get_IsEnum())
			{
				return literalCondition;
			}
			V_4 = EnumHelper.GetEnumExpression(V_3, literalCondition, V_2);
			if (V_4 as LiteralExpression != null)
			{
				V_4 = new ExplicitCastExpression(V_4, V_3, null);
			}
			return V_4;
		}

		public SwitchStatement VisitSwitchStatement(SwitchStatement node)
		{
			this.theSwitch = node;
			dummyVar0 = this.CheckAndModifyCondition();
			this.ChangeCasesLabel();
			return node;
		}
	}
}