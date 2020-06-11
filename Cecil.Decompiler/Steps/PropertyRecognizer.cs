#region license
//
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
using Mono.Cecil;
using Mono.Cecil.Extensions;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Steps
{
	public class PropertyRecognizer
	{
		private readonly TypeSystem typeSystem;
        private readonly TypeSpecificContext typeContext;
        private readonly ILanguage language;

        public PropertyRecognizer(TypeSystem typeSystem, TypeSpecificContext typeContext, ILanguage language)
		{
			this.typeSystem = typeSystem;
            this.typeContext = typeContext;
            this.language = language;
		}

		public ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			MethodReferenceExpression methodRef = node.MethodExpression;

			if (methodRef == null)
			{
				return null;
			}

			MethodDefinition method = methodRef.Method as MethodDefinition;

			//// must be resolved.
			if (method == null)
			{
				MethodReference methodReference = methodRef.Method;
				if (methodReference != null
					&& !string.IsNullOrEmpty(methodReference.Name)
					&& (methodReference.Name.StartsWith("set_") || methodReference.Name.StartsWith("get_") || methodReference.Name.StartsWith("put_") /*Setter prefix in winrt*/))
				{
					method = methodReference.Resolve();
				}
			}

			if (method != null)
			{
				if (method.IsGetter || method.IsSetter)
				{
					PropertyReferenceExpression propExpr = new PropertyReferenceExpression(node, null);
					if (propExpr.Property == null)
					{
						// sanity check - if the method is resolved and is determined to be getter/setter, then a
						// property record should be available.
						return node;
					}
					Expression result = propExpr;
					if (method.IsSetter)
					{
						int last = node.Arguments.Count - 1;
						result = new BinaryExpression(BinaryOperator.Assign, propExpr, node.Arguments[last], typeSystem, null);
					}
					return result;
				}
			}
			return null;
		}

        public ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
        {
            PropertyDefinition property;
            if (IsAutoPropertyConstructorInitializerExpression(node.Field, out property))
            {
                return new AutoPropertyConstructorInitializerExpression(property, node.Target, node.MappedInstructions);
            }

            return node;
        }

        private bool IsAutoPropertyConstructorInitializerExpression(FieldReference fieldReference, out PropertyDefinition property)
        {
            FieldDefinition fieldDefinition = fieldReference.Resolve();
            if (fieldDefinition != null)
            {
                Dictionary<FieldDefinition, PropertyDefinition> map = this.typeContext.GetFieldToPropertyMap(language);
                if (map.ContainsKey(fieldDefinition) &&
                    map[fieldDefinition] != null &&
                    !map[fieldDefinition].ShouldStaySplit())
                {
                    property = map[fieldDefinition];
                    return true;
                }
            }

            property = null;
            return false;
        }
    }
}