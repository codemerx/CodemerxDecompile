using System;
using System.Linq;
using Telerik.JustDecompiler.Ast;
using System.Collections.Generic;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler
{
    /// <summary>
    /// This visitor makes DependsOnAnalysis (needed for constucting the using statements) and find all CastExpressions which have unresolved references up the inheritance chain that we need to find out whether == or != is overloaded.
    /// </summary>
	public class DependsOnAnalysisVisitor : BaseCodeVisitor
	{
		public HashSet<TypeReference> TypesDependingOn { get; private set; }

        public HashSet<ExplicitCastExpression> AmbiguousCastsToObject { get; private set; }

        public DependsOnAnalysisVisitor(HashSet<TypeReference> typesDependingOn, HashSet<ExplicitCastExpression> ambiguousCastsToObject)
		{
			this.TypesDependingOn = typesDependingOn;
            this.AmbiguousCastsToObject = ambiguousCastsToObject;
		}

		public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.Variable.VariableType));
			base.VisitVariableDeclarationExpression(node);
		}

		public override void VisitEnumExpression(EnumExpression node)
		{
			TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.ExpressionType));
			base.VisitEnumExpression(node);
		}

		public override void VisitTypeOfExpression(TypeOfExpression node)
		{
			TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.Type));
			base.VisitTypeOfExpression(node);
		}

		public override void VisitExplicitCastExpression(ExplicitCastExpression node)
		{
			TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.ExpressionType));
            if (node.UnresolvedReferenceForAmbiguousCastToObject != null)
            {
                this.AmbiguousCastsToObject.Add(node);
            }

            base.VisitExplicitCastExpression(node);
		}

		public override void VisitSafeCastExpression(SafeCastExpression node)
		{
			TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.ExpressionType));
			base.VisitSafeCastExpression(node);
		}

		public override void VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.Field.DeclaringType));
			base.VisitFieldReferenceExpression(node);
		}

		public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.Property.DeclaringType));
			base.VisitPropertyReferenceExpression(node);
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.ExpressionType));
			base.VisitObjectCreationExpression(node);
		}

		public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpression node)
		{
			TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.ExpressionType));
			base.VisitAnonymousObjectCreationExpression(node);
		}

		public override void VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.ElementType));
			base.VisitArrayCreationExpression(node);
		}

		public override void VisitCanCastExpression(CanCastExpression node)
		{
			TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.TargetType));
			base.VisitCanCastExpression(node);
		}

		public override void VisitMethodReferenceExpression(MethodReferenceExpression node)
		{
			TypesDependingOn.UnionWith(Utilities.GetTypeReferenceTypesDepedningOn(node.Method.DeclaringType));
			Visit(node.Target);
		}

		public override void VisitBaseCtorExpression(BaseCtorExpression node)
		{
			Visit(node.Arguments);
		}

		public override void VisitThisCtorExpression(ThisCtorExpression node)
		{
			Visit(node.Arguments);
		}
	}
}
