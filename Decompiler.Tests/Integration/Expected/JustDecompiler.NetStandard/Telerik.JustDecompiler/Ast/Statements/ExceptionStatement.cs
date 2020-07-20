using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class ExceptionStatement : Statement
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				return new ExceptionStatement.u003cget_Childrenu003ed__10(-2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 67;
			}
		}

		public Exception ExceptionObject
		{
			get;
			private set;
		}

		public IMemberDefinition Member
		{
			get;
			private set;
		}

		public ExceptionStatement(Exception exceptionObject, IMemberDefinition member)
		{
			base();
			this.set_ExceptionObject(exceptionObject);
			this.set_Member(member);
			return;
		}

		public override Statement Clone()
		{
			return this.CloneStatementOnly();
		}

		public override Statement CloneStatementOnly()
		{
			return new ExceptionStatement(this.get_ExceptionObject(), this.get_Member());
		}
	}
}