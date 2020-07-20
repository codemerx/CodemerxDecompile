using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Steps
{
	public class DeclareTopLevelVariables : BaseCodeVisitor, IDecompilationStep
	{
		private readonly Stack<CodeNodeType> codeNodeTypes;

		private readonly Dictionary<VariableReference, bool> variableReferences;

		private TypeSystem typeSystem;

		private Collection<VariableDefinition> methodVariables;

		private int inLambdaCount;

		private DecompilationContext context;

		public DeclareTopLevelVariables()
		{
			this.codeNodeTypes = new Stack<CodeNodeType>();
			this.variableReferences = new Dictionary<VariableReference, bool>();
			base();
			return;
		}

		private int GetIndexOfCtorCall(BlockStatement block)
		{
			V_0 = 0;
			while (V_0 < block.get_Statements().get_Count())
			{
				if (block.get_Statements().get_Item(V_0).get_CodeNodeType() == 5)
				{
					V_1 = (block.get_Statements().get_Item(V_0) as ExpressionStatement).get_Expression();
					if (V_1.get_CodeNodeType() == 52 || V_1.get_CodeNodeType() == 53)
					{
						return V_0;
					}
				}
				V_0 = V_0 + 1;
			}
			return -1;
		}

		private void InsertTopLevelDeclarations(BlockStatement block)
		{
			V_0 = 0;
			if (this.context.get_MethodContext().get_Method().get_IsConstructor())
			{
				V_0 = this.GetIndexOfCtorCall(block) + 1;
			}
			V_1 = 0;
			while (V_1 < this.methodVariables.get_Count())
			{
				V_3 = this.context.get_MethodContext().get_VariableAssignmentData().TryGetValue(this.methodVariables.get_Item(V_1), out V_2);
				if (!V_3 || V_2 != AssignmentType.NotUsed)
				{
					if (!this.variableReferences.TryGetValue(this.methodVariables.get_Item(V_1), out V_4) || V_4 && !V_3 || V_2 != 1)
					{
						this.InsertVariableDeclaration(block, V_0, V_1);
					}
					else
					{
						this.InsertVariableDeclarationAndAssignment(block, V_0, V_1);
					}
				}
				else
				{
					V_0 = V_0 - 1;
				}
				V_1 = V_1 + 1;
				V_0 = V_0 + 1;
			}
			return;
		}

		private void InsertVariableDeclaration(BlockStatement block, int insertIndex, int variableIndex)
		{
			block.AddStatementAt(insertIndex, new ExpressionStatement(new VariableDeclarationExpression(this.methodVariables.get_Item(variableIndex), null)));
			return;
		}

		private void InsertVariableDeclarationAndAssignment(BlockStatement block, int insertIndex, int variableIndex)
		{
			V_0 = this.methodVariables.get_Item(variableIndex).get_VariableType().GetDefaultValueExpression(this.typeSystem);
			if (V_0 == null)
			{
				this.InsertVariableDeclaration(block, insertIndex, variableIndex);
				return;
			}
			V_1 = new BinaryExpression(26, new VariableDeclarationExpression(this.methodVariables.get_Item(variableIndex), null), V_0, this.typeSystem, null, false);
			block.AddStatementAt(insertIndex, new ExpressionStatement(V_1));
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			this.context = context;
			this.typeSystem = context.get_MethodContext().get_Method().get_Module().get_TypeSystem();
			this.methodVariables = new Collection<VariableDefinition>();
			V_0 = context.get_MethodContext().get_Variables().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (context.get_MethodContext().get_VariablesToNotDeclare().Contains(V_1))
					{
						continue;
					}
					this.methodVariables.Add(V_1);
				}
			}
			finally
			{
				V_0.Dispose();
			}
			this.codeNodeTypes.Push(0);
			this.Visit(block);
			dummyVar0 = this.codeNodeTypes.Pop();
			this.InsertTopLevelDeclarations(block);
			return block;
		}

		private void VisitAssignExpression(BinaryExpression node)
		{
			if (node.get_Left().get_CodeNodeType() == 26)
			{
				stackVariable4 = true;
			}
			else
			{
				stackVariable4 = node.get_Left().get_CodeNodeType() == 27;
			}
			if (stackVariable4)
			{
				this.codeNodeTypes.Push(24);
			}
			this.Visit(node.get_Left());
			if (stackVariable4)
			{
				dummyVar0 = this.codeNodeTypes.Pop();
			}
			this.Visit(node.get_Right());
			return;
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			if (node.get_IsAssignmentExpression())
			{
				this.VisitAssignExpression(node);
				return;
			}
			this.VisitBinaryExpression(node);
			return;
		}

		public override void VisitDelegateCreationExpression(DelegateCreationExpression node)
		{
			if (node.get_MethodExpression().get_CodeNodeType() != 50)
			{
				this.VisitDelegateCreationExpression(node);
				return;
			}
			this.VisitLambdaExpression((LambdaExpression)node.get_MethodExpression());
			return;
		}

		public override void VisitLambdaExpression(LambdaExpression node)
		{
			this.inLambdaCount = this.inLambdaCount + 1;
			this.VisitLambdaExpression(node);
			this.inLambdaCount = this.inLambdaCount - 1;
			return;
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			V_0 = node.get_MethodExpression().get_MethodDefinition();
			if (V_0 == null)
			{
				this.VisitMethodInvocationExpression(node);
				return;
			}
			this.Visit(node.get_MethodExpression());
			V_1 = 0;
			while (V_1 < node.get_Arguments().get_Count())
			{
				V_2 = node.get_Arguments().get_Item(V_1) as UnaryExpression;
				if (V_2 == null || V_2.get_Operator() != 7 || V_2.get_Operand().get_CodeNodeType() != 26 || !V_0.get_Parameters().get_Item(V_1).IsOutParameter())
				{
					this.Visit(node.get_Arguments().get_Item(V_1));
				}
				else
				{
					V_3 = (V_2.get_Operand() as VariableReferenceExpression).get_Variable();
					if (!this.variableReferences.ContainsKey(V_3))
					{
						this.variableReferences.Add(V_3, true);
					}
				}
				V_1 = V_1 + 1;
			}
			return;
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if (!this.variableReferences.ContainsKey(node.get_Variable()))
			{
				stackVariable8 = this.variableReferences;
				stackVariable10 = node.get_Variable();
				if (this.codeNodeTypes.Peek() != 24)
				{
					stackVariable15 = false;
				}
				else
				{
					stackVariable15 = this.inLambdaCount == 0;
				}
				stackVariable8.Add(stackVariable10, stackVariable15);
			}
			this.VisitVariableReferenceExpression(node);
			return;
		}
	}
}