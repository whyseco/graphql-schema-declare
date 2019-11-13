using GraphQL.Resolvers;
using GraphQL.SchemaDeclare.GenerationServices;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.SchemaDeclare.Resolvers
{
	public interface IFieldInfoResolver
	{
		IFieldResolver GenerateFieldResolver(IGraphType graphType, FieldInfo fieldInfo);
		IFieldResolver GenerateFieldResolverAsync(IComplexGraphType graphType, FieldInfo fieldInfo);
	}
}
