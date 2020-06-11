using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Cil;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler.Caching;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.Decompiler
{
    class PropertyDecompiler : ExceptionThrownNotifier, IExceptionThrownNotifier
    {
        private readonly PropertyDefinition propertyDef;
        private FieldDefinition propertyFieldDef;
        private readonly ILanguage language;
        private readonly bool renameInvalidMembers;
        private readonly IDecompilationCacheService cacheService;
        private readonly TypeSpecificContext typeContext;

        public ICollection<MethodDefinition> ExceptionsWhileDecompiling { get; private set; }

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

        public PropertyDecompiler(PropertyDefinition property, ILanguage language, TypeSpecificContext typeContext = null)
            : this(property, language, false, null, typeContext)
        {
        }

        private bool IsCachingEnabled
        {
            get
            {
                return this.cacheService != null;
            }
        }

        public bool IsAutoImplemented(out FieldDefinition propertyField)
        {
            bool result = IsAutoImplemented();
            propertyField = this.propertyFieldDef;
            return result;
        }

        public bool IsAutoImplemented()
        {
            if (propertyDef.GetMethod == null || propertyDef.GetMethod.Parameters.Count != 0 || !propertyDef.GetMethod.HasBody ||
                propertyDef.OtherMethods.Count != 0)
            {
                return false;
            }

            if (propertyDef.SetMethod != null)
            {
                if (propertyDef.SetMethod.Parameters.Count != 1 || !propertyDef.SetMethod.HasBody)
                {
                    return false;
                }

                // Auto-implemented property with getter and setter
                CachedDecompiledMember getMethod;
                CachedDecompiledMember setMethod;
                return DecompileAndCheckForAutoImplementedPropertyMethod(propertyDef.GetMethod, out getMethod, false, CheckGetter) &&
                       DecompileAndCheckForAutoImplementedPropertyMethod(propertyDef.SetMethod, out setMethod, false, CheckSetter);
            }
            else if (!this.language.SupportsGetterOnlyAutoProperties)
            {
                return false;
            }
            else
            {
                // Getter only auto-implemented property
                CachedDecompiledMember getMethod;
                return DecompileAndCheckForAutoImplementedPropertyMethod(propertyDef.GetMethod, out getMethod, false, CheckGetter);
            }
        }

        public void Decompile(out CachedDecompiledMember getMethod, out CachedDecompiledMember setMethod, out bool isAutoImplemented)
        {
            getMethod = null;
            setMethod = null;
            isAutoImplemented = false;

            if (propertyDef.GetMethod == null || propertyDef.GetMethod.Parameters.Count != 0 || !propertyDef.GetMethod.HasBody ||
                propertyDef.OtherMethods.Count != 0)
            {
                getMethod = DecompileMember(propertyDef.GetMethod);
                setMethod = DecompileMember(propertyDef.SetMethod);
                isAutoImplemented = false;
            }
            else if (propertyDef.SetMethod != null)
            {
                if (propertyDef.SetMethod.Parameters.Count != 1 || !propertyDef.SetMethod.HasBody)
                {
                    getMethod = DecompileMember(propertyDef.GetMethod);
                    setMethod = DecompileMember(propertyDef.SetMethod);
                    isAutoImplemented = false;
                }
                else
                {
                    // Auto-implemented property with getter and setter
                    isAutoImplemented = DecompileAndCheckForAutoImplementedPropertyMethod(propertyDef.GetMethod, out getMethod, true, CheckGetter) &
                                        DecompileAndCheckForAutoImplementedPropertyMethod(propertyDef.SetMethod, out setMethod, true, CheckSetter);
                }
            }
            else if (!this.language.SupportsGetterOnlyAutoProperties)
            {
                getMethod = DecompileMember(propertyDef.GetMethod);
                setMethod = DecompileMember(propertyDef.SetMethod);
                isAutoImplemented = false;
            }
            else
            {
                // Getter only auto-implemented property
                isAutoImplemented = DecompileAndCheckForAutoImplementedPropertyMethod(propertyDef.GetMethod, out getMethod, true, CheckGetter);
            }
        }

        private bool DecompileAndCheckForAutoImplementedPropertyMethod(MethodDefinition method, out CachedDecompiledMember decompiledMember,
            bool needDecompiledMember, Func<BlockStatement, bool> checker)
        {
            decompiledMember = null;

            DecompilationContext context;
            BlockStatement statements = this.DecompileMethodPartially(method.Body, out context, needDecompiledMember);

            if (statements == null && context == null)
            {
                if (needDecompiledMember)
                {
                    decompiledMember = DecompileMember(method);
                }

                return false;
            }
            else if (statements.Statements.Count == 1 && statements.Statements[0].CodeNodeType == CodeNodeType.ExceptionStatement)
            {
                if (needDecompiledMember)
                {
                    decompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), statements, new MethodSpecificContext(method.Body)));
                }

                return false;
            }

            if (checker(statements))
            {
                if (needDecompiledMember)
                {
                    if (this.propertyDef.ShouldStaySplit())
                    {
                        decompiledMember = FinishDecompilationOfMember(method, statements, context);
                    }
                    else
                    {
                        decompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), statements, context.MethodContext));
                    }
                }

                return true;
            }
            else
            {
                if (needDecompiledMember)
                {
                    decompiledMember = FinishDecompilationOfMember(method, statements, context);
                }

                return false;
            }
        }

        private bool CheckGetter(BlockStatement getterStatements)
        {
            if (getterStatements == null || getterStatements.Statements == null ||
                getterStatements.Statements.Count != 1 && getterStatements.Statements.Count != 2 ||
                getterStatements.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
            {
                return false;
            }

            FieldReferenceExpression fieldRefExpression;
            if (getterStatements.Statements.Count == 1)
            {
                ReturnExpression returnExpression = (getterStatements.Statements[0] as ExpressionStatement).Expression as ReturnExpression;
                if (returnExpression == null || returnExpression.Value == null ||
                    returnExpression.Value.CodeNodeType != CodeNodeType.FieldReferenceExpression)
                {
                    return false;
                }
                fieldRefExpression = returnExpression.Value as FieldReferenceExpression;
            }
            else
            {
                BinaryExpression binaryExpression = (getterStatements.Statements[0] as ExpressionStatement).Expression as BinaryExpression;
                if (binaryExpression == null || !binaryExpression.IsAssignmentExpression ||
                    binaryExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression ||
                    binaryExpression.Right.CodeNodeType != CodeNodeType.FieldReferenceExpression)
                {
                    return false;
                }

                if (getterStatements.Statements[1].CodeNodeType != CodeNodeType.ExpressionStatement)
                {
                    return false;
                }

                ReturnExpression returnExpression = (getterStatements.Statements[1] as ExpressionStatement).Expression as ReturnExpression;
                if (returnExpression == null || returnExpression.Value == null ||
                    returnExpression.Value.CodeNodeType != CodeNodeType.VariableReferenceExpression)
                {
                    return false;
                }

                fieldRefExpression = binaryExpression.Right as FieldReferenceExpression;
            }

            return CheckFieldReferenceExpression(fieldRefExpression);
        }

        private bool CheckSetter(BlockStatement setterStatements)
        {
            if (setterStatements == null || setterStatements.Statements == null || setterStatements.Statements.Count != 2 ||
                setterStatements.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement ||
                setterStatements.Statements[1].CodeNodeType != CodeNodeType.ExpressionStatement)
            {
                return false;
            }

            ReturnExpression returnExpression = (setterStatements.Statements[1] as ExpressionStatement).Expression as ReturnExpression;
            if (returnExpression == null || returnExpression.Value != null)
            {
                return false;
            }

            BinaryExpression binaryExpression = (setterStatements.Statements[0] as ExpressionStatement).Expression as BinaryExpression;
            if (binaryExpression == null || !binaryExpression.IsAssignmentExpression ||
                binaryExpression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression ||
                binaryExpression.Right.CodeNodeType != CodeNodeType.ArgumentReferenceExpression)
            {
                return false;
            }

            return CheckFieldReferenceExpression(binaryExpression.Left as FieldReferenceExpression);
        }

        private bool CheckFieldReferenceExpression(FieldReferenceExpression fieldRefExpression)
        {
            if (fieldRefExpression.Field == null)
            {
                return false;
            }

            if (propertyFieldDef != null)
            {
                return fieldRefExpression.Field.Resolve() == this.propertyFieldDef;
            }

            FieldDefinition fieldDef = fieldRefExpression.Field.Resolve();
            if (fieldDef == null || fieldDef.DeclaringType != propertyDef.DeclaringType)
            {
                return false;
            }

            if (!fieldDef.HasCompilerGeneratedAttribute())
            {
                return false;
            }

            propertyFieldDef = fieldDef;
            return true;
        }

        private CachedDecompiledMember DecompileMember(MethodDefinition method)
        {
            CachedDecompiledMember cachedDecompiledMember = null;
            if (method != null)
            {
                if (method.Body == null)
                {
                    cachedDecompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), null, null));
                }
                else if (this.IsCachingEnabled && this.cacheService.IsDecompiledMemberInCache(method, this.language, this.renameInvalidMembers))
                {
                    return this.cacheService.GetDecompiledMemberFromCache(method, this.language, this.renameInvalidMembers);
                }
                else
                {
                    MethodSpecificContext methodContext;
                    BlockStatement block = this.DecompileMethod(method.Body, out methodContext);
                    cachedDecompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), block, methodContext));
                    if (this.IsCachingEnabled)
                    {
                        this.cacheService.AddDecompiledMemberToCache(method, this.language, this.renameInvalidMembers, cachedDecompiledMember);
                    }
                }
            }

            return cachedDecompiledMember;
        }

        private CachedDecompiledMember FinishDecompilationOfMember(MethodDefinition method, BlockStatement block, DecompilationContext context)
        {
            if (this.IsCachingEnabled && this.cacheService.IsDecompiledMemberInCache(method, this.language, this.renameInvalidMembers))
            {
                return this.cacheService.GetDecompiledMemberFromCache(method, this.language, this.renameInvalidMembers);
            }

            MethodSpecificContext methodContext;
            BlockStatement fullyDecompiledBlock = this.FinishDecompilationOfMethod(block, context, out methodContext);
            CachedDecompiledMember cachedDecompiledMember = new CachedDecompiledMember(new DecompiledMember(Utilities.GetMemberUniqueName(method), fullyDecompiledBlock, methodContext));
            if (this.IsCachingEnabled)
            {
                this.cacheService.AddDecompiledMemberToCache(method, this.language, this.renameInvalidMembers, cachedDecompiledMember);
            }

            return cachedDecompiledMember;
        }

        private BlockStatement DecompileMethod(MethodBody body, out MethodSpecificContext methodContext)
        {
            methodContext = null;

            BlockStatement block;
            try
            {
                DecompilationContext decompilationContext =
                    new DecompilationContext(new MethodSpecificContext(body), this.typeContext ?? new TypeSpecificContext(body.Method.DeclaringType), this.language);

                DecompilationPipeline pipeline = this.language.CreatePipeline(decompilationContext);

                methodContext = pipeline.Run(body, this.language).MethodContext;

                block = pipeline.Body;
            }
            catch (Exception ex)
            {
                this.ExceptionsWhileDecompiling.Add(body.Method);

                methodContext = new MethodSpecificContext(body);

                block = new BlockStatement();
                block.AddStatement(new ExceptionStatement(ex, body.Method));

                OnExceptionThrown(ex);
            }

            return block;
        }

        private BlockStatement DecompileMethodPartially(MethodBody body, out DecompilationContext context, bool needDecompiledMember = false)
        {
            context = null;

            if (this.IsCachingEnabled && this.cacheService.IsDecompiledMemberInCache(body.Method, this.language, this.renameInvalidMembers))
            {
                CachedDecompiledMember cachedDecompiledMember = this.cacheService.GetDecompiledMemberFromCache(body.Method, this.language, this.renameInvalidMembers);
                return cachedDecompiledMember.Member.Statement as BlockStatement;
            }

            //Performance improvement
            ControlFlowGraph cfg = new ControlFlowGraphBuilder(body.Method).CreateGraph();
            if (cfg.Blocks.Length > 2)
            {
                return null;
            }

            BlockStatement block;
            try
            {
                DecompilationPipeline pipeline;
                DecompilationContext decompilationContext =
                    new DecompilationContext(new MethodSpecificContext(body), this.typeContext ?? new TypeSpecificContext(body.Method.DeclaringType), this.language);
                if (!needDecompiledMember)
                {
                    decompilationContext.MethodContext.EnableEventAnalysis = false;
                }

                pipeline = new DecompilationPipeline(BaseLanguage.IntermediateRepresenationPipeline.Steps, decompilationContext);

                context = pipeline.Run(body, this.language);

                block = pipeline.Body;
            }
            catch (Exception ex)
            {
                this.ExceptionsWhileDecompiling.Add(body.Method);

                block = new BlockStatement();
                block.AddStatement(new ExceptionStatement(ex, body.Method));

                OnExceptionThrown(ex);
            }

            return block;
        }

        private BlockStatement FinishDecompilationOfMethod(BlockStatement block, DecompilationContext context, out MethodSpecificContext methodContext)
        {
            methodContext = null;

            BlockStatement fullyDecompiledBlock;
            try
            {
                BlockDecompilationPipeline pipeline = this.language.CreatePropertyPipeline(context);

                methodContext = pipeline.Run(context.MethodContext.Method.Body, block, this.language).MethodContext;

                fullyDecompiledBlock = pipeline.Body;
            }
            catch (Exception ex)
            {
                this.ExceptionsWhileDecompiling.Add(context.MethodContext.Method);

                methodContext = new MethodSpecificContext(context.MethodContext.Method.Body);

                fullyDecompiledBlock = new BlockStatement();
                fullyDecompiledBlock.AddStatement(new ExceptionStatement(ex, context.MethodContext.Method));

                OnExceptionThrown(ex);
            }

            return fullyDecompiledBlock;
        }
    }
}
