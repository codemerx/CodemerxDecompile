using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildExpressionTreesStep : BaseCodeTransformer, IDecompilationStep
	{
		private DecompilationContext context;

		private TypeSystem typeSystem;

		private readonly Dictionary<VariableReference, Expression> variableToValueMap = new Dictionary<VariableReference, Expression>();

		private readonly Dictionary<VariableReference, HashSet<ExpressionStatement>> variableToAssigingStatementsMap = new Dictionary<VariableReference, HashSet<ExpressionStatement>>();

		private readonly HashSet<VariableReference> usedVariables = new HashSet<VariableReference>();

		private readonly Dictionary<VariableReference, int> variableToLastUninitializedIndex = new Dictionary<VariableReference, int>();

		private int conversionDepth;

		private int paramterNameIndex;

		private bool failure;

		public RebuildExpressionTreesStep()
		{
		}

		private ICodeNode ConvertArrayIndex(MethodInvocationExpression node)
		{
			if (node.Arguments.Count != 2)
			{
				return null;
			}
			ArrayCreationExpression arrayCreationExpression = this.Visit(node.Arguments[1]) as ArrayCreationExpression;
			if (arrayCreationExpression == null || arrayCreationExpression.Dimensions.Count != 1 || arrayCreationExpression.Initializer == null || arrayCreationExpression.Initializer.Expressions.Count == 0)
			{
				return null;
			}
			return new ArrayIndexerExpression((Expression)this.Visit(node.Arguments[0]), null)
			{
				Indices = arrayCreationExpression.Initializer.Expressions
			};
		}

		private ICodeNode ConvertArrayLength(MethodInvocationExpression node)
		{
			if (node.Arguments.Count != 1)
			{
				return null;
			}
			return new ArrayLengthExpression((Expression)this.Visit(node.Arguments[0]), this.typeSystem, null);
		}

		private BinaryExpression ConvertBinaryOperator(MethodInvocationExpression node, BinaryOperator binaryOperator, bool isChecked)
		{
			BinaryExpression binaryExpression;
			if (node.Arguments.Count < 2)
			{
				return null;
			}
			try
			{
				binaryExpression = new BinaryExpression(binaryOperator, (Expression)this.Visit(node.Arguments[0]), (Expression)this.Visit(node.Arguments[1]), this.typeSystem, isChecked, null, node.Arguments.Count > 2);
			}
			catch
			{
				binaryExpression = null;
			}
			return binaryExpression;
		}

		private BinaryExpression ConvertBinaryOperator(MethodInvocationExpression node, BinaryOperator binaryOperator)
		{
			return this.ConvertBinaryOperator(node, binaryOperator, false);
		}

		private ICodeNode ConvertBind(MethodInvocationExpression invocation)
		{
			MethodReference methodReference;
			if (invocation.Arguments.Count != 2 || !this.TryGetMethodReference((Expression)this.Visit(invocation.Arguments[0]), "System.Reflection.MethodInfo", out methodReference))
			{
				return null;
			}
			PropertyDefinition propertyDefinition = this.ResolveProperty(methodReference);
			if (propertyDefinition == null)
			{
				return null;
			}
			return new BinaryExpression(BinaryOperator.Assign, new PropertyInitializerExpression(propertyDefinition, propertyDefinition.get_PropertyType()), (Expression)this.Visit(invocation.Arguments[1]), this.typeSystem, null, false);
		}

		private ICodeNode ConvertCall(MethodInvocationExpression node)
		{
			MethodReference methodReference;
			if (node.Arguments.Count < 2 || !this.TryGetMethodReference((Expression)this.Visit(node.Arguments[1]), "System.Reflection.MethodInfo", out methodReference))
			{
				return null;
			}
			MethodInvocationExpression methodInvocationExpression = new MethodInvocationExpression(new MethodReferenceExpression(this.GetTarget(node.Arguments[0]), methodReference, null), null);
			if (node.Arguments.Count == 3)
			{
				ArrayCreationExpression arrayCreationExpression = this.Visit(node.Arguments[2]) as ArrayCreationExpression;
				if (arrayCreationExpression == null || arrayCreationExpression.Dimensions.Count != 1 || arrayCreationExpression.Initializer == null || arrayCreationExpression.Initializer.Expressions == null || arrayCreationExpression.Initializer.Expressions.Count != methodReference.get_Parameters().get_Count())
				{
					return null;
				}
				foreach (Expression expression in arrayCreationExpression.Initializer.Expressions)
				{
					methodInvocationExpression.Arguments.Add(expression);
				}
			}
			return methodInvocationExpression;
		}

		private ICodeNode ConvertCast(MethodInvocationExpression invocation, Func<Expression, TypeReference, Expression> createInstance)
		{
			if (invocation.Arguments.Count < 2 || invocation.Arguments[1].CodeNodeType != CodeNodeType.TypeOfExpression)
			{
				return null;
			}
			Expression expression = (Expression)this.Visit(invocation.Arguments[0]);
			TypeReference type = (invocation.Arguments[1] as TypeOfExpression).Type;
			return createInstance(expression, type);
		}

		private ICodeNode ConvertCast(MethodInvocationExpression node)
		{
			return this.ConvertCast(node, (Expression expression, TypeReference type) => new ExplicitCastExpression(expression, type, null));
		}

		private ICodeNode ConvertCastChecked(MethodInvocationExpression node)
		{
			ExplicitCastExpression explicitCastExpression = this.ConvertCast(node) as ExplicitCastExpression;
			if (explicitCastExpression == null)
			{
				return null;
			}
			return new CheckedExpression(explicitCastExpression, null);
		}

		private ICodeNode ConvertCondition(MethodInvocationExpression node)
		{
			if (node.Arguments.Count < 3)
			{
				return null;
			}
			Expression expression = (Expression)this.Visit(node.Arguments[0]);
			Expression expression1 = (Expression)this.Visit(node.Arguments[1]);
			Expression expression2 = (Expression)this.Visit(node.Arguments[2]);
			return new ConditionExpression(expression, expression1, expression2, null);
		}

		private ICodeNode ConvertConstant(MethodInvocationExpression invocation)
		{
			if (invocation.Arguments.Count < 1 || invocation.Arguments.Count > 2)
			{
				return null;
			}
			Expression expression = (Expression)this.Visit(invocation.Arguments[0]);
			if (invocation.Arguments.Count == 2)
			{
				this.Visit(invocation.Arguments[1]);
			}
			BoxExpression boxExpression = expression as BoxExpression;
			if (boxExpression == null || !boxExpression.IsAutoBox)
			{
				return expression;
			}
			return boxExpression.BoxedExpression;
		}

		private ICodeNode ConvertElementInit(MethodInvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 2)
			{
				return null;
			}
			ArrayCreationExpression arrayCreationExpression = this.Visit(invocation.Arguments[1]) as ArrayCreationExpression;
			if (arrayCreationExpression == null || arrayCreationExpression.Dimensions == null || arrayCreationExpression.Dimensions.Count != 1 || arrayCreationExpression.Initializer == null || arrayCreationExpression.Initializer.Expressions == null || arrayCreationExpression.Initializer.Expressions.Count == 0)
			{
				return null;
			}
			this.Visit(invocation.Arguments[0]);
			if (arrayCreationExpression.Initializer.Expressions.Count != 1)
			{
				return arrayCreationExpression.Initializer.Expression;
			}
			return arrayCreationExpression.Initializer.Expressions[0];
		}

		private ICodeNode ConvertField(MethodInvocationExpression invocation)
		{
			FieldReference fieldReference;
			if (invocation.Arguments.Count != 2 || !this.TryGetFieldReference((Expression)this.Visit(invocation.Arguments[1]), out fieldReference))
			{
				return null;
			}
			return new FieldReferenceExpression(this.GetTarget(invocation.Arguments[0]), fieldReference, null);
		}

		private ICodeNode ConvertInvocation(MethodInvocationExpression invocation)
		{
			if (invocation.MethodExpression == null || invocation.MethodExpression.Method == null || invocation.MethodExpression.Method.get_HasThis() || invocation.MethodExpression.Method.get_DeclaringType() == null || invocation.MethodExpression.Method.get_DeclaringType().get_FullName() != "System.Linq.Expressions.Expression")
			{
				return null;
			}
			if (this.conversionDepth == 0 && invocation.MethodExpression.Method.get_Name() != "Lambda")
			{
				return null;
			}
			ICodeNode codeNode = null;
			string name = invocation.MethodExpression.Method.get_Name();
			if (name != null)
			{
				switch (name)
				{
					case "Add":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.Add, false);
						break;
					}
					case "AddChecked":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.Add, true);
						break;
					}
					case "And":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.BitwiseAnd);
						break;
					}
					case "AndAlso":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.LogicalAnd);
						break;
					}
					case "ArrayAccess":
					case "ArrayIndex":
					{
						codeNode = this.ConvertArrayIndex(invocation);
						break;
					}
					case "ArrayLength":
					{
						codeNode = this.ConvertArrayLength(invocation);
						break;
					}
					case "Bind":
					{
						codeNode = this.ConvertBind(invocation);
						break;
					}
					case "Call":
					{
						codeNode = this.ConvertCall(invocation);
						break;
					}
					case "Coalesce":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.NullCoalesce);
						break;
					}
					case "Condition":
					{
						codeNode = this.ConvertCondition(invocation);
						break;
					}
					case "Constant":
					{
						codeNode = this.ConvertConstant(invocation);
						break;
					}
					case "Convert":
					{
						codeNode = this.ConvertCast(invocation);
						break;
					}
					case "ConvertChecked":
					{
						codeNode = this.ConvertCastChecked(invocation);
						break;
					}
					case "Divide":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.Divide);
						break;
					}
					case "ElementInit":
					{
						codeNode = this.ConvertElementInit(invocation);
						break;
					}
					case "Equal":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.ValueEquality);
						break;
					}
					case "ExclusiveOr":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.BitwiseXor);
						break;
					}
					case "Field":
					{
						codeNode = this.ConvertField(invocation);
						break;
					}
					case "GreaterThan":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.GreaterThan);
						break;
					}
					case "GreaterThanOrEqual":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.GreaterThanOrEqual);
						break;
					}
					case "Invoke":
					{
						codeNode = this.ConvertInvoke(invocation);
						break;
					}
					case "Lambda":
					{
						codeNode = this.ConvertLambda(invocation);
						break;
					}
					case "LeftShift":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.LeftShift);
						break;
					}
					case "LessThan":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.LessThan);
						break;
					}
					case "LessThanOrEqual":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.LessThanOrEqual);
						break;
					}
					case "ListBind":
					{
						codeNode = this.ConvertListBind(invocation);
						break;
					}
					case "ListInit":
					{
						codeNode = this.ConvertListInit(invocation);
						break;
					}
					case "MemberInit":
					{
						codeNode = this.ConvertMemberInit(invocation);
						break;
					}
					case "Modulo":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.Modulo);
						break;
					}
					case "Multiply":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.Multiply, false);
						break;
					}
					case "MultiplyChecked":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.Multiply, true);
						break;
					}
					case "Negate":
					{
						codeNode = this.ConvertUnaryOperator(invocation, UnaryOperator.Negate);
						break;
					}
					case "NegateChecked":
					{
						codeNode = this.ConvertUnaryOperatorChecked(invocation, UnaryOperator.Negate);
						break;
					}
					case "New":
					{
						codeNode = this.ConvertNewObject(invocation);
						break;
					}
					case "NewArrayBounds":
					{
						codeNode = this.ConvertNewArrayBounds(invocation);
						break;
					}
					case "NewArrayInit":
					{
						codeNode = this.ConvertNewArrayInit(invocation);
						break;
					}
					case "Not":
					{
						codeNode = this.ConvertUnaryOperator(invocation, UnaryOperator.LogicalNot);
						break;
					}
					case "NotEqual":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.ValueInequality);
						break;
					}
					case "OnesComplement":
					{
						codeNode = this.ConvertUnaryOperator(invocation, UnaryOperator.BitwiseNot);
						break;
					}
					case "Or":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.BitwiseOr);
						break;
					}
					case "OrElse":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.LogicalOr);
						break;
					}
					case "Parameter":
					{
						codeNode = this.ConvertParameter(invocation);
						break;
					}
					case "Property":
					{
						codeNode = this.ConvertProperty(invocation);
						break;
					}
					case "Quote":
					{
						if (invocation.Arguments.Count == 1)
						{
							codeNode = this.Visit(invocation.Arguments[0]);
						}
						break;
					}
					case "RightShift":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.RightShift);
						break;
					}
					case "Subtract":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.Subtract, false);
						break;
					}
					case "SubtractChecked":
					{
						codeNode = this.ConvertBinaryOperator(invocation, BinaryOperator.Subtract, true);
						break;
					}
					case "TypeAs":
					{
						codeNode = this.ConvertTypeAs(invocation);
						break;
					}
					default:
					{
						if (name != "TypeIs")
						{
							return null;
						}
						codeNode = this.ConvertTypeIs(invocation);
						break;
					}
				}
				this.failure = this.failure | codeNode == null;
				return codeNode;
			}
			return null;
		}

		private ICodeNode ConvertInvoke(MethodInvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 2)
			{
				return null;
			}
			ArrayCreationExpression arrayCreationExpression = this.Visit(invocation.Arguments[1]) as ArrayCreationExpression;
			if (arrayCreationExpression == null || arrayCreationExpression.Dimensions.Count != 1 || arrayCreationExpression.Initializer == null || arrayCreationExpression.Initializer.Expressions == null)
			{
				return null;
			}
			Expression expression = (Expression)this.Visit(invocation.Arguments[0]);
			MethodReference invokeMethodReference = this.GetInvokeMethodReference(expression);
			if (invokeMethodReference == null)
			{
				return null;
			}
			return new DelegateInvokeExpression(expression, arrayCreationExpression.Initializer.Expressions, invokeMethodReference, null);
		}

		private ICodeNode ConvertLambda(MethodInvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 2)
			{
				return null;
			}
			ArrayCreationExpression arrayCreationExpression = this.Visit(invocation.Arguments[1]) as ArrayCreationExpression;
			if (arrayCreationExpression != null && arrayCreationExpression.Initializer != null && arrayCreationExpression.Initializer.Expressions != null)
			{
				if (!arrayCreationExpression.Initializer.Expressions.Any<Expression>((Expression element) => element.CodeNodeType != CodeNodeType.ArgumentReferenceExpression))
				{
					List<ArgumentReferenceExpression> list = arrayCreationExpression.Initializer.Expressions.Cast<ArgumentReferenceExpression>().ToList<ArgumentReferenceExpression>();
					bool flag = list.Any<ArgumentReferenceExpression>((ArgumentReferenceExpression param) => param.Parameter.get_ParameterType().Resolve().IsAnonymous());
					BlockStatement blockStatement = new BlockStatement();
					blockStatement.AddStatement(new ExpressionStatement(new ShortFormReturnExpression((Expression)this.Visit(invocation.Arguments[0]), null)));
					return new LambdaExpression(new ExpressionCollection(
						from param in list
						select new LambdaParameterExpression(param.Parameter, !flag, null)), blockStatement, false, false, 
						from argRef in list
						select argRef.Parameter, true, null);
				}
			}
			return null;
		}

		private ICodeNode ConvertListBind(MethodInvocationExpression invocation)
		{
			MethodReference methodReference;
			if (invocation.Arguments.Count != 2 || !this.TryGetMethodReference((Expression)this.Visit(invocation.Arguments[0]), "System.Reflection.MethodInfo", out methodReference))
			{
				return null;
			}
			PropertyDefinition propertyDefinition = this.ResolveProperty(methodReference);
			if (propertyDefinition == null)
			{
				return null;
			}
			ArrayCreationExpression arrayCreationExpression = this.Visit(invocation.Arguments[1]) as ArrayCreationExpression;
			if (arrayCreationExpression == null || arrayCreationExpression.Dimensions == null || arrayCreationExpression.Dimensions.Count != 1 || arrayCreationExpression.Initializer == null || arrayCreationExpression.Initializer.Expressions == null)
			{
				return null;
			}
			arrayCreationExpression.Initializer.InitializerType = InitializerType.CollectionInitializer;
			arrayCreationExpression.Initializer.IsMultiLine = true;
			return new BinaryExpression(BinaryOperator.Assign, new PropertyInitializerExpression(propertyDefinition, propertyDefinition.get_PropertyType()), arrayCreationExpression.Initializer, this.typeSystem, null, false);
		}

		private ICodeNode ConvertListInit(MethodInvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 2)
			{
				return null;
			}
			ObjectCreationExpression initializerExpression = this.Visit(invocation.Arguments[0]) as ObjectCreationExpression;
			if (initializerExpression == null || initializerExpression.Initializer != null)
			{
				return null;
			}
			ArrayCreationExpression arrayCreationExpression = this.Visit(invocation.Arguments[1]) as ArrayCreationExpression;
			if (arrayCreationExpression == null || arrayCreationExpression.Dimensions == null || arrayCreationExpression.Dimensions.Count != 1 || arrayCreationExpression.Initializer == null || arrayCreationExpression.Initializer.Expressions == null)
			{
				return null;
			}
			initializerExpression.Initializer = new InitializerExpression(arrayCreationExpression.Initializer.Expression, InitializerType.CollectionInitializer)
			{
				IsMultiLine = true
			};
			return initializerExpression;
		}

		private ICodeNode ConvertMemberInit(MethodInvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 2)
			{
				return null;
			}
			ObjectCreationExpression initializerExpression = this.Visit(invocation.Arguments[0]) as ObjectCreationExpression;
			if (initializerExpression == null || initializerExpression.Initializer != null)
			{
				return null;
			}
			ArrayCreationExpression arrayCreationExpression = this.Visit(invocation.Arguments[1]) as ArrayCreationExpression;
			if (arrayCreationExpression != null && arrayCreationExpression.Dimensions != null && arrayCreationExpression.Dimensions.Count == 1 && arrayCreationExpression.Initializer != null && arrayCreationExpression.Initializer.Expressions != null)
			{
				if (!arrayCreationExpression.Initializer.Expressions.Any<Expression>((Expression expr) => {
					if (expr.CodeNodeType != CodeNodeType.BinaryExpression)
					{
						return true;
					}
					return !(expr as BinaryExpression).IsAssignmentExpression;
				}))
				{
					if (arrayCreationExpression.Initializer.Expressions.Count > 0)
					{
						initializerExpression.Initializer = new InitializerExpression(arrayCreationExpression.Initializer.Expression, InitializerType.ObjectInitializer)
						{
							IsMultiLine = true
						};
					}
					return initializerExpression;
				}
			}
			return null;
		}

		private ICodeNode ConvertNewArrayBounds(MethodInvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 2)
			{
				return null;
			}
			ArrayCreationExpression expressions = this.GenerateArrayCreationExpression(invocation.Arguments[0]);
			if (expressions == null)
			{
				return null;
			}
			ArrayCreationExpression arrayCreationExpression = this.Visit(invocation.Arguments[1]) as ArrayCreationExpression;
			if (arrayCreationExpression == null || arrayCreationExpression.Dimensions.Count != 1 || arrayCreationExpression.Initializer == null || arrayCreationExpression.Initializer.Expressions == null || arrayCreationExpression.Initializer.Expressions.Count == 0)
			{
				return null;
			}
			expressions.Dimensions = arrayCreationExpression.Initializer.Expressions;
			return expressions;
		}

		private ICodeNode ConvertNewArrayInit(MethodInvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 2)
			{
				return null;
			}
			ArrayCreationExpression dimensions = this.GenerateArrayCreationExpression(invocation.Arguments[0]);
			if (dimensions == null)
			{
				return null;
			}
			ArrayCreationExpression arrayCreationExpression = this.Visit(invocation.Arguments[1]) as ArrayCreationExpression;
			if (arrayCreationExpression == null || arrayCreationExpression.Dimensions == null || arrayCreationExpression.Initializer == null || arrayCreationExpression.Initializer.Expressions == null || arrayCreationExpression.Initializer.Expressions.Count == 0)
			{
				return null;
			}
			dimensions.Dimensions = arrayCreationExpression.Dimensions;
			dimensions.Initializer = arrayCreationExpression.Initializer;
			return dimensions;
		}

		private ICodeNode ConvertNewObject(MethodInvocationExpression invocation)
		{
			MethodReference methodReference;
			if (invocation.Arguments.Count < 1 || invocation.Arguments.Count > 3)
			{
				return null;
			}
			Expression expression = (Expression)this.Visit(invocation.Arguments[0]);
			if (!this.TryGetMethodReference(expression, "System.Reflection.ConstructorInfo", out methodReference))
			{
				if (invocation.Arguments.Count != 1 || expression.CodeNodeType != CodeNodeType.TypeOfExpression)
				{
					return null;
				}
				return new ObjectCreationExpression(null, (expression as TypeOfExpression).Type, null, null);
			}
			ObjectCreationExpression objectCreationExpression = new ObjectCreationExpression(methodReference, methodReference.get_DeclaringType(), null, null);
			if (invocation.Arguments.Count == 1)
			{
				return objectCreationExpression;
			}
			ArrayCreationExpression arrayCreationExpression = this.Visit(invocation.Arguments[1]) as ArrayCreationExpression;
			if (arrayCreationExpression == null || arrayCreationExpression.Dimensions.Count != 1 || arrayCreationExpression.Initializer == null || arrayCreationExpression.Initializer.Expressions == null || arrayCreationExpression.Initializer.Expressions.Count != methodReference.get_Parameters().get_Count())
			{
				return null;
			}
			foreach (Expression expression1 in arrayCreationExpression.Initializer.Expressions)
			{
				objectCreationExpression.Arguments.Add(expression1);
			}
			if (invocation.Arguments.Count == 2)
			{
				return objectCreationExpression;
			}
			this.Visit(invocation.Arguments[2]);
			return objectCreationExpression;
		}

		private ArgumentReferenceExpression ConvertParameter(MethodInvocationExpression node)
		{
			if (node.Arguments.Count < 1 || node.Arguments.Count > 2 || node.Arguments[0].CodeNodeType != CodeNodeType.TypeOfExpression)
			{
				return null;
			}
			TypeReference type = (node.Arguments[0] as TypeOfExpression).Type;
			string value = null;
			if (node.Arguments.Count == 2 && node.Arguments[1].CodeNodeType == CodeNodeType.LiteralExpression)
			{
				value = (node.Arguments[1] as LiteralExpression).Value as String;
			}
			string str = value;
			if (str == null)
			{
				int num = this.paramterNameIndex;
				this.paramterNameIndex = num + 1;
				str = String.Concat("expressionParameter", num.ToString());
			}
			return new ArgumentReferenceExpression(new ParameterDefinition(str, 0, type), null);
		}

		private ICodeNode ConvertProperty(MethodInvocationExpression invocation)
		{
			MethodInvocationExpression methodInvocationExpression = this.ConvertCall(invocation) as MethodInvocationExpression;
			if (methodInvocationExpression == null)
			{
				return null;
			}
			PropertyReferenceExpression propertyReferenceExpression = new PropertyReferenceExpression(methodInvocationExpression, null);
			if (propertyReferenceExpression.Property == null)
			{
				return null;
			}
			return propertyReferenceExpression;
		}

		private ICodeNode ConvertTypeAs(MethodInvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 2)
			{
				return null;
			}
			return this.ConvertCast(invocation, (Expression expression, TypeReference type) => new SafeCastExpression(expression, type, null));
		}

		private ICodeNode ConvertTypeIs(MethodInvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 2)
			{
				return null;
			}
			return this.ConvertCast(invocation, (Expression expression, TypeReference type) => new CanCastExpression(expression, type, null));
		}

		private UnaryExpression ConvertUnaryOperator(MethodInvocationExpression node, UnaryOperator unaryOperator)
		{
			if (node.Arguments.Count < 1)
			{
				return null;
			}
			return new UnaryExpression(unaryOperator, (Expression)this.Visit(node.Arguments[0]), null);
		}

		private ICodeNode ConvertUnaryOperatorChecked(MethodInvocationExpression node, UnaryOperator unaryOperator)
		{
			UnaryExpression unaryExpression = this.ConvertUnaryOperator(node, unaryOperator);
			if (unaryExpression == null)
			{
				return null;
			}
			return new CheckedExpression(unaryExpression, null);
		}

		private ArrayCreationExpression GenerateArrayCreationExpression(Expression unprocessedExpression)
		{
			TypeOfExpression typeOfExpression = this.Visit(unprocessedExpression) as TypeOfExpression;
			if (typeOfExpression == null)
			{
				return null;
			}
			return new ArrayCreationExpression(typeOfExpression.Type, null, null);
		}

		private int GetIntegerValue(LiteralExpression size)
		{
			int num;
			if (size == null)
			{
				return -1;
			}
			try
			{
				num = Convert.ToInt32(size.Value);
			}
			catch (InvalidCastException invalidCastException)
			{
				num = -1;
			}
			return num;
		}

		private MethodReference GetInvokeMethodReference(Expression target)
		{
			if (!target.HasType || target.ExpressionType == null)
			{
				return null;
			}
			TypeReference expressionType = target.ExpressionType;
			TypeDefinition typeDefinition = expressionType.Resolve();
			if (typeDefinition == null || !typeDefinition.IsDelegate())
			{
				return null;
			}
			MethodDefinition methodDefinition = typeDefinition.get_Methods().FirstOrDefault<MethodDefinition>((MethodDefinition method) => method.get_Name() == "Invoke");
			if (methodDefinition == null)
			{
				return null;
			}
			MethodReference methodReference = new MethodReference(methodDefinition.get_Name(), methodDefinition.get_ReturnType(), expressionType);
			methodReference.get_Parameters().AddRange(methodDefinition.get_Parameters());
			return methodReference;
		}

		private Expression GetTarget(Expression unprocessedTarget)
		{
			Expression expression = (Expression)this.Visit(unprocessedTarget);
			if (expression.CodeNodeType == CodeNodeType.LiteralExpression && (expression as LiteralExpression).Value == null)
			{
				return null;
			}
			return expression;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			if (!(new RebuildExpressionTreesStep.ExpressionTreesFinder()).ContainsExpressionTree(body))
			{
				return body;
			}
			this.context = context;
			this.typeSystem = context.TypeContext.CurrentType.get_Module().get_TypeSystem();
			this.failure = false;
			BlockStatement blockStatement = (BlockStatement)this.Visit(body.Clone());
			if (this.failure || this.usedVariables.Count == 0 || !this.TryRemoveUnusedVariableAssignments(blockStatement))
			{
				return body;
			}
			blockStatement = (BlockStatement)(new RebuildExpressionTreesStep.ClosureVariablesRemover(context.MethodContext)).Visit(blockStatement);
			blockStatement = (new CombinedTransformerStep()).Process(context, blockStatement);
			return blockStatement;
		}

		private void RecordVariableAssignment(ExpressionStatement node)
		{
			HashSet<ExpressionStatement> expressionStatements;
			BinaryExpression expression = node.Expression as BinaryExpression;
			VariableReference variable = (expression.Left as VariableReferenceExpression).Variable;
			expression.Right = (Expression)this.Visit(expression.Right);
			Expression expression1 = expression.Right.CloneExpressionOnly();
			this.variableToValueMap[variable] = expression1;
			if (!this.variableToAssigingStatementsMap.TryGetValue(variable, out expressionStatements))
			{
				expressionStatements = new HashSet<ExpressionStatement>();
				this.variableToAssigingStatementsMap[variable] = expressionStatements;
			}
			expressionStatements.Add(node);
			ArrayCreationExpression initializerExpression = expression1 as ArrayCreationExpression;
			if (initializerExpression == null || initializerExpression.Dimensions == null || initializerExpression.Dimensions.Count != 1 || initializerExpression.Initializer != null && initializerExpression.Initializer.Expressions != null && initializerExpression.Initializer.Expressions.Count > 0)
			{
				return;
			}
			initializerExpression.Initializer = new InitializerExpression(new BlockExpression(null), InitializerType.ArrayInitializer);
			this.variableToLastUninitializedIndex[variable] = 0;
		}

		private PropertyDefinition ResolveProperty(MethodReference methodRef)
		{
			MethodDefinition methodDefinition = methodRef.Resolve();
			if (methodDefinition == null || methodDefinition.get_DeclaringType() == null)
			{
				return null;
			}
			return methodDefinition.get_DeclaringType().get_Properties().FirstOrDefault<PropertyDefinition>((PropertyDefinition prop) => {
				if ((object)prop.get_GetMethod() == (object)methodDefinition)
				{
					return true;
				}
				return (object)prop.get_SetMethod() == (object)methodDefinition;
			});
		}

		private bool TryGetFieldReference(Expression expression, out FieldReference fieldRef)
		{
			fieldRef = null;
			MethodInvocationExpression methodInvocationExpression = expression as MethodInvocationExpression;
			if (methodInvocationExpression == null || methodInvocationExpression.Arguments.Count > 2 || methodInvocationExpression.Arguments.Count < 1 || methodInvocationExpression.Arguments[0].CodeNodeType != CodeNodeType.MemberHandleExpression || methodInvocationExpression.Arguments.Count == 2 && methodInvocationExpression.Arguments[1].CodeNodeType != CodeNodeType.MemberHandleExpression || methodInvocationExpression.MethodExpression.Method == null || methodInvocationExpression.MethodExpression.Method.get_Name() != "GetFieldFromHandle" || methodInvocationExpression.MethodExpression.Method.get_DeclaringType() == null || methodInvocationExpression.MethodExpression.Method.get_DeclaringType().get_FullName() != "System.Reflection.FieldInfo")
			{
				return false;
			}
			fieldRef = (methodInvocationExpression.Arguments[0] as MemberHandleExpression).MemberReference as FieldReference;
			return (object)fieldRef != (object)null;
		}

		private bool TryGetMethodReference(Expression expression, string castTargetTypeName, out MethodReference methodRef)
		{
			methodRef = null;
			ExplicitCastExpression explicitCastExpression = expression as ExplicitCastExpression;
			if (explicitCastExpression == null || explicitCastExpression.Expression.CodeNodeType != CodeNodeType.MethodInvocationExpression || explicitCastExpression.TargetType == null || explicitCastExpression.TargetType.get_FullName() != castTargetTypeName)
			{
				return false;
			}
			MethodInvocationExpression methodInvocationExpression = explicitCastExpression.Expression as MethodInvocationExpression;
			if (methodInvocationExpression == null || methodInvocationExpression.Arguments.Count > 2 || methodInvocationExpression.Arguments.Count < 1 || methodInvocationExpression.Arguments[0].CodeNodeType != CodeNodeType.MemberHandleExpression || methodInvocationExpression.Arguments.Count == 2 && methodInvocationExpression.Arguments[1].CodeNodeType != CodeNodeType.MemberHandleExpression || methodInvocationExpression.MethodExpression.Method == null || methodInvocationExpression.MethodExpression.Method.get_Name() != "GetMethodFromHandle" || methodInvocationExpression.MethodExpression.Method.get_DeclaringType() == null || methodInvocationExpression.MethodExpression.Method.get_DeclaringType().get_FullName() != "System.Reflection.MethodBase")
			{
				return false;
			}
			methodRef = (methodInvocationExpression.Arguments[0] as MemberHandleExpression).MemberReference as MethodReference;
			return (object)methodRef != (object)null;
		}

		private bool TryRemoveExpressionStatment(ExpressionStatement statement)
		{
			BlockStatement parent = statement.Parent as BlockStatement;
			if (parent == null)
			{
				return false;
			}
			parent.Statements.Remove(statement);
			return true;
		}

		private bool TryRemoveUnusedVariableAssignments(BlockStatement body)
		{
			bool flag;
			bool flag1;
			HashSet<VariableReference> variableReferences = new HashSet<VariableReference>();
			do
			{
				flag = false;
				HashSet<VariableReference>.Enumerator enumerator = this.usedVariables.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						VariableReference current = enumerator.Current;
						if (variableReferences.Contains(current))
						{
							continue;
						}
						HashSet<ExpressionStatement> item = this.variableToAssigingStatementsMap[current];
						if ((new RebuildExpressionTreesStep.VariableUsageFinder(current, item)).IsUsed(body))
						{
							continue;
						}
						HashSet<ExpressionStatement>.Enumerator enumerator1 = item.GetEnumerator();
						try
						{
							while (enumerator1.MoveNext())
							{
								if (this.TryRemoveExpressionStatment(enumerator1.Current))
								{
									continue;
								}
								flag1 = false;
								return flag1;
							}
						}
						finally
						{
							((IDisposable)enumerator1).Dispose();
						}
						flag = true;
						variableReferences.Add(current);
					}
					continue;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return flag1;
			}
			while (flag);
			return this.usedVariables.Count == variableReferences.Count;
		}

		private bool TryUpdateInitializer(ExpressionStatement node)
		{
			int num;
			BinaryExpression expression = node.Expression as BinaryExpression;
			ArrayIndexerExpression left = expression.Left as ArrayIndexerExpression;
			if (left.Target == null || left.Target.CodeNodeType != CodeNodeType.VariableReferenceExpression)
			{
				return false;
			}
			VariableReference variable = (left.Target as VariableReferenceExpression).Variable;
			if (!this.variableToLastUninitializedIndex.TryGetValue(variable, out num))
			{
				return false;
			}
			if (left.Indices == null || left.Indices.Count != 1)
			{
				return false;
			}
			int integerValue = this.GetIntegerValue(left.Indices[0] as LiteralExpression);
			if (integerValue == num)
			{
				(this.variableToValueMap[variable] as ArrayCreationExpression).Initializer.Expressions.Add((Expression)this.Visit(expression.Right.CloneExpressionOnly()));
				this.variableToAssigingStatementsMap[variable].Add(node);
				this.variableToLastUninitializedIndex[variable] = integerValue + 1;
				return true;
			}
			this.variableToLastUninitializedIndex.Remove(variable);
			this.variableToValueMap.Remove(variable);
			this.variableToAssigingStatementsMap.Remove(variable);
			if (this.usedVariables.Contains(variable))
			{
				this.failure = true;
			}
			return false;
		}

		public override ICodeNode Visit(ICodeNode node)
		{
			if (this.failure)
			{
				return node;
			}
			return base.Visit(node);
		}

		public override ICodeNode VisitBlockStatement(BlockStatement node)
		{
			this.variableToLastUninitializedIndex.Clear();
			this.variableToValueMap.Clear();
			return base.VisitBlockStatement(node);
		}

		public override ICodeNode VisitExpressionStatement(ExpressionStatement node)
		{
			if (node.Expression.CodeNodeType == CodeNodeType.BinaryExpression)
			{
				BinaryExpression expression = node.Expression as BinaryExpression;
				if (expression.IsAssignmentExpression)
				{
					if (expression.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
					{
						this.RecordVariableAssignment(node);
						return node;
					}
					if (expression.Left.CodeNodeType == CodeNodeType.ArrayIndexerExpression && this.TryUpdateInitializer(node))
					{
						return node;
					}
				}
			}
			return base.VisitExpressionStatement(node);
		}

		public override ICodeNode VisitLambdaExpression(LambdaExpression node)
		{
			if (!node.IsExpressionTreeLambda)
			{
				return base.VisitLambdaExpression(node);
			}
			return node;
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			this.conversionDepth++;
			this.conversionDepth--;
			return this.ConvertInvocation(node) ?? base.VisitMethodInvocationExpression(node);
		}

		public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			Expression expression;
			if (this.conversionDepth > 0)
			{
				if (this.variableToValueMap.TryGetValue(node.Variable, out expression))
				{
					this.usedVariables.Add(node.Variable);
					return this.Visit(expression.CloneExpressionOnly());
				}
				this.failure = !this.context.MethodContext.ClosureVariableToFieldValue.ContainsKey(node.Variable);
			}
			return base.VisitVariableReferenceExpression(node);
		}

		private class ClosureVariablesRemover : BaseCodeTransformer
		{
			private readonly MethodSpecificContext methodContext;

			public ClosureVariablesRemover(MethodSpecificContext methodContext)
			{
				this.methodContext = methodContext;
			}

			public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
			{
				Dictionary<FieldDefinition, Expression> fieldDefinitions;
				Expression expression;
				if (node.Target != null && node.Target.CodeNodeType == CodeNodeType.VariableReferenceExpression)
				{
					VariableReference variable = (node.Target as VariableReferenceExpression).Variable;
					if (this.methodContext.ClosureVariableToFieldValue.TryGetValue(variable, out fieldDefinitions))
					{
						FieldDefinition fieldDefinition = node.Field.Resolve();
						if (fieldDefinition != null && fieldDefinitions.TryGetValue(fieldDefinition, out expression))
						{
							return expression.CloneExpressionOnly();
						}
					}
				}
				return base.VisitFieldReferenceExpression(node);
			}
		}

		private class ExpressionTreesFinder : BaseCodeVisitor
		{
			private bool containsExpressionTree;

			public ExpressionTreesFinder()
			{
			}

			public bool ContainsExpressionTree(BlockStatement body)
			{
				this.containsExpressionTree = false;
				this.Visit(body);
				return this.containsExpressionTree;
			}

			public override void Visit(ICodeNode node)
			{
				if (!this.containsExpressionTree)
				{
					base.Visit(node);
				}
			}

			public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
			{
				if (node.MethodExpression != null && node.MethodExpression.Method != null && !node.MethodExpression.Method.get_HasThis() && node.MethodExpression.Method.get_Name() == "Lambda" && node.MethodExpression.Method.get_DeclaringType() != null && node.MethodExpression.Method.get_DeclaringType().get_FullName() == "System.Linq.Expressions.Expression")
				{
					this.containsExpressionTree = true;
					return;
				}
				base.VisitMethodInvocationExpression(node);
			}
		}

		private class VariableUsageFinder : BaseCodeVisitor
		{
			private readonly VariableReference variable;

			private readonly HashSet<ExpressionStatement> assignments;

			private bool isUsed;

			public VariableUsageFinder(VariableReference variable, HashSet<ExpressionStatement> assignments)
			{
				this.variable = variable;
				this.assignments = assignments;
			}

			public bool IsUsed(BlockStatement body)
			{
				this.isUsed = false;
				this.Visit(body);
				return this.isUsed;
			}

			public override void Visit(ICodeNode node)
			{
				if (!this.isUsed)
				{
					base.Visit(node);
				}
			}

			public override void VisitExpressionStatement(ExpressionStatement node)
			{
				if (!this.assignments.Contains(node))
				{
					base.VisitExpressionStatement(node);
				}
			}

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				if ((object)node.Variable == (object)this.variable)
				{
					this.isUsed = true;
				}
			}
		}
	}
}