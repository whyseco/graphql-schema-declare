using GraphQL.SchemaDeclare.GenerationServices;
using GraphQL.SchemaDeclare.Tests.Fixtures;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace GraphQL.SchemaDeclare.Tests
{
	public class TypeToGraphTypeTransformerTests: IClassFixture<ServiceProviderFixture>
	{
        private ServiceProvider serviceProvider;

        public TypeToGraphTypeTransformerTests(ServiceProviderFixture fixture)
        {
            this.serviceProvider = fixture.ServiceProvider;
        }

		[Theory, MemberData(nameof(Data))]
		public void WhenConvertingTypeThenGraphTypeIsAsExpected(Type originType, Type graphType, bool addNullableInfo, bool hasDefaultValue)
		{
			var typeToGraphTypeTransformer = this.serviceProvider.GetRequiredService<ITypeToGraphTypeTransformer>();

            var resultGraphType = typeToGraphTypeTransformer.GetGraphType(originType, hasDefaultValue, 
				Enumerable.Empty<CustomAttributeData>(),
				new TypeToGraphTypeTransformerOptions() { AddNullableInfo = addNullableInfo });

			Assert.Equal(graphType, resultGraphType);
		}

		public static IEnumerable<object[]> Data
		{
			get
			{
				return new[]
				{
					new object[] { typeof(string),		typeof(StringGraphType),			true, false },
					new object[] { typeof(int[]),		typeof(ListGraphType<NonNullGraphType<IntGraphType>>), true, false },
					new object[] { typeof(StringSplitOptions), typeof(NonNullGraphType<EnumerationGraphType<StringSplitOptions>>), true, false },
					new object[] { typeof(int?),		typeof(IntGraphType),				true, false },
					new object[] { typeof(Task<bool>),	typeof(NonNullGraphType<BooleanGraphType>), true, false },
					new object[] { typeof(int),			typeof(IntGraphType),				false, false },
					new object[] { typeof(Task<bool>),	typeof(BooleanGraphType),			false, false },
                    new object[] { typeof(int[]),      typeof(ListGraphType<IntGraphType>), false, false },
                    new object[] { typeof(int?[]),		typeof(ListGraphType<IntGraphType>), true, false },
					new object[] { typeof(int),			typeof(IntGraphType),				true, true },
					new object[] { typeof(int[]),		typeof(ListGraphType<NonNullGraphType<IntGraphType>>), true, true },
				};
			}
		}
	}
}
