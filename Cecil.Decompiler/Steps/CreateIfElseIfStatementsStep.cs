using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps
{
	class CreateIfElseIfStatementsStep : BaseCodeTransformer, IDecompilationStep
	{
		private DecompilationContext context;

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{ 
			this.context = context;
			BlockStatement result = (BlockStatement)Visit(body);
			return result;
		}

		public override ICodeNode VisitIfStatement(IfStatement node)
		{
			ICodeNode transformedNode = base.VisitIfStatement(node);
			if (transformedNode.CodeNodeType != CodeNodeType.IfStatement)
			{
				return transformedNode;
			}

			IfStatement theIf = (IfStatement)transformedNode;
			if (theIf.Else == null)
			{
				return theIf;
			}

			if (theIf.Else.Statements.Count == 1)
			{
				Statement elseStatement = theIf.Else.Statements[0];
				if (elseStatement.CodeNodeType == CodeNodeType.IfStatement || elseStatement.CodeNodeType == CodeNodeType.IfElseIfStatement)
				{
					IfElseIfStatement resultingIfElseIf = HandleDirectIfStatement(theIf);
					return resultingIfElseIf;
				}				
			}

			if (theIf.Then.Statements.Count != 1)
			{
				return theIf;
			}
			Statement thenStatement = theIf.Then.Statements[0];

			if (thenStatement.CodeNodeType != CodeNodeType.IfStatement && thenStatement.CodeNodeType != CodeNodeType.IfElseIfStatement)
			{
				return theIf;
			}

			InvertIfStatement(theIf);

			IfElseIfStatement result = BuildIfElseIfStatement(theIf);

			return result;
		}
  
		private IfElseIfStatement HandleDirectIfStatement(IfStatement theIf)
		{
			/// At this point we are sure, that if-else-if statement can be created without reverting the condition
			/// Now we need to perform checks if we can create if-else-if statement after reversing and to chose which one to create
			/// 

			if (theIf.Then.Statements.Count != 1)
			{ 
				/// There is only one way in which we can create the if-else-if statement

				IfElseIfStatement result = BuildIfElseIfStatement(theIf);
				return result;
			}
			Statement thenStatement = theIf.Then.Statements[0];
			if (thenStatement.CodeNodeType != CodeNodeType.IfElseIfStatement && thenStatement.CodeNodeType != CodeNodeType.IfStatement)
			{
				/// There is only one way in which we can create the if-else-if statement

				IfElseIfStatement result = BuildIfElseIfStatement(theIf);
				return result;
			}
			Statement elseStatement = theIf.Else.Statements[0];
			if (elseStatement.CodeNodeType == CodeNodeType.IfStatement)
			{
				/// If the else statement is only if-statement, then we invert the code anyways
				/// best case: the then statement was if-else-if, and we get one level nesting less
				/// worst case: the then statemnt was if-statement as well and we get the same level of nesting as if we haven't inverted
				InvertIfStatement(theIf);
				IfElseIfStatement result = BuildIfElseIfStatement(theIf);
				return result;
			}
			/// from this point the else statement is known to be if-else-if statement
			/// 

			if (thenStatement.CodeNodeType == CodeNodeType.IfStatement)
			{
				/// This way we will continue the if-else-if statement in the else block
				IfElseIfStatement result = BuildIfElseIfStatement(theIf);
				return result;
			}

			IfElseIfStatement elseIfElseStatement = (IfElseIfStatement)elseStatement;
			IfElseIfStatement thenIfElseStatement = (IfElseIfStatement)thenStatement;

			if (thenIfElseStatement.ConditionBlocks.Count >= elseIfElseStatement.ConditionBlocks.Count)
			{
				InvertIfStatement(theIf);
			}

			IfElseIfStatement endResult = BuildIfElseIfStatement(theIf);
			return endResult;
		}
  
		private IfElseIfStatement BuildIfElseIfStatement(IfStatement theIf)
		{
			// at this point we are sure, that the statement is either IfStatement or IfElseIfStatement
			Statement elseStatement = theIf.Else.Statements[0]; 
			if (elseStatement.CodeNodeType == CodeNodeType.IfStatement)
			{
				IfStatement innerIf = (IfStatement)elseStatement;
				List<KeyValuePair<Expression, BlockStatement>> ifChain = new List<KeyValuePair<Expression, BlockStatement>>();
				KeyValuePair<Expression, BlockStatement> theFirstPair = new KeyValuePair<Expression, BlockStatement>(theIf.Condition, theIf.Then);
				KeyValuePair<Expression, BlockStatement> theSecondPair = new KeyValuePair<Expression, BlockStatement>(innerIf.Condition, innerIf.Then);
				ifChain.Add(theFirstPair);
				ifChain.Add(theSecondPair);
				IfElseIfStatement result = new IfElseIfStatement(ifChain, innerIf.Else);
				return result;
			}
			else
			{
				IfElseIfStatement innerIfElseIf = (IfElseIfStatement)elseStatement;
				List<KeyValuePair<Expression, BlockStatement>> ifChain = innerIfElseIf.ConditionBlocks;
				KeyValuePair<Expression, BlockStatement> outerPair = new KeyValuePair<Expression, BlockStatement>(theIf.Condition, theIf.Then);
				ifChain.Insert(0, outerPair);
				theIf.Then.Parent = innerIfElseIf;
				return innerIfElseIf;
			}
		}
  
		private void InvertIfStatement(IfStatement theIf)
		{
			theIf.Condition = Negator.Negate(theIf.Condition, context.MethodContext.Method.Module.TypeSystem);
			BlockStatement swap = theIf.Else;
			theIf.Else = theIf.Then;
			theIf.Then = swap;
		}
	}
}
