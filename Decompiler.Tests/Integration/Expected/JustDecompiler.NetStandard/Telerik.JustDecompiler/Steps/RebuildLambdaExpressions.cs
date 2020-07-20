using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class RebuildLambdaExpressions : BaseCodeTransformer, IDecompilationStep
	{
		private DecompilationContext context;

		public RebuildLambdaExpressions()
		{
			base();
			return;
		}

		public bool CheckTypeForCompilerGeneratedAttribute(TypeDefinition typeDefinition)
		{
			while (typeDefinition.get_IsNested())
			{
				if (typeDefinition.HasCompilerGeneratedAttribute())
				{
					return true;
				}
				typeDefinition = typeDefinition.get_DeclaringType();
			}
			return false;
		}

		private DecompilationContext CreateDecompilationContext(MethodDefinition lambdaMethodDefinition)
		{
			if ((object)lambdaMethodDefinition.get_DeclaringType() != (object)this.context.get_TypeContext().get_CurrentType())
			{
				return new DecompilationContext(new MethodSpecificContext(lambdaMethodDefinition.get_Body()), new TypeSpecificContext(lambdaMethodDefinition.get_DeclaringType()), this.context.get_Language());
			}
			return new DecompilationContext(new MethodSpecificContext(lambdaMethodDefinition.get_Body()), this.context.get_TypeContext(), this.context.get_ModuleContext(), this.context.get_AssemblyContext(), this.context.get_Language());
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			return (BlockStatement)this.VisitBlockStatement(body);
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			node.set_MethodExpression((MethodReferenceExpression)this.VisitMethodReferenceExpression(node.get_MethodExpression()));
			node.set_Arguments((ExpressionCollection)this.Visit(node.get_Arguments()));
			return node;
		}

		public override ICodeNode VisitMethodReferenceExpression(MethodReferenceExpression node)
		{
			V_0 = node.get_Method().Resolve();
			V_1 = this.context.get_TypeContext().get_CurrentType();
			if (V_0 == null || (object)V_0.get_DeclaringType() != (object)V_1 && !V_0.get_DeclaringType().IsNestedIn(V_1))
			{
				return this.VisitMethodReferenceExpression(node);
			}
			if (V_0.get_IsGetter() || V_0.get_IsSetter() || !V_0.IsCompilerGenerated(true) && !this.CheckTypeForCompilerGeneratedAttribute(V_0.get_DeclaringType()))
			{
				return this.VisitMethodReferenceExpression(node);
			}
			V_2 = null;
			if (V_0.get_Body() != null)
			{
				V_5 = this.CreateDecompilationContext(V_0);
				V_2 = V_0.get_Body().DecompileLambda(this.context.get_Language(), V_5);
				if (V_2.get_Statements().get_Count() == 1 && V_2.get_Statements().get_Item(0).get_CodeNodeType() == 5 && (V_2.get_Statements().get_Item(0) as ExpressionStatement).get_Expression().get_CodeNodeType() == 57)
				{
					V_6 = (V_2.get_Statements().get_Item(0) as ExpressionStatement).get_Expression() as ReturnExpression;
					V_7 = new ShortFormReturnExpression(V_6.get_Value(), V_6.get_MappedInstructions());
					V_2 = new BlockStatement();
					V_2.get_Statements().Add(new ExpressionStatement(V_7));
				}
				this.context.get_MethodContext().get_VariableDefinitionToNameMap().AddRange<VariableDefinition, string>(V_5.get_MethodContext().get_VariableDefinitionToNameMap());
				this.context.get_MethodContext().get_VariableNamesCollection().UnionWith(V_5.get_MethodContext().get_VariableNamesCollection());
				this.context.get_MethodContext().AddInnerMethodParametersToContext(V_5.get_MethodContext());
				this.context.get_MethodContext().get_GotoStatements().AddRange(V_5.get_MethodContext().get_GotoStatements());
				this.context.get_MethodContext().get_GotoLabels().AddRange<string, Statement>(V_5.get_MethodContext().get_GotoLabels());
			}
			V_3 = new ExpressionCollection();
			V_4 = LambdaExpressionsHelper.HasAnonymousParameter(V_0.get_Parameters());
			V_8 = V_0.get_Parameters().GetEnumerator();
			try
			{
				while (V_8.MoveNext())
				{
					V_9 = V_8.get_Current();
					V_3.Add(new LambdaParameterExpression(V_9, !V_4, null));
				}
			}
			finally
			{
				V_8.Dispose();
			}
			return new LambdaExpression(V_3, V_2, V_0.IsAsync(), V_0.IsFunction(), node.get_Method().get_Parameters(), false, node.get_MappedInstructions());
		}

		public override ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			dummyVar0 = this.VisitObjectCreationExpression(node);
			if (node.get_Constructor() == null)
			{
				return node;
			}
			V_0 = node.get_Constructor().get_DeclaringType().Resolve();
			if (V_0 == null || V_0.get_BaseType() == null || String.op_Inequality(V_0.get_BaseType().get_FullName(), "System.MulticastDelegate") || node.get_Arguments().get_Count() != 2 || node.get_Arguments().get_Item(1).get_CodeNodeType() != 50)
			{
				return node;
			}
			(node.get_Arguments().get_Item(1) as LambdaExpression).set_ExpressionType(V_0);
			V_1 = node.get_Arguments().get_Item(0);
			return new DelegateCreationExpression(node.get_ExpressionType(), node.get_Arguments().get_Item(1), V_1, node.get_MappedInstructions());
		}
	}
}