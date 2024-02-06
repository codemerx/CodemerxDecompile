using Squidex.Areas.Api.Controllers.Schemas.Models;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models.Fields
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class ComponentFieldPropertiesDto : FieldPropertiesDto
	{
		public ReadonlyList<DomainId> SchemaIds
		{
			get;
			set;
		}

		public ComponentFieldPropertiesDto()
		{
		}

		[NullableContext(1)]
		public override FieldProperties ToProperties()
		{
			return SimpleMapper.Map<ComponentFieldPropertiesDto, ComponentFieldProperties>(this, new ComponentFieldProperties());
		}
	}
}