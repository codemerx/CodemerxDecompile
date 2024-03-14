using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
			this.typeSystem = typeSystem;
		}

		private Expression GetEventAssignExpression(EventReferenceExpression target, Expression argument, string eventMethodPrefix, IEnumerable<Instruction> instructions)
		{
			if (eventMethodPrefix == "add_")
			{
				return new BinaryExpression(BinaryOperator.AddAssign, target, argument, this.typeSystem, instructions, false);
			}
			if (eventMethodPrefix != "remove_")
			{
				return null;
			}
			return new BinaryExpression(BinaryOperator.SubtractAssign, target, argument, this.typeSystem, instructions, false);
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
			if (node.MethodExpression == null)
			{
				return null;
			}
			MethodReferenceExpression methodExpression = node.MethodExpression;
			MethodReference method = methodExpression.Method;
			if (method.get_Name() == null)
			{
				return null;
			}
			string eventMethodPrefix = this.GetEventMethodPrefix(method.get_Name());
			if (String.IsNullOrEmpty(eventMethodPrefix))
			{
				return null;
			}
			if (method.get_Parameters().get_Count() != 1)
			{
				return null;
			}
			TypeReference declaringType = method.get_DeclaringType();
			EventDefinition eventDefinition = null;
			do
			{
				if (declaringType == null)
				{
					break;
				}
				TypeDefinition typeDefinition = declaringType.Resolve();
				if (typeDefinition == null)
				{
					break;
				}
				string str = method.get_Name().Substring(eventMethodPrefix.Length);
				eventDefinition = typeDefinition.get_Events().FirstOrDefault<EventDefinition>((EventDefinition e) => e.get_Name() == str);
				if (eventDefinition != null)
				{
					continue;
				}
				declaringType = typeDefinition.get_BaseType();
			}
			while (declaringType != null && eventDefinition == null);
			if (eventDefinition == null)
			{
				return null;
			}
			EventReferenceExpression eventReferenceExpression = new EventReferenceExpression(methodExpression.Target, eventDefinition, null);
			return this.GetEventAssignExpression(eventReferenceExpression, node.Arguments[0], eventMethodPrefix, node.InvocationInstructions);
		}
	}
}