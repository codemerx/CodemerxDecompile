using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class UpsertSchemaFieldDto
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

		[Nullable(new byte[] { 2, 1 })]
		public UpsertSchemaNestedFieldDto[] Nested
		{
			[return: Nullable(new byte[] { 2, 1 })]
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

		public UpsertSchemaFieldDto()
		{
		}
	}
}