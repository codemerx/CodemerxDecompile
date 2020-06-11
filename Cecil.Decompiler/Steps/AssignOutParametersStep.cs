using System;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast;
using Mono.Cecil.Extensions;

namespace Telerik.JustDecompiler.Steps
{
    class AssignOutParametersStep : IDecompilationStep
    {
        private DecompilationContext context;
        private TypeSystem typeSystem;

        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            this.context = context;
            this.typeSystem = context.MethodContext.Method.Module.TypeSystem;
            InsertTopLevelParameterAssignments(body);
            return body;
        }

        private void InsertTopLevelParameterAssignments(BlockStatement block)
        {
            for (int i = 0; i < this.context.MethodContext.OutParametersToAssign.Count; i++)
            {
                ParameterDefinition parameter = this.context.MethodContext.OutParametersToAssign[i];
                TypeReference nonPointerType = parameter.ParameterType.IsByReference ? parameter.ParameterType.GetElementType() : parameter.ParameterType;
                UnaryExpression parameterDereference =
                    new UnaryExpression(UnaryOperator.AddressDereference, new ArgumentReferenceExpression(parameter, null), null);

                BinaryExpression assignExpression = new BinaryExpression(BinaryOperator.Assign, parameterDereference,
                    nonPointerType.GetDefaultValueExpression(typeSystem), nonPointerType, typeSystem, null);

                block.AddStatementAt(i, new ExpressionStatement(assignExpression));
            }
        }
    }
}
