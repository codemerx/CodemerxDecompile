using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildAnonymousDelegatesStep : BaseCodeVisitor, IDecompilationStep
	{
		private RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder theRebuilder;

		public RebuildAnonymousDelegatesStep()
		{
			base();
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.theRebuilder = new RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder(context, body);
			this.VisitBlockStatement(body);
			this.theRebuilder.CleanUpVariableCopyAssignments();
			return body;
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			V_0 = 0;
			while (V_0 < node.get_Statements().get_Count() - 1)
			{
				this.theRebuilder.Match(node, V_0);
				V_0 = V_0 + 1;
			}
			this.VisitBlockStatement(node);
			return;
		}

		private class AnonymousDelegateRebuilder : BaseCodeTransformer
		{
			private VariableReference delegateVariableReference;

			private readonly DecompilationContext context;

			private Dictionary<FieldDefinition, Expression> fieldDefToAssignedValueMap;

			private readonly BlockStatement methodBodyBlock;

			private RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder.State state;

			private int startIndex;

			private readonly RebuildAnonymousDelegatesStep.FieldReferenceVisitor fieldVisitor;

			private TypeDefinition delegateTypeDef;

			private HashSet<Statement> statementsToKeep;

			private readonly RebuildAnonymousDelegatesStep.VariableCopyFinder variableCopyFinder;

			private HashSet<VariableReference> delegateCopies;

			private readonly HashSet<Statement> assignmentsToRemove;

			private readonly Dictionary<VariableReference, Dictionary<FieldDefinition, Expression>> closuresArchive;

			private List<BlockStatement> delegatesFound;

			public AnonymousDelegateRebuilder(DecompilationContext context, BlockStatement methodBodyBlock)
			{
				base();
				this.context = context;
				this.methodBodyBlock = methodBodyBlock;
				if (context.get_MethodContext().get_FieldToExpression() == null)
				{
					context.get_MethodContext().set_FieldToExpression(new Dictionary<FieldDefinition, Expression>());
				}
				this.fieldDefToAssignedValueMap = context.get_MethodContext().get_FieldToExpression();
				this.fieldVisitor = new RebuildAnonymousDelegatesStep.FieldReferenceVisitor(methodBodyBlock);
				this.variableCopyFinder = new RebuildAnonymousDelegatesStep.VariableCopyFinder(methodBodyBlock);
				this.assignmentsToRemove = new HashSet<Statement>();
				this.closuresArchive = context.get_MethodContext().get_ClosureVariableToFieldValue();
				return;
			}

			private bool CheckAssignExpression(ExpressionStatement theStatement)
			{
				if (theStatement == null || theStatement.get_Expression().get_CodeNodeType() != 24)
				{
					return false;
				}
				V_0 = theStatement.get_Expression() as BinaryExpression;
				if (!V_0.get_IsAssignmentExpression())
				{
					return false;
				}
				V_1 = this.state;
				if (V_1 == RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder.State.DelegateCreate)
				{
					return this.CheckDelegateCreation(V_0);
				}
				if (V_1 != 1)
				{
					return false;
				}
				return this.CheckFieldAssignment(V_0, theStatement);
			}

			private bool CheckDelegateCreation(BinaryExpression theAssignExpression)
			{
				if (theAssignExpression.get_Left().get_CodeNodeType() != 26 || theAssignExpression.get_Right().get_CodeNodeType() != 40)
				{
					return false;
				}
				V_0 = theAssignExpression.get_Right() as ObjectCreationExpression;
				if (V_0.get_Arguments().get_Count() != 0)
				{
					return false;
				}
				V_1 = V_0.get_ExpressionType().Resolve();
				if (V_1 == null || !this.CheckTypeForCompilerGeneratedAttribute(V_1))
				{
					return false;
				}
				this.delegateTypeDef = V_1;
				this.delegateVariableReference = (theAssignExpression.get_Left() as VariableReferenceExpression).get_Variable();
				return true;
			}

			private bool CheckFieldAssignment(BinaryExpression theAssignExpression, ExpressionStatement theStatement)
			{
				if (theAssignExpression.get_Left().get_CodeNodeType() != 30)
				{
					return false;
				}
				V_0 = theAssignExpression.get_Left() as FieldReferenceExpression;
				if (V_0.get_Target() == null || V_0.get_Target().get_CodeNodeType() != 26 || (object)(V_0.get_Target() as VariableReferenceExpression).get_Variable() != (object)this.delegateVariableReference)
				{
					return false;
				}
				V_1 = V_0.get_Field().Resolve();
				if (theAssignExpression.get_Right().get_CodeNodeType() == 28 || theAssignExpression.get_Right().get_CodeNodeType() == 25 || this.IsClosureVariableReference(theAssignExpression.get_Right() as VariableReferenceExpression) && !this.fieldVisitor.CheckForAnotherAssignment(V_1, V_0, this.delegateCopies))
				{
					this.fieldDefToAssignedValueMap.set_Item(V_1, theAssignExpression.get_Right());
				}
				else
				{
					dummyVar0 = this.statementsToKeep.Add(theStatement);
				}
				return true;
			}

			private bool CheckTypeForCompilerGeneratedAttribute(TypeDefinition typeDefinition)
			{
				while (typeDefinition.get_IsNested())
				{
					if (typeDefinition.HasCompilerGeneratedAttribute())
					{
						return true;
					}
					typeDefinition = typeDefinition.get_DeclaringType();
				}
				return false;
			}

			public void CleanUpVariableCopyAssignments()
			{
				V_0 = (new List<Statement>(this.assignmentsToRemove)).GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						V_2 = V_1.get_Parent() as BlockStatement;
						if (V_2 == null)
						{
							continue;
						}
						dummyVar0 = V_2.get_Statements().Remove(V_1);
					}
				}
				finally
				{
					((IDisposable)V_0).Dispose();
				}
				return;
			}

			private bool IsClosureVariableReference(VariableReferenceExpression varRefExpression)
			{
				if (varRefExpression == null)
				{
					return false;
				}
				return this.closuresArchive.ContainsKey(varRefExpression.get_Variable());
			}

			private void MapTheRestOfTheFieldsToVariables()
			{
				V_0 = this.delegateTypeDef.get_Fields().GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						if (this.fieldDefToAssignedValueMap.ContainsKey(V_1))
						{
							continue;
						}
						stackVariable15 = this.context.get_MethodContext();
						V_3 = stackVariable15.get_LambdaVariablesCount();
						stackVariable15.set_LambdaVariablesCount(V_3 + 1);
						V_2 = new VariableDefinition(String.Concat("lambdaVar", V_3.ToString()), V_1.get_FieldType(), this.context.get_MethodContext().get_Method());
						this.context.get_MethodContext().get_Variables().Add(V_2);
						dummyVar0 = this.context.get_MethodContext().get_VariablesToRename().Add(V_2);
						this.fieldDefToAssignedValueMap.set_Item(V_1, new VariableReferenceExpression(V_2, null));
					}
				}
				finally
				{
					V_0.Dispose();
				}
				return;
			}

			public void Match(BlockStatement theBlock, int index)
			{
				this.delegatesFound = new List<BlockStatement>();
				this.statementsToKeep = new HashSet<Statement>();
				this.delegateCopies = new HashSet<VariableReference>();
				this.fieldDefToAssignedValueMap = new Dictionary<FieldDefinition, Expression>();
				this.state = 0;
				if (!this.CheckAssignExpression(theBlock.get_Statements().get_Item(index) as ExpressionStatement))
				{
					return;
				}
				dummyVar0 = this.delegateCopies.Add(this.delegateVariableReference);
				this.variableCopyFinder.FindCopiesOfDelegate(this.delegateCopies, this.assignmentsToRemove);
				this.startIndex = index;
				this.state = 1;
				do
				{
					stackVariable34 = index + 1;
					index = stackVariable34;
				}
				while (stackVariable34 < theBlock.get_Statements().get_Count() && this.CheckAssignExpression(theBlock.get_Statements().get_Item(index) as ExpressionStatement));
				if (index == theBlock.get_Statements().get_Count())
				{
					return;
				}
				this.MapTheRestOfTheFieldsToVariables();
				this.RemoveFieldAssignments(theBlock.get_Statements(), index);
				this.CleanUpVariableCopyAssignments();
				this.state = 3;
				dummyVar1 = this.VisitBlockStatement(this.methodBodyBlock);
				this.state = 2;
				V_0 = this.startIndex;
				while (V_0 < theBlock.get_Statements().get_Count())
				{
					theBlock.get_Statements().set_Item(V_0, (Statement)this.Visit(theBlock.get_Statements().get_Item(V_0)));
					this.state = 3;
					V_1 = this.delegatesFound.GetEnumerator();
					try
					{
						while (V_1.MoveNext())
						{
							V_2 = V_1.get_Current();
							dummyVar2 = this.VisitBlockStatement(V_2);
						}
					}
					finally
					{
						((IDisposable)V_1).Dispose();
					}
					this.state = 2;
					this.delegatesFound = new List<BlockStatement>();
					V_0 = V_0 + 1;
				}
				this.SaveClosureToArchive();
				return;
			}

			private void RemoveFieldAssignments(StatementCollection theCollection, int currentIndex)
			{
				V_0 = this.startIndex;
				while (V_0 < currentIndex)
				{
					if (this.statementsToKeep.Contains(theCollection.get_Item(V_0)))
					{
						V_1 = this.startIndex;
						this.startIndex = V_1 + 1;
						theCollection.set_Item(V_1, theCollection.get_Item(V_0));
					}
					V_0 = V_0 + 1;
				}
				this.RemoveRange(theCollection, this.startIndex, currentIndex);
				return;
			}

			private void RemoveRange(StatementCollection theCollection, int from, int to)
			{
				while (to < theCollection.get_Count())
				{
					theCollection.set_Item(from, theCollection.get_Item(to));
					from = from + 1;
					to = to + 1;
				}
				V_0 = theCollection.get_Count();
				while (from < V_0)
				{
					stackVariable21 = V_0 - 1;
					V_0 = stackVariable21;
					theCollection.RemoveAt(stackVariable21);
				}
				return;
			}

			private void SaveClosureToArchive()
			{
				V_0 = this.delegateCopies.GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						this.closuresArchive.Add(V_1, this.fieldDefToAssignedValueMap);
					}
				}
				finally
				{
					((IDisposable)V_0).Dispose();
				}
				return;
			}

			public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
			{
				if (this.state != 3)
				{
					return this.VisitFieldReferenceExpression(node);
				}
				V_1 = node.get_Field().Resolve();
				if (V_1 == null)
				{
					return this.VisitFieldReferenceExpression(node);
				}
				if (this.fieldDefToAssignedValueMap.TryGetValue(V_1, out V_0))
				{
					return V_0.CloneExpressionOnlyAndAttachInstructions(node.get_UnderlyingSameMethodInstructions());
				}
				dummyVar0 = this.VisitFieldReferenceExpression(node);
				if (node.get_Target() == null || node.get_Target().get_CodeNodeType() != 26)
				{
					return node;
				}
				V_2 = (node.get_Target() as VariableReferenceExpression).get_Variable();
				if (!this.closuresArchive.TryGetValue(V_2, out V_3) || !V_3.TryGetValue(V_1, out V_0))
				{
					return node;
				}
				return V_0.CloneExpressionOnlyAndAttachInstructions(node.get_UnderlyingSameMethodInstructions());
			}

			public override ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
			{
				if (this.state != 2 || node.get_Arguments() == null || node.get_Arguments().get_Count() != 2 || node.get_Arguments().get_Item(0).get_CodeNodeType() != 26 || node.get_Arguments().get_Item(1).get_CodeNodeType() != 20 || !this.delegateCopies.Contains((node.get_Arguments().get_Item(0) as VariableReferenceExpression).get_Variable()))
				{
					return this.VisitObjectCreationExpression(node);
				}
				V_0 = node.get_Constructor().get_DeclaringType().Resolve();
				if (V_0 == null || V_0.get_BaseType() == null || String.op_Inequality(V_0.get_BaseType().get_FullName(), "System.MulticastDelegate"))
				{
					return this.VisitObjectCreationExpression(node);
				}
				V_1 = (node.get_Arguments().get_Item(1) as MethodReferenceExpression).get_Method();
				V_2 = (node.get_Arguments().get_Item(1) as MethodReferenceExpression).get_MethodDefinition();
				stackVariable62 = new MethodSpecificContext(V_2.get_Body());
				V_3 = new DecompilationContext(stackVariable62, this.context.get_TypeContext(), this.context.get_ModuleContext(), this.context.get_AssemblyContext(), this.context.get_Language());
				stackVariable62.set_FieldToExpression(this.fieldDefToAssignedValueMap);
				V_4 = V_2.get_Body().DecompileLambda(this.context.get_Language(), V_3);
				if (V_4.get_Statements().get_Count() == 1 && V_4.get_Statements().get_Item(0).get_CodeNodeType() == 5 && (V_4.get_Statements().get_Item(0) as ExpressionStatement).get_Expression().get_CodeNodeType() == 57)
				{
					V_8 = (V_4.get_Statements().get_Item(0) as ExpressionStatement).get_Expression() as ReturnExpression;
					V_9 = new ShortFormReturnExpression(V_8.get_Value(), V_8.get_MappedInstructions());
					V_4 = new BlockStatement();
					V_4.get_Statements().Add(new ExpressionStatement(V_9));
				}
				this.context.get_MethodContext().get_VariableDefinitionToNameMap().AddRange<VariableDefinition, string>(V_3.get_MethodContext().get_VariableDefinitionToNameMap());
				this.context.get_MethodContext().get_VariableNamesCollection().UnionWith(V_3.get_MethodContext().get_VariableNamesCollection());
				this.context.get_MethodContext().AddInnerMethodParametersToContext(V_3.get_MethodContext());
				this.context.get_MethodContext().get_GotoStatements().AddRange(V_3.get_MethodContext().get_GotoStatements());
				this.context.get_MethodContext().get_GotoLabels().AddRange<string, Statement>(V_3.get_MethodContext().get_GotoLabels());
				V_5 = new ExpressionCollection();
				V_6 = LambdaExpressionsHelper.HasAnonymousParameter(V_2.get_Parameters());
				V_10 = V_2.get_Parameters().GetEnumerator();
				try
				{
					while (V_10.MoveNext())
					{
						V_11 = V_10.get_Current();
						V_5.Add(new LambdaParameterExpression(V_11, !V_6, null));
					}
				}
				finally
				{
					V_10.Dispose();
				}
				this.delegatesFound.Add(V_4);
				stackVariable157 = new LambdaExpression(V_5, V_4, V_2.IsAsync(), V_2.IsFunction(), V_1.get_Parameters(), false, node.get_Arguments().get_Item(1).get_MappedInstructions());
				stackVariable157.set_ExpressionType(V_0);
				V_7 = stackVariable157;
				return new DelegateCreationExpression(node.get_Constructor().get_DeclaringType(), V_7, node.get_Arguments().get_Item(0), node.get_MappedInstructions());
			}

			private enum State
			{
				DelegateCreate,
				FieldAssign,
				ReplaceDelegate,
				ReplaceFields
			}
		}

		private class FieldReferenceVisitor : BaseCodeVisitor
		{
			private readonly BlockStatement theBlockStatement;

			private FieldDefinition fieldDef;

			private FieldReferenceExpression assignedReference;

			private bool foundUsage;

			private HashSet<VariableReference> delegateVariableCopies;

			public FieldReferenceVisitor(BlockStatement theBlockStatement)
			{
				base();
				this.theBlockStatement = theBlockStatement;
				return;
			}

			private void CheckFieldReference(FieldReferenceExpression node)
			{
				if (node != this.assignedReference && node.get_Target() != null && node.get_Target().get_CodeNodeType() == 26 && this.delegateVariableCopies.Contains((node.get_Target() as VariableReferenceExpression).get_Variable()) && (object)node.get_Field().Resolve() == (object)this.fieldDef)
				{
					this.foundUsage = true;
				}
				return;
			}

			public bool CheckForAnotherAssignment(FieldDefinition fieldDef, FieldReferenceExpression assignedReference, HashSet<VariableReference> delegateVariableCopies)
			{
				this.fieldDef = fieldDef;
				this.assignedReference = assignedReference;
				this.delegateVariableCopies = delegateVariableCopies;
				this.foundUsage = false;
				this.Visit(this.theBlockStatement);
				return this.foundUsage;
			}

			public override void Visit(ICodeNode node)
			{
				if (!this.foundUsage)
				{
					this.Visit(node);
				}
				return;
			}

			public override void VisitBinaryExpression(BinaryExpression node)
			{
				if (this.foundUsage)
				{
					return;
				}
				if (node.get_IsAssignmentExpression() && node.get_Left().get_CodeNodeType() == 30)
				{
					this.CheckFieldReference(node.get_Left() as FieldReferenceExpression);
					if (this.foundUsage)
					{
						return;
					}
				}
				this.VisitBinaryExpression(node);
				return;
			}
		}

		private class VariableCopyFinder : BaseCodeVisitor
		{
			private readonly BlockStatement theBlock;

			private HashSet<VariableReference> delegateVariablesSet;

			private HashSet<Statement> statementsToRemove;

			public VariableCopyFinder(BlockStatement theBlock)
			{
				base();
				this.theBlock = theBlock;
				return;
			}

			private bool CheckBinaryExpression(BinaryExpression node)
			{
				if (node.get_Left().get_CodeNodeType() != 26 || node.get_Right().get_CodeNodeType() != 26 || !this.delegateVariablesSet.Contains((node.get_Right() as VariableReferenceExpression).get_Variable()))
				{
					return false;
				}
				dummyVar0 = this.delegateVariablesSet.Add((node.get_Left() as VariableReferenceExpression).get_Variable());
				return true;
			}

			public void FindCopiesOfDelegate(HashSet<VariableReference> delegateVariablesSet, HashSet<Statement> statementsToRemove)
			{
				this.delegateVariablesSet = delegateVariablesSet;
				this.statementsToRemove = statementsToRemove;
				this.Visit(this.theBlock);
				return;
			}

			public override void VisitExpressionStatement(ExpressionStatement node)
			{
				if (!node.IsAssignmentStatement() || !this.CheckBinaryExpression(node.get_Expression() as BinaryExpression))
				{
					this.VisitExpressionStatement(node);
					return;
				}
				dummyVar0 = this.statementsToRemove.Add(node);
				return;
			}
		}
	}
}