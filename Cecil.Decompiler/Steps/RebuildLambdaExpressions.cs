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
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Expressions;


namespace Telerik.JustDecompiler.Steps
{
    public class RebuildLambdaExpressions : BaseCodeTransformer, IDecompilationStep
    {
        private DecompilationContext context;

        public override ICodeNode VisitMethodReferenceExpression(MethodReferenceExpression node)
        {
            MethodDefinition methodDefinition = node.Method.Resolve();
            TypeDefinition currentType = context.TypeContext.CurrentType;
            if (methodDefinition == null ||
                methodDefinition.DeclaringType != currentType && !methodDefinition.DeclaringType.IsNestedIn(currentType))
            {
                return base.VisitMethodReferenceExpression(node);
            }
            
            if ((!methodDefinition.IsGetter) && (!methodDefinition.IsSetter) &&
                (methodDefinition.IsCompilerGenerated() || CheckTypeForCompilerGeneratedAttribute(methodDefinition.DeclaringType)))
            {
                BlockStatement statement = null;
                if (methodDefinition.Body != null)
                {
                    DecompilationContext innerContext = CreateDecompilationContext(methodDefinition);
                    statement = methodDefinition.Body.DecompileLambda(this.context.Language, innerContext);

					if ((statement.Statements.Count == 1) && (statement.Statements[0].CodeNodeType == CodeNodeType.ExpressionStatement) &&
						((statement.Statements[0] as ExpressionStatement).Expression.CodeNodeType == CodeNodeType.ReturnExpression))
					{
						ReturnExpression returnExpression = (statement.Statements[0] as ExpressionStatement).Expression as ReturnExpression;
						ShortFormReturnExpression shortFormReturnExpression = new ShortFormReturnExpression(returnExpression.Value, returnExpression.MappedInstructions);
						statement = new BlockStatement();
						statement.Statements.Add(new ExpressionStatement(shortFormReturnExpression));
					}

                    this.context.MethodContext.VariableDefinitionToNameMap.AddRange(innerContext.MethodContext.VariableDefinitionToNameMap);
					this.context.MethodContext.VariableNamesCollection.UnionWith(innerContext.MethodContext.VariableNamesCollection);
                    this.context.MethodContext.AddInnerMethodParametersToContext(innerContext.MethodContext);
					this.context.MethodContext.GotoStatements.AddRange(innerContext.MethodContext.GotoStatements);
					this.context.MethodContext.GotoLabels.AddRange(innerContext.MethodContext.GotoLabels);
                }

                ExpressionCollection expressionCollection = new ExpressionCollection();
                bool hasAnonymousParameter = LambdaExpressionsHelper.HasAnonymousParameter(methodDefinition.Parameters);
                foreach (ParameterDefinition parameter in methodDefinition.Parameters)
                {
                    expressionCollection.Add(new LambdaParameterExpression(parameter, !hasAnonymousParameter, null));
                }
                return new LambdaExpression(expressionCollection, statement, methodDefinition.IsAsync(), methodDefinition.IsFunction(), node.Method.Parameters, false, node.MappedInstructions);
            }
            return base.VisitMethodReferenceExpression(node);
        }

        private DecompilationContext CreateDecompilationContext(MethodDefinition lambdaMethodDefinition)
        {
            if (lambdaMethodDefinition.DeclaringType == context.TypeContext.CurrentType)
            {
                return new DecompilationContext(new MethodSpecificContext(lambdaMethodDefinition.Body), context.TypeContext, context.ModuleContext, context.AssemblyContext, context.Language);
            }
            else
            {
                return new DecompilationContext(new MethodSpecificContext(lambdaMethodDefinition.Body), new TypeSpecificContext(lambdaMethodDefinition.DeclaringType), context.Language);
            }
        }

        public override ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
        {
            base.VisitObjectCreationExpression(node);
            if (node.Constructor == null)
            {
                return node;
            }

            TypeDefinition objectType = node.Constructor.DeclaringType.Resolve();
            if (objectType == null || objectType.BaseType == null || objectType.BaseType.FullName != "System.MulticastDelegate" ||
                node.Arguments.Count != 2 || node.Arguments[1].CodeNodeType != CodeNodeType.LambdaExpression)
            {
                return node;
            }

			(node.Arguments[1] as LambdaExpression).ExpressionType = objectType;

			Expression target = node.Arguments[0];

			return new DelegateCreationExpression(node.ExpressionType, node.Arguments[1], target, node.MappedInstructions);
        }

        public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
        {
            node.MethodExpression = (MethodReferenceExpression)base.VisitMethodReferenceExpression(node.MethodExpression);
            node.Arguments = (ExpressionCollection)Visit(node.Arguments);
            return node;
        }

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			BlockStatement result = (BlockStatement)VisitBlockStatement(body);
			return result;
		}

        public bool CheckTypeForCompilerGeneratedAttribute(TypeDefinition typeDefinition)
        {
            while (typeDefinition.IsNested)
            {
                if (typeDefinition.HasCompilerGeneratedAttribute())
                {
                    return true;
                }

                typeDefinition = typeDefinition.DeclaringType;
            }

            return false;
        }
    }
}