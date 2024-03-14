using Squidex.Areas.Api.Controllers.Schemas.Models;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models.Fields
{
	public sealed class ArrayFieldPropertiesDto : FieldPropertiesDto
	{
		public int? MaxItems
		{
			get;
			set;
		}

		public int? MinItems
		{
			get;
			set;
		}

		[Nullable(new byte[] { 2, 1 })]
		public ReadonlyList<string> UniqueFields
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public ArrayFieldPropertiesDto()
		{
		}

		[NullableContext(1)]
		public override FieldProperties ToProperties()
		{
			return SimpleMapper.Map<ArrayFieldPropertiesDto, ArrayFieldProperties>(this, new ArrayFieldProperties());
		}
	}
}