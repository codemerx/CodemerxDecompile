using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    /// <summary>
    /// The following expression is used to contain auto-property initializations that are placed in the constructor. That can be an inline
    /// initialization of auto properties (with getter only or with getter and setter), or initialization of getter only auto property from
    /// the constructor. Both of the operation are just assignments of some value to the backing field of the property.
    /// </summary>
    public class AutoPropertyConstructorInitializerExpression : Expression
    {
        public AutoPropertyConstructorInitializerExpression(PropertyDefinition property, Expression target, IEnumerable<Instruction> instructions)
            : base(instructions)
        {
            this.Property = property;
            this.Target = target;
        }

        public PropertyDefinition Property { get; private set; }

        public Expression Target { get; private set; }

        public override bool Equals(Expression other)
        {
            AutoPropertyConstructorInitializerExpression expression = other as AutoPropertyConstructorInitializerExpression;
            if (expression == null)
            {
                return false;
            }

            if (this.Target == null)
            {
                if (expression.Target != null)
                {
                    return false;
                }
            }
            else if (!this.Target.Equals(expression.Target))
            {
                return false;
            }

            return this.Property.FullName == expression.Property.FullName;
        }

        public override Expression Clone()
        {
            return new AutoPropertyConstructorInitializerExpression(this.Property, this.Target != null ? this.Target.Clone() : null, this.MappedInstructions);
        }

        public override Expression CloneExpressionOnly()
        {
            return new AutoPropertyConstructorInitializerExpression(this.Property, this.Target != null ? this.Target.CloneExpressionOnly() : null, null);
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.AutoPropertyConstructorInitializerExpression; }
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                if (this.Target != null)
                {
                    yield return this.Target;
                }
            }
        }

        public override TypeReference ExpressionType
        {
            get
            {
                return this.Property.PropertyType;
            }
            set
            {
                throw new NotSupportedException("Auto-property constructor initializer cannot change its type.");
            }
        }

        public override bool HasType
        {
            get
            {
                return this.Property.PropertyType != null;
            }
        }
    }
}
