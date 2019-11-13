using GraphQL.Types;
using GraphQL.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Xunit;

namespace GraphQL.SchemaDeclare.Tests
{
	//public class StarWarsData
	//{
	//	private List<Human> _humans = new List<Human>();

	//	public Human AddHuman(Human human)
	//	{
	//		human.Id = Guid.NewGuid().ToString();
	//		_humans.Add(human);
	//		return human;
	//	}
	//}

	//public class Human
	//{
	//	public string Id { get; set; }
	//	public string Name { get; set; }
	//	public string HomePlanet { get; set; }
	//}

	//public class HumanInput
	//{
	//	public string Name { get; set; }
	//}

	//public class HumanInputType : InputObjectGraphType<HumanInput>
	//{
	//	public HumanInputType()
	//	{
	//		Name = "HumanInput";
	//		Field(_ => _.Name);
	//	}
	//}

	//public class HumanType : ObjectGraphType<Human>
	//{
	//	public HumanType()
	//	{
	//		Name = "Human";

	//		Field(h => h.Id).Description("The id of the human.");
	//		Field(h => h.Name, nullable: true).Description("The name of the human.");
	//		Field(h => h.HomePlanet, nullable: true).Description("The home planet of the human.");
	//	}
	//}

	//public class StarWarsController
	//{
	//	StarWarsData data;

	//	public StarWarsController(StarWarsData data) => this.data = data;

	//	public Human CreateHuman([Required]HumanInput human) => data.AddHuman(new Human() { Name = human.Name });
	//}

	//public class StarWarsMutation : ObjectGraphType
	//{
	//	public StarWarsMutation(StarWarsData data)
	//	{
	//		this.Mutation<StarWarsController>(c => c.CreateHuman(default));
	//	}
	//}

	//public class StarWarsSchema : Schema
	//{
	//	public StarWarsSchema(IDependencyResolver dependencyResolver) : base(dependencyResolver)
	//	{
	//		this.Mutation = new StarWarsMutation(new StarWarsData());
	//	}
	//}

	//public class ExampleTests
	//{
	//	[Fact]
	//	public void WhenDeclaringSchemaThenGenerationIsAsExpected()
	//	{
	//		GraphTypeTypeRegistry.Register<Human, HumanType>();
	//		GraphTypeTypeRegistry.Register<HumanInput, HumanInputType>();

	//		var schema = new StarWarsSchema(new FuncDependencyResolver());

	//		schema.Initialize();

	//		var type = schema.FindType("StarWarsMutation") as ObjectGraphType;

	//		Assert.NotNull(type);
	//		Assert.NotEmpty(type.Fields);

	//		var field = type.Fields.FirstOrDefault(_ => _.Name == "createHuman");
	//		Assert.NotNull(field);

	//		var human = new HumanInput() { Name = "Aggadeen" };
	//		var result = field.Resolver.Resolve(new ResolveFieldContext()
	//		{
	//			Arguments = new Dictionary<string, object>()
	//			{
	//				{ "human", human }
	//			}
	//		});

	//		var expectedData = new StarWarsData();
	//		var expectedResult = (new StarWarsController(expectedData)).CreateHuman(human);
	//		Assert.Equal(expectedResult.Name, ((Human)result).Name);
	//	}
	//}
}
