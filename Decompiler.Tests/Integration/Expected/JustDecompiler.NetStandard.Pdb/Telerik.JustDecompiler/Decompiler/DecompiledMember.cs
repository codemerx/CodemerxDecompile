using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Decompiler
{
	public class DecompiledMember
	{
		public MethodSpecificContext Context
		{
			get;
			private set;
		}

		public string MemberFullName
		{
			get;
			set;
		}

		public Telerik.JustDecompiler.Ast.Statements.Statement Statement
		{
			get;
			set;
		}

		public DecompiledMember(string memberName, Telerik.JustDecompiler.Ast.Statements.Statement statement, MethodSpecificContext context)
		{
			base();
			this.set_MemberFullName(memberName);
			this.set_Statement(statement);
			this.set_Context(context);
			return;
		}
	}
}