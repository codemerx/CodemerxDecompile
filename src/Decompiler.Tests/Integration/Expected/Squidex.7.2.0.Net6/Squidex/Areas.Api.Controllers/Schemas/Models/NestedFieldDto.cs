using Squidex.Areas.Api.Controllers.Schemas;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class NestedFieldDto : Resource
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

		[LocalizedRequired]
		public FieldPropertiesDto Properties
		{
			get;
			set;
		}

		public NestedFieldDto()
		{
		}

		public void CreateLinks(Resources resources, string schema, long parentId, bool allowUpdate)
		{
			allowUpdate = (!allowUpdate ? false : !this.IsLocked);
			if (allowUpdate)
			{
				var variable = new { app = resources.get_App(), schema = schema, parentId = parentId, id = this.FieldId };
				base.AddPutLink("update", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "PutNestedField", variable), null);
				if (!this.IsHidden)
				{
					base.AddPutLink("hide", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "HideNestedField", variable), null);
				}
				else
				{
					base.AddPutLink("show", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "ShowNestedField", variable), null);
				}
				if (!this.IsDisabled)
				{
					base.AddPutLink("disable", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "DisableNestedField", variable), null);
				}
				else
				{
					base.AddPutLink("enable", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "EnableNestedField", variable), null);
				}
				if (!this.IsLocked)
				{
					base.AddPutLink("lock", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "LockNestedField", variable), null);
				}
				base.AddDeleteLink("delete", resources.Url<SchemaFieldsController>((SchemaFieldsController x) => "DeleteNestedField", variable), null);
			}
		}
	}
}