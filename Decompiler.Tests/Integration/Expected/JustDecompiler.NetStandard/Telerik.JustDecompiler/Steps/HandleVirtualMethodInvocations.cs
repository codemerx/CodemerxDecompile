using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	public class HandleVirtualMethodInvocations
	{
		private MethodDefinition method;

		public HandleVirtualMethodInvocations(MethodDefinition method)
		{
			base();
			this.method = method;
			return;
		}

		private List<TypeDefinition> GetInheritanceChain(TypeDefinition targetType, string definingTypeFullName)
		{
			V_0 = new List<TypeDefinition>();
			V_1 = new List<int>();
			V_0.Add(targetType);
			V_1.Add(-1);
			V_2 = -1;
			V_4 = 0;
			while (V_4 < V_0.get_Count())
			{
				V_5 = V_0.get_Item(V_4);
				if (V_5 == null || String.op_Equality(V_5.get_FullName(), definingTypeFullName))
				{
					V_2 = V_4;
					break;
				}
				else
				{
					V_6 = V_5.get_BaseType();
					if (V_6 != null)
					{
						V_7 = V_6.Resolve();
						if (V_7 != null)
						{
							V_0.Add(V_7);
							V_1.Add(V_4);
						}
					}
					V_8 = V_5.get_Interfaces().GetEnumerator();
					try
					{
						while (V_8.MoveNext())
						{
							V_9 = V_8.get_Current();
							if (V_9 == null)
							{
								continue;
							}
							V_10 = V_9.Resolve();
							if (V_10 == null)
							{
								continue;
							}
							V_0.Add(V_10);
							V_1.Add(V_4);
						}
					}
					finally
					{
						V_8.Dispose();
					}
					V_4 = V_4 + 1;
				}
			}
			V_3 = new List<TypeDefinition>();
			while (V_2 != -1)
			{
				V_3.Add(V_0.get_Item(V_2));
				V_2 = V_1.get_Item(V_2);
			}
			return V_3;
		}

		public void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (!node.get_VirtualCall())
			{
				V_0 = node.get_MethodExpression();
				if (V_0 == null)
				{
					return;
				}
				if (V_0.get_Target() as ThisReferenceExpression != null)
				{
					V_7 = this.method.get_DeclaringType().Resolve();
					if (V_7 != null && (object)V_7 != (object)V_0.get_Method().get_DeclaringType().Resolve())
					{
						V_8 = this.method.get_DeclaringType().get_BaseType();
						if (V_8 == null || String.op_Equality(V_8.get_FullName(), Type.GetTypeFromHandle(// 
						// Current member / type: System.Void Telerik.JustDecompiler.Steps.HandleVirtualMethodInvocations::VisitMethodInvocationExpression(Telerik.JustDecompiler.Ast.Expressions.MethodInvocationExpression)
						// Exception in: System.Void VisitMethodInvocationExpression(Telerik.JustDecompiler.Ast.Expressions.MethodInvocationExpression)
						// Specified method is not supported.
						// 
						// mailto: JustDecompilePublicFeedback@telerik.com

	}
}