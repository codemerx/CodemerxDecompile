using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
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
			this.propertyDef = property;
			this.language = language;
			this.renameInvalidMembers = renameInvalidMembers;
			this.cacheService = cacheService;
			this.typeContext = typeContext;
			this.propertyFieldDef = null;
			this.ExceptionsWhileDecompiling = new List<MethodDefinition>();
		}

		public PropertyDecompiler(PropertyDefinition property, ILanguage language, TypeSpecificContext typeContext = null) : this(property, language, false, null, typeContext)
		{
		}

		private bool CheckFieldReferenceExpression(FieldReferenceExpression fieldRefExpression)
		{
			if (fieldRefExpression.Field == null)
			{
				return false;
			}
			if (this.propertyFieldDef != null)
			{
				return fieldRefExpression.Field.Resolve() == this.propertyFieldDef;
			}
			FieldDefinition fieldDefinition = fieldRefExpression.Field.Resolve();
			if (fieldDefinition == null || fieldDefinition.DeclaringType != this.propertyDef.DeclaringType)
			{
				return false;
			}
			if (!fieldDefinition.HasCompilerGeneratedAttribute())
			{
				return false;
			}
			this.propertyFieldDef = fieldDefinition;
			return true;
		}

		private bool CheckGetter(BlockStatement getterStatements)
		{
			FieldReferenceExpression right;
			if (getterStatements == null || getterStatements.Statements == null || getterStatements.Statements.Count != 1 && getterStatements.Statements.Count != 2 || getterStatements.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				return false;
			}
			if (getterStatements.Statements.Count != 1)
			{
				BinaryExpression expression = (getterStatements.Statements[0] as ExpressionStatement).Expression as BinaryExpression;
				if (expression == null || !expression.IsAssignmentExpression || expression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || expression.Right.CodeNodeType != CodeNodeType.FieldReferenceExpression)
				{
					return false;
				}
				if (getterStatements.Statements[1].CodeNodeType != CodeNodeType.ExpressionStatement)
				{
					return false;
				}
				ReturnExpression returnExpression = (getterStatements.Statements[1] as ExpressionStatement).Expression as ReturnExpression;
				if (returnExpression == null || returnExpression.Value == null || returnExpression.Value.CodeNodeType != CodeNodeType.VariableReferenceExpression)
				{
					return false;
				}
				right = expression.Right as FieldReferenceExpression;
			}
			else
			{
				ReturnExpression expression1 = (getterStatements.Statements[0] as ExpressionStatement).Expression as ReturnExpression;
				if (expression1 == null || expression1.Value == null || expression1.Value.CodeNodeType != CodeNodeType.FieldReferenceExpression)
				{
					return false;
				}
				right = expression1.Value as FieldReferenceExpression;
			}
			return this.CheckFieldReferenceExpression(right);
		}

		private bool CheckSetter(BlockStatement setterStatements)
		{
			if (setterStatements == null || setterStatements.Statements == null || setterStatements.Statements.Count != 2 || setterStatements.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement || setterStatements.Statements[1].CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				return false;
			}
			ReturnExpression expression = (setterStatements.Statements[1] as ExpressionStatement).Expression as ReturnExpression;
			if (expression == null || expression.Value != null)
			{
				return false;
			}
			BinaryExpression binaryExpression = (setterStatements.Statements[0] as ExpressionStatement).Expression as BinaryExpression;
			if (binaryExpression == null || !binaryExpression.IsAssignmentExpression || binaryExpression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression || binaryExpression.Right.CodeNodeType != CodeNodeType.ArgumentReferenceExpression)
			{
				return false;
			}
			return this.CheckFieldReferenceExpression(binaryExpression.Left as FieldReferenceExpression);
		}

		public void Decompile(out CachedDecompiledMember getMethod, out CachedDecompiledMember setMethod, out bool isAutoImplemented)
		{
			getMethod = null;
			setMethod = null;
			isAutoImplemented = false;
			if (this.propertyDef.GetMethod == null || this.propertyDef.GetMethod.Parameters.Count != 0 || !this.propertyDef.GetMethod.HasBody || this.propertyDef.OtherMethods.Count != 0)
			{
				getMethod = this.DecompileMember(this.propertyDef.GetMethod);
				setMethod = this.DecompileMember(this.propertyDef.SetMethod);
				isAutoImplemented = false;
				return;
			}
			if (this.propertyDef.SetMethod == null)
			{
				if (this.language.SupportsGetterOnlyAutoProperties)
				{
					isAutoImplemented = this.DecompileAndCheckForAutoImplementedPropertyMethod(this.propertyDef.GetMethod, out getMethod, true, new Func<BlockStatement, bool>(this.CheckGetter));
					return;
				}
				getMethod = this.DecompileMember(this.propertyDef.GetMethod);
				setMethod = this.DecompileMember(this.propertyDef.SetMethod);
				isAutoImplemented = false;
				return;
			}
			if (this.propertyDef.SetMethod.Parameters.Count != 1 || !this.propertyDef.SetMethod.HasBody)
			{
				getMethod = this.DecompileMember(this.propertyDef.GetMethod);
				setMethod = this.DecompileMember(this.propertyDef.SetMethod);
				isAutoImplemented = false;
				return;
			}
			isAutoImplemented = this.DecompileAndCheckForAutoImplementedPropertyMethod(this.propertyDef.GetMethod, out getMethod, true, new Func<BlockStatement, bool>(this.CheckGetter)) & this.DecompileAndCheckForAutoImplementedPropertyMethod(this.propertyDef.SetMethod, out setMethod, true, new Func<BlockStatement, bool>(this.CheckSetter));
		}

		private bool DecompileAndCheckForAutoImplementedPropertyMethod(MethodDefinition method, out CachedDecompiledMember decompiledMember, bool needDecompiledMember, Func<BlockStatement, bool> checker)
		{
			DecompilationContext decompilationContext;
			decompiledMember = null;
			BlockStatement blockStatement = this.DecompileMethodPartially(method.Body, out decompilationContext, needDecompiledMember);
			if (blockStatement == null && decompilationContext == null)
			{
				if (needDecompiledMember)
				{
					decompiledMember = this.DecompileMember(method);
				}
				return false;
			}
			if (blockStatement.Statements.Count == 1 && blockStatement.Statements[0].CodeNodeType == CodeNodeType.ExceptionStatement)
			{
				if (needDecompiledMember)
				{
					decompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), blockStatement, new MethodSpecificContext(method.Body)));
				}
				return false;
			}
			if (!checker(blockStatement))
			{
				if (needDecompiledMember)
				{
					decompiledMember = this.FinishDecompilationOfMember(method, blockStatement, decompilationContext);
				}
				return false;
			}
			if (needDecompiledMember)
			{
				if (!this.propertyDef.ShouldStaySplit())
				{
					decompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), blockStatement, decompilationContext.MethodContext));
				}
				else
				{
					decompiledMember = this.FinishDecompilationOfMember(method, blockStatement, decompilationContext);
				}
			}
			return true;
		}

		private CachedDecompiledMember DecompileMember(MethodDefinition method)
		{
			MethodSpecificContext methodSpecificContext;
			CachedDecompiledMember cachedDecompiledMember = null;
			if (method != null)
			{
				if (method.Body != null)
				{
					if (this.IsCachingEnabled && this.cacheService.IsDecompiledMemberInCache(method, this.language, this.renameInvalidMembers))
					{
						return this.cacheService.GetDecompiledMemberFromCache(method, this.language, this.renameInvalidMembers);
					}
					BlockStatement blockStatement = this.DecompileMethod(method.Body, out methodSpecificContext);
					cachedDecompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), blockStatement, methodSpecificContext));
					if (this.IsCachingEnabled)
					{
						this.cacheService.AddDecompiledMemberToCache(method, this.language, this.renameInvalidMembers, cachedDecompiledMember);
					}
				}
				else
				{
					cachedDecompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), null, null));
				}
			}
			return cachedDecompiledMember;
		}

		private BlockStatement DecompileMethod(MethodBody body, out MethodSpecificContext methodContext)
		{
			BlockStatement blockStatement;
			methodContext = null;
			try
			{
				DecompilationContext decompilationContext = new DecompilationContext(new MethodSpecificContext(body), this.typeContext ?? new TypeSpecificContext(body.Method.DeclaringType), this.language);
				DecompilationPipeline decompilationPipeline = this.language.CreatePipeline(decompilationContext);
				methodContext = decompilationPipeline.Run(body, this.language).MethodContext;
				blockStatement = decompilationPipeline.Body;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.ExceptionsWhileDecompiling.Add(body.Method);
				methodContext = new MethodSpecificContext(body);
				blockStatement = new BlockStatement();
				blockStatement.AddStatement(new ExceptionStatement(exception, body.Method));
				base.OnExceptionThrown(exception);
			}
			return blockStatement;
		}

		private BlockStatement DecompileMethodPartially(MethodBody body, out DecompilationContext context, bool needDecompiledMember = false)
		{
			BlockStatement blockStatement;
			context = null;
			if (this.IsCachingEnabled && this.cacheService.IsDecompiledMemberInCache(body.Method, this.language, this.renameInvalidMembers))
			{
				return this.cacheService.GetDecompiledMemberFromCache(body.Method, this.language, this.renameInvalidMembers).Member.Statement as BlockStatement;
			}
			if ((int)(new ControlFlowGraphBuilder(body.Method)).CreateGraph().Blocks.Length > 2)
			{
				return null;
			}
			try
			{
				DecompilationContext decompilationContext = new DecompilationContext(new MethodSpecificContext(body), this.typeContext ?? new TypeSpecificContext(body.Method.DeclaringType), this.language);
				if (!needDecompiledMember)
				{
					decompilationContext.MethodContext.EnableEventAnalysis = false;
				}
				DecompilationPipeline decompilationPipeline = new DecompilationPipeline(BaseLanguage.IntermediateRepresenationPipeline.Steps, decompilationContext);
				context = decompilationPipeline.Run(body, this.language);
				blockStatement = decompilationPipeline.Body;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.ExceptionsWhileDecompiling.Add(body.Method);
				blockStatement = new BlockStatement();
				blockStatement.AddStatement(new ExceptionStatement(exception, body.Method));
				base.OnExceptionThrown(exception);
			}
			return blockStatement;
		}

		private CachedDecompiledMember FinishDecompilationOfMember(MethodDefinition method, BlockStatement block, DecompilationContext context)
		{
			MethodSpecificContext methodSpecificContext;
			if (this.IsCachingEnabled && this.cacheService.IsDecompiledMemberInCache(method, this.language, this.renameInvalidMembers))
			{
				return this.cacheService.GetDecompiledMemberFromCache(method, this.language, this.renameInvalidMembers);
			}
			BlockStatement blockStatement = this.FinishDecompilationOfMethod(block, context, out methodSpecificContext);
			CachedDecompiledMember cachedDecompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), blockStatement, methodSpecificContext));
			if (this.IsCachingEnabled)
			{
				this.cacheService.AddDecompiledMemberToCache(method, this.language, this.renameInvalidMembers, cachedDecompiledMember);
			}
			return cachedDecompiledMember;
		}

		private BlockStatement FinishDecompilationOfMethod(BlockStatement block, DecompilationContext context, out MethodSpecificContext methodContext)
		{
			BlockStatement body;
			methodContext = null;
			try
			{
				BlockDecompilationPipeline blockDecompilationPipeline = this.language.CreatePropertyPipeline(context);
				methodContext = blockDecompilationPipeline.Run(context.MethodContext.Method.Body, block, this.language).MethodContext;
				body = blockDecompilationPipeline.Body;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.ExceptionsWhileDecompiling.Add(context.MethodContext.Method);
				methodContext = new MethodSpecificContext(context.MethodContext.Method.Body);
				body = new BlockStatement();
				body.AddStatement(new ExceptionStatement(exception, context.MethodContext.Method));
				base.OnExceptionThrown(exception);
			}
			return body;
		}

		public bool IsAutoImplemented(out FieldDefinition propertyField)
		{
			bool flag = this.IsAutoImplemented();
			propertyField = this.propertyFieldDef;
			return flag;
		}

		public bool IsAutoImplemented()
		{
			CachedDecompiledMember cachedDecompiledMember;
			CachedDecompiledMember cachedDecompiledMember1;
			CachedDecompiledMember cachedDecompiledMember2;
			if (this.propertyDef.GetMethod == null || this.propertyDef.GetMethod.Parameters.Count != 0 || !this.propertyDef.GetMethod.HasBody || this.propertyDef.OtherMethods.Count != 0)
			{
				return false;
			}
			if (this.propertyDef.SetMethod == null)
			{
				if (!this.language.SupportsGetterOnlyAutoProperties)
				{
					return false;
				}
				return this.DecompileAndCheckForAutoImplementedPropertyMethod(this.propertyDef.GetMethod, out cachedDecompiledMember2, false, new Func<BlockStatement, bool>(this.CheckGetter));
			}
			if (this.propertyDef.SetMethod.Parameters.Count != 1 || !this.propertyDef.SetMethod.HasBody)
			{
				return false;
			}
			if (!this.DecompileAndCheckForAutoImplementedPropertyMethod(this.propertyDef.GetMethod, out cachedDecompiledMember, false, new Func<BlockStatement, bool>(this.CheckGetter)))
			{
				return false;
			}
			return this.DecompileAndCheckForAutoImplementedPropertyMethod(this.propertyDef.SetMethod, out cachedDecompiledMember1, false, new Func<BlockStatement, bool>(this.CheckSetter));
		}
	}
}