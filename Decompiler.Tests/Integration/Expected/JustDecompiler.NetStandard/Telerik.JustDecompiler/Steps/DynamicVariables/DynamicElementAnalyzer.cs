using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Common;

namespace Telerik.JustDecompiler.Steps.DynamicVariables
{
	internal class DynamicElementAnalyzer
	{
		private const string InconsistentCountOfFlags = "Inconsistent count of positioning flags.";

		private readonly LinkedList<bool> dynamicPositioningFlags = new LinkedList<bool>();

		private DynamicElementAnalyzer()
		{
			this.dynamicPositioningFlags.AddFirst(true);
		}

		public static bool Analyze(Expression expression)
		{
			return (new DynamicElementAnalyzer()).AnalyzeExpression(expression);
		}

		private bool AnalyzeExpression(Expression expression)
		{
			GenericParameter genericParameterReturnType = null;
			Expression target = null;
			if (expression.CodeNodeType == CodeNodeType.ArgumentReferenceExpression)
			{
				return this.CheckParameter((expression as ArgumentReferenceExpression).Parameter.Resolve());
			}
			if (expression.CodeNodeType == CodeNodeType.VariableReferenceExpression || expression.CodeNodeType == CodeNodeType.VariableDeclarationExpression)
			{
				return this.FixDynamicFlags((expression.CodeNodeType == CodeNodeType.VariableReferenceExpression ? (expression as VariableReferenceExpression).Variable.Resolve() : (expression as VariableDeclarationExpression).Variable));
			}
			if (expression.CodeNodeType == CodeNodeType.MethodInvocationExpression || expression.CodeNodeType == CodeNodeType.PropertyReferenceExpression)
			{
				MethodInvocationExpression methodInvocationExpression = expression as MethodInvocationExpression;
				genericParameterReturnType = methodInvocationExpression.MethodExpression.Method.get_GenericParameterReturnType();
				target = methodInvocationExpression.MethodExpression.Target;
			}
			else
			{
				if (expression.CodeNodeType != CodeNodeType.FieldReferenceExpression)
				{
					if (expression.CodeNodeType == CodeNodeType.ExplicitCastExpression)
					{
						return this.FixDynamicFlags(expression as ExplicitCastExpression);
					}
					if (expression.CodeNodeType == CodeNodeType.ArrayIndexerExpression)
					{
						this.dynamicPositioningFlags.AddFirst(false);
						return this.AnalyzeExpression((expression as ArrayIndexerExpression).Target);
					}
					if (expression.CodeNodeType != CodeNodeType.UnaryExpression || (expression as UnaryExpression).Operator != UnaryOperator.AddressDereference && (expression as UnaryExpression).Operator != UnaryOperator.AddressOf && (expression as UnaryExpression).Operator != UnaryOperator.AddressReference)
					{
						return false;
					}
					this.dynamicPositioningFlags.AddFirst(false);
					return this.AnalyzeExpression((expression as UnaryExpression).Operand);
				}
				FieldReferenceExpression fieldReferenceExpression = expression as FieldReferenceExpression;
				genericParameterReturnType = fieldReferenceExpression.Field.get_FieldType() as GenericParameter;
				target = fieldReferenceExpression.Target;
			}
			if (target == null || genericParameterReturnType == null)
			{
				return false;
			}
			if (!(genericParameterReturnType.get_Owner() is TypeReference) || !target.ExpressionType.get_IsGenericInstance() || genericParameterReturnType.get_Name()[0] != '!' || genericParameterReturnType.get_Name()[1] == '!')
			{
				return false;
			}
			GenericInstanceType expressionType = target.ExpressionType as GenericInstanceType;
			int num = Int32.Parse(genericParameterReturnType.get_Name().Substring(1));
			this.dynamicPositioningFlags.AddFirst(false);
			for (int i = 0; i < expressionType.get_GenericArguments().get_Count(); i++)
			{
				if (i != num)
				{
					int num1 = 0;
					this.CountTypeTokens(expressionType.get_GenericArguments().get_Item(i), ref num1);
					for (int j = 0; j < num1; j++)
					{
						if (i >= num)
						{
							this.dynamicPositioningFlags.AddLast(false);
						}
						else
						{
							this.dynamicPositioningFlags.AddFirst(false);
						}
					}
				}
			}
			return this.AnalyzeExpression(target);
		}

		private bool CheckParameter(ParameterDefinition paramDef)
		{
			CustomAttribute customAttribute;
			if (!paramDef.TryGetDynamicAttribute(out customAttribute))
			{
				return false;
			}
			bool[] dynamicPositioningFlags = DynamicHelper.GetDynamicPositioningFlags(customAttribute);
			if ((int)dynamicPositioningFlags.Length != this.dynamicPositioningFlags.Count)
			{
				return false;
			}
			LinkedListNode<bool> first = this.dynamicPositioningFlags.First;
			bool[] flagArray = dynamicPositioningFlags;
			for (int i = 0; i < (int)flagArray.Length; i++)
			{
				if (flagArray[i] != first.Value)
				{
					return false;
				}
				first = first.Next;
			}
			return true;
		}

		private void CountTypeTokens(TypeReference typeRef, ref int count)
		{
			count++;
			if (typeRef is GenericInstanceType)
			{
				foreach (TypeReference genericArgument in (typeRef as GenericInstanceType).get_GenericArguments())
				{
					this.CountTypeTokens(genericArgument, ref count);
				}
			}
			else if (typeRef is TypeSpecification)
			{
				this.CountTypeTokens((typeRef as TypeSpecification).get_ElementType(), ref count);
			}
		}

		private bool FixDynamicFlags(IDynamicTypeContainer dynamicTypeContainer)
		{
			if (!dynamicTypeContainer.get_IsDynamic())
			{
				int num = 0;
				this.CountTypeTokens(dynamicTypeContainer.get_DynamicContainingType(), ref num);
				if (num != this.dynamicPositioningFlags.Count)
				{
					return false;
				}
				dynamicTypeContainer.set_DynamicPositioningFlags(new Boolean[num]);
			}
			else if ((int)dynamicTypeContainer.get_DynamicPositioningFlags().Length != this.dynamicPositioningFlags.Count)
			{
				throw new Exception("Inconsistent count of positioning flags.");
			}
			LinkedListNode<bool> first = this.dynamicPositioningFlags.First;
			for (int i = 0; i < (int)dynamicTypeContainer.get_DynamicPositioningFlags().Length; i++)
			{
				dynamicTypeContainer.get_DynamicPositioningFlags()[i] |= first.Value;
				first = first.Next;
			}
			return true;
		}
	}
}