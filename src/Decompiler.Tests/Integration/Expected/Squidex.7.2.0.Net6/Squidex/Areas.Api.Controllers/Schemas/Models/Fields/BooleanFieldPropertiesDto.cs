using Squidex.Areas.Api.Controllers.Schemas.Models;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models.Fields
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class BooleanFieldPropertiesDto : FieldPropertiesDto
	{
		public bool? DefaultValue
		{
			get;
			set;
		}

		public LocalizedValue<bool?> DefaultValues
		{
			get;
			set;
		}

		public BooleanFieldEditor Editor
		{
			get;
			set;
		}

		public bool InlineEditable
		{
			get;
			set;
		}

		public BooleanFieldPropertiesDto()
		{
		}

		[NullableContext(1)]
		public override FieldProperties ToProperties()
		{
			return SimpleMapper.Map<BooleanFieldPropertiesDto, BooleanFieldProperties>(this, new BooleanFieldProperties());
		}
	}
}