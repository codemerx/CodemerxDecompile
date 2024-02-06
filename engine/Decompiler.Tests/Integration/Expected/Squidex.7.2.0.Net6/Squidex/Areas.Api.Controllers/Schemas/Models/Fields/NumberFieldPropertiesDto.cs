using Squidex.Areas.Api.Controllers.Schemas.Models;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models.Fields
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class NumberFieldPropertiesDto : FieldPropertiesDto
	{
		public ReadonlyList<double> AllowedValues
		{
			get;
			set;
		}

		public double? DefaultValue
		{
			get;
			set;
		}

		public LocalizedValue<double?> DefaultValues
		{
			get;
			set;
		}

		public NumberFieldEditor Editor
		{
			get;
			set;
		}

		public bool InlineEditable
		{
			get;
			set;
		}

		public bool IsUnique
		{
			get;
			set;
		}

		public double? MaxValue
		{
			get;
			set;
		}

		public double? MinValue
		{
			get;
			set;
		}

		public NumberFieldPropertiesDto()
		{
		}

		[NullableContext(1)]
		public override FieldProperties ToProperties()
		{
			return SimpleMapper.Map<NumberFieldPropertiesDto, NumberFieldProperties>(this, new NumberFieldProperties());
		}
	}
}