using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Decompiler.Inlining;
using Telerik.JustDecompiler.Languages.CSharp;
using Telerik.JustDecompiler.Languages.VisualBasic;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Languages
{
	public static class LanguageFactory
	{
		public static ILanguage GetLanguage(CSharpVersion version)
		{
			switch (version)
			{
				case 0:
				{
					return LanguageFactory.CSharp.get_Instance();
				}
				case 1:
				{
					return LanguageFactory.CSharpV5.get_Instance();
				}
				case 2:
				{
					return LanguageFactory.CSharpV6.get_Instance();
				}
				case 3:
				{
					return LanguageFactory.CSharpV7.get_Instance();
				}
			}
			throw new ArgumentException();
		}

		public static ILanguage GetLanguage(VisualBasicVersion version)
		{
			if (version == VisualBasicVersion.None)
			{
				return LanguageFactory.VisualBasic.get_Instance();
			}
			if (version != 1)
			{
				throw new ArgumentException();
			}
			return LanguageFactory.VisualBasicV10.get_Instance();
		}

		private class CSharp : BaseLanguage, ICSharp
		{
			private static LanguageFactory.CSharp instance;

			private Dictionary<string, string> operators;

			public override HashSet<string> AttributesToHide
			{
				get
				{
					stackVariable1 = new String[9];
					stackVariable1[0] = "System.ParamArrayAttribute";
					stackVariable1[1] = "System.Runtime.CompilerServices.IteratorStateMachineAttribute";
					stackVariable1[2] = "Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute";
					stackVariable1[3] = "Windows.Foundation.Metadata.ActivatableAttribute";
					stackVariable1[4] = "System.Runtime.CompilerServices.DynamicAttribute";
					stackVariable1[5] = "System.Runtime.CompilerServices.ExtensionAttribute";
					stackVariable1[6] = "System.Diagnostics.DebuggerStepThroughAttribute";
					stackVariable1[7] = "System.Runtime.CompilerServices.AsyncStateMachineAttribute";
					stackVariable1[8] = "System.Runtime.CompilerServices.CompilerGeneratedAttribute";
					return new HashSet<string>(stackVariable1);
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
					return "";
				}
			}

			public override string EscapeSymbolBeforeKeyword
			{
				get
				{
					return "@";
				}
			}

			public override string FloatingLiteralsConstant
			{
				get
				{
					return "f";
				}
			}

			public override bool HasDelegateSpecificSyntax
			{
				get
				{
					return true;
				}
			}

			public static LanguageFactory.CSharp Instance
			{
				get
				{
					return LanguageFactory.CSharp.instance;
				}
			}

			public override string Name
			{
				get
				{
					V_0 = this.get_Version();
					return String.Concat("C#", V_0.ToString());
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
					return ".cs";
				}
			}

			public override string VSProjectFileExtension
			{
				get
				{
					return ".csproj";
				}
			}

			static CSharp()
			{
				LanguageFactory.CSharp.instance = new LanguageFactory.CSharp();
				return;
			}

			protected CSharp()
			{
				base();
				this.operators = new Dictionary<string, string>();
				this.InitializeOperators();
				return;
			}

			public override IAssemblyAttributeWriter GetAssemblyAttributeWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
			{
				return new CSharpAssemblyAttributeWriter(this, formatter, exceptionFormatter, settings);
			}

			protected override string GetCommentLine()
			{
				return "//";
			}

			public override ILanguageWriter GetWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
			{
				return new CSharpWriter(this, formatter, exceptionFormatter, settings);
			}

			private void InitializeOperators()
			{
				stackVariable2 = new string[29, 2];
				stackVariable2[0, 0] = "Decrement";
				stackVariable2[0, 1] = "--";
				stackVariable2[1, 0] = "Increment";
				stackVariable2[1, 1] = "++";
				stackVariable2[2, 0] = "UnaryNegation";
				stackVariable2[2, 1] = "-";
				stackVariable2[3, 0] = "UnaryPlus";
				stackVariable2[3, 1] = "+";
				stackVariable2[4, 0] = "LogicalNot";
				stackVariable2[4, 1] = "!";
				stackVariable2[5, 0] = "OnesComplement";
				stackVariable2[5, 1] = "~";
				stackVariable2[6, 0] = "True";
				stackVariable2[6, 1] = "true";
				stackVariable2[7, 0] = "False";
				stackVariable2[7, 1] = "false";
				stackVariable2[8, 0] = "Addition";
				stackVariable2[8, 1] = "+";
				stackVariable2[9, 0] = "Subtraction";
				stackVariable2[9, 1] = "-";
				stackVariable2[10, 0] = "Multiply";
				stackVariable2[10, 1] = "*";
				stackVariable2[11, 0] = "Division";
				stackVariable2[11, 1] = "/";
				stackVariable2[12, 0] = "Modulus";
				stackVariable2[12, 1] = "%";
				stackVariable2[13, 0] = "ExclusiveOr";
				stackVariable2[13, 1] = "^";
				stackVariable2[14, 0] = "BitwiseAnd";
				stackVariable2[14, 1] = "&";
				stackVariable2[15, 0] = "BitwiseOr";
				stackVariable2[15, 1] = "|";
				stackVariable2[16, 0] = "LeftShift";
				stackVariable2[16, 1] = "<<";
				stackVariable2[17, 0] = "RightShift";
				stackVariable2[17, 1] = ">>";
				stackVariable2[18, 0] = "Equality";
				stackVariable2[18, 1] = "==";
				stackVariable2[19, 0] = "GreaterThan";
				stackVariable2[19, 1] = ">";
				stackVariable2[20, 0] = "LessThan";
				stackVariable2[20, 1] = "<";
				stackVariable2[21, 0] = "Inequality";
				stackVariable2[21, 1] = "!=";
				stackVariable2[22, 0] = "GreaterThanOrEqual";
				stackVariable2[22, 1] = ">=";
				stackVariable2[23, 0] = "LessThanOrEqual";
				stackVariable2[23, 1] = "<=";
				stackVariable2[24, 0] = "MemberSelection";
				stackVariable2[24, 1] = "->";
				stackVariable2[25, 0] = "PointerToMemberSelection";
				stackVariable2[25, 1] = "->*";
				stackVariable2[26, 0] = "Comma";
				stackVariable2[26, 1] = ",";
				stackVariable2[27, 0] = "Implicit";
				stackVariable2[27, 1] = "";
				stackVariable2[28, 0] = "Explicit";
				stackVariable2[28, 1] = "";
				V_0 = stackVariable2;
				V_1 = 0;
				while (V_1 < V_0.GetLength(0))
				{
					this.operators.Add(V_0[V_1, 0], V_0[V_1, 1]);
					V_1 = V_1 + 1;
				}
				return;
			}

			protected override bool IsGlobalKeyword(string word, HashSet<string> globalKeywords)
			{
				return globalKeywords.Contains(word);
			}

			protected override bool IsLanguageKeyword(string word, HashSet<string> globalKeywords, HashSet<string> contextKeywords)
			{
				if (globalKeywords.Contains(word))
				{
					return true;
				}
				return contextKeywords.Contains(word);
			}

			public override bool IsOperatorKeyword(string operator)
			{
				return false;
			}

			public override bool IsValidLineStarter(CodeNodeType nodeType)
			{
				return true;
			}

			public override bool TryGetOperatorName(string operatorName, out string languageOperator)
			{
				return this.operators.TryGetValue(operatorName, out languageOperator);
			}
		}

		private class CSharpV5 : LanguageFactory.CSharp, ICSharp
		{
			private static LanguageFactory.CSharpV5 instance;

			public new static LanguageFactory.CSharpV5 Instance
			{
				get
				{
					return LanguageFactory.CSharpV5.instance;
				}
			}

			public override int Version
			{
				get
				{
					return 5;
				}
			}

			static CSharpV5()
			{
				LanguageFactory.CSharpV5.instance = new LanguageFactory.CSharpV5();
				return;
			}

			protected CSharpV5()
			{
				base();
				stackVariable2 = new String[77];
				stackVariable2[0] = "abstract";
				stackVariable2[1] = "as";
				stackVariable2[2] = "base";
				stackVariable2[3] = "bool";
				stackVariable2[4] = "break";
				stackVariable2[5] = "byte";
				stackVariable2[6] = "case";
				stackVariable2[7] = "catch";
				stackVariable2[8] = "char";
				stackVariable2[9] = "checked";
				stackVariable2[10] = "class";
				stackVariable2[11] = "const";
				stackVariable2[12] = "continue";
				stackVariable2[13] = "decimal";
				stackVariable2[14] = "default";
				stackVariable2[15] = "delegate";
				stackVariable2[16] = "do";
				stackVariable2[17] = "double";
				stackVariable2[18] = "else";
				stackVariable2[19] = "enum";
				stackVariable2[20] = "event";
				stackVariable2[21] = "explicit";
				stackVariable2[22] = "extern";
				stackVariable2[23] = "false";
				stackVariable2[24] = "finally";
				stackVariable2[25] = "fixed";
				stackVariable2[26] = "float";
				stackVariable2[27] = "for";
				stackVariable2[28] = "foreach";
				stackVariable2[29] = "goto";
				stackVariable2[30] = "if";
				stackVariable2[31] = "implicit";
				stackVariable2[32] = "in";
				stackVariable2[33] = "int";
				stackVariable2[34] = "interface";
				stackVariable2[35] = "internal";
				stackVariable2[36] = "is";
				stackVariable2[37] = "lock";
				stackVariable2[38] = "long";
				stackVariable2[39] = "namespace";
				stackVariable2[40] = "new";
				stackVariable2[41] = "null";
				stackVariable2[42] = "object";
				stackVariable2[43] = "operator";
				stackVariable2[44] = "out";
				stackVariable2[45] = "override";
				stackVariable2[46] = "params";
				stackVariable2[47] = "private";
				stackVariable2[48] = "protected";
				stackVariable2[49] = "public";
				stackVariable2[50] = "readonly";
				stackVariable2[51] = "ref";
				stackVariable2[52] = "return";
				stackVariable2[53] = "sbyte";
				stackVariable2[54] = "sealed";
				stackVariable2[55] = "short";
				stackVariable2[56] = "sizeof";
				stackVariable2[57] = "stackalloc";
				stackVariable2[58] = "static";
				stackVariable2[59] = "string";
				stackVariable2[60] = "struct";
				stackVariable2[61] = "switch";
				stackVariable2[62] = "this";
				stackVariable2[63] = "throw";
				stackVariable2[64] = "true";
				stackVariable2[65] = "try";
				stackVariable2[66] = "typeof";
				stackVariable2[67] = "uint";
				stackVariable2[68] = "ulong";
				stackVariable2[69] = "unchecked";
				stackVariable2[70] = "unsafe";
				stackVariable2[71] = "ushort";
				stackVariable2[72] = "using";
				stackVariable2[73] = "virtual";
				stackVariable2[74] = "void";
				stackVariable2[75] = "volatile";
				stackVariable2[76] = "while";
				V_0 = stackVariable2;
				V_1 = 0;
				while (V_1 < (int)V_0.Length)
				{
					V_2 = V_0[V_1];
					dummyVar0 = this.languageSpecificGlobalKeywords.Add(V_2);
					V_1 = V_1 + 1;
				}
				stackVariable173 = new String[23];
				stackVariable173[0] = "add";
				stackVariable173[1] = "alias";
				stackVariable173[2] = "ascending";
				stackVariable173[3] = "async";
				stackVariable173[4] = "await";
				stackVariable173[5] = "descending";
				stackVariable173[6] = "dynamic";
				stackVariable173[7] = "from";
				stackVariable173[8] = "get";
				stackVariable173[9] = "global";
				stackVariable173[10] = "group";
				stackVariable173[11] = "into";
				stackVariable173[12] = "join";
				stackVariable173[13] = "let";
				stackVariable173[14] = "orderby";
				stackVariable173[15] = "partial";
				stackVariable173[16] = "remove";
				stackVariable173[17] = "select";
				stackVariable173[18] = "set";
				stackVariable173[19] = "value";
				stackVariable173[20] = "var";
				stackVariable173[21] = "where";
				stackVariable173[22] = "yield";
				V_0 = stackVariable173;
				V_1 = 0;
				while (V_1 < (int)V_0.Length)
				{
					V_3 = V_0[V_1];
					dummyVar1 = this.languageSpecificContextualKeywords.Add(V_3);
					V_1 = V_1 + 1;
				}
				return;
			}

			internal override IDecompilationStep[] LanguageDecompilationSteps(bool inlineAggressively)
			{
				stackVariable1 = new IDecompilationStep[43];
				stackVariable1[0] = new OutParameterAssignmentAnalysisStep();
				stackVariable1[1] = new RebuildAsyncStatementsStep();
				stackVariable1[2] = new RebuildYieldStatementsStep();
				stackVariable1[3] = new RemoveDelegateCachingStep();
				stackVariable1[4] = new RebuildAnonymousDelegatesStep();
				stackVariable1[5] = new RebuildLambdaExpressions();
				stackVariable1[6] = new ResolveDynamicVariablesStep();
				stackVariable1[7] = new GotoCancelation();
				stackVariable1[8] = new CombinedTransformerStep();
				stackVariable1[9] = new MergeUnaryAndBinaryExpression();
				stackVariable1[10] = new RemoveLastReturn();
				stackVariable1[11] = new RebuildSwitchByString();
				stackVariable1[12] = new RebuildForeachStatements();
				stackVariable1[13] = new RebuildForeachArrayStatements();
				stackVariable1[14] = new RebuildForStatements();
				stackVariable1[15] = new RebuildLockStatements();
				stackVariable1[16] = new RebuildFixedStatements();
				stackVariable1[17] = new RebuildUsingStatements();
				stackVariable1[18] = new RenameEnumValues();
				stackVariable1[19] = new FixMethodOverloadsStep();
				stackVariable1[20] = new RebuildExpressionTreesStep();
				stackVariable1[21] = new TransformMemberHandlersStep();
				stackVariable1[22] = new SelfAssignment();
				stackVariable1[23] = new CodePatternsStep(inlineAggressively);
				stackVariable1[24] = new DetermineCtorInvocationStep();
				stackVariable1[25] = new DeduceImplicitDelegates();
				stackVariable1[26] = new RebuildLinqQueriesStep();
				stackVariable1[27] = new CreateIfElseIfStatementsStep();
				stackVariable1[28] = new CreateCompilerOptimizedSwitchByStringStatementsStep();
				stackVariable1[29] = new ParenthesizeExpressionsStep();
				stackVariable1[30] = new RemoveUnusedVariablesStep();
				stackVariable1[31] = new DeclareVariablesOnFirstAssignment();
				stackVariable1[32] = new DeclareTopLevelVariables();
				stackVariable1[33] = new AssignOutParametersStep();
				stackVariable1[34] = new RenameSplitPropertiesMethodsAndBackingFields();
				stackVariable1[35] = new RenameVariables();
				stackVariable1[36] = new CastEnumsToIntegersStep();
				stackVariable1[37] = new CastIntegersStep();
				stackVariable1[38] = new ArrayVariablesStep();
				stackVariable1[39] = new CaseGotoTransformerStep();
				stackVariable1[40] = new UnsafeMethodBodyStep();
				stackVariable1[41] = new DetermineDestructorStep();
				stackVariable1[42] = new DependsOnAnalysisStep();
				return stackVariable1;
			}
		}

		private class CSharpV6 : LanguageFactory.CSharpV5, ICSharp
		{
			private static LanguageFactory.CSharpV6 instance;

			public new static LanguageFactory.CSharpV6 Instance
			{
				get
				{
					return LanguageFactory.CSharpV6.instance;
				}
			}

			public override bool SupportsExceptionFilters
			{
				get
				{
					return true;
				}
			}

			public override bool SupportsGetterOnlyAutoProperties
			{
				get
				{
					return true;
				}
			}

			public override bool SupportsInlineInitializationOfAutoProperties
			{
				get
				{
					return true;
				}
			}

			public override int Version
			{
				get
				{
					return 6;
				}
			}

			static CSharpV6()
			{
				LanguageFactory.CSharpV6.instance = new LanguageFactory.CSharpV6();
				return;
			}

			protected CSharpV6()
			{
				base();
				return;
			}

			internal override IDecompilationStep[] LanguageDecompilationSteps(bool inlineAggressively)
			{
				stackVariable1 = new IDecompilationStep[45];
				stackVariable1[0] = new OutParameterAssignmentAnalysisStep();
				stackVariable1[1] = new RebuildAsyncStatementsStep();
				stackVariable1[2] = new RebuildYieldStatementsStep();
				stackVariable1[3] = new RemoveDelegateCachingStep();
				stackVariable1[4] = new RebuildAnonymousDelegatesStep();
				stackVariable1[5] = new RebuildLambdaExpressions();
				stackVariable1[6] = new ResolveDynamicVariablesStep();
				stackVariable1[7] = new GotoCancelation();
				stackVariable1[8] = new CombinedTransformerStep();
				stackVariable1[9] = new MergeUnaryAndBinaryExpression();
				stackVariable1[10] = new RemoveLastReturn();
				stackVariable1[11] = new RebuildSwitchByString();
				stackVariable1[12] = new RebuildForeachStatements();
				stackVariable1[13] = new RebuildForeachArrayStatements();
				stackVariable1[14] = new RebuildForStatements();
				stackVariable1[15] = new RebuildLockStatements();
				stackVariable1[16] = new RebuildFixedStatements();
				stackVariable1[17] = new RebuildUsingStatements();
				stackVariable1[18] = new RenameEnumValues();
				stackVariable1[19] = new FixMethodOverloadsStep();
				stackVariable1[20] = new RebuildExpressionTreesStep();
				stackVariable1[21] = new TransformMemberHandlersStep();
				stackVariable1[22] = new SelfAssignment();
				stackVariable1[23] = new CodePatternsStep(inlineAggressively);
				stackVariable1[24] = new TransformCatchClausesFilterExpressionStep();
				stackVariable1[25] = new DetermineCtorInvocationStep();
				stackVariable1[26] = new DeduceImplicitDelegates();
				stackVariable1[27] = new RebuildLinqQueriesStep();
				stackVariable1[28] = new CreateIfElseIfStatementsStep();
				stackVariable1[29] = new CreateCompilerOptimizedSwitchByStringStatementsStep();
				stackVariable1[30] = new ParenthesizeExpressionsStep();
				stackVariable1[31] = new RemoveUnusedVariablesStep();
				stackVariable1[32] = new RebuildCatchClausesFilterStep();
				stackVariable1[33] = new DeclareVariablesOnFirstAssignment();
				stackVariable1[34] = new DeclareTopLevelVariables();
				stackVariable1[35] = new AssignOutParametersStep();
				stackVariable1[36] = new RenameSplitPropertiesMethodsAndBackingFields();
				stackVariable1[37] = new RenameVariables();
				stackVariable1[38] = new CastEnumsToIntegersStep();
				stackVariable1[39] = new CastIntegersStep();
				stackVariable1[40] = new ArrayVariablesStep();
				stackVariable1[41] = new CaseGotoTransformerStep();
				stackVariable1[42] = new UnsafeMethodBodyStep();
				stackVariable1[43] = new DetermineDestructorStep();
				stackVariable1[44] = new DependsOnAnalysisStep();
				return stackVariable1;
			}

			protected override IDecompilationStep[] LanguageFilterMethodDecompilationSteps(bool inlineAggressively)
			{
				stackVariable1 = new IDecompilationStep[12];
				stackVariable1[0] = new DeclareVariablesOnFirstAssignment();
				stackVariable1[1] = new DeclareTopLevelVariables();
				stackVariable1[2] = new AssignOutParametersStep();
				stackVariable1[3] = new RenameSplitPropertiesMethodsAndBackingFields();
				stackVariable1[4] = new RenameVariables();
				stackVariable1[5] = new CastEnumsToIntegersStep();
				stackVariable1[6] = new CastIntegersStep();
				stackVariable1[7] = new ArrayVariablesStep();
				stackVariable1[8] = new CaseGotoTransformerStep();
				stackVariable1[9] = new UnsafeMethodBodyStep();
				stackVariable1[10] = new DetermineDestructorStep();
				stackVariable1[11] = new DependsOnAnalysisStep();
				return stackVariable1;
			}
		}

		private class CSharpV7 : LanguageFactory.CSharpV6, ICSharp
		{
			private static LanguageFactory.CSharpV7 instance;

			public new static LanguageFactory.CSharpV7 Instance
			{
				get
				{
					return LanguageFactory.CSharpV7.instance;
				}
			}

			public override int Version
			{
				get
				{
					return 7;
				}
			}

			static CSharpV7()
			{
				LanguageFactory.CSharpV7.instance = new LanguageFactory.CSharpV7();
				return;
			}

			protected CSharpV7()
			{
				base();
				return;
			}
		}

		private class VisualBasic : BaseLanguage, IVisualBasic
		{
			private static LanguageFactory.VisualBasic instance;

			private Dictionary<string, string> operators;

			private HashSet<string> operatorKeywords;

			public override HashSet<string> AttributesToHide
			{
				get
				{
					stackVariable1 = new String[7];
					stackVariable1[0] = "System.ParamArrayAttribute";
					stackVariable1[1] = "System.Runtime.CompilerServices.IteratorStateMachineAttribute";
					stackVariable1[2] = "Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute";
					stackVariable1[3] = "System.Runtime.CompilerServices.ExtensionAttribute";
					stackVariable1[4] = "System.Diagnostics.DebuggerStepThroughAttribute";
					stackVariable1[5] = "System.Runtime.CompilerServices.AsyncStateMachineAttribute";
					stackVariable1[6] = "System.Runtime.CompilerServices.CompilerGeneratedAttribute";
					return new HashSet<string>(stackVariable1);
				}
			}

			public override string CommentLineSymbol
			{
				get
				{
					return "'";
				}
			}

			public override string DocumentationLineStarter
			{
				get
				{
					return "'''";
				}
			}

			public override string EscapeSymbolAfterKeyword
			{
				get
				{
					return "]";
				}
			}

			public override string EscapeSymbolBeforeKeyword
			{
				get
				{
					return "[";
				}
			}

			public override string FloatingLiteralsConstant
			{
				get
				{
					return "!";
				}
			}

			public override bool HasDelegateSpecificSyntax
			{
				get
				{
					return true;
				}
			}

			public override bool HasOutKeyword
			{
				get
				{
					return false;
				}
			}

			public override StringComparer IdentifierComparer
			{
				get
				{
					return StringComparer.get_OrdinalIgnoreCase();
				}
			}

			public static LanguageFactory.VisualBasic Instance
			{
				get
				{
					return LanguageFactory.VisualBasic.instance;
				}
			}

			public override string Name
			{
				get
				{
					V_0 = this.get_Version();
					return String.Concat("VB.NET", V_0.ToString());
				}
			}

			public override bool SupportsExceptionFilters
			{
				get
				{
					return true;
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
					return new VisualBasicVariablesToNotInlineFinder(this);
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
					return ".vb";
				}
			}

			public override string VSProjectFileExtension
			{
				get
				{
					return ".vbproj";
				}
			}

			static VisualBasic()
			{
				LanguageFactory.VisualBasic.instance = new LanguageFactory.VisualBasic();
				return;
			}

			protected VisualBasic()
			{
				base();
				this.operators = new Dictionary<string, string>();
				this.operatorKeywords = new HashSet<string>();
				this.InitializeOperators();
				return;
			}

			private bool ExistsNonExplicitMember(IMemberDefinition explicitMember, string nonExplicitName)
			{
				V_0 = new LanguageFactory.VisualBasic.u003cu003ec__DisplayClass29_0();
				V_0.u003cu003e4__this = this;
				V_0.nonExplicitName = nonExplicitName;
				if (explicitMember as MethodDefinition != null)
				{
					return explicitMember.get_DeclaringType().get_Methods().Where<MethodDefinition>(new Func<MethodDefinition, bool>(V_0.u003cExistsNonExplicitMemberu003eb__0)).Count<MethodDefinition>() > 1;
				}
				if (explicitMember as PropertyDefinition != null)
				{
					return explicitMember.get_DeclaringType().get_Properties().Where<PropertyDefinition>(new Func<PropertyDefinition, bool>(V_0.u003cExistsNonExplicitMemberu003eb__1)).Count<PropertyDefinition>() > 1;
				}
				if (explicitMember as EventDefinition == null)
				{
					return false;
				}
				return explicitMember.get_DeclaringType().get_Events().Where<EventDefinition>(new Func<EventDefinition, bool>(V_0.u003cExistsNonExplicitMemberu003eb__2)).Count<EventDefinition>() > 1;
			}

			public override IAssemblyAttributeWriter GetAssemblyAttributeWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
			{
				return new VisualBasicAssemblyAttributeWriter(this, formatter, exceptionFormatter, settings);
			}

			protected override string GetCommentLine()
			{
				return "'";
			}

			public override string GetExplicitName(IMemberDefinition member)
			{
				V_0 = this.GetMemberNonExplicitName(member);
				if (!this.ExistsNonExplicitMember(member, V_0))
				{
					return V_0;
				}
				return this.GetMemberSpecialExplicitName(member);
			}

			private string GetMemberNonExplicitName(IMemberDefinition member)
			{
				V_0 = member.get_Name();
				V_1 = V_0.LastIndexOf('.');
				if (V_1 != -1)
				{
					V_0 = V_0.Substring(V_1 + 1);
				}
				return V_0;
			}

			private string GetMemberSpecialExplicitName(IMemberDefinition member)
			{
				V_0 = this.GetMemberNonExplicitName(member);
				if (!this.ExistsNonExplicitMember(member, V_0))
				{
					return V_0;
				}
				return String.Concat("Explicit", V_0);
			}

			public override ILanguageWriter GetWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
			{
				return new VisualBasicWriter(this, formatter, exceptionFormatter, settings);
			}

			private void InitializeOperators()
			{
				stackVariable2 = new string[31, 2];
				stackVariable2[0, 0] = "UnaryNegation";
				stackVariable2[0, 1] = "-";
				stackVariable2[1, 0] = "UnaryPlus";
				stackVariable2[1, 1] = "+";
				stackVariable2[2, 0] = "LogicalNot";
				stackVariable2[2, 1] = "Not";
				stackVariable2[3, 0] = "True";
				stackVariable2[3, 1] = "IsTrue";
				stackVariable2[4, 0] = "False";
				stackVariable2[4, 1] = "IsFalse";
				stackVariable2[5, 0] = "AddressOf";
				stackVariable2[5, 1] = "&";
				stackVariable2[6, 0] = "OnesComplement";
				stackVariable2[6, 1] = "Not";
				stackVariable2[7, 0] = "PointerDereference";
				stackVariable2[7, 1] = "*";
				stackVariable2[8, 0] = "Addition";
				stackVariable2[8, 1] = "+";
				stackVariable2[9, 0] = "Subtraction";
				stackVariable2[9, 1] = "-";
				stackVariable2[10, 0] = "Multiply";
				stackVariable2[10, 1] = "*";
				stackVariable2[11, 0] = "Division";
				stackVariable2[11, 1] = "/";
				stackVariable2[12, 0] = "Modulus";
				stackVariable2[12, 1] = "Mod";
				stackVariable2[13, 0] = "ExclusiveOr";
				stackVariable2[13, 1] = "Xor";
				stackVariable2[14, 0] = "BitwiseAnd";
				stackVariable2[14, 1] = "And";
				stackVariable2[15, 0] = "BitwiseOr";
				stackVariable2[15, 1] = "Or";
				stackVariable2[16, 0] = "LogicalAnd";
				stackVariable2[16, 1] = "AndAlso";
				stackVariable2[17, 0] = "LogicalOr";
				stackVariable2[17, 1] = "OrElse";
				stackVariable2[18, 0] = "LeftShift";
				stackVariable2[18, 1] = "<<";
				stackVariable2[19, 0] = "RightShift";
				stackVariable2[19, 1] = ">>";
				stackVariable2[20, 0] = "Equality";
				stackVariable2[20, 1] = "=";
				stackVariable2[21, 0] = "GreaterThan";
				stackVariable2[21, 1] = ">";
				stackVariable2[22, 0] = "LessThan";
				stackVariable2[22, 1] = "<";
				stackVariable2[23, 0] = "Inequality";
				stackVariable2[23, 1] = "<>";
				stackVariable2[24, 0] = "GreaterThanOrEqual";
				stackVariable2[24, 1] = ">=";
				stackVariable2[25, 0] = "LessThanOrEqual";
				stackVariable2[25, 1] = "<=";
				stackVariable2[26, 0] = "MemberSelection";
				stackVariable2[26, 1] = "->";
				stackVariable2[27, 0] = "PointerToMemberSelection";
				stackVariable2[27, 1] = "->*";
				stackVariable2[28, 0] = "Comma";
				stackVariable2[28, 1] = ",";
				stackVariable2[29, 0] = "Implicit";
				stackVariable2[29, 1] = "CType";
				stackVariable2[30, 0] = "Explicit";
				stackVariable2[30, 1] = "CType";
				V_0 = stackVariable2;
				V_2 = 0;
				while (V_2 < V_0.GetLength(0))
				{
					this.operators.Add(V_0[V_2, 0], V_0[V_2, 1]);
					V_2 = V_2 + 1;
				}
				stackVariable208 = new String[9];
				stackVariable208[0] = "And";
				stackVariable208[1] = "Or";
				stackVariable208[2] = "Xor";
				stackVariable208[3] = "AndAlso";
				stackVariable208[4] = "OrElse";
				stackVariable208[5] = "Mod";
				stackVariable208[6] = "Is";
				stackVariable208[7] = "IsNot";
				stackVariable208[8] = "Not";
				V_1 = stackVariable208;
				V_3 = 0;
				while (V_3 < (int)V_1.Length)
				{
					dummyVar0 = this.operatorKeywords.Add(V_1[V_3]);
					V_3 = V_3 + 1;
				}
				return;
			}

			protected override bool IsGlobalKeyword(string word, HashSet<string> globalKeywords)
			{
				V_0 = globalKeywords.GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						if (!V_0.get_Current().Equals(word, 5))
						{
							continue;
						}
						V_1 = true;
						goto Label1;
					}
					goto Label0;
				}
				finally
				{
					((IDisposable)V_0).Dispose();
				}
			Label1:
				return V_1;
			Label0:
				return false;
			}

			protected override bool IsLanguageKeyword(string word, HashSet<string> globalKeywords, HashSet<string> contextKeywords)
			{
				V_0 = globalKeywords.GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						if (!V_0.get_Current().Equals(word, 5))
						{
							continue;
						}
						V_1 = true;
						goto Label0;
					}
				}
				finally
				{
					((IDisposable)V_0).Dispose();
				}
				V_0 = contextKeywords.GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						if (!V_0.get_Current().Equals(word, 5))
						{
							continue;
						}
						V_1 = true;
						goto Label0;
					}
					goto Label1;
				}
				finally
				{
					((IDisposable)V_0).Dispose();
				}
			Label0:
				return V_1;
			Label1:
				return false;
			}

			public override bool IsOperatorKeyword(string operator)
			{
				return this.operatorKeywords.Contains(operator);
			}

			public override bool IsValidLineStarter(CodeNodeType nodeType)
			{
				if (nodeType == 30 || nodeType == 42 || nodeType == 19 || nodeType == 33 || nodeType == 31 || nodeType == 28 || nodeType == 29 || nodeType == 26 || nodeType == 25 || nodeType == 35)
				{
					return true;
				}
				return nodeType == 39;
			}

			public override bool TryGetOperatorName(string operatorName, out string languageOperator)
			{
				return this.operators.TryGetValue(operatorName, out languageOperator);
			}
		}

		private class VisualBasicV10 : LanguageFactory.VisualBasic, IVisualBasic
		{
			private static LanguageFactory.VisualBasicV10 instance;

			public new static LanguageFactory.VisualBasicV10 Instance
			{
				get
				{
					return LanguageFactory.VisualBasicV10.instance;
				}
			}

			public override bool SupportsGetterOnlyAutoProperties
			{
				get
				{
					return true;
				}
			}

			public override int Version
			{
				get
				{
					return 10;
				}
			}

			static VisualBasicV10()
			{
				LanguageFactory.VisualBasicV10.instance = new LanguageFactory.VisualBasicV10();
				return;
			}

			protected VisualBasicV10()
			{
				base();
				stackVariable2 = new String[157];
				stackVariable2[0] = "AddHandler";
				stackVariable2[1] = "AddressOf";
				stackVariable2[2] = "Alias";
				stackVariable2[3] = "And";
				stackVariable2[4] = "AndAlso";
				stackVariable2[5] = "As";
				stackVariable2[6] = "Boolean";
				stackVariable2[7] = "ByRef";
				stackVariable2[8] = "Byte";
				stackVariable2[9] = "ByVal";
				stackVariable2[10] = "Call";
				stackVariable2[11] = "Case";
				stackVariable2[12] = "Catch";
				stackVariable2[13] = "CBool";
				stackVariable2[14] = "CByte";
				stackVariable2[15] = "CChar";
				stackVariable2[16] = "CDate";
				stackVariable2[17] = "CDbl";
				stackVariable2[18] = "CDec";
				stackVariable2[19] = "Char";
				stackVariable2[20] = "CInt";
				stackVariable2[21] = "Class";
				stackVariable2[22] = "CLng";
				stackVariable2[23] = "CObj";
				stackVariable2[24] = "Const";
				stackVariable2[25] = "Continue";
				stackVariable2[26] = "CSByte";
				stackVariable2[27] = "CShort";
				stackVariable2[28] = "CSng";
				stackVariable2[29] = "CStr";
				stackVariable2[30] = "CType";
				stackVariable2[31] = "CUInt";
				stackVariable2[32] = "CULng";
				stackVariable2[33] = "CUShort";
				stackVariable2[34] = "Date";
				stackVariable2[35] = "Decimal";
				stackVariable2[36] = "Declare";
				stackVariable2[37] = "Default";
				stackVariable2[38] = "Delegate";
				stackVariable2[39] = "Dim";
				stackVariable2[40] = "DirectCast";
				stackVariable2[41] = "Do";
				stackVariable2[42] = "Double";
				stackVariable2[43] = "Each";
				stackVariable2[44] = "Else";
				stackVariable2[45] = "ElseIf";
				stackVariable2[46] = "End";
				stackVariable2[47] = "EndIf";
				stackVariable2[48] = "Enum";
				stackVariable2[49] = "Erase";
				stackVariable2[50] = "Error";
				stackVariable2[51] = "Event";
				stackVariable2[52] = "Exit";
				stackVariable2[53] = "False";
				stackVariable2[54] = "Finally";
				stackVariable2[55] = "For";
				stackVariable2[56] = "Friend";
				stackVariable2[57] = "Function";
				stackVariable2[58] = "Get";
				stackVariable2[59] = "GetType";
				stackVariable2[60] = "GetXMLNamespace";
				stackVariable2[61] = "Global";
				stackVariable2[62] = "GoSub";
				stackVariable2[63] = "GoTo";
				stackVariable2[64] = "Handles";
				stackVariable2[65] = "If";
				stackVariable2[66] = "Implements";
				stackVariable2[67] = "Imports";
				stackVariable2[68] = "In";
				stackVariable2[69] = "Inherits";
				stackVariable2[70] = "Integer";
				stackVariable2[71] = "Interface";
				stackVariable2[72] = "Is";
				stackVariable2[73] = "IsNot";
				stackVariable2[74] = "Let";
				stackVariable2[75] = "Lib";
				stackVariable2[76] = "Like";
				stackVariable2[77] = "Long";
				stackVariable2[78] = "Loop";
				stackVariable2[79] = "Me";
				stackVariable2[80] = "Mod";
				stackVariable2[81] = "Module";
				stackVariable2[82] = "MustInherit";
				stackVariable2[83] = "MustOverride";
				stackVariable2[84] = "MyBase";
				stackVariable2[85] = "MyClass";
				stackVariable2[86] = "Namespace";
				stackVariable2[87] = "Narrowing";
				stackVariable2[88] = "New";
				stackVariable2[89] = "Next";
				stackVariable2[90] = "Not";
				stackVariable2[91] = "Nothing";
				stackVariable2[92] = "NotInheritable";
				stackVariable2[93] = "NotOverridable";
				stackVariable2[94] = "Object";
				stackVariable2[95] = "Of";
				stackVariable2[96] = "On";
				stackVariable2[97] = "Operator";
				stackVariable2[98] = "Option";
				stackVariable2[99] = "Optional";
				stackVariable2[100] = "Or";
				stackVariable2[101] = "OrElse";
				stackVariable2[102] = "Overloads";
				stackVariable2[103] = "Overridable";
				stackVariable2[104] = "Overrides";
				stackVariable2[105] = "ParamArray";
				stackVariable2[106] = "Partial";
				stackVariable2[107] = "Private";
				stackVariable2[108] = "Property";
				stackVariable2[109] = "Protected";
				stackVariable2[110] = "Public";
				stackVariable2[111] = "RaiseEvent";
				stackVariable2[112] = "ReadOnly";
				stackVariable2[113] = "ReDim";
				stackVariable2[114] = "REM";
				stackVariable2[115] = "RemoveHandler";
				stackVariable2[116] = "Resume";
				stackVariable2[117] = "Return";
				stackVariable2[118] = "SByte";
				stackVariable2[119] = "Select";
				stackVariable2[120] = "Set";
				stackVariable2[121] = "Shadows";
				stackVariable2[122] = "Shared";
				stackVariable2[123] = "Short";
				stackVariable2[124] = "Single";
				stackVariable2[125] = "Static";
				stackVariable2[126] = "Step";
				stackVariable2[127] = "Stop";
				stackVariable2[128] = "String";
				stackVariable2[129] = "Structure";
				stackVariable2[130] = "Sub";
				stackVariable2[131] = "SyncLock";
				stackVariable2[132] = "Then";
				stackVariable2[133] = "Throw";
				stackVariable2[134] = "To";
				stackVariable2[135] = "True";
				stackVariable2[136] = "Try";
				stackVariable2[137] = "TryCast";
				stackVariable2[138] = "TypeOf";
				stackVariable2[139] = "UInteger";
				stackVariable2[140] = "ULong";
				stackVariable2[141] = "UShort";
				stackVariable2[142] = "Using";
				stackVariable2[143] = "Variant";
				stackVariable2[144] = "Wend";
				stackVariable2[145] = "When";
				stackVariable2[146] = "While";
				stackVariable2[147] = "Widening";
				stackVariable2[148] = "With";
				stackVariable2[149] = "WithEvents";
				stackVariable2[150] = "WriteOnly";
				stackVariable2[151] = "Xor";
				stackVariable2[152] = "#Const";
				stackVariable2[153] = "#Else";
				stackVariable2[154] = "#ElseIf";
				stackVariable2[155] = "#End";
				stackVariable2[156] = "#If";
				V_0 = stackVariable2;
				V_1 = 0;
				while (V_1 < (int)V_0.Length)
				{
					V_2 = V_0[V_1];
					dummyVar0 = this.languageSpecificGlobalKeywords.Add(V_2);
					V_1 = V_1 + 1;
				}
				return;
			}

			internal override IDecompilationStep[] LanguageDecompilationSteps(bool inlineAggressively)
			{
				stackVariable1 = new IDecompilationStep[42];
				stackVariable1[0] = new RebuildAsyncStatementsStep();
				stackVariable1[1] = new RebuildYieldStatementsStep();
				stackVariable1[2] = new VisualBasicRemoveDelegateCachingStep();
				stackVariable1[3] = new RebuildAnonymousDelegatesStep();
				stackVariable1[4] = new RebuildLambdaExpressions();
				stackVariable1[5] = new GotoCancelation();
				stackVariable1[6] = new CombinedTransformerStep();
				stackVariable1[7] = new MergeUnaryAndBinaryExpression();
				stackVariable1[8] = new RemoveLastReturn();
				stackVariable1[9] = new RebuildSwitchByString();
				stackVariable1[10] = new RebuildForeachStatements();
				stackVariable1[11] = new RebuildForeachArrayStatements();
				stackVariable1[12] = new RebuildVBForStatements();
				stackVariable1[13] = new RebuildDoWhileStatements();
				stackVariable1[14] = new RebuildLockStatements();
				stackVariable1[15] = new RebuildFixedStatements();
				stackVariable1[16] = new RebuildUsingStatements();
				stackVariable1[17] = new RenameEnumValues();
				stackVariable1[18] = new FixMethodOverloadsStep();
				stackVariable1[19] = new DetermineCtorInvocationStep();
				stackVariable1[20] = new RebuildExpressionTreesStep();
				stackVariable1[21] = new TransformMemberHandlersStep();
				stackVariable1[22] = new VBSelfAssignment();
				stackVariable1[23] = new VBCodePatternsStep(inlineAggressively);
				stackVariable1[24] = new TransformCatchClausesFilterExpressionStep();
				stackVariable1[25] = new DeduceImplicitDelegates();
				stackVariable1[26] = new CreateIfElseIfStatementsStep();
				stackVariable1[27] = new CreateCompilerOptimizedSwitchByStringStatementsStep();
				stackVariable1[28] = new ParenthesizeExpressionsStep();
				stackVariable1[29] = new VisualBasicRemoveUnusedVariablesStep();
				stackVariable1[30] = new RebuildCatchClausesFilterStep();
				stackVariable1[31] = new DeclareVariablesOnFirstAssignment();
				stackVariable1[32] = new DeclareTopLevelVariables();
				stackVariable1[33] = new RenameSplitPropertiesMethodsAndBackingFields();
				stackVariable1[34] = new RenameVBVariables();
				stackVariable1[35] = new CastEnumsToIntegersStep();
				stackVariable1[36] = new CastIntegersStep();
				stackVariable1[37] = new ArrayVariablesStep();
				stackVariable1[38] = new UnsafeMethodBodyStep();
				stackVariable1[39] = new DetermineDestructorStep();
				stackVariable1[40] = new DependsOnAnalysisStep();
				stackVariable1[41] = new DetermineNotSupportedVBCodeStep();
				return stackVariable1;
			}

			protected override IDecompilationStep[] LanguageFilterMethodDecompilationSteps(bool inlineAggressively)
			{
				stackVariable1 = new IDecompilationStep[11];
				stackVariable1[0] = new DeclareVariablesOnFirstAssignment();
				stackVariable1[1] = new DeclareTopLevelVariables();
				stackVariable1[2] = new RenameSplitPropertiesMethodsAndBackingFields();
				stackVariable1[3] = new RenameVBVariables();
				stackVariable1[4] = new CastEnumsToIntegersStep();
				stackVariable1[5] = new CastIntegersStep();
				stackVariable1[6] = new ArrayVariablesStep();
				stackVariable1[7] = new UnsafeMethodBodyStep();
				stackVariable1[8] = new DetermineDestructorStep();
				stackVariable1[9] = new DependsOnAnalysisStep();
				stackVariable1[10] = new DetermineNotSupportedVBCodeStep();
				return stackVariable1;
			}
		}
	}
}