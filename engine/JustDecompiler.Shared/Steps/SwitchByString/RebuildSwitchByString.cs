#region license
//   (C) 2007 - 2008 Novell, Inc. http://www.novell.com
//   (C) 2007 - 2008 Jb Evain http://evain.net
//Permission is hereby granted, free of charge, to any person obtaining
//a copy of this software and associated documentation files (the
//"Software"), to deal in the Software without restriction, including
//without limitation the rights to use, copy, modify, merge, publish,
//distribute, sublicense, and/or sell copies of the Software, and to
//permit persons to whom the Software is furnished to do so, subject to
//the following conditions:
//The above copyright notice and this permission notice shall be
//included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.SwitchByString
{
	/// <summary>
	/// This step reverts back the compiler-generated mapping of string to integer via dictionary.
	/// </summary>
	public class RebuildSwitchByString : BaseCodeTransformer, IDecompilationStep
	{
		private DecompilationContext context;
		private SwitchByStringMatcher matcher = new SwitchByStringMatcher();
		private SwitchByStringFixer fixer;

		public BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			this.context = context;
			fixer = new SwitchByStringFixer(context.MethodContext.Method.Module.TypeSystem);
			Visit(block);
			return block;
		}

		public override ICodeNode VisitIfStatement(IfStatement node)
		{
			if (matcher.TryMatch(node))
			{
				Statement result = fixer.FixToSwitch(node, matcher.StringVariable, matcher.IntVariable);
				return base.Visit(result);
			}
			return base.VisitIfStatement(node);
		}
	}
}