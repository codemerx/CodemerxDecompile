using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class CreateIfElseIfStatementsStep : BaseCodeTransformer, IDecompilationStep
	{
		private DecompilationContext context;

		public CreateIfElseIfStatementsStep()
		{
			base();
			return;
		}

		private IfElseIfStatement BuildIfElseIfStatement(IfStatement theIf)
		{
			V_0 = theIf.get_Else().get_Statements().get_Item(0);
			if (V_0.get_CodeNodeType() != 3)
			{
				V_4 = (IfElseIfStatement)V_0;
				stackVariable11 = V_4.get_ConditionBlocks();
				V_5 = new KeyValuePair<Expression, BlockStatement>(theIf.get_Condition(), theIf.get_Then());
				stackVariable11.Insert(0, V_5);
				theIf.get_Then().set_Parent(V_4);
				return V_4;
			}
			V_1 = (IfStatement)V_0;
			stackVariable25 = new List<KeyValuePair<Expression, BlockStatement>>();
			V_2 = new KeyValuePair<Expression, BlockStatement>(theIf.get_Condition(), theIf.get_Then());
			V_3 = new KeyValuePair<Expression, BlockStatement>(V_1.get_Condition(), V_1.get_Then());
			stackVariable25.Add(V_2);
			stackVariable25.Add(V_3);
			return new IfElseIfStatement(stackVariable25, V_1.get_Else());
		}

		private IfElseIfStatement HandleDirectIfStatement(IfStatement theIf)
		{
			if (theIf.get_Then().get_Statements().get_Count() != 1)
			{
				return this.BuildIfElseIfStatement(theIf);
			}
			V_0 = theIf.get_Then().get_Statements().get_Item(0);
			if (V_0.get_CodeNodeType() != 4 && V_0.get_CodeNodeType() != 3)
			{
				return this.BuildIfElseIfStatement(theIf);
			}
			V_1 = theIf.get_Else().get_Statements().get_Item(0);
			if (V_1.get_CodeNodeType() == 3)
			{
				this.InvertIfStatement(theIf);
				return this.BuildIfElseIfStatement(theIf);
			}
			if (V_0.get_CodeNodeType() == 3)
			{
				return this.BuildIfElseIfStatement(theIf);
			}
			V_2 = (IfElseIfStatement)V_1;
			if (((IfElseIfStatement)V_0).get_ConditionBlocks().get_Count() >= V_2.get_ConditionBlocks().get_Count())
			{
				this.InvertIfStatement(theIf);
			}
			return this.BuildIfElseIfStatement(theIf);
		}

		private void InvertIfStatement(IfStatement theIf)
		{
			theIf.set_Condition(Negator.Negate(theIf.get_Condition(), this.context.get_MethodContext().get_Method().get_Module().get_TypeSystem()));
			V_0 = theIf.get_Else();
			theIf.set_Else(theIf.get_Then());
			theIf.set_Then(V_0);
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			return (BlockStatement)this.Visit(body);
		}

		public override ICodeNode VisitIfStatement(IfStatement node)
		{
			V_0 = this.VisitIfStatement(node);
			if (V_0.get_CodeNodeType() != 3)
			{
				return V_0;
			}
			V_1 = (IfStatement)V_0;
			if (V_1.get_Else() == null)
			{
				return V_1;
			}
			if (V_1.get_Else().get_Statements().get_Count() == 1)
			{
				V_3 = V_1.get_Else().get_Statements().get_Item(0);
				if (V_3.get_CodeNodeType() == 3 || V_3.get_CodeNodeType() == 4)
				{
					return this.HandleDirectIfStatement(V_1);
				}
			}
			if (V_1.get_Then().get_Statements().get_Count() != 1)
			{
				return V_1;
			}
			V_2 = V_1.get_Then().get_Statements().get_Item(0);
			if (V_2.get_CodeNodeType() != 3 && V_2.get_CodeNodeType() != 4)
			{
				return V_1;
			}
			this.InvertIfStatement(V_1);
			return this.BuildIfElseIfStatement(V_1);
		}
	}
}