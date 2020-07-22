using Mono.Cecil;
using System;
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
			base();
			return;
		}

		private void InsertTopLevelParameterAssignments(BlockStatement block)
		{
			V_0 = 0;
			while (V_0 < this.context.get_MethodContext().get_OutParametersToAssign().get_Count())
			{
				V_1 = this.context.get_MethodContext().get_OutParametersToAssign().get_Item(V_0);
				if (V_1.get_ParameterType().get_IsByReference())
				{
					stackVariable18 = V_1.get_ParameterType().GetElementType();
				}
				else
				{
					stackVariable18 = V_1.get_ParameterType();
				}
				V_2 = stackVariable18;
				V_3 = new UnaryExpression(8, new ArgumentReferenceExpression(V_1, null), null);
				V_4 = new BinaryExpression(26, V_3, V_2.GetDefaultValueExpression(this.typeSystem), V_2, this.typeSystem, null, false);
				block.AddStatementAt(V_0, new ExpressionStatement(V_4));
				V_0 = V_0 + 1;
			}
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.typeSystem = context.get_MethodContext().get_Method().get_Module().get_TypeSystem();
			this.InsertTopLevelParameterAssignments(body);
			return body;
		}
	}
}