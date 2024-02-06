using NodaTime;
using Squidex.Areas.Api.Controllers.Contents;
using Squidex.Areas.Api.Controllers.Schemas;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Entities.Schemas;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public class SchemaDto : Resource
	{
		[Nullable(2)]
		public string Category
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		public Instant Created
		{
			get;
			set;
		}

		[LocalizedRequired]
		public RefToken CreatedBy
		{
			get;
			set;
		}

		public List<FieldRuleDto> FieldRules { get; set; } = new List<FieldRuleDto>();

		[LocalizedRequired]
		public List<FieldDto> Fields { get; set; } = new List<FieldDto>();

		[LocalizedRequired]
		public FieldNames FieldsInLists
		{
			get;
			set;
		}

		[LocalizedRequired]
		public FieldNames FieldsInReferences
		{
			get;
			set;
		}

		public DomainId Id
		{
			get;
			set;
		}

		public bool IsPublished
		{
			get;
			set;
		}

		[Obsolete("Use 'type' field now.")]
		public bool IsSingleton
		{
			get
			{
				return this.Type == 1;
			}
		}

		public Instant LastModified
		{
			get;
			set;
		}

		[LocalizedRequired]
		public RefToken LastModifiedBy
		{
			get;
			set;
		}

		[LocalizedRegularExpression("^[a-z0-9]+(\\-[a-z0-9]+)*$")]
		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		[LocalizedRequired]
		public ReadonlyDictionary<string, string> PreviewUrls
		{
			get;
			set;
		}

		[LocalizedRequired]
		public SchemaPropertiesDto Properties { get; set; } = new SchemaPropertiesDto();

		[LocalizedRequired]
		public SchemaScriptsDto Scripts { get; set; } = new SchemaScriptsDto();

		public SchemaType Type
		{
			get;
			set;
		}

		public long Version
		{
			get;
			set;
		}

		public SchemaDto()
		{
		}

		protected virtual void CreateLinks(Resources resources)
		{
			var variable = new { app = resources.get_App(), schema = this.Name };
			bool flag = resources.CanUpdateSchema(this.Name);
			base.AddSelfLink(resources.Url<SchemasController>((SchemasController x) => "GetSchema", variable));
			if (resources.CanReadContent(this.Name) && this.Type != 2)
			{
				base.AddGetLink("contents", resources.Url<ContentsController>((ContentsController x) => "GetContents", variable), null);
			}
			if (resources.CanCreateContent(this.Name) && this.Type == null)
			{
				base.AddPostLink("contents/create", resources.Url<ContentsController>((ContentsController x) => "PostContent", variable), null);
				base.AddPostLink("contents/create/publish", string.Concat(resources.Url<ContentsController>((ContentsController x) => "PostContent", variable), "?publish=true"), null);
			}
			if (resources.CanPublishSchema(this.Name))
			{
				if (!this.IsPublished)
				{
					base.AddPutLink("publish", resources.Url<SchemasController>((SchemasController x) => "PublishSchema", variable), null);
				}
				else
				{
					base.AddPutLink("unpublish", resources.Url<SchemasController>((SchemasController x) => "UnpublishSchema", variable), null);
				}
			}
			if (flag)
			{
				base.AddPostLink("fields/add", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "PostField", variable), null);
				base.AddPutLink("fields/order", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "PutSchemaFieldOrdering", variable), null);
				base.AddPutLink("fields/ui", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "PutSchemaUIFields", variable), null);
				base.AddPutLink("update", resources.Url<SchemasController>((SchemasController x) => "PutSchema", variable), null);
				base.AddPutLink("update/category", resources.Url<SchemasController>((SchemasController x) => "PutCategory", variable), null);
				base.AddPutLink("update/rules", resources.Url<SchemasController>((SchemasController x) => "PutRules", variable), null);
				base.AddPutLink("update/sync", resources.Url<SchemasController>((SchemasController x) => "PutSchemaSync", variable), null);
				base.AddPutLink("update/urls", resources.Url<SchemasController>((SchemasController x) => "PutPreviewUrls", variable), null);
			}
			if (resources.CanUpdateSchemaScripts(this.Name))
			{
				base.AddPutLink("update/scripts", resources.Url<SchemasController>((SchemasController x) => "PutScripts", variable), null);
			}
			if (resources.CanDeleteSchema(this.Name))
			{
				base.AddDeleteLink("delete", resources.Url<SchemasController>((SchemasController x) => "DeleteSchema", variable), null);
			}
			if (this.Fields != null)
			{
				foreach (FieldDto field in this.Fields)
				{
					field.CreateLinks(resources, this.Name, flag);
				}
			}
		}

		public static SchemaDto FromDomain(ISchemaEntity schema, Resources resources)
		{
			SchemaDto schemaDto = new SchemaDto();
			SimpleMapper.Map<ISchemaEntity, SchemaDto>(schema, schemaDto);
			SimpleMapper.Map<Schema, SchemaDto>(schema.get_SchemaDef(), schemaDto);
			SimpleMapper.Map<SchemaScripts, SchemaScriptsDto>(schema.get_SchemaDef().get_Scripts(), schemaDto.Scripts);
			SimpleMapper.Map<SchemaProperties, SchemaPropertiesDto>(schema.get_SchemaDef().get_Properties(), schemaDto.Properties);
			foreach (FieldRule fieldRule in schema.get_SchemaDef().get_FieldRules())
			{
				schemaDto.FieldRules.Add(FieldRuleDto.FromDomain(fieldRule));
			}
			foreach (RootField field in schema.get_SchemaDef().get_Fields())
			{
				schemaDto.Fields.Add(FieldDto.FromDomain(field));
			}
			schemaDto.CreateLinks(resources);
			return schemaDto;
		}
	}
}