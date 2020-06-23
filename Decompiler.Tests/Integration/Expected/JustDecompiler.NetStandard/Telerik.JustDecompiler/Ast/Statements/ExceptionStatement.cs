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
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.ExceptionStatement;
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
			this.ExceptionObject = exceptionObject;
			this.Member = member;
		}

		public override Statement Clone()
		{
			return this.CloneStatementOnly();
		}

		public override Statement CloneStatementOnly()
		{
			return new ExceptionStatement(this.ExceptionObject, this.Member);
		}
	}
}