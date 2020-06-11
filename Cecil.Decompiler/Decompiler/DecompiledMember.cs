using System;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Decompiler
{
	public class DecompiledMember
	{
		public string MemberFullName { get; set; }
		public Statement Statement { get; set; }
		public MethodSpecificContext Context { get; private set; }

		public DecompiledMember(string memberName, Statement statement, MethodSpecificContext context)
		{
			this.MemberFullName = memberName;
			this.Statement = statement;
			this.Context = context;
		}
	}
}
