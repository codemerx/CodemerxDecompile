using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class ExplicitCastExpression : CastExpressionBase, IDynamicTypeContainer
	{
		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 31;
			}
		}

		public bool[] DynamicPositioningFlags
		{
			get;
			set;
		}

		public bool IsChecked
		{
			get;
			private set;
		}

		public bool IsDynamic
		{
			get
			{
				return this.get_DynamicPositioningFlags() != null;
			}
		}

		internal bool IsExplicitInterfaceCast
		{
			get;
			set;
		}

		TypeReference Mono.Cecil.Cil.IDynamicTypeContainer.DynamicContainingType
		{
			get
			{
				return this.get_TargetType();
			}
		}

		public MemberReference UnresolvedReferenceForAmbiguousCastToObject
		{
			get;
			internal set;
		}

		public ExplicitCastExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, TypeReference targetType, IEnumerable<Instruction> instructions)
		{
			base(expression, targetType, instructions);
			this.DetermineIsChecked();
			return;
		}

		public ExplicitCastExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, TypeReference targetType, IEnumerable<Instruction> instructions, MemberReference unresolvedReferenceForAmbiguousCastToObject)
		{
			this(expression, targetType, instructions);
			this.set_UnresolvedReferenceForAmbiguousCastToObject(unresolvedReferenceForAmbiguousCastToObject);
			return;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			stackVariable7 = new ExplicitCastExpression(this.get_Expression().Clone(), this.get_TargetType(), this.instructions);
			stackVariable7.set_IsChecked(this.get_IsChecked());
			stackVariable7.set_IsExplicitInterfaceCast(this.get_IsExplicitInterfaceCast());
			stackVariable7.set_UnresolvedReferenceForAmbiguousCastToObject(this.get_UnresolvedReferenceForAmbiguousCastToObject());
			if (this.get_DynamicPositioningFlags() != null)
			{
				stackVariable19 = (bool[])this.get_DynamicPositioningFlags().Clone();
			}
			else
			{
				stackVariable19 = null;
			}
			stackVariable7.set_DynamicPositioningFlags(stackVariable19);
			return stackVariable7;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			stackVariable6 = new ExplicitCastExpression(this.get_Expression().CloneExpressionOnly(), this.get_TargetType(), null);
			stackVariable6.set_IsChecked(this.get_IsChecked());
			stackVariable6.set_IsExplicitInterfaceCast(this.get_IsExplicitInterfaceCast());
			stackVariable6.set_UnresolvedReferenceForAmbiguousCastToObject(this.get_UnresolvedReferenceForAmbiguousCastToObject());
			if (this.get_DynamicPositioningFlags() != null)
			{
				stackVariable18 = (bool[])this.get_DynamicPositioningFlags().Clone();
			}
			else
			{
				stackVariable18 = null;
			}
			stackVariable6.set_DynamicPositioningFlags(stackVariable18);
			return stackVariable6;
		}

		private void DetermineIsChecked()
		{
			V_0 = this.instructions.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_2 = V_0.get_Current().get_OpCode();
					V_1 = V_2.get_Code();
					if (V_1 - 127 > 9 && V_1 - 163 > 7 && V_1 - 178 > 1)
					{
						continue;
					}
					this.set_IsChecked(true);
					goto Label0;
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			this.set_IsChecked(false);
		Label0:
			return;
		}
	}
}