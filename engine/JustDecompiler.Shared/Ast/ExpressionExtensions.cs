using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.Decompiler.WriterContextServices;
using Telerik.JustDecompiler.Languages;
using System.IO;
using Telerik.JustDecompiler.Languages.TestCaseWriters;
using System.Linq;

namespace Telerik.JustDecompiler.Ast
{
	public static class ExpressionExtensions
	{
        public static string ToCodeString(this Expression expression)
        {
            var writer = new StringWriter();
            ILanguageTestCaseWriter languageWriter = new IntermediateDecompilationCSharpLanguageWriter(new PlainTextFormatter(writer), expression.UnderlyingSameMethodInstructions.First().ContainingMethod);
			languageWriter.SetContext(expression.UnderlyingSameMethodInstructions.First().ContainingMethod);
			languageWriter.Write(expression);

            return writer.ToString();
        }

        public static bool CheckInnerReferenceExpressions(this Expression first, Expression second)
        {
            if ((first is VariableReferenceExpression) && (second is VariableReferenceExpression))
            {
                return ((VariableReferenceExpression) first).Variable == ((VariableReferenceExpression) second).Variable;
            }
            if ((first is ArgumentReferenceExpression) && (second is ArgumentReferenceExpression))
            {
                return ((ArgumentReferenceExpression) first).Parameter == ((ArgumentReferenceExpression) second).Parameter;
            }
			if ((first is FieldReferenceExpression) && (second is FieldReferenceExpression))
			{
				return ((FieldReferenceExpression) first).Field == ((FieldReferenceExpression) second).Field;
			}
			if ((first is PropertyReferenceExpression) && (second is PropertyReferenceExpression))
			{
				return ((PropertyReferenceExpression) first).Property == ((PropertyReferenceExpression) second).Property;
			}
			return false;
		}

        public static bool IsReferenceExpression(this Expression expression)
        {
            return ((expression is VariableReferenceExpression) || (expression is ArgumentReferenceExpression) || (expression is FieldReferenceExpression) ||
                    (expression is PropertyReferenceExpression));
		}

        public static TypeReference GetTargetTypeReference(this Expression target)
        {
            if (target != null && target.HasType)
            {
				return target.ExpressionType;
				//if (target.CodeNodeType == CodeNodeType.VariableReferenceExpression)
				//{
				//    return ((VariableReferenceExpression)target).Variable.VariableType;
				//}
				//if (target.CodeNodeType == CodeNodeType.ArgumentReferenceExpression)
				//{
				//    return ((ArgumentReferenceExpression)target).Parameter.ParameterType;
				//}
				//if (target.CodeNodeType == CodeNodeType.FieldReferenceExpression)
				//{
				//    return ((FieldReferenceExpression)target).Field.FieldType;
				//}
				//if (target.CodeNodeType == CodeNodeType.PropertyReferenceExpression)
				//{
				//    return ((PropertyReferenceExpression)target).Property.PropertyType;
				//}
				//if (target.CodeNodeType == CodeNodeType.ArrayIndexerExpression)
				//{
				//    return ((ArrayIndexerExpression)target).Target.GetTargetTypeReference();
				//}
				//else if (target.CodeNodeType == CodeNodeType.BaseReferenceExpression)
				//{
				//    return ((BaseReferenceExpression)target).TargetType;
				//}
				//else if (target.CodeNodeType == CodeNodeType.ThisReferenceExpression)
				//{
				//    return ((ThisReferenceExpression)target).TargetType;
				//}
				//else if (target.CodeNodeType == CodeNodeType.CastExpression)
				//{
				//    return ((CastExpression)target).TargetType;
				//}
				//else if (target.CodeNodeType == CodeNodeType.MethodInvocationExpression)
				//{
				//    return ((MethodInvocationExpression)target).Method.Method.ReturnType;
				//}

            }
			return null;
		}

        public static bool IsTypeOfExpression(this MethodInvocationExpression self, out TypeReference typeReference)
        {
            typeReference = null;

            MethodReferenceExpression method_ref = self.MethodExpression;
            if (method_ref == null)
                return false;

            var method = method_ref.Method;
            if ((method.CallingConvention == Mono.Cecil.MethodCallingConvention.StdCall) ||
                (method.DeclaringType.FullName != "System.Type") ||
                (method.Name != "GetTypeFromHandle"))
                return false;

            if (self.Arguments.Count != 1)
                return false;

            MemberHandleExpression memberHandle = self.Arguments[0] as MemberHandleExpression;
            if (memberHandle == null)
            {
                return false;
            }

            typeReference = memberHandle.MemberReference as TypeReference;
            return typeReference != null;
        }
	}
}
