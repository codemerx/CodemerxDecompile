using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Steps
{
	internal class ManagedPointersRemovalStep : BaseCodeVisitor, IDecompilationStep
	{
		private DecompilationContext context;

		private readonly Dictionary<VariableDefinition, BinaryExpression> variableToAssignExpression = new Dictionary<VariableDefinition, BinaryExpression>();

		public ManagedPointersRemovalStep()
		{
		}

		private bool CheckForAssignment(BinaryExpression node)
		{
			if (node.Left.CodeNodeType == CodeNodeType.ArgumentReferenceExpression && (node.Left as ArgumentReferenceExpression).Parameter.get_ParameterType().get_IsByReference())
			{
				throw new Exception("Managed pointer usage not in SSA");
			}
			if (node.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || !(node.Left as VariableReferenceExpression).Variable.get_VariableType().get_IsByReference())
			{
				return false;
			}
			VariableDefinition variableDefinition = (node.Left as VariableReferenceExpression).Variable.Resolve();
			if (this.variableToAssignExpression.ContainsKey(variableDefinition))
			{
				throw new Exception("Managed pointer usage not in SSA");
			}
			if (node.Right.CodeNodeType != CodeNodeType.VariableReferenceExpression && node.Right.CodeNodeType != CodeNodeType.ArgumentReferenceExpression && node.Right.CodeNodeType != CodeNodeType.UnaryExpression)
			{
				return false;
			}
			if (node.Right.CodeNodeType == CodeNodeType.UnaryExpression)
			{
				UnaryExpression right = node.Right as UnaryExpression;
				if (right.Operator != UnaryOperator.AddressReference)
				{
					return false;
				}
				if (right.Operand.CodeNodeType != CodeNodeType.VariableReferenceExpression && right.Operand.CodeNodeType != CodeNodeType.ArgumentReferenceExpression)
				{
					return false;
				}
			}
			this.variableToAssignExpression.Add(variableDefinition, node);
			return true;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.VisitExpressions();
			this.TransformExpressions(new ManagedPointersRemovalStep.VariableReplacer(this.variableToAssignExpression));
			this.TransformExpressions(new ManagedPointersRemovalStep.ComplexDereferencer());
			this.RemoveVariablesFromContext();
			return body;
		}

		private void RemoveVariablesFromContext()
		{
			foreach (VariableDefinition key in this.variableToAssignExpression.Keys)
			{
				this.context.MethodContext.RemoveVariable(key);
				this.context.MethodContext.VariablesToRename.Remove(key);
				this.context.MethodContext.VariableAssignmentData.Remove(key);
			}
		}

		public void TransformExpressions(BaseCodeTransformer transformer)
		{
			foreach (IList<Expression> value in this.context.MethodContext.Expressions.BlockExpressions.Values)
			{
				int num = 0;
				for (int i = 0; i < value.Count; i++)
				{
					Expression expression = (Expression)transformer.Visit(value[i]);
					if (expression != null)
					{
						int num1 = num;
						num = num1 + 1;
						value[num1] = expression;
					}
				}
				for (int j = value.Count - num; j > 0; j--)
				{
					value.RemoveAt(num + j - 1);
				}
			}
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			if (!node.IsAssignmentExpression || !this.CheckForAssignment(node))
			{
				base.VisitBinaryExpression(node);
			}
		}

		public void VisitExpressions()
		{
			foreach (IList<Expression> value in this.context.MethodContext.Expressions.BlockExpressions.Values)
			{
				foreach (Expression expression in value)
				{
					this.Visit(expression);
				}
			}
		}

		public override void VisitUnaryExpression(UnaryExpression node)
		{
			if (node.Operator != UnaryOperator.AddressDereference || node.Operand.CodeNodeType != CodeNodeType.VariableReferenceExpression || !this.variableToAssignExpression.ContainsKey((node.Operand as VariableReferenceExpression).Variable.Resolve()))
			{
				base.VisitUnaryExpression(node);
			}
		}

		private class ComplexDereferencer : SimpleDereferencer
		{
			public ComplexDereferencer()
			{
			}

			public override ICodeNode VisitArrayIndexerExpression(ArrayIndexerExpression node)
			{
				node.Target = (Expression)this.VisitTargetExpression(node.Target);
				node.Indices = (ExpressionCollection)this.Visit(node.Indices);
				return node;
			}

			public override ICodeNode VisitArrayLengthExpression(ArrayLengthExpression node)
			{
				node.Target = (Expression)this.VisitTargetExpression(node.Target);
				return node;
			}

			public override ICodeNode VisitEventReferenceExpression(EventReferenceExpression node)
			{
				node.Target = (Expression)this.VisitTargetExpression(node.Target);
				return node;
			}

			public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
			{
				node.Target = (Expression)this.VisitTargetExpression(node.Target);
				return node;
			}

			public override ICodeNode VisitMethodReferenceExpression(MethodReferenceExpression node)
			{
				node.Target = (Expression)this.VisitTargetExpression(node.Target);
				return node;
			}

			public override ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node)
			{
				node.Target = (Expression)this.VisitTargetExpression(node.Target);
				node.Arguments = (ExpressionCollection)this.Visit(node.Arguments);
				return node;
			}

			private ICodeNode VisitTargetExpression(Expression target)
			{
				// 
				// Current member / type: Telerik.JustDecompiler.Ast.ICodeNode Telerik.JustDecompiler.Steps.ManagedPointersRemovalStep/ComplexDereferencer::VisitTargetExpression(Telerik.JustDecompiler.Ast.Expressions.Expression)
				// File path: C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\Decompiler.Tests\bin\Release\netcoreapp2.1\Integration\Actual\JustDecompiler.NetStandard.dll
				// 
				// Product version: 0.0.0.0
				// Exception in: Telerik.JustDecompiler.Ast.ICodeNode VisitTargetExpression(Telerik.JustDecompiler.Ast.Expressions.Expression)
				// 
				// Object reference not set to an instance of an object.
				//    at Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.FindLowestCommonAncestor(ICollection`1 typeNodes) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\TypeInference\TypeInferer.cs:line 510
				//    at Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.MergeWithLowestCommonAncestor() in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\TypeInference\TypeInferer.cs:line 445
				//    at Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.ProcessSingleConstraints() in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\TypeInference\TypeInferer.cs:line 363
				//    at Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.InferTypes() in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\TypeInference\TypeInferer.cs:line 307
				//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.Process(DecompilationContext theContext, BlockStatement body) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\ExpressionDecompilerStep.cs:line 86
				//    at Telerik.JustDecompiler.Decompiler.DecompilationPipeline.RunInternal(MethodBody body, BlockStatement block, ILanguage language) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\DecompilationPipeline.cs:line 81
				//    at Telerik.JustDecompiler.Decompiler.DecompilationPipeline.Run(MethodBody body, ILanguage language) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\DecompilationPipeline.cs:line 70
				//    at Telerik.JustDecompiler.Decompiler.Extensions.Decompile(MethodBody body, ILanguage language, DecompilationContext& context, TypeSpecificContext typeContext) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\Extensions.cs:line 61
				//    at Telerik.JustDecompiler.Decompiler.WriterContextServices.BaseWriterContextService.DecompileMethod(ILanguage language, MethodDefinition method, TypeSpecificContext typeContext) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com

			}

			public override ICodeNode VisitUnaryExpression(UnaryExpression node)
			{
				if (node.Operator == UnaryOperator.AddressDereference)
				{
					if (node.Operand.CodeNodeType == CodeNodeType.ThisReferenceExpression)
					{
						return node.Operand;
					}
					ExplicitCastExpression operand = node.Operand as ExplicitCastExpression;
					if (operand != null && operand.TargetType.get_IsByReference())
					{
						TypeReference elementType = (operand.TargetType as ByReferenceType).get_ElementType();
						return new ExplicitCastExpression((Expression)this.Visit(operand.Expression), elementType, null);
					}
				}
				return base.VisitUnaryExpression(node);
			}
		}

		private class VariableReplacer : BaseCodeTransformer
		{
			private readonly Dictionary<VariableDefinition, BinaryExpression> variableToAssignExpression;

			private readonly HashSet<BinaryExpression> expressionsToSkip;

			public VariableReplacer(Dictionary<VariableDefinition, BinaryExpression> variableToAssignExpression)
			{
				this.variableToAssignExpression = variableToAssignExpression;
				this.expressionsToSkip = new HashSet<BinaryExpression>(variableToAssignExpression.Values);
			}

			private bool TryGetVariableValue(VariableDefinition variable, out Expression value)
			{
				BinaryExpression binaryExpression;
				if (!this.variableToAssignExpression.TryGetValue(variable, out binaryExpression))
				{
					value = null;
					return false;
				}
				value = binaryExpression.Right.CloneExpressionOnly();
				return true;
			}

			public override ICodeNode VisitBinaryExpression(BinaryExpression node)
			{
				if (!this.expressionsToSkip.Contains(node))
				{
					return base.VisitBinaryExpression(node);
				}
				base.VisitBinaryExpression(node);
				return null;
			}

			public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				Expression expression;
				if (!this.TryGetVariableValue(node.Variable.Resolve(), out expression))
				{
					return node;
				}
				return expression;
			}
		}
	}
}