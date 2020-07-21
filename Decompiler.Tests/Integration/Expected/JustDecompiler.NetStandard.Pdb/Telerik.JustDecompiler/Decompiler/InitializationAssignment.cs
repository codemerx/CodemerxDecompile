using Mono.Cecil;
using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler
{
	public class InitializationAssignment
	{
		public Expression AssignmentExpression
		{
			get;
			private set;
		}

		public MethodDefinition AssignmentMethod
		{
			get;
			private set;
		}

		public InitializationAssignment(MethodDefinition assignmentMethod, Expression assignmentExpression)
		{
			base();
			this.set_AssignmentMethod(assignmentMethod);
			this.set_AssignmentExpression(assignmentExpression);
			return;
		}
	}
}