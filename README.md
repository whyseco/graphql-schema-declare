# graphql-schema-declare
An elegant and simple way to declare your graphql queries and mutations with a single line.


## Example

### The old way :dizzy_face:

```csharp
public class StarWarsMutation : ObjectGraphType
{
  public StarWarsMutation(StarWarsData data)
  {
    Field<HumanType>(
      "createHuman",
      arguments: new QueryArguments(
        new QueryArgument<NonNullGraphType<HumanInputType>> {Name = "human"}
      ),
      resolve: context =>
      {
        var human = context.GetArgument<Human>("human");
        return data.AddHuman(human);
      });
  }
}
```


### The elegant way using SchemaDeclare :heart_eyes:

```csharp
using GraphQL.SchemaDeclare;

public class StarWarsController
{
	StarWarsData data;

	public StarWarsController(StarWarsData data) => this.data = data;

	public Human CreateHuman([Required]HumanInput human) => data.AddHuman(new Human() { Name = human.Name });
}

public class StarWarsMutation : ObjectGraphType
{
  public StarWarsMutation()
  {
	this.Mutation<StarWarsController>(c => c.CreateHuman(default));
  }
}
```


## Getting started

### Installation

```
PM> Install-Package GraphQL.SchemaDeclare
```

### Usage

Add the following namespace where you wanna use the extension methods :
```csharp
using GraphQL.SchemaDeclare;
```

Then in your query/mutation construction you can use :
```csharp
public class StarWarsMutation : ObjectGraphType
{
  public StarWarsMutation()
  {
	this.Mutation<StarWarsController>(c => c.CreateHuman(default));
	this.MutationAsync<StarWarsController>(c => c.CreateHumanAsync(default));
	this.Mutation<StarWarsController>("anotherCreateHuman", c => c.CreateHuman(default));
  }
}

public class StarWarsQuery : ObjectGraphType
{
  public StarWarsQuery()
  {
	this.Query<StarWarsController>(c => c.GetHuman(default));
	this.QueryAsync<StarWarsController>(c => c.GetHumanAsync(default));
	this.Query<StarWarsController>("anotherGetHuman", c => c.GetHuman(default));
  }
}
```

:rotating_light: Don't forget to register your complex type using `GraphTypeTypeRegistry.Register`

Example :
```csharp
GraphTypeTypeRegistry.Register<Human, HumanType>();
GraphTypeTypeRegistry.Register<HumanInput, HumanInputType>();
```


You can add the `[Required]` attribute on method parameters to force non null parameter on reference type.

## DependencyInjection

To use dependency injection in your controller, you must set `ObjectGraphTypeExtensions.DependencyResolver` and register :
- ITypeToGraphTypeTransformer
- IFieldInfoToFieldTypeTransformer
- IExpressionToFieldInfoGenerator
- IFieldInfoResolver

Example :
```csharp
public void ConfigureServices(IServiceCollection services)
{
	// Register 
	services.AddSingleton<GraphQL.IDependencyResolver>(s =>  new GraphQL.FuncDependencyResolver(type =>
		{
			var accessor = s.GetRequiredService<IHttpContextAccessor>();
			return accessor.HttpContext.RequestServices.GetRequiredService(type);
		}));
	services.AddSingleton<GraphQL.SchemaDeclare.GenerationServices.IExpressionToFieldInfoGenerator,
			GraphQL.SchemaDeclare.GenerationServices.ExpressionToFieldInfoGenerator>();
	services.AddSingleton<GraphQL.SchemaDeclare.GenerationServices.IFieldInfoToFieldTypeTransformer,
			GraphQL.SchemaDeclare.GenerationServices.FieldInfoToFieldTypeTransformer>();
	services.AddSingleton<GraphQL.SchemaDeclare.GenerationServices.ITypeToGraphTypeTransformer,
			GraphQL.SchemaDeclare.GenerationServices.TypeToGraphTypeTransformer>();
	services.AddSingleton<GraphQL.SchemaDeclare.GenerationServices.TypeToGraphTypeTransformerOptions>();
	services.AddSingleton<GraphQL.SchemaDeclare.Resolvers.IFieldInfoResolver,
			GraphQL.SchemaDeclare.Resolvers.FieldInfoResolver>();
}

public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
{
	// Set DependencyResolver
	GraphQL.SchemaDeclare.ObjectGraphTypeExtensions.DependencyResolver = app.ApplicationServices.GetRequiredService<GraphQL.IDependencyResolver>();
}
```

## How does it works

We read the expression in argument to discover :
 - Method name : used as field name
 - Argument name : used as query argument name
 - Argument type : Converted using `TypeToGraphTypeTransformer` and using mostly `GraphTypeTypeRegistry`
 - Return type : used as field type

Value types are marked as `NonNullGraphType` unless nullable (see : TypeToGraphTypeTransformerTests). Reference types can be generated as `NonNullGraphType` if you add the attribute `[Required]` on the parameter.
