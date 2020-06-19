#region license
//
//	(C) 2005 - 2007 db4objects Inc. http://www.db4o.com
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
// Warning: generated do not edit
using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class LiteralExpression : Expression
    {
        private object value;
        private readonly TypeSystem typeSystem;

		public LiteralExpression(object value, TypeSystem typeSystem, IEnumerable<Instruction> instructions)
            :base(instructions)
		{
            this.typeSystem = typeSystem;
            this.Value = value;
        }

        public override IEnumerable<ICodeNode> Children
        {
            get { yield break; }
        }

		public override bool Equals(Expression other)
		{
            if (!(other is LiteralExpression))
            {
                return false;
            }
            if (this.Value == null)
            {
                return (other as LiteralExpression).Value == null;
            }
            return Value.Equals((other as LiteralExpression).Value);
        }

        public override Expression Clone()
        {
            LiteralExpression result = new LiteralExpression(value, typeSystem, this.instructions);
			return result;
        }

        public override Expression CloneExpressionOnly()
        {
            LiteralExpression result = new LiteralExpression(value, typeSystem, null);
            return result;
        }

		public object Value
		{
			get { return value; }
            set { this.value = value; ResolveType(); }
		}

		public override CodeNodeType CodeNodeType
		{
			get { return CodeNodeType.LiteralExpression; }
		}

        /// <summary>
        /// Resolves the type of a literal expression. Also sets the ExpressionType property of the literal expression with the found result.
        /// </summary>
        /// <param name="literal">The literal expression whose type is being resolved.</param>
        /// <param name="typeSystem">The type system for the current assembly.</param>
        /// <returns>Returns TypeReference to the type of the literal expression.</returns>
        private void ResolveType()
        {
            if (Value == null)
            {
                ExpressionType = typeSystem.Object;
                return;
            }
            Type t = Value.GetType();
            switch (t.Name)
            {
                case "String":
                    ExpressionType = typeSystem.String;
                    return;
                case "Boolean":
                    ExpressionType = typeSystem.Boolean;
                    return;
                case "Byte":
                    ExpressionType = typeSystem.Byte;
                    return;
                case "SByte":
                    ExpressionType = typeSystem.SByte;
                    return;
                case "Char":
                    ExpressionType = typeSystem.Char;
                    return;
                case "UInt16":
                    ExpressionType = typeSystem.UInt16;
                    return;
                case "Int16":
                    ExpressionType = typeSystem.Int16;
                    return;
                case "Int32":
                    ExpressionType = typeSystem.Int32;
                    return;
                case "UInt32":
                    ExpressionType = typeSystem.UInt32;
                    return;
                case "Int64":
                    ExpressionType = typeSystem.Int64;
                    return;
                case "UInt64":
                    ExpressionType = typeSystem.UInt64;
                    return;
                case "Single":
                    ExpressionType = typeSystem.Single;
                    return;
                case "Double":
                    ExpressionType = typeSystem.Double;
                    return;
                case "IntPtr":
                    ExpressionType = typeSystem.IntPtr;
                    return;
                default:
                    throw new ArgumentOutOfRangeException("Unknown type for literal expression.");
            }
        }

	}
}