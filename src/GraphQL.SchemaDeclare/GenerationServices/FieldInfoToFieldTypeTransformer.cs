using System;
using System.Collections.Generic;
using System.Text;
using GraphQL.Types;

namespace GraphQL.SchemaDeclare.GenerationServices
{
	public class FieldInfoToFieldTypeTransformer : IFieldInfoToFieldTypeTransformer
	{
		public FieldType TransformToFieldType(FieldInfo fieldInfo, Type returnType = null)
		{
			return new FieldType()
			{
				Arguments = fieldInfo.Arguments,
				Name = fieldInfo.GraphName,
				Type = returnType ?? fieldInfo.ReturnType,
			};
		}
	}
}
