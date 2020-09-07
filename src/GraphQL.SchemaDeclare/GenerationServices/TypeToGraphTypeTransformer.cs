using GraphQL.Types;
using GraphQL.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraphQL.SchemaDeclare.GenerationServices
{
	public class TypeToGraphTypeTransformer : ITypeToGraphTypeTransformer
	{
		protected bool IsTypeNullable(Type rootType, Type realType)
		{
			var isRootTypeANullable = this.IsNullable(rootType);
			var isRealTypeNullable = !realType.IsValueType || this.IsNullable(realType);
			var isNullable = isRootTypeANullable || isRealTypeNullable;

			return isNullable;
		}

		protected bool HasRequiredAttribute(IEnumerable<CustomAttributeData> customAttributes)
		{
			return customAttributes.Any(_ => _.AttributeType == typeof(RequiredAttribute));
		}

		protected bool IsNullable(Type rootType, Type realType, IEnumerable<CustomAttributeData> customAttributes)
		{
			var isTypeNullable = this.IsTypeNullable(rootType, realType);
			var hasRequiredAttribute = this.HasRequiredAttribute(customAttributes);

			return isTypeNullable && !hasRequiredAttribute;
		}

		protected bool IsTypeList(Type type)
		{
			return (type.IsGenericType 
				&&  typeof(IEnumerable).IsAssignableFrom(type.GetGenericTypeDefinition()) 
				&& type != typeof(string)) 
				|| type.IsArray;
		}

		protected Type GenerateNonNullGraphType(Type graphType)
		{
			if (graphType is null)
				return null;
			var gType = typeof(NonNullGraphType<>);
			Type[] typeArgs = { graphType };
			var finalType = gType.MakeGenericType(typeArgs);
			return finalType;
		}

		protected Type GetGraphTypeWithNullableInfo(Type rootType, IEnumerable<CustomAttributeData> customAttributes, TypeToGraphTypeTransformerOptions options)
		{
			var realType = this.GetRealType(rootType);
			var graphType = GetGraphTypeFromRealType(realType, options);

			var isNullable = this.IsNullable(rootType, realType, customAttributes);

			if (!isNullable)
				graphType = GenerateNonNullGraphType(graphType);
			return graphType;
		}

		protected Type GetGraphTypeWithoutNullableInfo(Type rootType, TypeToGraphTypeTransformerOptions options)
		{
			var realType = this.GetRealType(rootType);
			var graphType = GetGraphTypeFromRealType(realType, options);

			return graphType;
		}

		protected bool IsNullable(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		protected Type GetRealType(Type originType)
		{
			if (this.IsNullable(originType))
				originType = originType.GenericTypeArguments.First();
			if (originType.IsGenericType && originType.GetGenericTypeDefinition() == typeof(Task<>))
				originType = originType.GenericTypeArguments.First();

			return originType;
		}

		protected Type GetGraphTypeFromRealType(Type realType, TypeToGraphTypeTransformerOptions options)
		{
			var isTypeList = this.IsTypeList(realType);
			if (isTypeList)
			{
				var gType = typeof(ListGraphType<>);
				var clrType = realType.IsArray ? realType.GetElementType() : realType.GenericTypeArguments.First();

                realType = this.GetRealType(clrType);

                var graphType = GraphTypeTypeRegistry.Get(realType);

				if (options.AddNullableInfo && !this.IsTypeNullable(clrType, clrType))
				{
					graphType = GenerateNonNullGraphType(graphType);
				}

				Type[] typeArgs = { graphType };
				var finalType = gType.MakeGenericType(typeArgs);
				return finalType;
			}

			if (realType == typeof(DateTime))
				return typeof(DateTimeGraphType);
			if (realType.IsEnum)
			{
				var gType = typeof(EnumerationGraphType<>);
				Type[] typeArgs = { realType };
				var finalType = gType.MakeGenericType(typeArgs);
				return finalType;
			}

			return GraphTypeTypeRegistry.Get(realType);
		}

		public Type GetGraphType(Type originType, bool hasDefaultValue, IEnumerable<CustomAttributeData> customAttributes, TypeToGraphTypeTransformerOptions options)
		{
			if (options.AddNullableInfo && !hasDefaultValue)
				return this.GetGraphTypeWithNullableInfo(originType, customAttributes, options);
			return this.GetGraphTypeWithoutNullableInfo(originType, options);
		}
	}
}
