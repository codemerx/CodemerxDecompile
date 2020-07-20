using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	public class RebuildEventsStep
	{
		private readonly TypeSystem typeSystem;

		public RebuildEventsStep(TypeSystem typeSystem)
		{
			base();
			this.typeSystem = typeSystem;
			return;
		}

		private Expression GetEventAssignExpression(EventReferenceExpression target, Expression argument, string eventMethodPrefix, IEnumerable<Instruction> instructions)
		{
			if (String.op_Equality(eventMethodPrefix, "add_"))
			{
				return new BinaryExpression(2, target, argument, this.typeSystem, instructions, false);
			}
			if (!String.op_Equality(eventMethodPrefix, "remove_"))
			{
				return null;
			}
			return new BinaryExpression(4, target, argument, this.typeSystem, instructions, false);
		}

		private string GetEventMethodPrefix(string eventMethodName)
		{
			if (eventMethodName.StartsWith("add_"))
			{
				return "add_";
			}
			if (eventMethodName.StartsWith("remove_"))
			{
				return "remove_";
			}
			return null;
		}

		public ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (node.get_MethodExpression() == null)
			{
				return null;
			}
			V_0 = node.get_MethodExpression();
			V_1 = V_0.get_Method();
			if (V_1.get_Name() == null)
			{
				return null;
			}
			V_2 = this.GetEventMethodPrefix(V_1.get_Name());
			if (String.IsNullOrEmpty(V_2))
			{
				return null;
			}
			if (V_1.get_Parameters().get_Count() != 1)
			{
				return null;
			}
			V_3 = V_1.get_DeclaringType();
			V_4 = null;
			do
			{
				V_6 = new RebuildEventsStep.u003cu003ec__DisplayClass2_0();
				if (V_3 == null)
				{
					break;
				}
				V_7 = V_3.Resolve();
				if (V_7 == null)
				{
					break;
				}
				V_6.eventName = V_1.get_Name().Substring(V_2.get_Length());
				V_4 = V_7.get_Events().FirstOrDefault<EventDefinition>(new Func<EventDefinition, bool>(V_6.u003cVisitMethodInvocationExpressionu003eb__0));
				if (V_4 != null)
				{
					continue;
				}
				V_3 = V_7.get_BaseType();
			}
			while (V_3 != null && V_4 == null);
			if (V_4 == null)
			{
				return null;
			}
			V_5 = new EventReferenceExpression(V_0.get_Target(), V_4, null);
			return this.GetEventAssignExpression(V_5, node.get_Arguments().get_Item(0), V_2, node.get_InvocationInstructions());
		}
	}
}