using Mono.Cecil;
using Mono.Cecil.Extensions;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class AssignOutParametersStep : IDecompilationStep
	{
		private DecompilationContext context;

		private TypeSystem typeSystem;

		public AssignOutParametersStep()
		{
		}

		private void InsertTopLevelParameterAssignments(BlockStatement block)
		{
			for (int i = 0; i < this.context.MethodContext.OutParametersToAssign.Count; i++)
			{
				ParameterDefinition item = this.context.MethodContext.OutParametersToAssign[i];
				TypeReference typeReference = (item.get_ParameterType().get_IsByReference() ? item.get_ParameterType().GetElementType() : item.get_ParameterType());
				UnaryExpression unaryExpression = new UnaryExpression(UnaryOperator.AddressDereference, new ArgumentReferenceExpression(item, null), null);
				BinaryExpression binaryExpression = new BinaryExpression(BinaryOperator.Assign, unaryExpression, typeReference.GetDefaultValueExpression(this.typeSystem), typeReference, this.typeSystem, null, false);
				block.AddStatementAt(i, new ExpressionStatement(binaryExpression));
			}
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.typeSystem = context.MethodContext.Method.get_Module().get_TypeSystem();
			this.InsertTopLevelParameterAssignments(body);
			return body;
		}
	}
}