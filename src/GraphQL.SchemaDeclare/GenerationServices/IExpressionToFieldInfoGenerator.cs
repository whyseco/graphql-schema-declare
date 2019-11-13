using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GraphQL.SchemaDeclare.GenerationServices
{
	public interface IExpressionToFieldInfoGenerator
	{
		FieldInfo FromExpression(LambdaExpression methodCall, Type explicitType, bool getReturnGraphType = true);

		FieldInfo FromExpressionAsync(LambdaExpression methodCall, Type explicitType);
	}
}
