using Mono.Cecil;
using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Decompiler
{
	public class GeneratedMethod
	{
		public Statement Body
		{
			get;
			private set;
		}

		public MethodSpecificContext Context
		{
			get;
			private set;
		}

		public MethodDefinition Method
		{
			get;
			private set;
		}

		public GeneratedMethod(MethodDefinition method, Statement body, MethodSpecificContext context)
		{
			base();
			this.set_Method(method);
			this.set_Body(body);
			this.set_Context(context);
			return;
		}
	}
}