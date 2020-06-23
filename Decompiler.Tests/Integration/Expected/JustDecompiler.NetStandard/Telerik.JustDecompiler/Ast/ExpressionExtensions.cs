using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.TestCaseWriters;

namespace Telerik.JustDecompiler.Ast
{
	public static class ExpressionExtensions
	{
		public static bool CheckInnerReferenceExpressions(this Expression first, Expression second)
		{
			if (first is VariableReferenceExpression && second is VariableReferenceExpression)
			{
				return ((VariableReferenceExpression)first).Variable == ((VariableReferenceExpression)second).Variable;
			}
			if (first is ArgumentReferenceExpression && second is ArgumentReferenceExpression)
			{
				return ((ArgumentReferenceExpression)first).Parameter == ((ArgumentReferenceExpression)second).Parameter;
			}
			if (first is FieldReferenceExpression && second is FieldReferenceExpression)
			{
				return ((FieldReferenceExpression)first).Field == ((FieldReferenceExpression)second).Field;
			}
			if (!(first is PropertyReferenceExpression) || !(second is PropertyReferenceExpression))
			{
				return false;
			}
			return ((PropertyReferenceExpression)first).Property == ((PropertyReferenceExpression)second).Property;
		}

		public static TypeReference GetTargetTypeReference(this Expression target)
		{
			if (target == null || !target.HasType)
			{
				return null;
			}
			return target.ExpressionType;
		}

		public static bool IsReferenceExpression(this Expression expression)
		{
			if (expression is VariableReferenceExpression || expression is ArgumentReferenceExpression || expression is FieldReferenceExpression)
			{
				return true;
			}
			return expression is PropertyReferenceExpression;
		}

		public static bool IsTypeOfExpression(this MethodInvocationExpression self, out TypeReference typeReference)
		{
			typeReference = null;
			MethodReferenceExpression methodExpression = self.MethodExpression;
			if (methodExpression == null)
			{
				return false;
			}
			MethodReference method = methodExpression.Method;
			if (method.CallingConvention == MethodCallingConvention.StdCall || method.DeclaringType.FullName != "System.Type" || method.Name != "GetTypeFromHandle")
			{
				return false;
			}
			if (self.Arguments.Count != 1)
			{
				return false;
			}
			MemberHandleExpression item = self.Arguments[0] as MemberHandleExpression;
			if (item == null)
			{
				return false;
			}
			typeReference = item.MemberReference as TypeReference;
			return typeReference != null;
		}

		public static string ToCodeString(this Expression expression)
		{
			StringWriter stringWriter = new StringWriter();
			IntermediateDecompilationCSharpLanguageWriter intermediateDecompilationCSharpLanguageWriter = new IntermediateDecompilationCSharpLanguageWriter(new PlainTextFormatter(stringWriter));
			((ILanguageTestCaseWriter)intermediateDecompilationCSharpLanguageWriter).SetContext(expression.UnderlyingSameMethodInstructions.First<Instruction>().ContainingMethod);
			((ILanguageTestCaseWriter)intermediateDecompilationCSharpLanguageWriter).Write(expression);
			return stringWriter.ToString();
		}
	}
}