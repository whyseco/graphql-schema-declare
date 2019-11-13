using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.SchemaDeclare.GenerationServices
{
	public interface IFieldInfoToFieldTypeTransformer
	{
		FieldType TransformToFieldType(FieldInfo fieldInfo, Type returnType = null);
	}
}
