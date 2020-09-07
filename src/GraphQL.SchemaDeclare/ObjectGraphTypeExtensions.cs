using GraphQL.SchemaDeclare.GenerationServices;
using GraphQL.SchemaDeclare.Resolvers;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GraphQL.SchemaDeclare
{
	public static class ObjectGraphTypeExtensions
	{
		public static IServiceProvider ServiceProvider { get; set; }

		private static ExpressionToFieldTypeGenerator ExpressionToFieldTypeGenerator { get {
				if (ServiceProvider is null)
				{
					return new ExpressionToFieldTypeGenerator(
						new ExpressionToFieldInfoGenerator(new TypeToGraphTypeTransformer(), new TypeToGraphTypeTransformerOptions()),
						new FieldInfoToFieldTypeTransformer(),
						new FieldInfoResolver(ServiceProvider));
				}
				return ServiceProvider.GetRequiredService<ExpressionToFieldTypeGenerator>();
			}
		}

		public static FieldType MutationAsync<Controller>(this IComplexGraphType graphType, Expression<Func<Controller, Task>> methodCall)
		{
			var fieldType = ExpressionToFieldTypeGenerator.GenerateAsyncFieldType(graphType, methodCall);
			graphType.AddField(fieldType);
			return fieldType;
		}

		public static FieldType Mutation<Controller>(this IComplexGraphType graphType, Expression<Func<Controller, object>> methodCall)
		{
			var fieldType = ExpressionToFieldTypeGenerator.GenerateFieldType(graphType, methodCall);
			graphType.AddField(fieldType);
			return fieldType;
		}

		public static FieldType MutationAsync<Controller>(this IComplexGraphType graphType, string name, Expression<Func<Controller, Task>> methodCall)
		{
			var fieldType = ExpressionToFieldTypeGenerator.GenerateAsyncFieldType(graphType, methodCall, name);
			graphType.AddField(fieldType);
			return fieldType;
		}

		public static FieldType Mutation<Controller>(this IComplexGraphType graphType, string name, Expression<Func<Controller, object>> methodCall)
		{
			var fieldType = ExpressionToFieldTypeGenerator.GenerateFieldType(graphType, methodCall, name);
			graphType.AddField(fieldType);
			return fieldType;
		}

		public static FieldType QueryAsync<Controller>(this IComplexGraphType graphType, Expression<Func<Controller, Task>> methodCall)
		{
			var fieldType = ExpressionToFieldTypeGenerator.GenerateAsyncFieldType(graphType, methodCall);
			graphType.AddField(fieldType);
			return fieldType;
		}

		public static FieldType Query<Controller>(this IComplexGraphType graphType, Expression<Func<Controller, object>> methodCall)
		{
			var fieldType = ExpressionToFieldTypeGenerator.GenerateFieldType(graphType, methodCall);
			graphType.AddField(fieldType);
			return fieldType;
		}

		public static FieldType QueryAsync<Controller>(this IComplexGraphType graphType, Type returnGraphType, Expression<Func<Controller, Task>> methodCall)
		{
			var fieldType = ExpressionToFieldTypeGenerator.GenerateAsyncFieldType(graphType, methodCall, returnGraphType: returnGraphType);
			graphType.AddField(fieldType);
			return fieldType;
		}

		public static FieldType Query<Controller>(this IComplexGraphType graphType, Type returnGraphType, Expression<Func<Controller, object>> methodCall)
		{
			var fieldType = ExpressionToFieldTypeGenerator.GenerateFieldType(graphType, methodCall, returnGraphType: returnGraphType);
			graphType.AddField(fieldType);
			return fieldType;
		}


		public static FieldType QueryAsync<Controller>(this IComplexGraphType graphType, string name, Expression<Func<Controller, Task>> methodCall)
		{
			var fieldType = ExpressionToFieldTypeGenerator.GenerateAsyncFieldType(graphType, methodCall, name);
			graphType.AddField(fieldType);
			return fieldType;
		}

		public static FieldType Query<Controller>(this IComplexGraphType graphType, string name, Expression<Func<Controller, object>> methodCall)
		{
			var fieldType = ExpressionToFieldTypeGenerator.GenerateFieldType(graphType, methodCall, name);
			graphType.AddField(fieldType);
			return fieldType;
		}
	}
}
