using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Config.OpenApi
{
	public sealed class FixProcessor : IOperationProcessor
	{
		[Nullable(1)]
		private readonly static JsonSchema StringSchema;

		static FixProcessor()
		{
			JsonSchema jsonSchema = new JsonSchema();
			jsonSchema.set_Type(64);
			FixProcessor.StringSchema = jsonSchema;
		}

		public FixProcessor()
		{
		}

		[NullableContext(1)]
		public bool Process(OperationProcessorContext context)
		{
			ParameterInfo parameterInfo;
			OpenApiParameter openApiParameter;
			foreach (KeyValuePair<ParameterInfo, OpenApiParameter> parameter in context.get_Parameters())
			{
				parameter.Deconstruct(out parameterInfo, out openApiParameter);
				OpenApiParameter openApiParameter1 = openApiParameter;
				if (!openApiParameter1.get_IsRequired())
				{
					continue;
				}
				JsonSchema schema = openApiParameter1.get_Schema();
				if (schema == null || schema.get_Type() != 64)
				{
					continue;
				}
				openApiParameter1.set_Schema(FixProcessor.StringSchema);
			}
			return true;
		}
	}
}