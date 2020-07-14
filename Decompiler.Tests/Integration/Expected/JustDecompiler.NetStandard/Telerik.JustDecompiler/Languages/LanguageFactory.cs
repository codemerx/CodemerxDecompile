using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Decompiler.GotoElimination;
using Telerik.JustDecompiler.Decompiler.Inlining;
using Telerik.JustDecompiler.Languages.CSharp;
using Telerik.JustDecompiler.Languages.VisualBasic;
using Telerik.JustDecompiler.Steps;
using Telerik.JustDecompiler.Steps.SwitchByString;

namespace Telerik.JustDecompiler.Languages
{
	public static class LanguageFactory
	{
		public static ILanguage GetLanguage(CSharpVersion version)
		{
			switch (version)
			{
				case CSharpVersion.None:
				{
					return LanguageFactory.CSharp.Instance;
				}
				case CSharpVersion.V5:
				{
					return LanguageFactory.CSharpV5.Instance;
				}
				case CSharpVersion.V6:
				{
					return LanguageFactory.CSharpV6.Instance;
				}
				case CSharpVersion.V7:
				{
					return LanguageFactory.CSharpV7.Instance;
				}
			}
			throw new ArgumentException();
		}

		public static ILanguage GetLanguage(VisualBasicVersion version)
		{
			if (version == VisualBasicVersion.None)
			{
				return LanguageFactory.VisualBasic.Instance;
			}
			if (version != VisualBasicVersion.V10)
			{
				throw new ArgumentException();
			}
			return LanguageFactory.VisualBasicV10.Instance;
		}

		private class CSharp : BaseLanguage, ICSharp
		{
			private static LanguageFactory.CSharp instance;

			private Dictionary<string, string> operators;

