using System.Linq;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Steps
{
	public class RebuildEventsStep
	{
		private readonly TypeSystem typeSystem;

		public RebuildEventsStep(TypeSystem typeSystem)
		{
			this.typeSystem = typeSystem;
		}

		public ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (!(node.MethodExpression is MethodReferenceExpression))
			{
				return null;
			}

			MethodReferenceExpression methodReferenceExpression = node.MethodExpression;

			MethodReference methodReference = methodReferenceExpression.Method;

			if (methodReference.Name == null)
			{
				return null;
			}

			string eventMethodPrefix = GetEventMethodPrefix(methodReference.Name);
			if (string.IsNullOrEmpty(eventMethodPrefix))
			{
				return null;
			}

			if (methodReference.Parameters.Count != 1)
			{
				return null;
			}

            TypeReference typeReference = methodReference.DeclaringType;
			EventDefinition targetEvent = null;
			do
			{
				if (typeReference == null)
				{
					break;
				}

				TypeDefinition typeDefinition = typeReference.Resolve();

				if (typeDefinition == null)
				{
					break;
				}

				string eventName = methodReference.Name.Substring(eventMethodPrefix.Length);
				targetEvent = typeDefinition.Events.FirstOrDefault(e => e.Name == eventName);
				if (targetEvent == null)
				{
					typeReference = typeDefinition.BaseType;
				}
			} while (typeReference != null && targetEvent == null);

			if (targetEvent == null)
			{
				return null;
			}

			EventReferenceExpression target = new EventReferenceExpression(methodReferenceExpression.Target, targetEvent, null);
			return GetEventAssignExpression(target, node.Arguments[0], eventMethodPrefix, node.InvocationInstructions);
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

		private Expression GetEventAssignExpression(EventReferenceExpression target, Expression argument, string eventMethodPrefix, IEnumerable<Instruction> instructions)
		{
			if (eventMethodPrefix == "add_")
			{
				return new BinaryExpression(BinaryOperator.AddAssign, target, argument, typeSystem, instructions);
			}
			if (eventMethodPrefix == "remove_")
			{
				return new BinaryExpression(BinaryOperator.SubtractAssign, target, argument, typeSystem, instructions);
			}
			return null;
		}
	}
}