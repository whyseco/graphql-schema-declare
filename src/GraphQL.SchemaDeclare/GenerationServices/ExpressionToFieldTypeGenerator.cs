using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GraphQL.SchemaDeclare.GenerationServices
{
	public class ExpressionToFieldTypeGenerator
	{
		private readonly IExpressionToFieldInfoGenerator expressionToFieldInfoTransformer;
		private readonly IFieldInfoToFieldTypeTransformer fieldInfoToFieldTypeTransformer;
		private readonly Resolvers.IFieldInfoResolver fieldInfoResolver;

		public ExpressionToFieldTypeGenerator(IExpressionToFieldInfoGenerator expressionToFieldInfoTransformer,
			IFieldInfoToFieldTypeTransformer fieldInfoToFieldTypeTransformer,
			Resolvers.IFieldInfoResolver fieldInfoResolver)
		{
			this.expressionToFieldInfoTransformer = expressionToFieldInfoTransformer;
			this.fieldInfoToFieldTypeTransformer = fieldInfoToFieldTypeTransformer;
			this.fieldInfoResolver = fieldInfoResolver;
		}

		public FieldType GenerateAsyncFieldType<Controller>(IComplexGraphType graphType, Expression<Func<Controller, Task>> methodCall, string name = null, Type returnGraphType = null)
		{
			var fieldInfo = expressionToFieldInfoTransformer.FromExpressionAsync(methodCall, typeof(Controller));
			var fieldType = fieldInfoToFieldTypeTransformer.TransformToFieldType(fieldInfo, returnGraphType);
			if (!(name is null))
				fieldType.Name = name;
			fieldType.Resolver = fieldInfoResolver.GenerateFieldResolverAsync(graphType, fieldInfo);

			return fieldType;
		}

		public FieldType GenerateFieldType<Controller, ReturnType>(IComplexGraphType graphType, Expression<Func<Controller, ReturnType>> methodCall, string name = null, Type returnGraphType = null)
		{
			var fieldInfo = expressionToFieldInfoTransformer.FromExpression(methodCall, typeof(Controller));
			var fieldType = fieldInfoToFieldTypeTransformer.TransformToFieldType(fieldInfo, returnGraphType);
			if (!(name is null))
				fieldType.Name = name;
			fieldType.Resolver = fieldInfoResolver.GenerateFieldResolver(graphType, fieldInfo);

			return fieldType;
		}
	}
}
