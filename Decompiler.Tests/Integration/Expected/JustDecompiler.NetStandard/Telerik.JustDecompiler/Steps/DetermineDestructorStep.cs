using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class DetermineDestructorStep : IDecompilationStep
	{
		public DetermineDestructorStep()
		{
			base();
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			V_0 = context.get_MethodContext().get_Method();
			if (String.op_Equality(V_0.get_Name(), "Finalize") && V_0.get_IsVirtual() && body.get_Statements().get_Count() == 1 && body.get_Statements().get_Item(0) as TryStatement != null)
			{
				V_1 = body.get_Statements().get_Item(0) as TryStatement;
				if (V_1.get_Finally() != null && V_1.get_Finally().get_Body().get_Statements().get_Count() == 1 && V_1.get_Finally().get_Body().get_Statements().get_Item(0) as ExpressionStatement != null)
				{
					V_2 = V_1.get_Finally().get_Body().get_Statements().get_Item(0) as ExpressionStatement;
					if (V_2.get_Expression() as MethodInvocationExpression != null)
					{
						V_3 = (V_2.get_Expression() as MethodInvocationExpression).get_MethodExpression().get_MethodDefinition();
						if (V_3 != null && String.op_Equality(V_3.get_Name(), "Finalize") && String.op_Equality(V_3.get_DeclaringType().get_FullName(), V_0.get_DeclaringType().get_BaseType().get_FullName()))
						{
							context.get_MethodContext().set_IsDestructor(true);
							stackVariable71 = context.get_MethodContext();
							stackVariable72 = new BlockStatement();
							stackVariable72.set_Statements(V_1.get_Try().get_Statements());
							stackVariable71.set_DestructorStatements(stackVariable72);
						}
					}
				}
			}
			return body;
		}
	}
}