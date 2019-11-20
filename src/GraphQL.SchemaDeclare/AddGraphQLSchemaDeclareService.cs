using GraphQL.SchemaDeclare.GenerationServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.SchemaDeclare
{
    public static class GraphQLSchemaDeclareServiceExtension
    {
        public static void AddGraphQLSchemaDeclareService(this IServiceCollection services, Action<TypeToGraphTypeTransformerOptions> setTypeToGraphTypeOptions = null)
        {
            services.AddSingleton<GraphQL.IDependencyResolver>(s => new GraphQL.FuncDependencyResolver(type =>
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
            services.AddSingleton<GraphQL.SchemaDeclare.Resolvers.IFieldInfoResolver,
                    GraphQL.SchemaDeclare.Resolvers.FieldInfoResolver>();

            if (setTypeToGraphTypeOptions != null)
            {
                services.AddSingleton(s =>
                {
                    var TypeToGraphTypeTransformerOptions = new TypeToGraphTypeTransformerOptions();
                    setTypeToGraphTypeOptions(TypeToGraphTypeTransformerOptions);
                    return TypeToGraphTypeTransformerOptions;
                });
            }
            else
                services.AddSingleton<GraphQL.SchemaDeclare.GenerationServices.TypeToGraphTypeTransformerOptions>();
        }
    }
}
