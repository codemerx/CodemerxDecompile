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
				return Telerik.JustDecompiler.Ast.CodeNodeType.ExplicitCastExpression;
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
				return this.DynamicPositioningFlags != null;
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
				return base.TargetType;
			}
		}

		public MemberReference UnresolvedReferenceForAmbiguousCastToObject
		{
			get;
			internal set;
		}

		public ExplicitCastExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, TypeReference targetType, IEnumerable<Instruction> instructions) : base(expression, targetType, instructions)
		{
			this.DetermineIsChecked();
		}

		public ExplicitCastExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, TypeReference targetType, IEnumerable<Instruction> instructions, MemberReference unresolvedReferenceForAmbiguousCastToObject) : this(expression, targetType, instructions)
		{
			this.UnresolvedReferenceForAmbiguousCastToObject = unresolvedReferenceForAmbiguousCastToObject;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			bool[] flagArray;
			ExplicitCastExpression explicitCastExpression = new ExplicitCastExpression(base.Expression.Clone(), base.TargetType, this.instructions)
			{
				IsChecked = this.IsChecked,
				IsExplicitInterfaceCast = this.IsExplicitInterfaceCast,
				UnresolvedReferenceForAmbiguousCastToObject = this.UnresolvedReferenceForAmbiguousCastToObject
			};
			if (this.DynamicPositioningFlags != null)
			{
				flagArray = (bool[])this.DynamicPositioningFlags.Clone();
			}
			else
			{
				flagArray = null;
			}
			explicitCastExpression.DynamicPositioningFlags = flagArray;
			return explicitCastExpression;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			bool[] flagArray;
			ExplicitCastExpression explicitCastExpression = new ExplicitCastExpression(base.Expression.CloneExpressionOnly(), base.TargetType, null)
			{
				IsChecked = this.IsChecked,
				IsExplicitInterfaceCast = this.IsExplicitInterfaceCast,
				UnresolvedReferenceForAmbiguousCastToObject = this.UnresolvedReferenceForAmbiguousCastToObject
			};
			if (this.DynamicPositioningFlags != null)
			{
				flagArray = (bool[])this.DynamicPositioningFlags.Clone();
			}
			else
			{
				flagArray = null;
			}
			explicitCastExpression.DynamicPositioningFlags = flagArray;
			return explicitCastExpression;
		}

		private void DetermineIsChecked()
		{
			foreach (Instruction instruction in this.instructions)
			{
				Code code = instruction.get_OpCode().get_Code();
				if (code - 127 > 9 && code - 163 > 7 && code - 178 > 1)
				{
					continue;
				}
				this.IsChecked = true;
				return;
			}
			this.IsChecked = false;
		}
	}
}