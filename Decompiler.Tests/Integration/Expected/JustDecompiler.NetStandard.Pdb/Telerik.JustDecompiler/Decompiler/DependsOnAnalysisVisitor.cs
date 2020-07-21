using Mono.Cecil;
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
			base();
			this.set_TypesDependingOn(typesDependingOn);
			this.set_AmbiguousCastsToObject(ambiguousCastsToObject);
			return;
		}

		public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpression node)
		{
			this.get_TypesDependingOn().UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.get_ExpressionType()));
			this.VisitAnonymousObjectCreationExpression(node);
			return;
		}

		public override void VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			this.get_TypesDependingOn().UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.get_ElementType()));
			this.VisitArrayCreationExpression(node);
			return;
		}

		public override void VisitBaseCtorExpression(BaseCtorExpression node)
		{
			this.Visit(node.get_Arguments());
			return;
		}

		public override void VisitCanCastExpression(CanCastExpression node)
		{
			this.get_TypesDependingOn().UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.get_TargetType()));
			this.VisitCanCastExpression(node);
			return;
		}

		public override void VisitEnumExpression(EnumExpression node)
		{
			this.get_TypesDependingOn().UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.get_ExpressionType()));
			this.VisitEnumExpression(node);
			return;
		}

		public override void VisitExplicitCastExpression(ExplicitCastExpression node)
		{
			this.get_TypesDependingOn().UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.get_ExpressionType()));
			if (node.get_UnresolvedReferenceForAmbiguousCastToObject() != null)
			{
				dummyVar0 = this.get_AmbiguousCastsToObject().Add(node);
			}
			this.VisitExplicitCastExpression(node);
			return;
		}

		public override void VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			this.get_TypesDependingOn().UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.get_Field().get_DeclaringType()));
			this.VisitFieldReferenceExpression(node);
			return;
		}

		public override void VisitMethodReferenceExpression(MethodReferenceExpression node)
		{
			this.get_TypesDependingOn().UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.get_Method().get_DeclaringType()));
			this.Visit(node.get_Target());
			return;
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			this.get_TypesDependingOn().UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.get_ExpressionType()));
			this.VisitObjectCreationExpression(node);
			return;
		}

		public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			this.get_TypesDependingOn().UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.get_Property().get_DeclaringType()));
			this.VisitPropertyReferenceExpression(node);
			return;
		}

		public override void VisitSafeCastExpression(SafeCastExpression node)
		{
			this.get_TypesDependingOn().UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.get_ExpressionType()));
			this.VisitSafeCastExpression(node);
			return;
		}

		public override void VisitThisCtorExpression(ThisCtorExpression node)
		{
			this.Visit(node.get_Arguments());
			return;
		}

		public override void VisitTypeOfExpression(TypeOfExpression node)
		{
			this.get_TypesDependingOn().UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.get_Type()));
			this.VisitTypeOfExpression(node);
			return;
		}

		public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			this.get_TypesDependingOn().UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.get_Variable().get_VariableType()));
			this.VisitVariableDeclarationExpression(node);
			return;
		}
	}
}