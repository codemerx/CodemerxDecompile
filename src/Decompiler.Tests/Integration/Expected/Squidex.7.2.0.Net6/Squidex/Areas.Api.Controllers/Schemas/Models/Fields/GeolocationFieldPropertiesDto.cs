using Squidex.Areas.Api.Controllers.Schemas.Models;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models.Fields
{
	public sealed class GeolocationFieldPropertiesDto : FieldPropertiesDto
	{
		public GeolocationFieldEditor Editor
		{
			get;
			set;
		}

		public GeolocationFieldPropertiesDto()
		{
		}

		[NullableContext(1)]
		public override FieldProperties ToProperties()
		{
			return SimpleMapper.Map<GeolocationFieldPropertiesDto, GeolocationFieldProperties>(this, new GeolocationFieldProperties());
		}
	}
}