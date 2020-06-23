using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler
{
	public class DependsOnAnalysisVisitor : BaseCodeVisitor
	{
		public HashSet<ExplicitCastExpression> AmbiguousCastsToObject
		{
			get;
			private set;
		}

		public HashSet<TypeReference> TypesDependingOn
		{
			get;
			private set;
		}

		public DependsOnAnalysisVisitor(HashSet<TypeReference> typesDependingOn, HashSet<ExplicitCastExpression> ambiguousCastsToObject)
		{
			this.TypesDependingOn = typesDependingOn;
			this.AmbiguousCastsToObject = ambiguousCastsToObject;
		}

		public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpression node)
		{
			this.TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.ExpressionType));
			base.VisitAnonymousObjectCreationExpression(node);
		}

		public override void VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			this.TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.ElementType));
			base.VisitArrayCreationExpression(node);
		}

		public override void VisitBaseCtorExpression(BaseCtorExpression node)
		{
			this.Visit(node.Arguments);
		}

		public override void VisitCanCastExpression(CanCastExpression node)
		{
			this.TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.TargetType));
			base.VisitCanCastExpression(node);
		}

		public override void VisitEnumExpression(EnumExpression node)
		{
			this.TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.ExpressionType));
			base.VisitEnumExpression(node);
		}

		public override void VisitExplicitCastExpression(ExplicitCastExpression node)
		{
			this.TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.ExpressionType));
			if (node.UnresolvedReferenceForAmbiguousCastToObject != null)
			{
				this.AmbiguousCastsToObject.Add(node);
			}
			base.VisitExplicitCastExpression(node);
		}

		public override void VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			this.TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.Field.DeclaringType));
			base.VisitFieldReferenceExpression(node);
		}

		public override void VisitMethodReferenceExpression(MethodReferenceExpression node)
		{
			this.TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.Method.DeclaringType));
			this.Visit(node.Target);
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			this.TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.ExpressionType));
			base.VisitObjectCreationExpression(node);
		}

		public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			this.TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.Property.DeclaringType));
			base.VisitPropertyReferenceExpression(node);
		}

		public override void VisitSafeCastExpression(SafeCastExpression node)
		{
			this.TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.ExpressionType));
			base.VisitSafeCastExpression(node);
		}

		public override void VisitThisCtorExpression(ThisCtorExpression node)
		{
			this.Visit(node.Arguments);
		}

		public override void VisitTypeOfExpression(TypeOfExpression node)
		{
			this.TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.Type));
			base.VisitTypeOfExpression(node);
		}

		public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			this.TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.Variable.VariableType));
			base.VisitVariableDeclarationExpression(node);
		}
	}
}