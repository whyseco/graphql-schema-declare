using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace GraphQL.SchemaDeclare.GenerationServices
{
	public class ExpressionToFieldInfoGenerator : IExpressionToFieldInfoGenerator
	{
		private readonly ITypeToGraphTypeTransformer typeToGraphTypeTransformer;
		private readonly TypeToGraphTypeTransformerOptions typeToGraphTypeTransformerOptions;

		public ExpressionToFieldInfoGenerator(ITypeToGraphTypeTransformer typeToGraphTypeTransformer, TypeToGraphTypeTransformerOptions typeToGraphTypeTransformerOptions)
		{
			this.typeToGraphTypeTransformer = typeToGraphTypeTransformer;
			this.typeToGraphTypeTransformerOptions = typeToGraphTypeTransformerOptions;
		}

		private Type GetGraphType(ParameterInfo parameter)
		{
			var hasDefaultValue = parameter.DefaultValue != DBNull.Value;
			var graphType = this.typeToGraphTypeTransformer.GetGraphType(parameter.ParameterType, hasDefaultValue, 
				parameter.CustomAttributes, typeToGraphTypeTransformerOptions);

			if (graphType is null)
				throw new NullReferenceException($"Could not determine Graph Type for type : {parameter.ParameterType}");

			return graphType;
		}

		private (MethodCallExpression callExpression, QueryArguments arguments) GetSignature(LambdaExpression methodCall)
		{
			if (methodCall == null) throw new ArgumentNullException(nameof(methodCall));

			var callExpression = methodCall.Body as MethodCallExpression;
			if (callExpression == null)
			{
				// Handle Convert when return type is boolean (MethodCall is wrap in a Convert)
				if (methodCall.Body is UnaryExpression ue && ue.NodeType == ExpressionType.Convert)
				{
					callExpression = ue.Operand as MethodCallExpression;
				}
				if (callExpression == null)
					throw new ArgumentException("Expression body should be of type `MethodCallExpression`", nameof(methodCall));
			}

			var method = callExpression.Method;

			var list = new List<QueryArgument>();
			foreach (var parameter in method.GetParameters())
			{
				var defaultValue = parameter.DefaultValue == DBNull.Value ? null : parameter.DefaultValue;

				var argType = this.GetGraphType(parameter);
				var argName = parameter.Name.ToCamelCase();

				list.Add(new QueryArgument(argType) { Name = argName, DefaultValue = defaultValue });
			}

			return (callExpression, list.Count == 0 ? null : new QueryArguments(list));
		}

		public FieldInfo FromExpression(LambdaExpression methodCall, Type explicitType, bool getReturnGraphType = true)
		{
			var (callExpression, arguments) = this.GetSignature(methodCall);
			var type = explicitType ?? callExpression.Method.DeclaringType;
			var method = callExpression.Method;
			var returnType = getReturnGraphType ? this.GetGraphType(method.ReturnParameter) : null;

			return new FieldInfo(returnType, method.Name.ToCamelCase(), arguments, type, method);

		}

		public FieldInfo FromExpressionAsync(LambdaExpression methodCall, Type explicitType)
		{
			var (callExpression, arguments) = this.GetSignature(methodCall);
			var type = explicitType ?? callExpression.Method.DeclaringType;
			var method = callExpression.Method;
			var returnType = this.GetGraphType(method.ReturnParameter);

			return new FieldInfo(returnType, method.Name.ToCamelCase(), arguments, type, method);

		}
	}
}
