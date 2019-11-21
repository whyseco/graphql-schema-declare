using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.SchemaDeclare.Tests.Fixtures
{
    public class ServiceProviderFixture
    {
        public ServiceProvider ServiceProvider { get; private set; }
        public ServiceProviderFixture()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddGraphQLSchemaDeclareService();
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
