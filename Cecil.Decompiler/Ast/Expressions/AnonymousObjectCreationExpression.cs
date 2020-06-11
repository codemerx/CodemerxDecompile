using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class AnonymousObjectCreationExpression : Expression
	{
		public AnonymousObjectCreationExpression(MethodReference constructor, TypeReference type, InitializerExpression initializer, IEnumerable<Instruction> instructions)
            :base(instructions)
		{
            this.Constructor = constructor;
            this.Type = type;
            this.Initializer = initializer;
		}

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                if (Initializer != null)
                {
                    yield return Initializer;
                }
            }
        }

		public override bool Equals(Expression other)
		{
            if (!(other is AnonymousObjectCreationExpression))
            {
                return false;
            }

			AnonymousObjectCreationExpression anonymousObjectCreation = other as AnonymousObjectCreationExpression;

            #region CheckConstructor

            if (this.Constructor == null)
            {
                if (anonymousObjectCreation.Constructor != null)
                {
                    return false;
                }
            }
            else
            {
                if (anonymousObjectCreation.Constructor == null || this.Constructor.FullName != anonymousObjectCreation.Constructor.FullName)
                {
                    return false;
                }
            }
            #endregion

            #region CheckType

            if (this.Type == null)
            {
                if (anonymousObjectCreation.Type != null)
                { 
                    return false;
                }
            }
            else if (anonymousObjectCreation.Type == null || this.Type.FullName != anonymousObjectCreation.Type.FullName)
            {
                return false;
            }
            #endregion

            #region CheckInitializer
            if (this.Initializer == null)
            {
                if (anonymousObjectCreation.Initializer != null)
                {
                    return false;
                }
            }
            else if (anonymousObjectCreation.Initializer == null || !this.Initializer.Equals(anonymousObjectCreation.Initializer))
            {
                return false;
            }
            #endregion

            return true;
        }

        public override Expression Clone()
        {
			InitializerExpression initializerClone = Initializer != null ? Initializer.Clone() as InitializerExpression : null;
			AnonymousObjectCreationExpression result = new AnonymousObjectCreationExpression(Constructor, Type, initializerClone, this.instructions);
			return result;
        }

        public override Expression CloneExpressionOnly()
        {
			InitializerExpression initializerClone = Initializer != null ? Initializer.CloneExpressionOnly() as InitializerExpression : null;
			AnonymousObjectCreationExpression result = new AnonymousObjectCreationExpression(Constructor, Type, initializerClone, null);
            return result;
        }

        public MethodReference Constructor { get; set; }

        public TypeReference Type { get; set; }

		public InitializerExpression Initializer { get; set; }

        public override bool HasType
        {
            get
            {
                return true;
            }
        }

        public override TypeReference ExpressionType
        {
            get
            {
                if (Type != null)
                {
                    return Type;
                }
                return Constructor.DeclaringType;
            }
            set
            {
                throw new NotSupportedException("Anonymous object creation expression cannot change it's type");
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.AnonymousObjectCreationExpression; }
        }
	}
}
