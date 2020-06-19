using System;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast
{
    public interface IIndexerExpression
    {
        Expression Target { get; set; }
        ExpressionCollection Indices { get; set; }
    }
}
