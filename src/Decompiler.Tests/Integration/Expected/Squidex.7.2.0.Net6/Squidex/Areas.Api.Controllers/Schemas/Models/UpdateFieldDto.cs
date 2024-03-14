using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Entities.Schemas.Commands;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class UpdateFieldDto
	{
		[LocalizedRequired]
		public FieldPropertiesDto Properties
		{
			get;
			set;
		}

		public UpdateFieldDto()
		{
		}

		public UpdateField ToCommand(long id, long? parentId = null)
		{
			FieldProperties properties;
			UpdateField updateField = new UpdateField();
			updateField.set_ParentFieldId(parentId);
			updateField.set_FieldId(id);
			FieldPropertiesDto fieldPropertiesDto = this.Properties;
			if (fieldPropertiesDto != null)
			{
				properties = fieldPropertiesDto.ToProperties();
			}
			else
			{
				properties = null;
			}
			updateField.set_Properties(properties);
			return updateField;
		}
	}
}