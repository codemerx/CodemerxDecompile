using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class ExceptionStatement : Statement
	{
		public Exception ExceptionObject { get; private set; }
		public IMemberDefinition Member { get; private set; }

		public ExceptionStatement(Exception exceptionObject, IMemberDefinition member)
		{
			this.ExceptionObject = exceptionObject;
			this.Member = member;
		}

        public override IEnumerable<ICodeNode> Children
        {
            get { yield break; }
        }

		public override Statement Clone()
		{
            return CloneStatementOnly();
        }

        public override Statement CloneStatementOnly()
        {
            return new ExceptionStatement(this.ExceptionObject, this.Member);
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.ExceptionStatement; }
        }
	}
}
