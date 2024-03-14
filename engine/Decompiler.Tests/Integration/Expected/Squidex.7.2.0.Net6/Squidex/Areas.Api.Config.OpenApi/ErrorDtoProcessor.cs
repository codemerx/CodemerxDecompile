using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag;
using NSwag.Collections;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Config.OpenApi
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class ErrorDtoProcessor : IDocumentProcessor
	{
		public ErrorDtoProcessor()
		{
		}

		private static void AddErrorResponses(OpenApiOperation operation, JsonSchema errorSchema)
		{
			string str;
			OpenApiResponse openApiResponse;
			if (!operation.get_Responses().ContainsKey("500"))
			{
				OpenApiResponse openApiResponse1 = new OpenApiResponse();
				openApiResponse1.set_Description("Operation failed.");
				openApiResponse1.set_Schema(errorSchema);
				operation.get_Responses()["500"] = openApiResponse1;
			}
			foreach (KeyValuePair<string, OpenApiResponse> response in operation.get_Responses())
			{
				response.Deconstruct(out str, out openApiResponse);
				string str1 = str;
				OpenApiResponse openApiResponse2 = openApiResponse;
				if (!(str1 != "404") || !str1.StartsWith("4", StringComparison.OrdinalIgnoreCase) || openApiResponse2.get_Schema() != null)
				{
					continue;
				}
				openApiResponse2.set_Schema(errorSchema);
			}
		}

		private static void CleanupResponses(OpenApiOperation operation)
		{
			string str;
			OpenApiResponse openApiResponse;
			bool flag;
			bool flag1;
			foreach (KeyValuePair<string, OpenApiResponse> list in operation.get_Responses().ToList<KeyValuePair<string, OpenApiResponse>>())
			{
				list.Deconstruct(out str, out openApiResponse);
				string str1 = str;
				OpenApiResponse openApiResponse1 = openApiResponse;
				if (!string.IsNullOrWhiteSpace(openApiResponse1.get_Description()))
				{
					string description = openApiResponse1.get_Description();
					if (description != null)
					{
						flag = description.Contains("=&gt;", StringComparison.Ordinal);
					}
					else
					{
						flag = false;
					}
					if (!flag)
					{
						string description1 = openApiResponse1.get_Description();
						if (description1 != null)
						{
							flag1 = description1.Contains("=>", StringComparison.Ordinal);
						}
						else
						{
							flag1 = false;
						}
						if (!flag1)
						{
							continue;
						}
					}
				}
				operation.get_Responses().Remove(str1);
			}
		}

		private static JsonSchema GetErrorSchema(DocumentProcessorContext context)
		{
			ContextualType contextualType = ContextualTypeExtensions.ToContextualType(typeof(ErrorDto));
			return context.get_SchemaGenerator().GenerateWithReference<JsonSchema>(contextualType, context.get_SchemaResolver(), null);
		}

		public void Process(DocumentProcessorContext context)
		{
			JsonSchema errorSchema = ErrorDtoProcessor.GetErrorSchema(context);
			foreach (OpenApiOperation openApiOperation in context.get_Document().get_Paths().Values.SelectMany<OpenApiPathItem, OpenApiOperation>((OpenApiPathItem x) => x.get_Values()))
			{
				ErrorDtoProcessor.AddErrorResponses(openApiOperation, errorSchema);
				ErrorDtoProcessor.CleanupResponses(openApiOperation);
			}
		}
	}
}