			public override HashSet<string> AttributesToHide
			{
				get
				{
					return new HashSet<string>(new String[] { "System.ParamArrayAttribute", "System.Runtime.CompilerServices.IteratorStateMachineAttribute", "Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute", "Windows.Foundation.Metadata.ActivatableAttribute", "System.Runtime.CompilerServices.DynamicAttribute", "System.Runtime.CompilerServices.ExtensionAttribute", "System.Diagnostics.DebuggerStepThroughAttribute", "System.Runtime.CompilerServices.AsyncStateMachineAttribute", "System.Runtime.CompilerServices.CompilerGeneratedAttribute" });
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
					return String.Concat("C#", this.Version.ToString());
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
			}

			protected CSharp()
			{
				this.operators = new Dictionary<string, string>();
				this.InitializeOperators();
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
				string[,] strArray = new string[29, 2];
				strArray[0, 0] = "Decrement";
				strArray[0, 1] = "--";
				strArray[1, 0] = "Increment";
				strArray[1, 1] = "++";
				strArray[2, 0] = "UnaryNegation";
				strArray[2, 1] = "-";
				strArray[3, 0] = "UnaryPlus";
				strArray[3, 1] = "+";
				strArray[4, 0] = "LogicalNot";
				strArray[4, 1] = "!";
				strArray[5, 0] = "OnesComplement";
				strArray[5, 1] = "~";
				strArray[6, 0] = "True";
				strArray[6, 1] = "true";
				strArray[7, 0] = "False";
				strArray[7, 1] = "false";
				strArray[8, 0] = "Addition";
				strArray[8, 1] = "+";
				strArray[9, 0] = "Subtraction";
				strArray[9, 1] = "-";
				strArray[10, 0] = "Multiply";
				strArray[10, 1] = "*";
				strArray[11, 0] = "Division";
				strArray[11, 1] = "/";
				strArray[12, 0] = "Modulus";
				strArray[12, 1] = "%";
				strArray[13, 0] = "ExclusiveOr";
				strArray[13, 1] = "^";
				strArray[14, 0] = "BitwiseAnd";
				strArray[14, 1] = "&";
				strArray[15, 0] = "BitwiseOr";
				strArray[15, 1] = "|";
				strArray[16, 0] = "LeftShift";
				strArray[16, 1] = "<<";
				strArray[17, 0] = "RightShift";
				strArray[17, 1] = ">>";
				strArray[18, 0] = "Equality";
				strArray[18, 1] = "==";
				strArray[19, 0] = "GreaterThan";
				strArray[19, 1] = ">";
				strArray[20, 0] = "LessThan";
				strArray[20, 1] = "<";
				strArray[21, 0] = "Inequality";
				strArray[21, 1] = "!=";
				strArray[22, 0] = "GreaterThanOrEqual";
				strArray[22, 1] = ">=";
				strArray[23, 0] = "LessThanOrEqual";
				strArray[23, 1] = "<=";
				strArray[24, 0] = "MemberSelection";
				strArray[24, 1] = "->";
				strArray[25, 0] = "PointerToMemberSelection";
				strArray[25, 1] = "->*";
				strArray[26, 0] = "Comma";
				strArray[26, 1] = ",";
				strArray[27, 0] = "Implicit";
				strArray[27, 1] = "";
				strArray[28, 0] = "Explicit";
				strArray[28, 1] = "";
				string[,] strArray1 = strArray;
				for (int i = 0; i < strArray1.GetLength(0); i++)
				{
					this.operators.Add(strArray1[i, 0], strArray1[i, 1]);
				}
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

			public override bool IsOperatorKeyword(string @operator)
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
			}

			protected CSharpV5()
			{
				int i;
				string[] strArray = new String[] { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while" };
				for (i = 0; i < (int)strArray.Length; i++)
				{
					string str = strArray[i];
					this.languageSpecificGlobalKeywords.Add(str);
				}
				strArray = new String[] { "add", "alias", "ascending", "async", "await", "descending", "dynamic", "from", "get", "global", "group", "into", "join", "let", "orderby", "partial", "remove", "select", "set", "value", "var", "where", "yield" };
				for (i = 0; i < (int)strArray.Length; i++)
				{
					string str1 = strArray[i];
					this.languageSpecificContextualKeywords.Add(str1);
				}
			}

			internal override IDecompilationStep[] LanguageDecompilationSteps(bool inlineAggressively)
			{
				return new IDecompilationStep[] { new OutParameterAssignmentAnalysisStep(), new RebuildAsyncStatementsStep(), new RebuildYieldStatementsStep(), new RemoveDelegateCachingStep(), new RebuildAnonymousDelegatesStep(), new RebuildLambdaExpressions(), new ResolveDynamicVariablesStep(), new GotoCancelation(), new CombinedTransformerStep(), new MergeUnaryAndBinaryExpression(), new RemoveLastReturn(), new RebuildSwitchByString(), new RebuildForeachStatements(), new RebuildForeachArrayStatements(), new RebuildForStatements(), new RebuildLockStatements(), new RebuildFixedStatements(), new RebuildUsingStatements(), new RenameEnumValues(), new FixMethodOverloadsStep(), new RebuildExpressionTreesStep(), new TransformMemberHandlersStep(), new SelfAssignment(), new CodePatternsStep(inlineAggressively), new DetermineCtorInvocationStep(), new DeduceImplicitDelegates(), new RebuildLinqQueriesStep(), new CreateIfElseIfStatementsStep(), new CreateCompilerOptimizedSwitchByStringStatementsStep(), new ParenthesizeExpressionsStep(), new RemoveUnusedVariablesStep(), new DeclareVariablesOnFirstAssignment(), new DeclareTopLevelVariables(), new AssignOutParametersStep(), new RenameSplitPropertiesMethodsAndBackingFields(), new RenameVariables(), new CastEnumsToIntegersStep(), new CastIntegersStep(), new ArrayVariablesStep(), new CaseGotoTransformerStep(), new UnsafeMethodBodyStep(), new DetermineDestructorStep(), new DependsOnAnalysisStep() };
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
			}

			protected CSharpV6()
			{
			}

			internal override IDecompilationStep[] LanguageDecompilationSteps(bool inlineAggressively)
			{
				return new IDecompilationStep[] { new OutParameterAssignmentAnalysisStep(), new RebuildAsyncStatementsStep(), new RebuildYieldStatementsStep(), new RemoveDelegateCachingStep(), new RebuildAnonymousDelegatesStep(), new RebuildLambdaExpressions(), new ResolveDynamicVariablesStep(), new GotoCancelation(), new CombinedTransformerStep(), new MergeUnaryAndBinaryExpression(), new RemoveLastReturn(), new RebuildSwitchByString(), new RebuildForeachStatements(), new RebuildForeachArrayStatements(), new RebuildForStatements(), new RebuildLockStatements(), new RebuildFixedStatements(), new RebuildUsingStatements(), new RenameEnumValues(), new FixMethodOverloadsStep(), new RebuildExpressionTreesStep(), new TransformMemberHandlersStep(), new SelfAssignment(), new CodePatternsStep(inlineAggressively), new TransformCatchClausesFilterExpressionStep(), new DetermineCtorInvocationStep(), new DeduceImplicitDelegates(), new RebuildLinqQueriesStep(), new CreateIfElseIfStatementsStep(), new CreateCompilerOptimizedSwitchByStringStatementsStep(), new ParenthesizeExpressionsStep(), new RemoveUnusedVariablesStep(), new RebuildCatchClausesFilterStep(), new DeclareVariablesOnFirstAssignment(), new DeclareTopLevelVariables(), new AssignOutParametersStep(), new RenameSplitPropertiesMethodsAndBackingFields(), new RenameVariables(), new CastEnumsToIntegersStep(), new CastIntegersStep(), new ArrayVariablesStep(), new CaseGotoTransformerStep(), new UnsafeMethodBodyStep(), new DetermineDestructorStep(), new DependsOnAnalysisStep() };
			}

			protected override IDecompilationStep[] LanguageFilterMethodDecompilationSteps(bool inlineAggressively)
			{
				return new IDecompilationStep[] { new DeclareVariablesOnFirstAssignment(), new DeclareTopLevelVariables(), new AssignOutParametersStep(), new RenameSplitPropertiesMethodsAndBackingFields(), new RenameVariables(), new CastEnumsToIntegersStep(), new CastIntegersStep(), new ArrayVariablesStep(), new CaseGotoTransformerStep(), new UnsafeMethodBodyStep(), new DetermineDestructorStep(), new DependsOnAnalysisStep() };
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
			}

			protected CSharpV7()
			{
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
					return new HashSet<string>(new String[] { "System.ParamArrayAttribute", "System.Runtime.CompilerServices.IteratorStateMachineAttribute", "Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute", "System.Runtime.CompilerServices.ExtensionAttribute", "System.Diagnostics.DebuggerStepThroughAttribute", "System.Runtime.CompilerServices.AsyncStateMachineAttribute", "System.Runtime.CompilerServices.CompilerGeneratedAttribute" });
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
					return StringComparer.OrdinalIgnoreCase;
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
					return String.Concat("VB.NET", this.Version.ToString());
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
			}

			protected VisualBasic()
			{
				this.operators = new Dictionary<string, string>();
				this.operatorKeywords = new HashSet<string>();
				this.InitializeOperators();
			}

			private bool ExistsNonExplicitMember(IMemberDefinition explicitMember, string nonExplicitName)
			{
				if (explicitMember is MethodDefinition)
				{
					return (
						from t in explicitMember.get_DeclaringType().get_Methods()
						where this.GetMemberNonExplicitName(t) == nonExplicitName
						select t).Count<MethodDefinition>() > 1;
				}
				if (explicitMember is PropertyDefinition)
				{
					return (
						from t in explicitMember.get_DeclaringType().get_Properties()
						where this.GetMemberNonExplicitName(t) == nonExplicitName
						select t).Count<PropertyDefinition>() > 1;
				}
				if (!(explicitMember is EventDefinition))
				{
					return false;
				}
				return (
					from t in explicitMember.get_DeclaringType().get_Events()
					where this.GetMemberNonExplicitName(t) == nonExplicitName
					select t).Count<EventDefinition>() > 1;
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
				string memberNonExplicitName = this.GetMemberNonExplicitName(member);
				if (!this.ExistsNonExplicitMember(member, memberNonExplicitName))
				{
					return memberNonExplicitName;
				}
				return this.GetMemberSpecialExplicitName(member);
			}

			private string GetMemberNonExplicitName(IMemberDefinition member)
			{
				string name = member.get_Name();
				int num = name.LastIndexOf('.');
				if (num != -1)
				{
					name = name.Substring(num + 1);
				}
				return name;
			}

			private string GetMemberSpecialExplicitName(IMemberDefinition member)
			{
				string memberNonExplicitName = this.GetMemberNonExplicitName(member);
				if (!this.ExistsNonExplicitMember(member, memberNonExplicitName))
				{
					return memberNonExplicitName;
				}
				return String.Concat("Explicit", memberNonExplicitName);
			}

			public override ILanguageWriter GetWriter(IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
			{
				return new VisualBasicWriter(this, formatter, exceptionFormatter, settings);
			}

			private void InitializeOperators()
			{
				string[,] strArray = new string[31, 2];
				strArray[0, 0] = "UnaryNegation";
				strArray[0, 1] = "-";
				strArray[1, 0] = "UnaryPlus";
				strArray[1, 1] = "+";
				strArray[2, 0] = "LogicalNot";
				strArray[2, 1] = "Not";
				strArray[3, 0] = "True";
				strArray[3, 1] = "IsTrue";
				strArray[4, 0] = "False";
				strArray[4, 1] = "IsFalse";
				strArray[5, 0] = "AddressOf";
				strArray[5, 1] = "&";
				strArray[6, 0] = "OnesComplement";
				strArray[6, 1] = "Not";
				strArray[7, 0] = "PointerDereference";
				strArray[7, 1] = "*";
				strArray[8, 0] = "Addition";
				strArray[8, 1] = "+";
				strArray[9, 0] = "Subtraction";
				strArray[9, 1] = "-";
				strArray[10, 0] = "Multiply";
				strArray[10, 1] = "*";
				strArray[11, 0] = "Division";
				strArray[11, 1] = "/";
				strArray[12, 0] = "Modulus";
				strArray[12, 1] = "Mod";
				strArray[13, 0] = "ExclusiveOr";
				strArray[13, 1] = "Xor";
				strArray[14, 0] = "BitwiseAnd";
				strArray[14, 1] = "And";
				strArray[15, 0] = "BitwiseOr";
				strArray[15, 1] = "Or";
				strArray[16, 0] = "LogicalAnd";
				strArray[16, 1] = "AndAlso";
				strArray[17, 0] = "LogicalOr";
				strArray[17, 1] = "OrElse";
				strArray[18, 0] = "LeftShift";
				strArray[18, 1] = "<<";
				strArray[19, 0] = "RightShift";
				strArray[19, 1] = ">>";
				strArray[20, 0] = "Equality";
				strArray[20, 1] = "=";
				strArray[21, 0] = "GreaterThan";
				strArray[21, 1] = ">";
				strArray[22, 0] = "LessThan";
				strArray[22, 1] = "<";
				strArray[23, 0] = "Inequality";
				strArray[23, 1] = "<>";
				strArray[24, 0] = "GreaterThanOrEqual";
				strArray[24, 1] = ">=";
				strArray[25, 0] = "LessThanOrEqual";
				strArray[25, 1] = "<=";
				strArray[26, 0] = "MemberSelection";
				strArray[26, 1] = "->";
				strArray[27, 0] = "PointerToMemberSelection";
				strArray[27, 1] = "->*";
				strArray[28, 0] = "Comma";
				strArray[28, 1] = ",";
				strArray[29, 0] = "Implicit";
				strArray[29, 1] = "CType";
				strArray[30, 0] = "Explicit";
				strArray[30, 1] = "CType";
				string[,] strArray1 = strArray;
				for (int i = 0; i < strArray1.GetLength(0); i++)
				{
					this.operators.Add(strArray1[i, 0], strArray1[i, 1]);
				}
				string[] strArray2 = new String[] { "And", "Or", "Xor", "AndAlso", "OrElse", "Mod", "Is", "IsNot", "Not" };
				for (int j = 0; j < (int)strArray2.Length; j++)
				{
					this.operatorKeywords.Add(strArray2[j]);
				}
			}

			protected override bool IsGlobalKeyword(string word, HashSet<string> globalKeywords)
			{
				bool flag;
				HashSet<string>.Enumerator enumerator = globalKeywords.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.Equals(word, StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}
						flag = true;
						return flag;
					}
					return false;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return flag;
			}

			protected override bool IsLanguageKeyword(string word, HashSet<string> globalKeywords, HashSet<string> contextKeywords)
			{
				bool flag;
				foreach (string globalKeyword in globalKeywords)
				{
					if (!globalKeyword.Equals(word, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				HashSet<string>.Enumerator enumerator = contextKeywords.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.Equals(word, StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}
						flag = true;
						return flag;
					}
					return false;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return flag;
			}

			public override bool IsOperatorKeyword(string @operator)
			{
				return this.operatorKeywords.Contains(@operator);
			}

			public override bool IsValidLineStarter(CodeNodeType nodeType)
			{
				if (nodeType == CodeNodeType.FieldReferenceExpression || nodeType == CodeNodeType.PropertyReferenceExpression || nodeType == CodeNodeType.MethodInvocationExpression || nodeType == CodeNodeType.SafeCastExpression || nodeType == CodeNodeType.ExplicitCastExpression || nodeType == CodeNodeType.ThisReferenceExpression || nodeType == CodeNodeType.BaseReferenceExpression || nodeType == CodeNodeType.VariableReferenceExpression || nodeType == CodeNodeType.ArgumentReferenceExpression || nodeType == CodeNodeType.TypeOfExpression)
				{
					return true;
				}
				return nodeType == CodeNodeType.ArrayIndexerExpression;
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
			}

			protected VisualBasicV10()
			{
				string[] strArray = new String[] { "AddHandler", "AddressOf", "Alias", "And", "AndAlso", "As", "Boolean", "ByRef", "Byte", "ByVal", "Call", "Case", "Catch", "CBool", "CByte", "CChar", "CDate", "CDbl", "CDec", "Char", "CInt", "Class", "CLng", "CObj", "Const", "Continue", "CSByte", "CShort", "CSng", "CStr", "CType", "CUInt", "CULng", "CUShort", "Date", "Decimal", "Declare", "Default", "Delegate", "Dim", "DirectCast", "Do", "Double", "Each", "Else", "ElseIf", "End", "EndIf", "Enum", "Erase", "Error", "Event", "Exit", "False", "Finally", "For", "Friend", "Function", "Get", "GetType", "GetXMLNamespace", "Global", "GoSub", "GoTo", "Handles", "If", "Implements", "Imports", "In", "Inherits", "Integer", "Interface", "Is", "IsNot", "Let", "Lib", "Like", "Long", "Loop", "Me", "Mod", "Module", "MustInherit", "MustOverride", "MyBase", "MyClass", "Namespace", "Narrowing", "New", "Next", "Not", "Nothing", "NotInheritable", "NotOverridable", "Object", "Of", "On", "Operator", "Option", "Optional", "Or", "OrElse", "Overloads", "Overridable", "Overrides", "ParamArray", "Partial", "Private", "Property", "Protected", "Public", "RaiseEvent", "ReadOnly", "ReDim", "REM", "RemoveHandler", "Resume", "Return", "SByte", "Select", "Set", "Shadows", "Shared", "Short", "Single", "Static", "Step", "Stop", "String", "Structure", "Sub", "SyncLock", "Then", "Throw", "To", "True", "Try", "TryCast", "TypeOf", "UInteger", "ULong", "UShort", "Using", "Variant", "Wend", "When", "While", "Widening", "With", "WithEvents", "WriteOnly", "Xor", "#Const", "#Else", "#ElseIf", "#End", "#If" };
				for (int i = 0; i < (int)strArray.Length; i++)
				{
					string str = strArray[i];
					this.languageSpecificGlobalKeywords.Add(str);
				}
			}

			internal override IDecompilationStep[] LanguageDecompilationSteps(bool inlineAggressively)
			{
				return new IDecompilationStep[] { new RebuildAsyncStatementsStep(), new RebuildYieldStatementsStep(), new VisualBasicRemoveDelegateCachingStep(), new RebuildAnonymousDelegatesStep(), new RebuildLambdaExpressions(), new GotoCancelation(), new CombinedTransformerStep(), new MergeUnaryAndBinaryExpression(), new RemoveLastReturn(), new RebuildSwitchByString(), new RebuildForeachStatements(), new RebuildForeachArrayStatements(), new RebuildVBForStatements(), new RebuildDoWhileStatements(), new RebuildLockStatements(), new RebuildFixedStatements(), new RebuildUsingStatements(), new RenameEnumValues(), new FixMethodOverloadsStep(), new DetermineCtorInvocationStep(), new RebuildExpressionTreesStep(), new TransformMemberHandlersStep(), new VBSelfAssignment(), new VBCodePatternsStep(inlineAggressively), new TransformCatchClausesFilterExpressionStep(), new DeduceImplicitDelegates(), new CreateIfElseIfStatementsStep(), new CreateCompilerOptimizedSwitchByStringStatementsStep(), new ParenthesizeExpressionsStep(), new VisualBasicRemoveUnusedVariablesStep(), new RebuildCatchClausesFilterStep(), new DeclareVariablesOnFirstAssignment(), new DeclareTopLevelVariables(), new RenameSplitPropertiesMethodsAndBackingFields(), new RenameVBVariables(), new CastEnumsToIntegersStep(), new CastIntegersStep(), new ArrayVariablesStep(), new UnsafeMethodBodyStep(), new DetermineDestructorStep(), new DependsOnAnalysisStep(), new DetermineNotSupportedVBCodeStep() };
			}

			protected override IDecompilationStep[] LanguageFilterMethodDecompilationSteps(bool inlineAggressively)
			{
				return new IDecompilationStep[] { new DeclareVariablesOnFirstAssignment(), new DeclareTopLevelVariables(), new RenameSplitPropertiesMethodsAndBackingFields(), new RenameVBVariables(), new CastEnumsToIntegersStep(), new CastIntegersStep(), new ArrayVariablesStep(), new UnsafeMethodBodyStep(), new DetermineDestructorStep(), new DependsOnAnalysisStep(), new DetermineNotSupportedVBCodeStep() };
			}
		}
	}
}