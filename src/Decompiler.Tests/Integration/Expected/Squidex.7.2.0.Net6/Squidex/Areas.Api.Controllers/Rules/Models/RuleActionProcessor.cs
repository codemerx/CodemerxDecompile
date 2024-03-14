using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Squidex.Domain.Apps.Core.HandleRules;
using Squidex.Domain.Apps.Core.Rules;
using Squidex.Infrastructure.Json.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Rules.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class RuleActionProcessor : IDocumentProcessor
	{
		private readonly RuleTypeProvider ruleRegistry;

		public RuleActionProcessor(RuleTypeProvider ruleRegistry)
		{
			this.ruleRegistry = ruleRegistry;
		}

		public void Process(DocumentProcessorContext context)
		{
			string str;
			RuleActionDefinition ruleActionDefinition;
			try
			{
				JsonSchema schema = context.get_SchemaResolver().GetSchema(typeof(RuleAction), false);
				if (schema != null)
				{
					RuleActionConverter ruleActionConverter = new RuleActionConverter();
					OpenApiDiscriminator openApiDiscriminator = new OpenApiDiscriminator();
					openApiDiscriminator.set_PropertyName(ruleActionConverter.get_DiscriminatorName());
					openApiDiscriminator.set_JsonInheritanceConverter(ruleActionConverter);
					schema.set_DiscriminatorObject(openApiDiscriminator);
					IDictionary<string, JsonSchemaProperty> properties = schema.get_Properties();
					string discriminatorName = ruleActionConverter.get_DiscriminatorName();
					JsonSchemaProperty jsonSchemaProperty = new JsonSchemaProperty();
					jsonSchemaProperty.set_Type(64);
					jsonSchemaProperty.set_IsRequired(true);
					jsonSchemaProperty.set_IsNullableRaw(new bool?(true));
					properties[discriminatorName] = jsonSchemaProperty;
					foreach (KeyValuePair<string, RuleActionDefinition> action in this.ruleRegistry.get_Actions())
					{
						action.Deconstruct(out str, out ruleActionDefinition);
						string str1 = str;
						RuleActionDefinition ruleActionDefinition1 = ruleActionDefinition;
						JsonSchema jsonSchema = context.get_SchemaGenerator().Generate<JsonSchema>(ContextualTypeExtensions.ToContextualType(ruleActionDefinition1.get_Type()), context.get_SchemaResolver());
						KeyValuePair<string, JsonSchema> keyValuePair = context.get_Document().get_Definitions().FirstOrDefault<KeyValuePair<string, JsonSchema>>((KeyValuePair<string, JsonSchema> x) => (object)x.Value == (object)jsonSchema);
						string key = keyValuePair.Key;
						if (key == null)
						{
							continue;
						}
						context.get_Document().get_Definitions().Remove(key);
						context.get_Document().get_Definitions().Add(string.Concat(str1, "RuleActionDto"), jsonSchema);
					}
					RuleActionProcessor.RemoveFreezable(context, schema);
				}
			}
			catch (KeyNotFoundException keyNotFoundException)
			{
			}
		}

		private static void RemoveFreezable(DocumentProcessorContext context, JsonSchema schema)
		{
			context.get_Document().get_Definitions().Remove("Freezable");
			schema.get_AllOf().Clear();
		}
	}
}