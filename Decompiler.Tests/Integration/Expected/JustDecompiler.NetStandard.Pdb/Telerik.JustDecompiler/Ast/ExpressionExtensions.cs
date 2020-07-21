using Mono.Cecil;
using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast
{
	public static class ExpressionExtensions
	{
		public static bool CheckInnerReferenceExpressions(this Expression first, Expression second)
		{
			if (first as VariableReferenceExpression != null && second as VariableReferenceExpression != null)
			{
				return (object)((VariableReferenceExpression)first).get_Variable() == (object)((VariableReferenceExpression)second).get_Variable();
			}
			if (first as ArgumentReferenceExpression != null && second as ArgumentReferenceExpression != null)
			{
				return (object)((ArgumentReferenceExpression)first).get_Parameter() == (object)((ArgumentReferenceExpression)second).get_Parameter();
			}
			if (first as FieldReferenceExpression != null && second as FieldReferenceExpression != null)
			{
				return (object)((FieldReferenceExpression)first).get_Field() == (object)((FieldReferenceExpression)second).get_Field();
			}
			if (first as PropertyReferenceExpression == null || second as PropertyReferenceExpression == null)
			{
				return false;
			}
			return (object)((PropertyReferenceExpression)first).get_Property() == (object)((PropertyReferenceExpression)second).get_Property();
		}

		public static TypeReference GetTargetTypeReference(this Expression target)
		{
			if (target == null || !target.get_HasType())
			{
				return null;
			}
			return target.get_ExpressionType();
		}

		public static bool IsReferenceExpression(this Expression expression)
		{
			if (expression as VariableReferenceExpression != null || expression as ArgumentReferenceExpression != null || expression as FieldReferenceExpression != null)
			{
				return true;
			}
			return expression as PropertyReferenceExpression != null;
		}

		public static bool IsTypeOfExpression(this MethodInvocationExpression self, out TypeReference typeReference)
		{
			typeReference = null;
			V_0 = self.get_MethodExpression();
			if (V_0 == null)
			{
				return false;
			}
			V_1 = V_0.get_Method();
			if (V_1.get_CallingConvention() == 2 || String.op_Inequality(V_1.get_DeclaringType().get_FullName(), "System.Type") || String.op_Inequality(V_1.get_Name(), "GetTypeFromHandle"))
			{
				return false;
			}
			if (self.get_Arguments().get_Count() != 1)
			{
				return false;
			}
			V_2 = self.get_Arguments().get_Item(0) as MemberHandleExpression;
			if (V_2 == null)
			{
				return false;
			}
			typeReference = V_2.get_MemberReference() as TypeReference;
			return (object)typeReference != (object)null;
		}

		public static string ToCodeString(this Expression expression)
		{
			stackVariable0 = new StringWriter();
			stackVariable2 = new IntermediateDecompilationCSharpLanguageWriter(new PlainTextFormatter(stackVariable0));
			((ILanguageTestCaseWriter)stackVariable2).SetContext(expression.get_UnderlyingSameMethodInstructions().First<Instruction>().get_ContainingMethod());
			((ILanguageTestCaseWriter)stackVariable2).Write(expression);
			return stackVariable0.ToString();
		}
	}
}