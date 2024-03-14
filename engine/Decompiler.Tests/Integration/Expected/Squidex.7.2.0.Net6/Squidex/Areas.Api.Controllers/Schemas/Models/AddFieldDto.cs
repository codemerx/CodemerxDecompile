using Squidex.Domain.Apps.Entities.Schemas.Commands;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AddFieldDto
	{
		[LocalizedRegularExpression("^[a-zA-Z0-9]+(\\-[a-zA-Z0-9]+)*$")]
		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		[Nullable(2)]
		public string Partitioning
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		[LocalizedRequired]
		public FieldPropertiesDto Properties
		{
			get;
			set;
		}

		public AddFieldDto()
		{
		}

		public AddField ToCommand(long? parentId = null)
		{
			AddField addField = new AddField();
			addField.set_ParentFieldId(parentId);
			addField.set_Properties(this.Properties.ToProperties());
			return SimpleMapper.Map<AddFieldDto, AddField>(this, addField);
		}
	}
}