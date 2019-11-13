using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GraphQL.SchemaDeclare.GenerationServices
{
	public class TypeToGraphTypeTransformerOptions
	{
		public TypeToGraphTypeTransformerOptions()
		{
			this.AddNullableInfo = true;
		}

		public bool AddNullableInfo { get; set; }
	}

	public interface ITypeToGraphTypeTransformer
	{
		Type GetGraphType(Type originType, bool hasDefaultValue, IEnumerable<CustomAttributeData> customAttributes, TypeToGraphTypeTransformerOptions options);
	}
}
