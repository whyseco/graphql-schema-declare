using GraphQL.SchemaDeclare.GenerationServices;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace GraphQL.SchemaDeclare.Tests
{
	public class ExpressionToFieldInfoGeneratorTests
	{
		public string Display([Required]string s)
		{
			return s;
		}

		[Fact]
		void WhenGeneratingMethodWithRequiredInputThenNotNullableGraphTypeAreGenerated()
		{
			var generator = new ExpressionToFieldInfoGenerator(new TypeToGraphTypeTransformer(), new TypeToGraphTypeTransformerOptions());

			Expression<Func<ExpressionToFieldInfoGeneratorTests, string>> methodCall = (s) => s.Display(default);

			var fieldType = generator.FromExpression(methodCall, typeof(ExpressionToFieldInfoGeneratorTests));

			var firstArgument = fieldType.Arguments.FirstOrDefault();

			Assert.Equal(typeof(NonNullGraphType<StringGraphType>), firstArgument.Type);
		}
	}
}
