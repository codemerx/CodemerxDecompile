using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class EnumExpression : Expression
	{
		public EnumExpression(FieldDefinition fieldDefinition, IEnumerable<Instruction> instructions)
            :base(instructions)
		{
			this.Field = fieldDefinition;
        }

        public override IEnumerable<ICodeNode> Children
        {
            get { yield break; }
        }

		public override bool Equals(Expression other)
		{
            if (!(other is EnumExpression))
            {
                return false;
            }
			return this.Field.FullName == (other as EnumExpression).Field.FullName;
		}

        public override Expression Clone()
        {
			EnumExpression result = new EnumExpression(Field, instructions);
			return result;
        }

        public override Expression CloneExpressionOnly()
        {
            EnumExpression result = new EnumExpression(Field, null);
            return result;
        }

        public string FieldName
        {
			get
			{
				return Field.Name;
			}
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.EnumExpression; }
        }

        public override TypeReference ExpressionType
        {
			get
			{
				return Field.DeclaringType;
			}
            set
            {
                throw new NotSupportedException("Enum Expression cannot change its type.");
            }
        }

		public FieldDefinition Field { get; private set; }

        public override bool HasType
        {
            get
            {
                return true;
            }
        }
    }
}
