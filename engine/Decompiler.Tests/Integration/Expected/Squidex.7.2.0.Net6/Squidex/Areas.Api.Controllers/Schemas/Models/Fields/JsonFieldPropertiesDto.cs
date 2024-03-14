using Squidex.Areas.Api.Controllers.Schemas.Models;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models.Fields
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class JsonFieldPropertiesDto : FieldPropertiesDto
	{
		public string GraphQLSchema
		{
			get;
			set;
		}

		public JsonFieldPropertiesDto()
		{
		}

		[NullableContext(1)]
		public override FieldProperties ToProperties()
		{
			return SimpleMapper.Map<JsonFieldPropertiesDto, JsonFieldProperties>(this, new JsonFieldProperties());
		}
	}
}