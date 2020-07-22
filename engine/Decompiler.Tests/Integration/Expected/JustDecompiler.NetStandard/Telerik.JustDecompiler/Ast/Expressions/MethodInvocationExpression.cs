using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class MethodInvocationExpression : Expression
	{
		public ExpressionCollection Arguments
		{
			get;
			set;
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new MethodInvocationExpression.u003cget_Childrenu003ed__20(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 19;
			}
		}

		public TypeReference ConstraintType
		{
			get;
			set;
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_MethodExpression().get_ExpressionType();
			}
			set
			{
				throw new NotSupportedException("Expression type of method invocation expression can not be changed.");
			}
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public IList<Instruction> InvocationInstructions
		{
			get
			{
				stackVariable2 = new List<Instruction>(this.instructions);
				stackVariable2.AddRange(this.get_MethodExpression().get_MappedInstructions());
				return stackVariable2;
			}
		}

		public bool IsByReference
		{
			get
			{
				return this.get_MethodExpression().get_Method().get_ReturnType().get_IsByReference();
			}
		}

		public bool IsConstrained
		{
			get
			{
				return (object)this.get_ConstraintType() != (object)null;
			}
		}

		public MethodReferenceExpression MethodExpression
		{
			get;
			set;
		}

		public bool VirtualCall
		{
			get;
			set;
		}

		public MethodInvocationExpression(MethodReferenceExpression method, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_MethodExpression(method);
			this.set_Arguments(new ExpressionCollection());
			return;
		}

		public override Expression Clone()
		{
			stackVariable6 = new MethodInvocationExpression(this.get_MethodExpression().Clone() as MethodReferenceExpression, this.instructions);
			stackVariable6.set_Arguments(this.get_Arguments().Clone());
			stackVariable6.set_VirtualCall(this.get_VirtualCall());
			stackVariable6.set_ConstraintType(this.get_ConstraintType());
			return stackVariable6;
		}

		public override Expression CloneExpressionOnly()
		{
			stackVariable5 = new MethodInvocationExpression(this.get_MethodExpression().CloneExpressionOnly() as MethodReferenceExpression, null);
			stackVariable5.set_Arguments(this.get_Arguments().CloneExpressionsOnly());
			stackVariable5.set_VirtualCall(this.get_VirtualCall());
			stackVariable5.set_ConstraintType(this.get_ConstraintType());
			return stackVariable5;
		}

		public override bool Equals(Expression other)
		{
			if (other as MethodInvocationExpression == null)
			{
				return false;
			}
			V_0 = other as MethodInvocationExpression;
			if (!this.get_MethodExpression().Equals(V_0.get_MethodExpression()))
			{
				return false;
			}
			if (this.get_VirtualCall() != V_0.get_VirtualCall() || !this.get_Arguments().Equals(V_0.get_Arguments()))
			{
				return false;
			}
			return (object)this.get_ConstraintType() == (object)V_0.get_ConstraintType();
		}

		public Expression GetTarget()
		{
			if (this.get_MethodExpression().get_MethodDefinition() == null || !this.get_MethodExpression().get_MethodDefinition().get_IsExtensionMethod())
			{
				return this.get_MethodExpression().get_Target();
			}
			if (this.get_Arguments().get_Count() < 1)
			{
				throw new Exception("Extension methods invocations should have at least 1 argument.");
			}
			return this.get_Arguments().get_Item(0);
		}
	}
}