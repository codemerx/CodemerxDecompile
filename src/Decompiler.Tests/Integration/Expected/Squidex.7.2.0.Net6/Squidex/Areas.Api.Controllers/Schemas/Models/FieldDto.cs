using Squidex.Areas.Api.Controllers.Schemas;
using Squidex.Areas.Api.Controllers.Schemas.Models.Converters;
using Squidex.Domain.Apps.Core;
using Squidex.Domain.Apps.Core.Schemas;
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
	public sealed class FieldDto : Resource
	{
		public long FieldId
		{
			get;
			set;
		}

		public bool IsDisabled
		{
			get;
			set;
		}

		public bool IsHidden
		{
			get;
			set;
		}

		public bool IsLocked
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

		[Nullable(new byte[] { 2, 1 })]
		public List<NestedFieldDto> Nested
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		[LocalizedRequired]
		public string Partitioning
		{
			get;
			set;
		}

		[LocalizedRequired]
		public FieldPropertiesDto Properties
		{
			get;
			set;
		}

		public FieldDto()
		{
		}

		public void CreateLinks(Resources resources, string schema, bool allowUpdate)
		{
			allowUpdate = (!allowUpdate ? false : !this.IsLocked);
			if (allowUpdate)
			{
				var variable = new { app = resources.get_App(), schema = schema, id = this.FieldId };
				base.AddPutLink("update", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "PutField", variable), null);
				if (!this.IsHidden)
				{
					base.AddPutLink("hide", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "HideField", variable), null);
				}
				else
				{
					base.AddPutLink("show", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "ShowField", variable), null);
				}
				if (!this.IsDisabled)
				{
					base.AddPutLink("disable", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "DisableField", variable), null);
				}
				else
				{
					base.AddPutLink("enable", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "EnableField", variable), null);
				}
				if (this.Nested != null)
				{
					var variable1 = new { app = variable.app, schema = variable.schema, parentId = this.FieldId };
					base.AddPostLink("fields/add", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "PostNestedField", variable1), null);
					if (this.Nested.Count > 0)
					{
						base.AddPutLink("fields/order", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "PutNestedFieldOrdering", variable1), null);
					}
				}
				if (!this.IsLocked)
				{
					base.AddPutLink("lock", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "LockField", variable), null);
				}
				base.AddDeleteLink("delete", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "DeleteField", variable), null);
			}
			if (this.Nested != null)
			{
				foreach (NestedFieldDto nested in this.Nested)
				{
					nested.CreateLinks(resources, schema, this.FieldId, allowUpdate);
				}
			}
		}

		public static NestedFieldDto FromDomain(NestedField field)
		{
			FieldPropertiesDto fieldPropertiesDto = FieldPropertiesDtoFactory.Create(field.get_RawProperties());
			return SimpleMapper.Map<NestedField, NestedFieldDto>(field, new NestedFieldDto()
			{
				FieldId = field.get_Id(),
				Properties = fieldPropertiesDto
			});
		}

		public static FieldDto FromDomain(RootField field)
		{
			FieldPropertiesDto fieldPropertiesDto = FieldPropertiesDtoFactory.Create(field.get_RawProperties());
			FieldDto nestedFieldDtos = SimpleMapper.Map<RootField, FieldDto>(field, new FieldDto()
			{
				FieldId = field.get_Id(),
				Properties = fieldPropertiesDto,
				Partitioning = field.get_Partitioning().get_Key()
			});
			IArrayField arrayField = field as IArrayField;
			if (arrayField != null)
			{
				nestedFieldDtos.Nested = new List<NestedFieldDto>();
				foreach (NestedField nestedField in arrayField.get_Fields())
				{
					nestedFieldDtos.Nested.Add(FieldDto.FromDomain(nestedField));
				}
			}
			return nestedFieldDtos;
		}
	}
}