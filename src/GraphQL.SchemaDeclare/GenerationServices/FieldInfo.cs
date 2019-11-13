using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GraphQL.SchemaDeclare.GenerationServices
{
	public class FieldInfo
	{
		public FieldInfo(Type returnType, string graphName, QueryArguments arguments, Type controllerType, MethodInfo action)
		{
			ReturnType = returnType;
			GraphName = graphName;
			Arguments = arguments;
			ControllerType = controllerType;
			Action = action;
		}

		public Type ReturnType { get; }

		public string GraphName { get; }

		public QueryArguments Arguments { get; }

		public Type ControllerType { get; }

		public MethodInfo Action { get; }
	}
}
