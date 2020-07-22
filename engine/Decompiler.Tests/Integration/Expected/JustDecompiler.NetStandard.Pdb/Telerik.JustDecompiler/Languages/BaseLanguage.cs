using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Inlining;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Languages
{
	public abstract class BaseLanguage : ILanguage, IEquatable<ILanguage>
	{
		protected HashSet<string> languageSpecificGlobalKeywords;

		protected HashSet<string> languageSpecificContextualKeywords;

		public abstract HashSet<string> AttributesToHide
		{
			get;
		}

		public abstract string CommentLineSymbol
		{
			get;
		}

		public abstract string DocumentationLineStarter
		{
			get;
		}

		public abstract string EscapeSymbolAfterKeyword
		{
			get;
		}

		public abstract string EscapeSymbolBeforeKeyword
		{
			get;
		}

		public virtual string FloatingLiteralsConstant
		{
			get
			{
				return "";
			}
		}

		public abstract bool HasDelegateSpecificSyntax
		{
			get;
		}

		public virtual bool HasOutKeyword
		{
			get
			{
				return true;
			}
		}

		public virtual StringComparer IdentifierComparer
		{
			get
			{
				return StringComparer.get_Ordinal();
			}
		}

		public static DecompilationPipeline IntermediateRepresenationPipeline
		{
			get
			{
				stackVariable1 = new IDecompilationStep[10];
				stackVariable1[0] = new RemoveUnreachableBlocksStep();
				stackVariable1[1] = new StackUsageAnalysis();
				stackVariable1[2] = new ExpressionDecompilerStep();
				stackVariable1[3] = new RemoveCompilerOptimizationsStep();
				stackVariable1[4] = new ManagedPointersRemovalStep();
				stackVariable1[5] = new VariableAssignmentAnalysisStep();
				stackVariable1[6] = new LogicalFlowBuilderStep();
				stackVariable1[7] = new FollowNodeLoopCleanUpStep();
				stackVariable1[8] = new StatementDecompilerStep();
				stackVariable1[9] = new MapUnconditionalBranchesStep();
				return new DecompilationPipeline(stackVariable1);
			}
		}

		public abstract string Name
		{
			get;
		}

		public abstract bool SupportsExceptionFilters
		{
			get;
		}

		public abstract bool SupportsGetterOnlyAutoProperties
		{
			get;
		}

		public abstract bool SupportsInlineInitializationOfAutoProperties
		{
			get;
		}

		public abstract IVariablesToNotInlineFinder VariablesToNotInlineFinder
		{
			get;
		}

		public abstract int Version
		{
			get;
		}

		public abstract string VSCodeFileExtension
		{
			get;
		}

		public abstract string VSProjectFileExtension
		{
			get;
		}

		public BaseLanguage()
		{
			base();
			this.languageSpecificGlobalKeywords = new HashSet<string>();
			this.languageSpecificContextualKeywords = new HashSet<string>();
			return;
		}

		public string CommentLines(string text)
		{
			return this.CommentLines(text, this.GetCommentLine());
		}

		private string CommentLines(string text, string lineCommentString)
		{
			V_0 = new StringBuilder();
			V_1 = new StringReader(text);
			V_2 = V_1;
			try
			{
				V_3 = V_1.ReadLine();
				while (V_3 != null)
				{
					dummyVar0 = V_0.AppendLine(String.Format("{0} {1}", lineCommentString, V_3));
					V_3 = V_1.ReadLine();
				}
			}
			finally
			{
				if (V_2 != null)
				{
					((IDisposable)V_2).Dispose();
				}
			}
			return V_0.ToString();
		}

		public virtual BlockDecompilationPipeline CreateFilterMethodPipeline(DecompilationContext context)
		{
			return new BlockDecompilationPipeline(this.LanguageFilterMethodDecompilationSteps(false), context);
		}

		public virtual DecompilationPipeline CreateLambdaPipeline(DecompilationContext context)
		{
			return this.CreatePipelineInternal(context, true);
		}

		public virtual DecompilationPipeline CreatePipeline()
		{
			stackVariable1 = new DecompilationPipeline(Array.Empty<IDecompilationStep>());
			stackVariable1.AddSteps(BaseLanguage.get_IntermediateRepresenationPipeline().get_Steps());
			stackVariable1.AddSteps(this.LanguageDecompilationSteps(false));
			return stackVariable1;
		}

		public virtual DecompilationPipeline CreatePipeline(DecompilationContext context)
		{
			return this.CreatePipelineInternal(context, false);
		}

		private DecompilationPipeline CreatePipelineInternal(DecompilationContext context, bool inlineAggressively)
		{
			stackVariable3 = new DecompilationPipeline(BaseLanguage.get_IntermediateRepresenationPipeline().get_Steps(), context);
			stackVariable3.AddSteps(this.LanguageDecompilationSteps(inlineAggressively));
			return stackVariable3;
		}

		public virtual BlockDecompilationPipeline CreatePropertyPipeline(DecompilationContext context)
		{
			return new BlockDecompilationPipeline(this.LanguageDecompilationSteps(false), context);
		}

		public bool Equals(ILanguage other)
		{
			if (other == null)
			{
				return false;
			}
			return String.Equals(this.get_Name(), other.get_Name(), 5);
		}

		public virtual string EscapeWord(string word)
		{
			return String.Concat(this.get_EscapeSymbolBeforeKeyword(), word, this.get_EscapeSymbolAfterKeyword());
		}

		public abstract IAssemblyAttributeWriter GetAssemblyAttributeWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings);

		protected abstract string GetCommentLine();

		public virtual string GetExplicitName(IMemberDefinition member)
		{
			return member.get_Name();
		}

		public abstract ILanguageWriter GetWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings);

		public virtual bool IsEscapedWord(string word)
		{
			if (!word.StartsWith(this.get_EscapeSymbolBeforeKeyword()))
			{
				return false;
			}
			return word.EndsWith(this.get_EscapeSymbolAfterKeyword());
		}

		public virtual bool IsEscapedWord(string escapedWord, string originalWord)
		{
			return String.op_Equality(escapedWord, this.EscapeWord(originalWord));
		}

		public virtual bool IsGlobalKeyword(string word)
		{
			return this.IsGlobalKeyword(word, this.languageSpecificGlobalKeywords);
		}

		protected abstract bool IsGlobalKeyword(string word, HashSet<string> globalKeywords);

		public virtual bool IsLanguageKeyword(string word)
		{
			return this.IsLanguageKeyword(word, this.languageSpecificGlobalKeywords, this.languageSpecificContextualKeywords);
		}

		protected abstract bool IsLanguageKeyword(string word, HashSet<string> globalKeywords, HashSet<string> contextKeywords);

		public abstract bool IsOperatorKeyword(string operator);

		public bool IsValidIdentifier(string identifier)
		{
			return identifier.IsValidIdentifier();
		}

		public bool IsValidIdentifierCharacter(char currentChar)
		{
			return currentChar.IsValidIdentifierCharacter();
		}

		public bool IsValidIdentifierFirstCharacter(char firstCharacter)
		{
			return firstCharacter.IsValidIdentifierFirstCharacter();
		}

		public abstract bool IsValidLineStarter(CodeNodeType nodeType);

		internal virtual IDecompilationStep[] LanguageDecompilationSteps(bool inlineAggressively)
		{
			return new IDecompilationStep[0];
		}

		protected virtual IDecompilationStep[] LanguageFilterMethodDecompilationSteps(bool inlineAggressively)
		{
			return new IDecompilationStep[0];
		}

		public string ReplaceInvalidCharactersInIdentifier(string identifier)
		{
			if (identifier == null)
			{
				return null;
			}
			if (String.op_Equality(identifier, String.Empty))
			{
				return String.Empty;
			}
			V_0 = new StringBuilder();
			V_1 = identifier.get_Chars(0);
			if (!this.IsValidIdentifierFirstCharacter(V_1))
			{
				dummyVar1 = V_0.Append(String.Format("u{0:x4}", (Int32)V_1));
			}
			else
			{
				dummyVar0 = V_0.Append(V_1);
			}
			V_2 = 1;
			while (V_2 < identifier.get_Length())
			{
				V_3 = identifier.get_Chars(V_2);
				if (!this.IsValidIdentifierCharacter(V_3))
				{
					dummyVar3 = V_0.Append(String.Format("u{0:x4}", (Int32)V_3));
				}
				else
				{
					dummyVar2 = V_0.Append(V_3);
				}
				V_2 = V_2 + 1;
			}
			return V_0.ToString();
		}

		public virtual bool TryGetOperatorName(string operatorName, out string languageOperator)
		{
			languageOperator = null;
			return false;
		}
	}
}