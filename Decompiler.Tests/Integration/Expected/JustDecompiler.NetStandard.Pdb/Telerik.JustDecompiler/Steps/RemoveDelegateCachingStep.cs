using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RemoveDelegateCachingStep : BaseCodeTransformer, IDecompilationStep
	{
		protected DecompilationContext context;

		protected Dictionary<FieldDefinition, Expression> fieldToReplacingExpressionMap;

		protected Dictionary<VariableReference, Expression> variableToReplacingExpressionMap;

		protected Dictionary<VariableReference, Statement> initializationsToRemove;

		private RemoveDelegateCachingStep.DelegateCachingVersion cachingVersion;

		public RemoveDelegateCachingStep()
		{
			base();
			return;
		}

		private bool CheckFieldCaching(IfStatement theIf)
		{
			V_0 = theIf.get_Condition() as BinaryExpression;
			if (V_0.get_Operator() != 9 || V_0.get_Right().get_CodeNodeType() != 22 || (V_0.get_Right() as LiteralExpression).get_Value() != null)
			{
				return false;
			}
			V_1 = (V_0.get_Left() as FieldReferenceExpression).get_Field().Resolve();
			if (V_1 == null || !V_1.get_IsStatic() || !V_1.get_IsPrivate())
			{
				return false;
			}
			V_2 = (theIf.get_Then().get_Statements().get_Item(0) as ExpressionStatement).get_Expression() as BinaryExpression;
			if (V_2 == null || !V_2.get_IsAssignmentExpression() || V_2.get_Left().get_CodeNodeType() != 30 || (object)(V_2.get_Left() as FieldReferenceExpression).get_Field().Resolve() != (object)V_1)
			{
				return false;
			}
			if (this.fieldToReplacingExpressionMap.ContainsKey(V_1))
			{
				throw new Exception("A caching field cannot be assigned more than once.");
			}
			if (!V_1.IsCompilerGenerated(true))
			{
				return false;
			}
			V_3 = V_1.get_FieldType().Resolve();
			if (V_3 == null || V_3.get_BaseType() == null || String.op_Inequality(V_3.get_BaseType().get_FullName(), "System.MulticastDelegate"))
			{
				return false;
			}
			this.fieldToReplacingExpressionMap.set_Item(V_1, V_2.get_Right());
			return true;
		}

		private bool CheckIfStatement(IfStatement theIf)
		{
			if (!this.CheckIfStatementStructure(theIf))
			{
				return false;
			}
			V_0 = theIf.get_Condition() as BinaryExpression;
			if (V_0.get_Left().get_CodeNodeType() == 30)
			{
				return this.CheckFieldCaching(theIf);
			}
			if (V_0.get_Left().get_CodeNodeType() != 26)
			{
				return false;
			}
			return this.CheckVariableCaching(theIf);
		}

		private bool CheckIfStatementStructure(IfStatement theIf)
		{
			if (theIf.get_Else() == null && theIf.get_Condition().get_CodeNodeType() == 24)
			{
				if (theIf.get_Then().get_Statements().get_Count() == 1 && theIf.get_Then().get_Statements().get_Item(0).get_CodeNodeType() == 5)
				{
					this.cachingVersion = 0;
					return true;
				}
				if (theIf.get_Then().get_Statements().get_Count() == 3)
				{
					V_0 = theIf.get_Then().get_Statements().get_Item(0) as ExpressionStatement;
					if (V_0 == null)
					{
						return false;
					}
					V_1 = V_0.get_Expression() as BinaryExpression;
					if (V_1 == null)
					{
						return false;
					}
					if (!V_1.get_IsAssignmentExpression() || V_1.get_Left().get_CodeNodeType() != 26 || V_1.get_Right().get_CodeNodeType() != 26 || !this.initializationsToRemove.ContainsKey((V_1.get_Right() as VariableReferenceExpression).get_Variable()))
					{
						return false;
					}
					if (theIf.get_Then().get_Statements().get_Item(1).get_CodeNodeType() != 5)
					{
						return false;
					}
					V_2 = theIf.get_Then().get_Statements().get_Item(2) as ExpressionStatement;
					if (V_2 == null)
					{
						return false;
					}
					V_3 = V_2.get_Expression() as BinaryExpression;
					if (V_3 == null)
					{
						return false;
					}
					if (!V_3.get_IsAssignmentExpression() || V_3.get_Left().get_CodeNodeType() != 30 || V_3.get_Right().get_CodeNodeType() != 26 || !this.initializationsToRemove.ContainsKey((V_3.get_Right() as VariableReferenceExpression).get_Variable()))
					{
						return false;
					}
					this.cachingVersion = 1;
					return true;
				}
			}
			return false;
		}

		private bool CheckVariableCaching(IfStatement theIf)
		{
			V_0 = theIf.get_Condition() as BinaryExpression;
			if (V_0.get_Operator() != 9 || V_0.get_Right().get_CodeNodeType() != 22 || (V_0.get_Right() as LiteralExpression).get_Value() != null)
			{
				return false;
			}
			V_1 = (V_0.get_Left() as VariableReferenceExpression).get_Variable();
			if (!this.initializationsToRemove.ContainsKey(V_1))
			{
				return false;
			}
			if (this.cachingVersion == RemoveDelegateCachingStep.DelegateCachingVersion.V1)
			{
				stackVariable25 = 0;
			}
			else
			{
				stackVariable25 = 1;
			}
			V_2 = stackVariable25;
			V_3 = (theIf.get_Then().get_Statements().get_Item(V_2) as ExpressionStatement).get_Expression() as BinaryExpression;
			if (V_3 == null || !V_3.get_IsAssignmentExpression() || V_3.get_Left().get_CodeNodeType() != 26 || (object)(V_3.get_Left() as VariableReferenceExpression).get_Variable() != (object)V_1)
			{
				return false;
			}
			if (this.variableToReplacingExpressionMap.ContainsKey(V_1))
			{
				throw new Exception("A caching variable cannot be assigned more than once.");
			}
			V_4 = V_1.get_VariableType().Resolve();
			if (V_4 == null || V_4.get_BaseType() == null || String.op_Inequality(V_4.get_BaseType().get_FullName(), "System.MulticastDelegate"))
			{
				return false;
			}
			this.variableToReplacingExpressionMap.set_Item(V_1, V_3.get_Right());
			return true;
		}

		private bool CheckVariableInitialization(ExpressionStatement node)
		{
			if (!node.IsAssignmentStatement())
			{
				return false;
			}
			V_0 = node.get_Expression() as BinaryExpression;
			if (V_0.get_Left().get_CodeNodeType() != 26)
			{
				return false;
			}
			V_1 = V_0.get_Right();
			if (V_1.get_CodeNodeType() == 31)
			{
				V_1 = (V_1 as ExplicitCastExpression).get_Expression();
			}
			if (V_1.get_CodeNodeType() != 22 || (V_1 as LiteralExpression).get_Value() != null && V_1.get_CodeNodeType() != 30)
			{
				return false;
			}
			if (V_1.get_CodeNodeType() == 30)
			{
				V_2 = V_1 as FieldReferenceExpression;
				V_3 = V_2.get_ExpressionType().Resolve();
				if (V_3 == null || V_3.get_BaseType() == null || String.op_Inequality(V_3.get_BaseType().get_FullName(), "System.MulticastDelegate"))
				{
					return false;
				}
				V_4 = V_2.get_Field().Resolve();
				if ((object)V_4.get_DeclaringType() != (object)this.context.get_MethodContext().get_Method().get_DeclaringType() && !V_4.get_DeclaringType().IsNestedIn(this.context.get_MethodContext().get_Method().get_DeclaringType()) || !V_4.get_DeclaringType().IsCompilerGenerated())
				{
					return false;
				}
			}
			this.initializationsToRemove.set_Item((V_0.get_Left() as VariableReferenceExpression).get_Variable(), node);
			return true;
		}

		protected virtual ICodeNode GetIfSubstitution(IfStatement node)
		{
			return null;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.fieldToReplacingExpressionMap = new Dictionary<FieldDefinition, Expression>();
			this.variableToReplacingExpressionMap = new Dictionary<VariableReference, Expression>();
			this.initializationsToRemove = new Dictionary<VariableReference, Statement>();
			stackVariable11 = (BlockStatement)this.Visit(body);
			this.ProcessInitializations();
			return stackVariable11;
		}

		protected virtual void ProcessInitializations()
		{
			this.RemoveInitializations();
			return;
		}

		protected void RemoveInitializations()
		{
			V_0 = this.initializationsToRemove.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!this.variableToReplacingExpressionMap.ContainsKey(V_1.get_Key()))
					{
						continue;
					}
					stackVariable15 = V_1.get_Value().get_Parent() as BlockStatement;
					if (stackVariable15 == null)
					{
						throw new Exception("Invalid parent statement.");
					}
					dummyVar0 = this.context.get_MethodContext().get_Variables().Remove(V_1.get_Key().Resolve());
					dummyVar1 = stackVariable15.get_Statements().Remove(V_1.get_Value());
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			if (node.get_IsAssignmentExpression())
			{
				if (node.get_Left().get_CodeNodeType() != 30)
				{
					if (node.get_Left().get_CodeNodeType() == 26 && this.variableToReplacingExpressionMap.ContainsKey((node.get_Left() as VariableReferenceExpression).get_Variable()))
					{
						throw new Exception("A caching variable cannot be assigned more than once.");
					}
				}
				else
				{
					V_0 = (node.get_Left() as FieldReferenceExpression).get_Field().Resolve();
					if (V_0 != null && this.fieldToReplacingExpressionMap.ContainsKey(V_0))
					{
						throw new Exception("A caching field cannot be assigned more than once.");
					}
				}
			}
			return this.VisitBinaryExpression(node);
		}

		public override ICodeNode VisitExpressionStatement(ExpressionStatement node)
		{
			if (this.CheckVariableInitialization(node))
			{
				return node;
			}
			return this.VisitExpressionStatement(node);
		}

		public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			V_1 = node.get_Field().Resolve();
			if (V_1 == null || !this.fieldToReplacingExpressionMap.TryGetValue(V_1, out V_0))
			{
				return this.VisitFieldReferenceExpression(node);
			}
			return V_0.CloneExpressionOnlyAndAttachInstructions(node.get_UnderlyingSameMethodInstructions());
		}

		public override ICodeNode VisitIfStatement(IfStatement node)
		{
			if (this.CheckIfStatement(node))
			{
				return this.GetIfSubstitution(node);
			}
			return this.VisitIfStatement(node);
		}

		public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if (!this.variableToReplacingExpressionMap.TryGetValue(node.get_Variable(), out V_0))
			{
				return this.VisitVariableReferenceExpression(node);
			}
			return V_0.CloneExpressionOnlyAndAttachInstructions(node.get_UnderlyingSameMethodInstructions());
		}

		private enum DelegateCachingVersion
		{
			V1,
			V2
		}
	}
}