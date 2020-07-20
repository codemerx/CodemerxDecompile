using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps.DynamicVariables
{
	internal class DynamicElementAnalyzer
	{
		private const string InconsistentCountOfFlags = "Inconsistent count of positioning flags.";

		private readonly LinkedList<bool> dynamicPositioningFlags;

		private DynamicElementAnalyzer()
		{
			this.dynamicPositioningFlags = new LinkedList<bool>();
			base();
			dummyVar0 = this.dynamicPositioningFlags.AddFirst(true);
			return;
		}

		public static bool Analyze(Expression expression)
		{
			return (new DynamicElementAnalyzer()).AnalyzeExpression(expression);
		}

		private bool AnalyzeExpression(Expression expression)
		{
			V_0 = null;
			V_1 = null;
			if (expression.get_CodeNodeType() == 25)
			{
				return this.CheckParameter((expression as ArgumentReferenceExpression).get_Parameter().Resolve());
			}
			if (expression.get_CodeNodeType() == 26 || expression.get_CodeNodeType() == 27)
			{
				if (expression.get_CodeNodeType() == 26)
				{
					stackVariable14 = (expression as VariableReferenceExpression).get_Variable().Resolve();
				}
				else
				{
					stackVariable14 = (expression as VariableDeclarationExpression).get_Variable();
				}
				return this.FixDynamicFlags(stackVariable14);
			}
			if (expression.get_CodeNodeType() == 19 || expression.get_CodeNodeType() == 42)
			{
				stackVariable27 = expression as MethodInvocationExpression;
				V_0 = stackVariable27.get_MethodExpression().get_Method().get_GenericParameterReturnType();
				V_1 = stackVariable27.get_MethodExpression().get_Target();
			}
			else
			{
				if (expression.get_CodeNodeType() != 30)
				{
					if (expression.get_CodeNodeType() == 31)
					{
						return this.FixDynamicFlags(expression as ExplicitCastExpression);
					}
					if (expression.get_CodeNodeType() == 39)
					{
						dummyVar0 = this.dynamicPositioningFlags.AddFirst(false);
						return this.AnalyzeExpression((expression as ArrayIndexerExpression).get_Target());
					}
					if (expression.get_CodeNodeType() != 23 || (expression as UnaryExpression).get_Operator() != 8 && (expression as UnaryExpression).get_Operator() != 9 && (expression as UnaryExpression).get_Operator() != 7)
					{
						return false;
					}
					dummyVar1 = this.dynamicPositioningFlags.AddFirst(false);
					return this.AnalyzeExpression((expression as UnaryExpression).get_Operand());
				}
				stackVariable152 = expression as FieldReferenceExpression;
				V_0 = stackVariable152.get_Field().get_FieldType() as GenericParameter;
				V_1 = stackVariable152.get_Target();
			}
			if (V_1 == null || V_0 == null)
			{
				return false;
			}
			if (V_0.get_Owner() as TypeReference == null || !V_1.get_ExpressionType().get_IsGenericInstance() || V_0.get_Name().get_Chars(0) != '!' || V_0.get_Name().get_Chars(1) == '!')
			{
				return false;
			}
			V_3 = V_1.get_ExpressionType() as GenericInstanceType;
			V_4 = Int32.Parse(V_0.get_Name().Substring(1));
			dummyVar2 = this.dynamicPositioningFlags.AddFirst(false);
			V_5 = 0;
			while (V_5 < V_3.get_GenericArguments().get_Count())
			{
				if (V_5 != V_4)
				{
					V_6 = 0;
					this.CountTypeTokens(V_3.get_GenericArguments().get_Item(V_5), ref V_6);
					V_7 = 0;
					while (V_7 < V_6)
					{
						if (V_5 >= V_4)
						{
							dummyVar4 = this.dynamicPositioningFlags.AddLast(false);
						}
						else
						{
							dummyVar3 = this.dynamicPositioningFlags.AddFirst(false);
						}
						V_7 = V_7 + 1;
					}
				}
				V_5 = V_5 + 1;
			}
			return this.AnalyzeExpression(V_1);
		}

		private bool CheckParameter(ParameterDefinition paramDef)
		{
			if (!paramDef.TryGetDynamicAttribute(out V_0))
			{
				return false;
			}
			V_1 = DynamicHelper.GetDynamicPositioningFlags(V_0);
			if ((int)V_1.Length != this.dynamicPositioningFlags.get_Count())
			{
				return false;
			}
			V_2 = this.dynamicPositioningFlags.get_First();
			V_3 = V_1;
			V_4 = 0;
			while (V_4 < (int)V_3.Length)
			{
				if (V_3[V_4] != V_2.get_Value())
				{
					return false;
				}
				V_2 = V_2.get_Next();
				V_4 = V_4 + 1;
			}
			return true;
		}

		private void CountTypeTokens(TypeReference typeRef, ref int count)
		{
			count = count + 1;
			if (typeRef as GenericInstanceType == null)
			{
				if (typeRef as TypeSpecification != null)
				{
					this.CountTypeTokens((typeRef as TypeSpecification).get_ElementType(), ref count);
				}
			}
			else
			{
				V_0 = (typeRef as GenericInstanceType).get_GenericArguments().GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						this.CountTypeTokens(V_1, ref count);
					}
				}
				finally
				{
					V_0.Dispose();
				}
			}
			return;
		}

		private bool FixDynamicFlags(IDynamicTypeContainer dynamicTypeContainer)
		{
			if (!dynamicTypeContainer.get_IsDynamic())
			{
				V_1 = 0;
				this.CountTypeTokens(dynamicTypeContainer.get_DynamicContainingType(), ref V_1);
				if (V_1 != this.dynamicPositioningFlags.get_Count())
				{
					return false;
				}
				dynamicTypeContainer.set_DynamicPositioningFlags(new Boolean[V_1]);
			}
			else
			{
				if ((int)dynamicTypeContainer.get_DynamicPositioningFlags().Length != this.dynamicPositioningFlags.get_Count())
				{
					throw new Exception("Inconsistent count of positioning flags.");
				}
			}
			V_0 = this.dynamicPositioningFlags.get_First();
			V_2 = 0;
			while (V_2 < (int)dynamicTypeContainer.get_DynamicPositioningFlags().Length)
			{
				stackVariable26 = &dynamicTypeContainer.get_DynamicPositioningFlags()[V_2];
				stackVariable26 = stackVariable26 | V_0.get_Value();
				V_0 = V_0.get_Next();
				V_2 = V_2 + 1;
			}
			return true;
		}
	}
}