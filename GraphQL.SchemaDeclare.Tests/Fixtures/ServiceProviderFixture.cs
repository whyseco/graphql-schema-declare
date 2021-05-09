using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphQL.SchemaDeclare.GenerationServices;
using GraphQL.Types;

namespace GraphQL.SchemaDeclare.Tests.Fixtures
{
    public class ServiceProviderFixture
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public ServiceProviderFixture()
        {
            var clrToGraphTypeMappings = new ClrToGraphTypeMappings();
            var schema = new EmptySchema();
            IEnumerable<(Type clrType, Type graphType)> typeMappings = schema.BuiltInTypeMappings;

            foreach (var (clrType, graphType) in typeMappings)
            {
                if (!clrToGraphTypeMappings.ContainsKey(clrType))
                {
                    clrToGraphTypeMappings.Add(clrType, graphType);
                }
            }

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddGraphQLSchemaDeclareService(null, () => clrToGraphTypeMappings);
            ServiceProvider = serviceCollection.BuildServiceProvider();

        }
    }

    public class EmptySchema : Schema { }
}
