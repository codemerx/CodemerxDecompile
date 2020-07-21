using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Inlining;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Languages.IL
{
	public class IntermediateLanguage : BaseLanguage
	{
		public override HashSet<string> AttributesToHide
		{
			get
			{
				return new HashSet<string>();
			}
		}

		public override string CommentLineSymbol
		{
			get
			{
				return "//";
			}
		}

		public override string DocumentationLineStarter
		{
			get
			{
				return "///";
			}
		}

		public override string EscapeSymbolAfterKeyword
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string EscapeSymbolBeforeKeyword
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override bool HasDelegateSpecificSyntax
		{
			get
			{
				return false;
			}
		}

		public override string Name
		{
			get
			{
				return "IL";
			}
		}

		public override bool SupportsExceptionFilters
		{
			get
			{
				return false;
			}
		}

		public override bool SupportsGetterOnlyAutoProperties
		{
			get
			{
				return false;
			}
		}

		public override bool SupportsInlineInitializationOfAutoProperties
		{
			get
			{
				return false;
			}
		}

		public override IVariablesToNotInlineFinder VariablesToNotInlineFinder
		{
			get
			{
				return new EmptyVariablesToNotInlineFinder();
			}
		}

		public override int Version
		{
			get
			{
				return 0;
			}
		}

		public override string VSCodeFileExtension
		{
			get
			{
				return "";
			}
		}

		public override string VSProjectFileExtension
		{
			get
			{
				return "";
			}
		}

		public IntermediateLanguage()
		{
			base();
			return;
		}

		public override BlockDecompilationPipeline CreateFilterMethodPipeline(DecompilationContext context)
		{
			return new BlockDecompilationPipeline(Array.Empty<IDecompilationStep>());
		}

		public override DecompilationPipeline CreateLambdaPipeline(DecompilationContext context)
		{
			return new DecompilationPipeline(Array.Empty<IDecompilationStep>());
		}

		public override DecompilationPipeline CreatePipeline()
		{
			return new DecompilationPipeline(Array.Empty<IDecompilationStep>());
		}

		public override DecompilationPipeline CreatePipeline(DecompilationContext context)
		{
			return new DecompilationPipeline(Array.Empty<IDecompilationStep>());
		}

		public override BlockDecompilationPipeline CreatePropertyPipeline(DecompilationContext context)
		{
			return new BlockDecompilationPipeline(Array.Empty<IDecompilationStep>());
		}

		public override IAssemblyAttributeWriter GetAssemblyAttributeWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
		{
			return new IntermediateLanguageAssemblyAttributeWriter(this, formatter, exceptionFormatter, settings);
		}

		protected override string GetCommentLine()
		{
			return "";
		}

		public override ILanguageWriter GetWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
		{
			return new IntermediateLanguageWriter(this, formatter, exceptionFormatter, settings);
		}

		public override bool IsGlobalKeyword(string word)
		{
			return false;
		}

		protected override bool IsGlobalKeyword(string word, HashSet<string> globalKeywords)
		{
			return false;
		}

		public override bool IsLanguageKeyword(string word)
		{
			return false;
		}

		protected override bool IsLanguageKeyword(string word, HashSet<string> globalKeywords, HashSet<string> contextKeywords)
		{
			return false;
		}

		public override bool IsOperatorKeyword(string operator)
		{
			return false;
		}

		public override bool IsValidLineStarter(CodeNodeType nodeType)
		{
			return false;
		}
	}
}