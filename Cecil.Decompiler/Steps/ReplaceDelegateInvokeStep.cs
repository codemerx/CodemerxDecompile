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
using Telerik.JustDecompiler.Ast;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	public class ReplaceDelegateInvokeStep
	{
        private readonly BaseCodeTransformer codeTransformer;

        public ReplaceDelegateInvokeStep(BaseCodeTransformer codeTransformer)
        {
            this.codeTransformer = codeTransformer;
        }

        public ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
        {
			if (node.MethodExpression.CodeNodeType == CodeNodeType.MethodReferenceExpression)
			{
                MethodReferenceExpression methodReferenceExpression = node.MethodExpression;
				MethodReference methodReference = methodReferenceExpression.Method;
				if (IsDelegateInvokeMethod(methodReference))
				{
                    ExpressionCollection visitedArguments = (ExpressionCollection)codeTransformer.Visit(node.Arguments);
					return new DelegateInvokeExpression(methodReferenceExpression.Target, visitedArguments, methodReference, node.InvocationInstructions);
				}
			}
			return null;
		}

		private bool IsDelegateInvokeMethod(MethodReference methodReference)
		{
			if (methodReference == null)
			{
				return false;
			}
			if (methodReference.Name != "Invoke")
			{
				return false;
			}

            TypeDefinition declaringType = methodReference.DeclaringType.Resolve();
            if (declaringType != null && declaringType.BaseType != null)
            {
                if (declaringType.BaseType.FullName == "System.MulticastDelegate")
                {
                    return true;
                }
            }
			return false;
		}
	}
}