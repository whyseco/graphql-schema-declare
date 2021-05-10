using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Types;

namespace GraphQL.SchemaDeclare.GenerationServices
{
    /// <summary>
    /// 
    /// </summary>
    public class TypeToGraphTypeTransformer : ITypeToGraphTypeTransformer
    {
        private readonly ClrToGraphTypeMappings typeMappings;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeToGraphTypeTransformer"/> class.
        /// </summary>
        /// <param name="typeMappings"></param>
        public TypeToGraphTypeTransformer(ClrToGraphTypeMappings typeMappings)
        {
            this.typeMappings = typeMappings;
        }

        protected bool IsTypeNullable(Type rootType, Type realType)
        {
            var isRootTypeANullable = IsNullable(rootType);
            var isRealTypeNullable = !realType.IsValueType || IsNullable(realType);
            var isNullable = isRootTypeANullable || isRealTypeNullable;

            return isNullable;
        }

        protected bool HasRequiredAttribute(IEnumerable<CustomAttributeData> customAttributes)
        {
            return customAttributes.Any(_ => _.AttributeType == typeof(RequiredAttribute));
        }

        protected bool IsNullable(Type rootType, Type realType, IEnumerable<CustomAttributeData> customAttributes)
        {
            var isTypeNullable = IsTypeNullable(rootType, realType);
            var hasRequiredAttribute = HasRequiredAttribute(customAttributes);

            return isTypeNullable && !hasRequiredAttribute;
        }

        protected static bool IsTypeList(Type type)
        {
            return (type.IsGenericType
                && typeof(IEnumerable).IsAssignableFrom(type.GetGenericTypeDefinition())
                && type != typeof(string))
                || type.IsArray;
        }

        protected Type GenerateNonNullGraphType(Type graphType)
        {
            if (graphType is null)
            {
                return null;
            }

            var genericType = typeof(NonNullGraphType<>);
            Type[] typeArgs = { graphType };
            var finalType = genericType.MakeGenericType(typeArgs);
            return finalType;
        }

        protected Type GetGraphTypeWithNullableInfo(Type rootType, IEnumerable<CustomAttributeData> customAttributes, TypeToGraphTypeTransformerOptions options)
        {
            var realType = GetRealType(rootType);
            var graphType = GetGraphTypeFromRealType(realType, options);

            var isNullable = IsNullable(rootType, realType, customAttributes);

            if (!isNullable)
            {
                graphType = GenerateNonNullGraphType(graphType);
            }

            return graphType;
        }

        protected Type GetGraphTypeWithoutNullableInfo(Type rootType, TypeToGraphTypeTransformerOptions options)
        {
            var realType = GetRealType(rootType);
            var graphType = GetGraphTypeFromRealType(realType, options);

            return graphType;
        }

        protected bool IsNullable(Type type)
        {
            return type?.IsGenericType == true && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        protected Type GetRealType(Type originType)
        {
            if (IsNullable(originType))
            {
                originType = originType.GenericTypeArguments.First();
            }

            if (originType.IsGenericType && originType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                originType = originType.GenericTypeArguments.First();
            }

            return originType;
        }

        protected Type GetGraphTypeFromRealType(Type realType, TypeToGraphTypeTransformerOptions options)
        {
            var isTypeList = IsTypeList(realType);
            if (isTypeList)
            {
                var genericType = typeof(ListGraphType<>);
                var clrType = realType.IsArray ? realType.GetElementType() : realType.GenericTypeArguments.First();

                realType = GetRealType(clrType);

                typeMappings.TryGetValue(realType, out Type graphType);

                if (options.AddNullableInfo && !IsTypeNullable(clrType, clrType))
                {
                    graphType = GenerateNonNullGraphType(graphType);
                }

                Type[] typeArgs = { graphType };
                var finalType = genericType.MakeGenericType(typeArgs);
                return finalType;
            }

            if (realType == typeof(DateTime))
            {
                return typeof(DateTimeGraphType);
            }

            if (realType.IsEnum)
            {
                var gType = typeof(EnumerationGraphType<>);
                Type[] typeArgs = { realType };
                var finalType = gType.MakeGenericType(typeArgs);
                return finalType;
            }

            return typeMappings.ContainsKey(realType) ? typeMappings[realType] : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originType"></param>
        /// <param name="hasDefaultValue"></param>
        /// <param name="customAttributes"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Type GetGraphType(
            Type originType,
            bool hasDefaultValue,
            IEnumerable<CustomAttributeData> customAttributes,
            TypeToGraphTypeTransformerOptions options)
        {
            return options.AddNullableInfo && !hasDefaultValue
                ? GetGraphTypeWithNullableInfo(originType, customAttributes, options)
                : GetGraphTypeWithoutNullableInfo(originType, options);
        }
    }
}
