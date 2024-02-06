using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Entities.Schemas.Commands;
using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Reflection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public abstract class UpsertSchemaDto
	{
		public string Category
		{
			get;
			set;
		}

		[Nullable(new byte[] { 2, 1 })]
		public List<FieldRuleDto> FieldRules
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		[Nullable(new byte[] { 2, 1 })]
		public UpsertSchemaFieldDto[] Fields
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public FieldNames FieldsInLists
		{
			get;
			set;
		}

		public FieldNames FieldsInReferences
		{
			get;
			set;
		}

		public bool IsPublished
		{
			get;
			set;
		}

		[Nullable(new byte[] { 2, 1, 1 })]
		public ReadonlyDictionary<string, string> PreviewUrls
		{
			[return: Nullable(new byte[] { 2, 1, 1 })]
			get;
			set;
		}

		public SchemaPropertiesDto Properties
		{
			get;
			set;
		}

		public SchemaScriptsDto Scripts
		{
			get;
			set;
		}

		protected UpsertSchemaDto()
		{
		}

		[NullableContext(1)]
		public static T ToCommand<T, TSoure>(TSoure dto, T command)
		where T : SchemaCommandBase, IUpsertCommand
		where TSoure : UpsertSchemaDto
		{
			bool length;
			bool count;
			FieldProperties properties;
			int? nullable;
			FieldProperties fieldProperty;
			SimpleMapper.Map<TSoure, T>(dto, command);
			if (dto.Properties != null)
			{
				command.set_Properties(new SchemaProperties());
				SimpleMapper.Map<SchemaPropertiesDto, SchemaProperties>(dto.Properties, command.get_Properties());
			}
			if (dto.Scripts != null)
			{
				command.set_Scripts(new SchemaScripts());
				SimpleMapper.Map<SchemaScriptsDto, SchemaScripts>(dto.Scripts, command.get_Scripts());
			}
			UpsertSchemaFieldDto[] fields = dto.Fields;
			if (fields != null)
			{
				length = fields.Length != 0;
			}
			else
			{
				length = false;
			}
			if (length)
			{
				List<UpsertSchemaField> upsertSchemaFields = new List<UpsertSchemaField>();
				UpsertSchemaFieldDto[] upsertSchemaFieldDtoArray = dto.Fields;
				for (int i = 0; i < (int)upsertSchemaFieldDtoArray.Length; i++)
				{
					UpsertSchemaFieldDto upsertSchemaFieldDto = upsertSchemaFieldDtoArray[i];
					if (upsertSchemaFieldDto != null)
					{
						FieldPropertiesDto fieldPropertiesDto = upsertSchemaFieldDto.Properties;
						if (fieldPropertiesDto != null)
						{
							properties = fieldPropertiesDto.ToProperties();
						}
						else
						{
							properties = null;
						}
					}
					else
					{
						properties = null;
					}
					FieldProperties fieldProperty1 = properties;
					UpsertSchemaField upsertSchemaField = new UpsertSchemaField();
					upsertSchemaField.set_Properties(fieldProperty1);
					UpsertSchemaField upsertSchemaField1 = upsertSchemaField;
					if (upsertSchemaFieldDto != null)
					{
						SimpleMapper.Map<UpsertSchemaFieldDto, UpsertSchemaField>(upsertSchemaFieldDto, upsertSchemaField1);
						if (upsertSchemaFieldDto != null)
						{
							UpsertSchemaNestedFieldDto[] nested = upsertSchemaFieldDto.Nested;
							if (nested != null)
							{
								nullable = new int?((int)nested.Length);
							}
							else
							{
								nullable = null;
							}
							int? nullable1 = nullable;
							if (nullable1.GetValueOrDefault() > 0 & nullable1.HasValue)
							{
								List<UpsertSchemaNestedField> upsertSchemaNestedFields = new List<UpsertSchemaNestedField>();
								UpsertSchemaNestedFieldDto[] upsertSchemaNestedFieldDtoArray = upsertSchemaFieldDto.Nested;
								for (int j = 0; j < (int)upsertSchemaNestedFieldDtoArray.Length; j++)
								{
									UpsertSchemaNestedFieldDto upsertSchemaNestedFieldDto = upsertSchemaNestedFieldDtoArray[j];
									if (upsertSchemaNestedFieldDto != null)
									{
										FieldPropertiesDto properties1 = upsertSchemaNestedFieldDto.Properties;
										if (properties1 != null)
										{
											fieldProperty = properties1.ToProperties();
										}
										else
										{
											fieldProperty = null;
										}
									}
									else
									{
										fieldProperty = null;
									}
									FieldProperties fieldProperty2 = fieldProperty;
									UpsertSchemaNestedField upsertSchemaNestedField = new UpsertSchemaNestedField();
									upsertSchemaNestedField.set_Properties(fieldProperty2);
									UpsertSchemaNestedField upsertSchemaNestedField1 = upsertSchemaNestedField;
									if (upsertSchemaNestedFieldDto != null)
									{
										SimpleMapper.Map<UpsertSchemaNestedFieldDto, UpsertSchemaNestedField>(upsertSchemaNestedFieldDto, upsertSchemaNestedField1);
									}
									upsertSchemaNestedFields.Add(upsertSchemaNestedField1);
								}
								upsertSchemaField1.set_Nested(upsertSchemaNestedFields.ToArray());
							}
						}
					}
					upsertSchemaFields.Add(upsertSchemaField1);
				}
				command.set_Fields(upsertSchemaFields.ToArray());
			}
			List<FieldRuleDto> fieldRules = dto.FieldRules;
			if (fieldRules != null)
			{
				count = fieldRules.Count > 0;
			}
			else
			{
				count = false;
			}
			if (count)
			{
				List<FieldRuleCommand> fieldRuleCommands = new List<FieldRuleCommand>();
				foreach (FieldRuleDto fieldRule in dto.FieldRules)
				{
					fieldRuleCommands.Add(fieldRule.ToCommand());
				}
				command.set_FieldRules(fieldRuleCommands.ToArray());
			}
			return command;
		}
	}
}