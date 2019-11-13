using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GraphQL.SchemaDeclare.Tests
{
	public class Controller
	{
		public string HelloWorld(int test)
		{
			return $"Hello World {test} !";
		}

		public async Task<string> HelloWorldAsync(int test)
		{
			await Task.Delay(10);
			return $"Hello World {test} !";
		}

		public bool Boolean(bool test)
		{
			return test;
		}

		public string DefaultValue(string test = "Hello Default")
		{
			return test;
		}
	}

	public class TestQuery : ObjectGraphType<object>
	{
		public TestQuery()
		{
			this.Query<Controller>(_ => _.HelloWorld(default));
			this.QueryAsync<Controller>(_ => _.HelloWorldAsync(default));
			this.Query<Controller>("renamedBoolean", _ => _.Boolean(default));
			this.Query<Controller>(_ => _.DefaultValue(default));
		}
	}

	public class TestSchema : Schema
	{
		public TestSchema()
		{
			Query = new TestQuery();
		}
	}

	public class SchemaGenerationTests
	{
		[Fact]
		public void WhenDeclaringQueryThenGenerationIsAsExpected()
		{
			var schema = new TestSchema();

			schema.Initialize();

			var type = schema.FindType("TestQuery") as ObjectGraphType<object>;

			Assert.NotNull(type);
			Assert.NotEmpty(type.Fields);

			var field = type.Fields.FirstOrDefault(_ => _.Name == "helloWorld");
			Assert.NotNull(field);

			var test = 5;
			var result = field.Resolver.Resolve(new ResolveFieldContext()
			{
				Arguments = new Dictionary<string, object>()
				{
					{ "test", test }
				}
			});

			var expectedResult = (new Controller()).HelloWorld(test);
			Assert.Equal(expectedResult, result);
		}

		[Fact]
		public async Task WhenDeclaringAsyncQueryThenGenerationIsAsExpected()
		{
			var schema = new TestSchema();

			schema.Initialize();

			var type = schema.FindType("TestQuery") as ObjectGraphType<object>;

			Assert.NotNull(type);
			Assert.NotEmpty(type.Fields);

			var field = type.Fields.FirstOrDefault(_ => _.Name == "helloWorldAsync");
			Assert.NotNull(field);

			var test = 6;
			var result = ((Task<object>)field.Resolver.Resolve(new ResolveFieldContext()
			{
				Arguments = new Dictionary<string, object>()
				{
					{ "test", test }
				}
			})).Result;

			var expectedResult = await (new Controller()).HelloWorldAsync(test);
			Assert.Equal(expectedResult, result);
		}

		[Fact]
		public void WhenDeclaringBooleanQueryWithNameThenGenerationIsAsExpected()
		{
			var schema = new TestSchema();

			schema.Initialize();

			var type = schema.FindType("TestQuery") as ObjectGraphType<object>;

			Assert.NotNull(type);
			Assert.NotEmpty(type.Fields);

			var field = type.Fields.FirstOrDefault(_ => _.Name == "renamedBoolean");
			Assert.NotNull(field);

			var test = true;
			var result = field.Resolver.Resolve(new ResolveFieldContext()
			{
				Arguments = new Dictionary<string, object>()
				{
					{ "test", test }
				}
			});

			var expectedResult = (new Controller()).Boolean(test);
			Assert.Equal(expectedResult, result);
		}

		[Fact]
		public void WhenDeclaringQueryWithDefaultValueThenGenerationIsAsExpected()
		{
			var schema = new TestSchema();

			schema.Initialize();

			var type = schema.FindType("TestQuery") as ObjectGraphType<object>;

			Assert.NotNull(type);
			Assert.NotEmpty(type.Fields);

			var field = type.Fields.FirstOrDefault(_ => _.Name == "defaultValue");
			Assert.NotNull(field);

			var result = field.Resolver.Resolve(new ResolveFieldContext());

			var expectedResult = (new Controller()).DefaultValue();
			Assert.Equal(expectedResult, result);
		}
	}
}
