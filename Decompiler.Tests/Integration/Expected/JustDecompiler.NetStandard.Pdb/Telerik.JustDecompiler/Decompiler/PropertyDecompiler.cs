using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler
{
	internal class PropertyDecompiler : ExceptionThrownNotifier, IExceptionThrownNotifier
	{
		private readonly PropertyDefinition propertyDef;

		private FieldDefinition propertyFieldDef;

		private readonly ILanguage language;

		private readonly bool renameInvalidMembers;

		private readonly IDecompilationCacheService cacheService;

		private readonly TypeSpecificContext typeContext;

		public ICollection<MethodDefinition> ExceptionsWhileDecompiling
		{
			get;
			private set;
		}

		private bool IsCachingEnabled
		{
			get
			{
				return this.cacheService != null;
			}
		}

		public PropertyDecompiler(PropertyDefinition property, ILanguage language, bool renameInvalidMembers, IDecompilationCacheService cacheService, TypeSpecificContext typeContext = null)
		{
			base();
			this.propertyDef = property;
			this.language = language;
			this.renameInvalidMembers = renameInvalidMembers;
			this.cacheService = cacheService;
			this.typeContext = typeContext;
			this.propertyFieldDef = null;
			this.set_ExceptionsWhileDecompiling(new List<MethodDefinition>());
			return;
		}

		public PropertyDecompiler(PropertyDefinition property, ILanguage language, TypeSpecificContext typeContext = null)
		{
			this(property, language, false, null, typeContext);
			return;
		}

		private bool CheckFieldReferenceExpression(FieldReferenceExpression fieldRefExpression)
		{
			if (fieldRefExpression.get_Field() == null)
			{
				return false;
			}
			if (this.propertyFieldDef != null)
			{
				return (object)fieldRefExpression.get_Field().Resolve() == (object)this.propertyFieldDef;
			}
			V_0 = fieldRefExpression.get_Field().Resolve();
			if (V_0 == null || (object)V_0.get_DeclaringType() != (object)this.propertyDef.get_DeclaringType())
			{
				return false;
			}
			if (!V_0.HasCompilerGeneratedAttribute())
			{
				return false;
			}
			this.propertyFieldDef = V_0;
			return true;
		}

		private bool CheckGetter(BlockStatement getterStatements)
		{
			if (getterStatements == null || getterStatements.get_Statements() == null || getterStatements.get_Statements().get_Count() != 1 && getterStatements.get_Statements().get_Count() != 2 || getterStatements.get_Statements().get_Item(0).get_CodeNodeType() != 5)
			{
				return false;
			}
			if (getterStatements.get_Statements().get_Count() != 1)
			{
				V_2 = (getterStatements.get_Statements().get_Item(0) as ExpressionStatement).get_Expression() as BinaryExpression;
				if (V_2 == null || !V_2.get_IsAssignmentExpression() || V_2.get_Left().get_CodeNodeType() != 26 || V_2.get_Right().get_CodeNodeType() != 30)
				{
					return false;
				}
				if (getterStatements.get_Statements().get_Item(1).get_CodeNodeType() != 5)
				{
					return false;
				}
				V_3 = (getterStatements.get_Statements().get_Item(1) as ExpressionStatement).get_Expression() as ReturnExpression;
				if (V_3 == null || V_3.get_Value() == null || V_3.get_Value().get_CodeNodeType() != 26)
				{
					return false;
				}
				V_0 = V_2.get_Right() as FieldReferenceExpression;
			}
			else
			{
				V_1 = (getterStatements.get_Statements().get_Item(0) as ExpressionStatement).get_Expression() as ReturnExpression;
				if (V_1 == null || V_1.get_Value() == null || V_1.get_Value().get_CodeNodeType() != 30)
				{
					return false;
				}
				V_0 = V_1.get_Value() as FieldReferenceExpression;
			}
			return this.CheckFieldReferenceExpression(V_0);
		}

		private bool CheckSetter(BlockStatement setterStatements)
		{
			if (setterStatements == null || setterStatements.get_Statements() == null || setterStatements.get_Statements().get_Count() != 2 || setterStatements.get_Statements().get_Item(0).get_CodeNodeType() != 5 || setterStatements.get_Statements().get_Item(1).get_CodeNodeType() != 5)
			{
				return false;
			}
			V_0 = (setterStatements.get_Statements().get_Item(1) as ExpressionStatement).get_Expression() as ReturnExpression;
			if (V_0 == null || V_0.get_Value() != null)
			{
				return false;
			}
			V_1 = (setterStatements.get_Statements().get_Item(0) as ExpressionStatement).get_Expression() as BinaryExpression;
			if (V_1 == null || !V_1.get_IsAssignmentExpression() || V_1.get_Left().get_CodeNodeType() != 30 || V_1.get_Right().get_CodeNodeType() != 25)
			{
				return false;
			}
			return this.CheckFieldReferenceExpression(V_1.get_Left() as FieldReferenceExpression);
		}

		public void Decompile(out CachedDecompiledMember getMethod, out CachedDecompiledMember setMethod, out bool isAutoImplemented)
		{
			getMethod = null;
			setMethod = null;
			isAutoImplemented = false;
			if (this.propertyDef.get_GetMethod() == null || this.propertyDef.get_GetMethod().get_Parameters().get_Count() != 0 || !this.propertyDef.get_GetMethod().get_HasBody() || this.propertyDef.get_OtherMethods().get_Count() != 0)
			{
				getMethod = this.DecompileMember(this.propertyDef.get_GetMethod());
				setMethod = this.DecompileMember(this.propertyDef.get_SetMethod());
				isAutoImplemented = false;
				return;
			}
			if (this.propertyDef.get_SetMethod() == null)
			{
				if (this.language.get_SupportsGetterOnlyAutoProperties())
				{
					isAutoImplemented = this.DecompileAndCheckForAutoImplementedPropertyMethod(this.propertyDef.get_GetMethod(), out getMethod, true, new Func<BlockStatement, bool>(this.CheckGetter));
					return;
				}
				getMethod = this.DecompileMember(this.propertyDef.get_GetMethod());
				setMethod = this.DecompileMember(this.propertyDef.get_SetMethod());
				isAutoImplemented = false;
				return;
			}
			if (this.propertyDef.get_SetMethod().get_Parameters().get_Count() != 1 || !this.propertyDef.get_SetMethod().get_HasBody())
			{
				getMethod = this.DecompileMember(this.propertyDef.get_GetMethod());
				setMethod = this.DecompileMember(this.propertyDef.get_SetMethod());
				isAutoImplemented = false;
				return;
			}
			isAutoImplemented = this.DecompileAndCheckForAutoImplementedPropertyMethod(this.propertyDef.get_GetMethod(), out getMethod, true, new Func<BlockStatement, bool>(this.CheckGetter)) & this.DecompileAndCheckForAutoImplementedPropertyMethod(this.propertyDef.get_SetMethod(), out setMethod, true, new Func<BlockStatement, bool>(this.CheckSetter));
			return;
		}

		private bool DecompileAndCheckForAutoImplementedPropertyMethod(MethodDefinition method, out CachedDecompiledMember decompiledMember, bool needDecompiledMember, Func<BlockStatement, bool> checker)
		{
			decompiledMember = null;
			V_1 = this.DecompileMethodPartially(method.get_Body(), out V_0, needDecompiledMember);
			if (V_1 == null && V_0 == null)
			{
				if (needDecompiledMember)
				{
					decompiledMember = this.DecompileMember(method);
				}
				return false;
			}
			if (V_1.get_Statements().get_Count() == 1 && V_1.get_Statements().get_Item(0).get_CodeNodeType() == 67)
			{
				if (needDecompiledMember)
				{
					decompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), V_1, new MethodSpecificContext(method.get_Body())));
				}
				return false;
			}
			if (!checker.Invoke(V_1))
			{
				if (needDecompiledMember)
				{
					decompiledMember = this.FinishDecompilationOfMember(method, V_1, V_0);
				}
				return false;
			}
			if (needDecompiledMember)
			{
				if (!this.propertyDef.ShouldStaySplit())
				{
					decompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), V_1, V_0.get_MethodContext()));
				}
				else
				{
					decompiledMember = this.FinishDecompilationOfMember(method, V_1, V_0);
				}
			}
			return true;
		}

		private CachedDecompiledMember DecompileMember(MethodDefinition method)
		{
			V_0 = null;
			if (method != null)
			{
				if (method.get_Body() != null)
				{
					if (this.get_IsCachingEnabled() && this.cacheService.IsDecompiledMemberInCache(method, this.language, this.renameInvalidMembers))
					{
						return this.cacheService.GetDecompiledMemberFromCache(method, this.language, this.renameInvalidMembers);
					}
					V_2 = this.DecompileMethod(method.get_Body(), out V_1);
					V_0 = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), V_2, V_1));
					if (this.get_IsCachingEnabled())
					{
						this.cacheService.AddDecompiledMemberToCache(method, this.language, this.renameInvalidMembers, V_0);
					}
				}
				else
				{
					V_0 = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), null, null));
				}
			}
			return V_0;
		}

		private BlockStatement DecompileMethod(MethodBody body, out MethodSpecificContext methodContext)
		{
			methodContext = null;
			try
			{
				stackVariable3 = new MethodSpecificContext(body);
				stackVariable5 = this.typeContext;
				if (stackVariable5 == null)
				{
					dummyVar0 = stackVariable5;
					stackVariable5 = new TypeSpecificContext(body.get_Method().get_DeclaringType());
				}
				V_1 = new DecompilationContext(stackVariable3, stackVariable5, this.language);
				V_2 = this.language.CreatePipeline(V_1);
				methodContext = V_2.Run(body, this.language).get_MethodContext();
				V_0 = V_2.get_Body();
			}
			catch (Exception exception_0)
			{
				V_3 = exception_0;
				this.get_ExceptionsWhileDecompiling().Add(body.get_Method());
				methodContext = new MethodSpecificContext(body);
				V_0 = new BlockStatement();
				V_0.AddStatement(new ExceptionStatement(V_3, body.get_Method()));
				this.OnExceptionThrown(V_3);
			}
			return V_0;
		}

		private BlockStatement DecompileMethodPartially(MethodBody body, out DecompilationContext context, bool needDecompiledMember = false)
		{
			context = null;
			if (this.get_IsCachingEnabled() && this.cacheService.IsDecompiledMemberInCache(body.get_Method(), this.language, this.renameInvalidMembers))
			{
				return this.cacheService.GetDecompiledMemberFromCache(body.get_Method(), this.language, this.renameInvalidMembers).get_Member().get_Statement() as BlockStatement;
			}
			if ((int)(new ControlFlowGraphBuilder(body.get_Method())).CreateGraph().get_Blocks().Length > 2)
			{
				return null;
			}
			try
			{
				stackVariable13 = new MethodSpecificContext(body);
				stackVariable15 = this.typeContext;
				if (stackVariable15 == null)
				{
					dummyVar0 = stackVariable15;
					stackVariable15 = new TypeSpecificContext(body.get_Method().get_DeclaringType());
				}
				V_2 = new DecompilationContext(stackVariable13, stackVariable15, this.language);
				if (!needDecompiledMember)
				{
					V_2.get_MethodContext().set_EnableEventAnalysis(false);
				}
				V_1 = new DecompilationPipeline(BaseLanguage.get_IntermediateRepresenationPipeline().get_Steps(), V_2);
				context = V_1.Run(body, this.language);
				V_0 = V_1.get_Body();
			}
			catch (Exception exception_0)
			{
				V_3 = exception_0;
				this.get_ExceptionsWhileDecompiling().Add(body.get_Method());
				V_0 = new BlockStatement();
				V_0.AddStatement(new ExceptionStatement(V_3, body.get_Method()));
				this.OnExceptionThrown(V_3);
			}
			return V_0;
		}

		private CachedDecompiledMember FinishDecompilationOfMember(MethodDefinition method, BlockStatement block, DecompilationContext context)
		{
			if (this.get_IsCachingEnabled() && this.cacheService.IsDecompiledMemberInCache(method, this.language, this.renameInvalidMembers))
			{
				return this.cacheService.GetDecompiledMemberFromCache(method, this.language, this.renameInvalidMembers);
			}
			V_1 = this.FinishDecompilationOfMethod(block, context, out V_0);
			V_2 = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), V_1, V_0));
			if (this.get_IsCachingEnabled())
			{
				this.cacheService.AddDecompiledMemberToCache(method, this.language, this.renameInvalidMembers, V_2);
			}
			return V_2;
		}

		private BlockStatement FinishDecompilationOfMethod(BlockStatement block, DecompilationContext context, out MethodSpecificContext methodContext)
		{
			methodContext = null;
			try
			{
				V_1 = this.language.CreatePropertyPipeline(context);
				methodContext = V_1.Run(context.get_MethodContext().get_Method().get_Body(), block, this.language).get_MethodContext();
				V_0 = V_1.get_Body();
			}
			catch (Exception exception_0)
			{
				V_2 = exception_0;
				this.get_ExceptionsWhileDecompiling().Add(context.get_MethodContext().get_Method());
				methodContext = new MethodSpecificContext(context.get_MethodContext().get_Method().get_Body());
				V_0 = new BlockStatement();
				V_0.AddStatement(new ExceptionStatement(V_2, context.get_MethodContext().get_Method()));
				this.OnExceptionThrown(V_2);
			}
			return V_0;
		}

		public bool IsAutoImplemented(out FieldDefinition propertyField)
		{
			stackVariable1 = this.IsAutoImplemented();
			propertyField = this.propertyFieldDef;
			return stackVariable1;
		}

		public bool IsAutoImplemented()
		{
			if (this.propertyDef.get_GetMethod() == null || this.propertyDef.get_GetMethod().get_Parameters().get_Count() != 0 || !this.propertyDef.get_GetMethod().get_HasBody() || this.propertyDef.get_OtherMethods().get_Count() != 0)
			{
				return false;
			}
			if (this.propertyDef.get_SetMethod() == null)
			{
				if (!this.language.get_SupportsGetterOnlyAutoProperties())
				{
					return false;
				}
				return this.DecompileAndCheckForAutoImplementedPropertyMethod(this.propertyDef.get_GetMethod(), out V_2, false, new Func<BlockStatement, bool>(this.CheckGetter));
			}
			if (this.propertyDef.get_SetMethod().get_Parameters().get_Count() != 1 || !this.propertyDef.get_SetMethod().get_HasBody())
			{
				return false;
			}
			if (!this.DecompileAndCheckForAutoImplementedPropertyMethod(this.propertyDef.get_GetMethod(), out V_0, false, new Func<BlockStatement, bool>(this.CheckGetter)))
			{
				return false;
			}
			return this.DecompileAndCheckForAutoImplementedPropertyMethod(this.propertyDef.get_SetMethod(), out V_1, false, new Func<BlockStatement, bool>(this.CheckSetter));
		}
	}
}