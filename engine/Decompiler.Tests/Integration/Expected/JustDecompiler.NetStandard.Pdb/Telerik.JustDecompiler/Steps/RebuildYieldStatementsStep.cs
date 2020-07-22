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
	internal class RebuildYieldStatementsStep : BaseCodeTransformer, IDecompilationStep
	{
		private DecompilationContext decompilationContext;

		private TypeDefinition yieldDeclaringType;

		private StatementCollection statements;

		private YieldData yieldData;

		private readonly Dictionary<FieldDefinition, Expression> parameterMappings;

		private StatementCollection newStatements;

		public RebuildYieldStatementsStep()
		{
			this.parameterMappings = new Dictionary<FieldDefinition, Expression>();
			base();
			return;
		}

		private bool CheckFieldReference(Expression expression)
		{
			if (expression.get_CodeNodeType() == 30)
			{
				V_0 = (expression as FieldReferenceExpression).get_Field().Resolve();
				if (V_0 == this.yieldData.get_FieldsInfo().get_CurrentItemField() || V_0 == this.yieldData.get_FieldsInfo().get_StateHolderField())
				{
					return true;
				}
			}
			return false;
		}

		private bool CheckVariableReference(Expression expression)
		{
			V_0 = null;
			if (expression.get_CodeNodeType() != 26)
			{
				if (expression.get_CodeNodeType() == 27)
				{
					V_0 = (expression as VariableDeclarationExpression).get_Variable();
				}
			}
			else
			{
				V_0 = (expression as VariableReferenceExpression).get_Variable();
			}
			if (V_0 != null)
			{
				V_1 = this.yieldData.get_FieldsInfo();
				if ((object)V_0 == (object)V_1.get_ReturnFlagVariable())
				{
					return true;
				}
			}
			return false;
		}

		private StatementCollection GetEnumeratorStatements()
		{
			V_0 = null;
			V_1 = this.yieldDeclaringType.get_Methods().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (!V_2.get_Name().EndsWith(".GetEnumerator"))
					{
						continue;
					}
					V_0 = V_2;
					goto Label0;
				}
			}
			finally
			{
				V_1.Dispose();
			}
		Label0:
			if (V_0 == null)
			{
				return null;
			}
			if (V_0.get_Body() != null)
			{
				stackVariable23 = V_0.get_Body().Decompile(this.decompilationContext.get_Language(), null);
			}
			else
			{
				stackVariable23 = null;
			}
			return stackVariable23.get_Statements();
		}

		private string GetFriendlyName(string fieldName)
		{
			if (fieldName.get_Chars(0) == '<')
			{
				V_0 = fieldName.IndexOf('>');
				if (V_0 > 1)
				{
					return fieldName.Substring(1, V_0 - 1);
				}
			}
			return fieldName;
		}

		private TypeDefinition GetGeneratedType()
		{
			if (this.statements.get_Item(0).get_CodeNodeType() != 5)
			{
				return null;
			}
			V_0 = this.statements.get_Item(0) as ExpressionStatement;
			if (V_0.get_Expression().get_CodeNodeType() != 24)
			{
				if (V_0.get_Expression().get_CodeNodeType() != 57)
				{
					return null;
				}
				V_5 = V_0.get_Expression() as ReturnExpression;
				if (V_5.get_Value() == null || V_5.get_Value().get_CodeNodeType() != 40)
				{
					return null;
				}
				V_1 = V_5.get_Value() as ObjectCreationExpression;
			}
			else
			{
				if (!(V_0.get_Expression() as BinaryExpression).get_IsAssignmentExpression())
				{
					return null;
				}
				V_4 = V_0.get_Expression() as BinaryExpression;
				if (V_4.get_Right().get_CodeNodeType() != 40)
				{
					return null;
				}
				V_1 = V_4.get_Right() as ObjectCreationExpression;
			}
			V_2 = V_1.get_Constructor();
			if (V_2 == null || V_2.get_DeclaringType() == null)
			{
				return null;
			}
			V_3 = V_2.get_DeclaringType().Resolve();
			if (V_3 != null && V_3.get_IsNestedPrivate() && V_3.HasCompilerGeneratedAttribute())
			{
				return V_3;
			}
			return null;
		}

		private IEnumerable<Statement> GetStatements()
		{
			V_0 = null;
			V_2 = this.yieldDeclaringType.get_Methods().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (!String.op_Equality(V_3.get_Name(), "MoveNext"))
					{
						continue;
					}
					V_0 = V_3;
					goto Label0;
				}
			}
			finally
			{
				V_2.Dispose();
			}
		Label0:
			if (V_0 == null || V_0.get_Body() == null)
			{
				return null;
			}
			V_1 = V_0.get_Body().DecompileYieldStateMachine(this.decompilationContext, out this.yieldData);
			if (V_1 == null)
			{
				return null;
			}
			return this.GetStatements(V_1);
		}

		private IEnumerable<Statement> GetStatements(BlockStatement moveNextBody)
		{
			V_0 = new List<Statement>();
			V_1 = 0;
			while (V_1 < moveNextBody.get_Statements().get_Count())
			{
				V_2 = moveNextBody.get_Statements().get_Item(V_1);
				V_3 = V_2 as TryStatement;
				if (this.yieldData.get_StateMachineVersion() != 1 || V_3 == null || V_3.get_Fault() == null && this.yieldData.get_StateMachineVersion() != 2 || V_3 == null || V_3.get_CatchClauses().get_Count() != 1)
				{
					V_0.Add(V_2);
				}
				else
				{
					V_0.AddRange(V_3.get_Try().get_Statements());
				}
				V_1 = V_1 + 1;
			}
			return V_0;
		}

		private bool Match(StatementCollection statements)
		{
			this.statements = statements;
			this.yieldDeclaringType = this.GetGeneratedType();
			if (this.yieldDeclaringType == null)
			{
				return false;
			}
			V_0 = this.GetStatements();
			if (V_0 == null || this.yieldData == null)
			{
				return false;
			}
			if (statements.get_Count() > 2)
			{
				this.SetParameterMappings();
				V_1 = this.GetEnumeratorStatements();
				if (V_1 != null)
				{
					this.PostProcessMappings(V_1);
				}
			}
			this.newStatements = new StatementCollection();
			V_2 = V_0.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					this.newStatements.Add(V_3);
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			return true;
		}

		private void PostProcessMappings(StatementCollection getEnumeratorStatements)
		{
			V_0 = getEnumeratorStatements.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1.get_CodeNodeType() != 5 || (V_1 as ExpressionStatement).get_Expression().get_CodeNodeType() != 24)
					{
						continue;
					}
					V_2 = (V_1 as ExpressionStatement).get_Expression() as BinaryExpression;
					if (!V_2.get_IsAssignmentExpression() || V_2.get_Left().get_CodeNodeType() != 30 || V_2.get_Right().get_CodeNodeType() != 30)
					{
						continue;
					}
					V_3 = (V_2.get_Left() as FieldReferenceExpression).get_Field().Resolve();
					V_4 = (V_2.get_Right() as FieldReferenceExpression).get_Field().Resolve();
					if (!this.parameterMappings.TryGetValue(V_4, out V_5))
					{
						continue;
					}
					dummyVar0 = this.parameterMappings.Remove(V_4);
					this.parameterMappings.set_Item(V_3, V_5);
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.decompilationContext = context;
			if (!this.Match(body.get_Statements()))
			{
				return body;
			}
			body.set_Statements(this.newStatements);
			body = (BlockStatement)this.Visit(body);
			this.RemoveLastIfYieldBreak(body.get_Statements());
			return body;
		}

		private void RemoveLastIfYieldBreak(StatementCollection collection)
		{
			V_0 = collection.get_Count() - 1;
			V_1 = collection.get_Item(V_0);
			if (V_1.get_CodeNodeType() == 5 && (V_1 as ExpressionStatement).get_Expression().get_CodeNodeType() == 55 && String.IsNullOrEmpty(V_1.get_Label()) && this.yieldData.get_YieldBreaks().get_Count() != 1 || this.yieldData.get_YieldReturns().get_Count() != 0)
			{
				collection.RemoveAt(V_0);
			}
			return;
		}

		private void SetParameterMappings()
		{
			V_0 = 1;
			while (V_0 < this.statements.get_Count())
			{
				if (this.statements.get_Item(V_0).get_CodeNodeType() == 5)
				{
					V_1 = this.statements.get_Item(V_0) as ExpressionStatement;
					if (V_1.get_Expression().get_CodeNodeType() == 24 && (V_1.get_Expression() as BinaryExpression).get_IsAssignmentExpression())
					{
						V_2 = V_1.get_Expression() as BinaryExpression;
						if (V_2.get_Left().get_CodeNodeType() == 30)
						{
							V_3 = (V_2.get_Left() as FieldReferenceExpression).get_Field();
							if ((object)V_3.get_DeclaringType().Resolve() == (object)this.yieldDeclaringType)
							{
								this.parameterMappings.set_Item(V_3.Resolve(), V_2.get_Right());
							}
						}
					}
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			if (node.get_IsAssignmentExpression() && this.CheckFieldReference(node.get_Left()) || this.CheckFieldReference(node.get_Right()) || this.CheckVariableReference(node.get_Left()) || this.CheckVariableReference(node.get_Right()) || node.get_Right().get_CodeNodeType() == 28)
			{
				return null;
			}
			return this.VisitBinaryExpression(node);
		}

		public override ICodeNode VisitExpressionStatement(ExpressionStatement node)
		{
			V_0 = (Expression)this.Visit(node.get_Expression());
			if (V_0 != null)
			{
				node.set_Expression(V_0);
				return node;
			}
			if (!String.IsNullOrEmpty(node.get_Label()))
			{
				V_1 = node.GetNextStatement();
				if (V_1 == null || !String.IsNullOrEmpty(V_1.get_Label()))
				{
					stackVariable16 = new EmptyStatement();
					stackVariable16.set_Label(node.get_Label());
					return stackVariable16;
				}
				V_1.set_Label(node.get_Label());
			}
			return null;
		}

		public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			if ((object)node.get_Field().get_DeclaringType().Resolve() != (object)this.yieldDeclaringType)
			{
				return this.VisitFieldReferenceExpression(node);
			}
			V_0 = node.get_Field().Resolve();
			if (this.parameterMappings.ContainsKey(V_0))
			{
				return this.parameterMappings.get_Item(V_0).CloneExpressionOnlyAndAttachInstructions(node.get_UnderlyingSameMethodInstructions());
			}
			V_1 = new VariableDefinition(this.GetFriendlyName(V_0.get_Name()), V_0.get_FieldType(), this.decompilationContext.get_MethodContext().get_Method());
			this.decompilationContext.get_MethodContext().get_Variables().Add(V_1);
			this.decompilationContext.get_MethodContext().get_VariableAssignmentData().Add(V_1, this.yieldData.get_FieldAssignmentData().get_Item(V_0));
			dummyVar0 = this.decompilationContext.get_MethodContext().get_VariablesToRename().Add(V_1);
			V_2 = new VariableReferenceExpression(V_1, node.get_UnderlyingSameMethodInstructions());
			this.parameterMappings.set_Item(V_0, V_2);
			return V_2;
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (node.get_MethodExpression().get_Target() != null && node.get_MethodExpression().get_Target().get_CodeNodeType() == 28)
			{
				return null;
			}
			return this.VisitMethodInvocationExpression(node);
		}

		public override ICodeNode VisitTryStatement(TryStatement node)
		{
			if (node.get_Finally() != null && node.get_Finally().get_Body().get_Statements().get_Count() == 1 && node.get_Finally().get_Body().get_Statements().get_Item(0).get_CodeNodeType() == 5)
			{
				V_0 = node.get_Finally().get_Body().get_Statements().get_Item(0) as ExpressionStatement;
				if (V_0.get_Expression().get_CodeNodeType() == 19)
				{
					V_1 = (V_0.get_Expression() as MethodInvocationExpression).get_MethodExpression();
					if (V_1 != null && V_1.get_Method() != null && V_1.get_Method().get_DeclaringType() != null && (object)V_1.get_Method().get_DeclaringType().Resolve() == (object)this.yieldDeclaringType)
					{
						node.set_Finally(new FinallyClause(V_1.get_Method().Resolve().get_Body().Decompile(this.decompilationContext.get_Language(), null), node.get_Finally().get_UnderlyingSameMethodInstructions()));
					}
				}
			}
			return this.VisitTryStatement(node);
		}

		public override ICodeNode VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			dummyVar0 = this.decompilationContext.get_MethodContext().get_VariablesToRename().Add(node.get_Variable());
			return this.VisitVariableDeclarationExpression(node);
		}

		public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			dummyVar0 = this.decompilationContext.get_MethodContext().get_VariablesToRename().Add(node.get_Variable().Resolve());
			return this.VisitVariableReferenceExpression(node);
		}
	}
}