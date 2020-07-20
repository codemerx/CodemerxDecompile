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
	internal class RebuildAsyncStatementsStep : BaseCodeTransformer, IDecompilationStep
	{
		private readonly Dictionary<FieldDefinition, Expression> parameterMappings;

		private DecompilationContext context;

		private MethodSpecificContext methodContext;

		private TypeDefinition stateMachineTypeDef;

		private FieldDefinition builderField;

		private RebuildAsyncStatementsStep.MatcherState matcherState;

		private StatementCollection asyncStatements;

		private StatementCollection originalStatements;

		private AsyncData asyncData;

		private VariableReference currentAwaiterVariable;

		private Expression awaitedExpression;

		public RebuildAsyncStatementsStep()
		{
			this.parameterMappings = new Dictionary<FieldDefinition, Expression>();
			base();
			return;
		}

		private bool GetBuilderField()
		{
			stackVariable2 = this.GetStateMachineMethod("SetStateMachine");
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = this.GetStateMachineMethod("System.Runtime.CompilerServices.IAsyncStateMachine.SetStateMachine");
			}
			V_0 = stackVariable2;
			if (V_0 == null || V_0.get_Body() == null)
			{
				return false;
			}
			if (V_0.get_Body().get_Instructions().get_Count() <= 1)
			{
				V_5 = this.stateMachineTypeDef.get_Fields().GetEnumerator();
				try
				{
					while (V_5.MoveNext())
					{
						V_6 = V_5.get_Current();
						if (!String.op_Equality(V_6.get_FieldType().get_Name(), "AsyncVoidMethodBuilder") && !String.op_Equality(V_6.get_FieldType().get_Name(), "AsyncTaskMethodBuilder") && !String.op_Equality(V_6.get_FieldType().get_Name(), "AsyncTaskMethodBuilder`1"))
						{
							continue;
						}
						this.builderField = V_6;
						V_4 = true;
						goto Label1;
					}
					goto Label0;
				}
				finally
				{
					V_5.Dispose();
				}
			}
			else
			{
				V_1 = V_0.get_Body().get_Instructions().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						if (V_2.get_OpCode().get_Code() != 121)
						{
							continue;
						}
						this.builderField = ((FieldReference)V_2.get_Operand()).Resolve();
						V_4 = true;
						goto Label1;
					}
					goto Label0;
				}
				finally
				{
					V_1.Dispose();
				}
			}
		Label1:
			return V_4;
		Label0:
			return false;
		}

		private string GetFriendlyName(string name)
		{
			V_0 = name.LastIndexOf('\u005F');
			if (V_0 != -1 && V_0 + 1 < name.get_Length())
			{
				name = name.Substring(V_0 + 1);
			}
			return name;
		}

		private StatementCollection GetMoveNextStatements()
		{
			V_0 = this.GetStateMachineMethod("MoveNext");
			if (V_0 == null || V_0.get_Body() == null)
			{
				return null;
			}
			V_1 = V_0.get_Body().DecompileAsyncStateMachine(this.context, out this.asyncData);
			if (V_1 == null)
			{
				return null;
			}
			return V_1.get_Statements();
		}

		private MethodDefinition GetStateMachineMethod(string name)
		{
			V_0 = this.stateMachineTypeDef.get_Methods().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!String.op_Equality(V_1.get_Name(), name))
					{
						continue;
					}
					V_2 = V_1;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				V_0.Dispose();
			}
		Label1:
			return V_2;
		Label0:
			return null;
		}

		private bool IsAsyncFirstAssignmentStatement(Statement statement, out TypeDefinition asyncStateMachineType)
		{
			asyncStateMachineType = null;
			if (statement as ExpressionStatement != null)
			{
				V_0 = statement as ExpressionStatement;
				if (V_0.get_Expression() as BinaryExpression != null)
				{
					V_1 = V_0.get_Expression() as BinaryExpression;
					if (V_1.get_Right() as ThisReferenceExpression != null && V_1.get_Left() as FieldReferenceExpression != null)
					{
						V_2 = (V_1.get_Left() as FieldReferenceExpression).get_Field().get_DeclaringType();
						if (V_2 == null)
						{
							return false;
						}
						V_3 = V_2.Resolve();
						if (V_3 == null || (object)V_3.get_DeclaringType() != (object)this.methodContext.get_Method().get_DeclaringType() || !V_3.IsAsyncStateMachine())
						{
							return false;
						}
						asyncStateMachineType = V_3;
						return true;
					}
				}
			}
			return false;
		}

		private bool Match()
		{
			if (this.originalStatements.get_Count() == 0)
			{
				return false;
			}
			if (!this.methodContext.get_Method().IsAsync(out this.stateMachineTypeDef) && !this.methodContext.get_Method().HasAsyncAttributes() || !this.IsAsyncFirstAssignmentStatement(this.originalStatements.get_Item(0), out this.stateMachineTypeDef) || !this.methodContext.get_Method().HasAsyncStateMachineVariable())
			{
				return false;
			}
			if (!this.GetBuilderField())
			{
				return false;
			}
			this.asyncStatements = this.GetMoveNextStatements();
			if (this.asyncStatements == null || !this.TryRemoveOuterTryCatch(this.asyncStatements))
			{
				return false;
			}
			this.SetParameterMappings(this.originalStatements);
			this.matcherState = 0;
			this.asyncStatements = (StatementCollection)this.Visit(this.asyncStatements);
			if (this.matcherState == RebuildAsyncStatementsStep.MatcherState.FindAwaitExpression)
			{
				return true;
			}
			return this.matcherState == 4;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.methodContext = context.get_MethodContext();
			this.originalStatements = body.get_Statements();
			if (this.Match())
			{
				body.set_Statements(this.asyncStatements);
			}
			return body;
		}

		private void SetParameterMappings(StatementCollection statements)
		{
			V_0 = 0;
			while (V_0 < statements.get_Count())
			{
				if (statements.get_Item(V_0).IsAssignmentStatement())
				{
					V_1 = (statements.get_Item(V_0) as ExpressionStatement).get_Expression() as BinaryExpression;
					if (V_1.get_Left().get_CodeNodeType() == 30)
					{
						V_2 = (V_1.get_Left() as FieldReferenceExpression).get_Field();
						if ((object)V_2.get_DeclaringType().Resolve() == (object)this.stateMachineTypeDef)
						{
							this.parameterMappings.set_Item(V_2.Resolve(), V_1.get_Right());
						}
					}
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		private bool TryGetStateField(BlockStatement theCatch)
		{
			V_0 = theCatch.get_Statements().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!V_1.IsAssignmentStatement())
					{
						continue;
					}
					V_2 = (V_1 as ExpressionStatement).get_Expression() as BinaryExpression;
					if (V_2.get_Left().get_CodeNodeType() != 30 || V_2.get_Right().get_CodeNodeType() != 22)
					{
						continue;
					}
					V_3 = (V_2.get_Left() as FieldReferenceExpression).get_Field().Resolve();
					if (V_3 == null || V_3.get_DeclaringType() == null)
					{
						V_4 = false;
						goto Label1;
					}
					else
					{
						if ((object)V_3.get_DeclaringType().Resolve() == (object)this.stateMachineTypeDef)
						{
							this.asyncData.set_StateField(V_3);
							V_4 = true;
							goto Label1;
						}
						else
						{
							V_4 = false;
							goto Label1;
						}
					}
				}
				goto Label0;
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
		Label1:
			return V_4;
		Label0:
			return false;
		}

		private bool TryRemoveOuterTryCatch(StatementCollection statements)
		{
			V_0 = 0;
			while (V_0 < statements.get_Count())
			{
				V_3 = statements.get_Item(V_0);
				if (V_3.get_CodeNodeType() == 17 && (V_3 as TryStatement).get_CatchClauses().get_Count() == 1)
				{
					break;
				}
				V_0 = V_0 + 1;
			}
			if (V_0 == statements.get_Count())
			{
				return false;
			}
			V_1 = statements.get_Item(V_0) as TryStatement;
			V_2 = V_1.get_Try().get_Statements();
			if (this.asyncData.get_StateField() == null && !this.TryGetStateField(V_1.get_CatchClauses().get_Item(0).get_Body()))
			{
				return false;
			}
			statements.RemoveAt(V_0);
			V_4 = 0;
			while (V_4 < V_2.get_Count())
			{
				statements.Insert(V_0 + V_4, V_2.get_Item(V_4));
				V_4 = V_4 + 1;
			}
			return true;
		}

		public override ICodeNode Visit(ICodeNode node)
		{
			if (this.matcherState == 8)
			{
				return node;
			}
			return this.Visit(node);
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			if (node.get_Operator() == 26)
			{
				if (node.get_Left().get_CodeNodeType() == 26 && this.asyncData.get_AwaiterVariables().Contains((node.get_Left() as VariableReferenceExpression).get_Variable()))
				{
					V_0 = (node.get_Left() as VariableReferenceExpression).get_Variable();
					if (node.get_Right().get_CodeNodeType() != 19 || !String.op_Equality((node.get_Right() as MethodInvocationExpression).get_MethodExpression().get_Method().get_Name(), "GetAwaiter"))
					{
						if (node.get_Right().get_CodeNodeType() == 40 || node.get_Right().get_CodeNodeType() == 22 && (node.get_Right() as LiteralExpression).get_Value() == null && this.matcherState & 4 == 4 && (object)this.currentAwaiterVariable == (object)V_0)
						{
							this.matcherState = this.matcherState ^ 4;
							return null;
						}
					}
					else
					{
						V_1 = null;
						V_2 = node.get_Right() as MethodInvocationExpression;
						if (V_2.get_MethodExpression().get_Target() == null)
						{
							if (V_2.get_Arguments().get_Count() == 1)
							{
								V_1 = (Expression)this.Visit(V_2.get_Arguments().get_Item(0));
							}
						}
						else
						{
							if (V_2.get_Arguments().get_Count() == 0)
							{
								V_1 = (Expression)this.Visit(V_2.get_MethodExpression().get_Target());
							}
						}
						if (V_1 != null && this.matcherState == RebuildAsyncStatementsStep.MatcherState.FindAwaitExpression || this.matcherState == 4)
						{
							this.currentAwaiterVariable = V_0;
							this.awaitedExpression = V_1;
							this.matcherState = 1;
							return null;
						}
					}
					this.matcherState = 8;
					return node;
				}
				if (node.get_Left().get_CodeNodeType() == 30 && (object)(node.get_Left() as FieldReferenceExpression).get_Field().Resolve() == (object)this.asyncData.get_StateField() || node.get_Right().get_CodeNodeType() == 28)
				{
					return null;
				}
			}
			return this.VisitBinaryExpression(node);
		}

		public override ICodeNode VisitExpressionStatement(ExpressionStatement node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			if (node.get_Expression() != null)
			{
				return node;
			}
			if (node.get_Label() != null && String.op_Inequality(node.get_Label(), String.Empty))
			{
				V_0 = node.GetNextStatement();
				if (V_0 == null || V_0.get_Label() != null && String.op_Inequality(V_0.get_Label(), String.Empty))
				{
					stackVariable19 = new EmptyStatement();
					stackVariable19.set_Label(node.get_Label());
					V_1 = stackVariable19;
					this.methodContext.get_GotoLabels().set_Item(node.get_Label(), V_1);
					return V_1;
				}
				V_0.set_Label(node.get_Label());
				this.methodContext.get_GotoLabels().set_Item(node.get_Label(), V_0);
			}
			return null;
		}

		public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			if ((object)node.get_Field().get_DeclaringType().Resolve() != (object)this.stateMachineTypeDef)
			{
				return this.VisitFieldReferenceExpression(node);
			}
			V_0 = node.get_Field().Resolve();
			if (this.parameterMappings.ContainsKey(V_0))
			{
				return this.parameterMappings.get_Item(V_0).CloneExpressionOnlyAndAttachInstructions(node.get_UnderlyingSameMethodInstructions());
			}
			V_1 = new VariableDefinition(this.GetFriendlyName(V_0.get_Name()), V_0.get_FieldType(), this.methodContext.get_Method());
			this.methodContext.get_Variables().Add(V_1);
			this.methodContext.get_VariableAssignmentData().Add(V_1, this.asyncData.get_FieldAssignmentData().get_Item(V_0));
			dummyVar0 = this.methodContext.get_VariablesToRename().Add(V_1);
			V_2 = new VariableReferenceExpression(V_1, node.get_UnderlyingSameMethodInstructions());
			this.parameterMappings.set_Item(V_0, V_2);
			return V_2;
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			V_0 = node.get_MethodExpression();
			if (V_0.get_Target() != null)
			{
				if (V_0.get_Target().get_CodeNodeType() == 26 && this.asyncData.get_AwaiterVariables().Contains((V_0.get_Target() as VariableReferenceExpression).get_Variable()))
				{
					V_1 = (V_0.get_Target() as VariableReferenceExpression).get_Variable();
					if ((object)this.currentAwaiterVariable == (object)V_1)
					{
						if (!String.op_Equality(V_0.get_Method().get_Name(), "get_IsCompleted"))
						{
							if (String.op_Equality(V_0.get_Method().get_Name(), "GetResult") && this.matcherState & 2 == 2)
							{
								this.matcherState = this.matcherState ^ 2;
								return new AwaitExpression((Expression)this.Visit(this.awaitedExpression), V_0.get_Method().get_ReturnType(), node.get_UnderlyingSameMethodInstructions());
							}
						}
						else
						{
							if (this.matcherState == 1)
							{
								this.matcherState = 6;
								return null;
							}
						}
					}
					this.matcherState = 8;
					return node;
				}
				if (V_0.get_Target().get_CodeNodeType() == 30 && (object)(V_0.get_Target() as FieldReferenceExpression).get_Field().Resolve() == (object)this.builderField && String.op_Equality(V_0.get_Method().get_Name(), "SetResult"))
				{
					if (node.get_Arguments().get_Count() > 0)
					{
						stackVariable34 = node.get_Arguments().get_Item(0);
					}
					else
					{
						stackVariable34 = null;
					}
					return new ReturnExpression(stackVariable34, V_0.get_UnderlyingSameMethodInstructions());
				}
			}
			return this.VisitMethodInvocationExpression(node);
		}

		public override ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			V_0 = node.get_MethodExpression();
			if (V_0.get_Target() == null || V_0.get_Target().get_CodeNodeType() != 26 || !this.asyncData.get_AwaiterVariables().Contains((V_0.get_Target() as VariableReferenceExpression).get_Variable()))
			{
				return this.VisitPropertyReferenceExpression(node);
			}
			V_1 = (V_0.get_Target() as VariableReferenceExpression).get_Variable();
			if ((object)this.currentAwaiterVariable == (object)V_1 && String.op_Equality(V_0.get_Method().get_Name(), "get_IsCompleted") && this.matcherState == 1)
			{
				this.matcherState = 6;
				return null;
			}
			this.matcherState = 8;
			return node;
		}

		public override ICodeNode VisitUnaryExpression(UnaryExpression node)
		{
			node.set_Operand((Expression)this.Visit(node.get_Operand()));
			if (node.get_Operand() == null)
			{
				return null;
			}
			return node;
		}

		public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if (this.asyncData.get_VariableToFieldMap().ContainsKey(node.get_Variable()))
			{
				V_0 = this.asyncData.get_VariableToFieldMap().get_Item(node.get_Variable()).Resolve();
				if (V_0 != null && this.parameterMappings.ContainsKey(V_0))
				{
					return this.parameterMappings.get_Item(V_0).CloneExpressionOnly();
				}
			}
			dummyVar0 = this.methodContext.get_VariablesToRename().Add(node.get_Variable().Resolve());
			return this.VisitVariableReferenceExpression(node);
		}

		private enum MatcherState
		{
			FindAwaitExpression = 0,
			FindIsCompletedInvoke = 1,
			FindGetResultInvoke = 2,
			FindInitObj = 4,
			Stopped = 8
		}
	}
}