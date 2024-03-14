using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class UpsertSchemaNestedFieldDto
	{
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

		[LocalizedRegularExpression("^[a-zA-Z0-9]+(\\-[a-zA-Z0-9]+)*$")]
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

		public UpsertSchemaNestedFieldDto()
		{
		}
	}
}