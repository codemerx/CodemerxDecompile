using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class RenameEnumValues : BaseCodeTransformer, IDecompilationStep
	{
		private TypeSystem typeSystem;

		private DecompilationContext context;

		public RenameEnumValues()
		{
		}

		private bool NeedsCast(TypeReference from, TypeReference to)
		{
			TypeDefinition typeDefinition = to.Resolve();
			if (typeDefinition == null || from == null || !typeDefinition.get_IsEnum() || to.get_IsArray())
			{
				return false;
			}
			return from.get_FullName() != to.get_FullName();
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			this.typeSystem = context.MethodContext.Method.get_Module().get_TypeSystem();
			this.context = context;
			return (BlockStatement)this.VisitBlockStatement(block);
		}

		public override ICodeNode VisitBaseCtorExpression(BaseCtorExpression node)
		{
			node = (BaseCtorExpression)base.VisitBaseCtorExpression(node);
			this.VisitInvocationArguments(node.Arguments, node.MethodExpression.Method);
			return node;
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			node = (BinaryExpression)base.VisitBinaryExpression(node);
			if (!node.Left.HasType)
			{
				return node;
			}
			TypeDefinition typeDefinition = node.Left.ExpressionType.Resolve();
			if (typeDefinition == null)
			{
				return node;
			}
			if (!typeDefinition.get_IsEnum() || node.Left.ExpressionType.get_IsArray())
			{
				if (!node.Right.HasType)
				{
					return node;
				}
				TypeDefinition typeDefinition1 = node.Right.ExpressionType.Resolve();
				if (typeDefinition1 == null)
				{
					return node;
				}
				if (typeDefinition1.get_IsEnum() && !node.Right.ExpressionType.get_IsArray())
				{
					if (node.Left is LiteralExpression)
					{
						node.Left = EnumHelper.GetEnumExpression(typeDefinition1, node.Left as LiteralExpression, this.typeSystem);
					}
					else if (node.Left is ExplicitCastExpression && (node.Left as ExplicitCastExpression).Expression is LiteralExpression)
					{
						node.Left = EnumHelper.GetEnumExpression(typeDefinition1, (node.Left as ExplicitCastExpression).Expression as LiteralExpression, this.typeSystem);
					}
				}
			}
			else if (node.Right is LiteralExpression)
			{
				node.Right = EnumHelper.GetEnumExpression(typeDefinition, node.Right as LiteralExpression, this.typeSystem);
			}
			else if (node.Right is ExplicitCastExpression && (node.Right as ExplicitCastExpression).Expression is LiteralExpression)
			{
				node.Right = EnumHelper.GetEnumExpression(typeDefinition, (node.Right as ExplicitCastExpression).Expression as LiteralExpression, this.typeSystem);
			}
			return node;
		}

		public override ICodeNode VisitBoxExpression(BoxExpression node)
		{
			node = (BoxExpression)base.VisitBoxExpression(node);
			TypeDefinition typeDefinition = node.BoxedAs.Resolve();
			if (typeDefinition != null && typeDefinition.get_IsEnum() && !node.BoxedAs.get_IsArray() && node.BoxedExpression is LiteralExpression)
			{
				node.BoxedExpression = EnumHelper.GetEnumExpression(typeDefinition, node.BoxedExpression as LiteralExpression, this.typeSystem);
			}
			return node;
		}

		public override ICodeNode VisitExplicitCastExpression(ExplicitCastExpression node)
		{
			if (node.Expression.CodeNodeType != CodeNodeType.LiteralExpression)
			{
				return base.VisitExplicitCastExpression(node);
			}
			TypeDefinition typeDefinition = node.ExpressionType.Resolve();
			if (typeDefinition == null || !typeDefinition.get_IsEnum() || node.ExpressionType.get_IsArray())
			{
				return node;
			}
			return EnumHelper.GetEnumExpression(node.ExpressionType.Resolve(), node.Expression as LiteralExpression, this.typeSystem);
		}

		private void VisitInvocationArguments(ExpressionCollection arguments, MethodReference method)
		{
			Mono.Collections.Generic.Collection<ParameterDefinition> parameters = method.get_Parameters();
			for (int i = 0; i < arguments.Count; i++)
			{
				TypeReference typeReference = parameters.get_Item(i).ResolveParameterType(method);
				if (this.NeedsCast(arguments[i].ExpressionType, typeReference))
				{
					if (arguments[i].CodeNodeType != CodeNodeType.LiteralExpression)
					{
						arguments[i] = new ExplicitCastExpression(arguments[i], typeReference, null);
					}
					else
					{
						arguments[i] = EnumHelper.GetEnumExpression(typeReference.Resolve(), arguments[i] as LiteralExpression, this.typeSystem);
					}
				}
			}
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			node = (MethodInvocationExpression)base.VisitMethodInvocationExpression(node);
			this.VisitInvocationArguments(node.Arguments, node.MethodExpression.Method);
			if (node.IsConstrained && node.MethodExpression.Target.CodeNodeType == CodeNodeType.LiteralExpression)
			{
				TypeDefinition typeDefinition = node.ConstraintType.Resolve();
				if (typeDefinition.get_IsEnum())
				{
					node.MethodExpression.Target = EnumHelper.GetEnumExpression(typeDefinition, node.MethodExpression.Target as LiteralExpression, this.typeSystem);
				}
			}
			return node;
		}

		public override ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			node = (ObjectCreationExpression)base.VisitObjectCreationExpression(node);
			if (node.Arguments.Count != 0)
			{
				this.VisitInvocationArguments(node.Arguments, node.Constructor);
			}
			return node;
		}

		public override ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			PropertyReferenceExpression propertyReferenceExpression = (PropertyReferenceExpression)base.VisitPropertyReferenceExpression(node);
			if (node.Arguments.Count > 0)
			{
				this.VisitInvocationArguments(propertyReferenceExpression.Arguments, propertyReferenceExpression.MethodExpression.Method);
			}
			return propertyReferenceExpression;
		}

		public override ICodeNode VisitReturnExpression(ReturnExpression node)
		{
			node = (ReturnExpression)base.VisitReturnExpression(node);
			TypeDefinition typeDefinition = this.context.MethodContext.Method.get_ReturnType().Resolve();
			if (typeDefinition == null)
			{
				return node;
			}
			if (node.Value == null)
			{
				return node;
			}
			if (typeDefinition.get_IsEnum() && !this.context.MethodContext.Method.get_ReturnType().get_IsArray() && (node.Value.ExpressionType == null || node.Value.ExpressionType.get_FullName() != typeDefinition.get_FullName()))
			{
				if (node.Value is LiteralExpression)
				{
					node.Value = EnumHelper.GetEnumExpression(typeDefinition, node.Value as LiteralExpression, this.typeSystem);
				}
				else if (node.Value is ExplicitCastExpression && (node.Value as ExplicitCastExpression).Expression is LiteralExpression)
				{
					node.Value = EnumHelper.GetEnumExpression(typeDefinition, (node.Value as ExplicitCastExpression).Expression as LiteralExpression, this.typeSystem);
				}
				else if (node.Value.HasType && this.NeedsCast(node.Value.ExpressionType, typeDefinition))
				{
					node.Value = new ExplicitCastExpression(node.Value, typeDefinition, null);
				}
			}
			return node;
		}

		public override ICodeNode VisitSwitchStatement(SwitchStatement node)
		{
			node.Condition = (Expression)this.Visit(node.Condition);
			foreach (SwitchCase @case in node.Cases)
			{
				if (!(@case is ConditionCase))
				{
					continue;
				}
				ConditionCase explicitCastExpression = @case as ConditionCase;
				if (!this.NeedsCast(explicitCastExpression.Condition.ExpressionType, node.Condition.ExpressionType))
				{
					continue;
				}
				if (!(explicitCastExpression.Condition is LiteralExpression))
				{
					explicitCastExpression.Condition = new ExplicitCastExpression(explicitCastExpression.Condition, node.Condition.ExpressionType, null);
				}
				else
				{
					explicitCastExpression.Condition = EnumHelper.GetEnumExpression(node.Condition.ExpressionType.Resolve(), explicitCastExpression.Condition as LiteralExpression, this.typeSystem);
				}
			}
			node = (SwitchStatement)base.VisitSwitchStatement(node);
			return node;
		}

		public override ICodeNode VisitThisCtorExpression(ThisCtorExpression node)
		{
			node = (ThisCtorExpression)base.VisitThisCtorExpression(node);
			this.VisitInvocationArguments(node.Arguments, node.MethodExpression.Method);
			return node;
		}
	}
}