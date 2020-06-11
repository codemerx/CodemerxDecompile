using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class DynamicMemberReferenceExpression : Expression
    {
        public DynamicMemberReferenceExpression(Expression target, string memberName, TypeReference type, IEnumerable<Instruction> instructions)
            :base(instructions)
        {
            this.IsMethodInvocation = false;
            this.Target = target;
            this.MemberName = memberName;
            this.ExpressionType = type;
        }

        public DynamicMemberReferenceExpression(Expression target, string memberName, TypeReference type, IEnumerable<Instruction> instructions,
            IEnumerable<Expression> invocationArguments,
            IEnumerable<TypeReference> genericTypeArguments = null)
            :base(instructions)
        {
            this.IsMethodInvocation = true;
            this.Target = target;
            this.MemberName = memberName;
            this.ExpressionType = type;
            this.InvocationArguments = new ExpressionCollection();
            foreach (Expression item in invocationArguments)
            {
                this.InvocationArguments.Add(item);
            }

            if (genericTypeArguments != null)
            {
                this.GenericTypeArguments = new List<TypeReference>(genericTypeArguments);
            }
        }

        public Expression Target { get; internal set; }

        public string MemberName { get; private set; }

        public bool IsMethodInvocation { get; private set; }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                yield return Target;
                if (InvocationArguments != null)
                {
                    foreach (ICodeNode argument in InvocationArguments)
                    {
                        yield return argument;
                    }
                }
            }
        }

		public bool IsGenericMethod
		{
            get
            {
                return GenericTypeArguments != null;
            }
        }

        public ExpressionCollection InvocationArguments { get; internal set; }

        public List<TypeReference> GenericTypeArguments { get; private set; }

        public override bool Equals(Expression other)
        {
            if(other.CodeNodeType != CodeNodeType.DynamicMemberReferenceExpression)
            {
                return false;
            }

            DynamicMemberReferenceExpression otherExpression = other as DynamicMemberReferenceExpression;
            if(!this.Target.Equals(otherExpression.Target) || this.MemberName != otherExpression.MemberName ||
                this.IsMethodInvocation != otherExpression.IsMethodInvocation)
            {
                return false;
            }

            if(!this.IsMethodInvocation)
            {
                return true;
            }

            if(!this.InvocationArguments.Equals(otherExpression.InvocationArguments) || this.IsGenericMethod != otherExpression.IsGenericMethod)
            {
                return false;
            }

            if (!this.IsGenericMethod)
            {
                return true;
            }

            using(IEnumerator<TypeReference> thisEnumerator = this.GenericTypeArguments.GetEnumerator())
            {
                using (IEnumerator<TypeReference> otherEnumerator = otherExpression.GenericTypeArguments.GetEnumerator())
                {
                    while(true)
                    {
                        bool thisMoveNext = thisEnumerator.MoveNext();
                        bool otherMoveNext = otherEnumerator.MoveNext();
                        if(thisMoveNext != otherMoveNext)
                        {
                            return false;
                        }

                        if(!thisMoveNext)
                        {
                            return true;
                        }

                        if(thisEnumerator.Current != otherEnumerator.Current)
                        {
                            return false;
                        }
                    }
                }
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.DynamicMemberReferenceExpression; }
        }

        public override Expression Clone()
        {
            DynamicMemberReferenceExpression clone = new DynamicMemberReferenceExpression(this.Target.Clone(), this.MemberName, this.ExpressionType, this.instructions);
            if(this.IsMethodInvocation)
            {
                clone.IsMethodInvocation = true;
                clone.InvocationArguments = this.InvocationArguments.Clone();
                if(this.IsGenericMethod)
                {
                    clone.GenericTypeArguments = new List<TypeReference>(this.GenericTypeArguments);
                }
            }
            return clone;
        }

        public override Expression CloneExpressionOnly()
        {
            DynamicMemberReferenceExpression result = new DynamicMemberReferenceExpression(Target.CloneExpressionOnly(), this.MemberName, this.ExpressionType, (IEnumerable<Instruction>)null);
            if (this.IsMethodInvocation)
            {
                result.IsMethodInvocation = true;
                result.InvocationArguments = this.InvocationArguments.CloneExpressionsOnly();
                if (this.IsGenericMethod)
                {
                    result.GenericTypeArguments = new List<TypeReference>(this.GenericTypeArguments);
                }
            }
            return result;
        }
    }
}
