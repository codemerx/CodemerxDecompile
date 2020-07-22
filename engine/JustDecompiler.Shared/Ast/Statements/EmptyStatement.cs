using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Statements
{
	/// <summary>
	/// Contains no code just a goto label.
	/// </summary>
	public class EmptyStatement : Statement
	{

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
            EmptyStatement result = new EmptyStatement();
            CopyParentAndLabel(result);
            return result;
        }

        public override CodeNodeType CodeNodeType
        {
			get
			{
				return Ast.CodeNodeType.EmptyStatement;
			}
		}
	}
}
