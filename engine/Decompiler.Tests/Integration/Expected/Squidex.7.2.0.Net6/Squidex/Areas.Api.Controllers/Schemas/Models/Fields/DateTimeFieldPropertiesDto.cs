using NodaTime;
using Squidex.Areas.Api.Controllers.Schemas.Models;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models.Fields
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class DateTimeFieldPropertiesDto : FieldPropertiesDto
	{
		public DateTimeCalculatedDefaultValue? CalculatedDefaultValue
		{
			get;
			set;
		}

		public Instant? DefaultValue
		{
			get;
			set;
		}

		public LocalizedValue<Instant?> DefaultValues
		{
			get;
			set;
		}

		public DateTimeFieldEditor Editor
		{
			get;
			set;
		}

		public string Format
		{
			get;
			set;
		}

		public Instant? MaxValue
		{
			get;
			set;
		}

		public Instant? MinValue
		{
			get;
			set;
		}

		public DateTimeFieldPropertiesDto()
		{
		}

		[NullableContext(1)]
		public override FieldProperties ToProperties()
		{
			return SimpleMapper.Map<DateTimeFieldPropertiesDto, DateTimeFieldProperties>(this, new DateTimeFieldProperties());
		}
	}
}