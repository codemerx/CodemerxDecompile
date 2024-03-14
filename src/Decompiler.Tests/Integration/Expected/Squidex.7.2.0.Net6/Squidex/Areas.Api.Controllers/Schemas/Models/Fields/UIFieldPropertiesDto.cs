using Squidex.Areas.Api.Controllers.Schemas.Models;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models.Fields
{
	public sealed class UIFieldPropertiesDto : FieldPropertiesDto
	{
		public UIFieldEditor Editor
		{
			get;
			set;
		}

		public UIFieldPropertiesDto()
		{
		}

		[NullableContext(1)]
		public override FieldProperties ToProperties()
		{
			return SimpleMapper.Map<UIFieldPropertiesDto, UIFieldProperties>(this, new UIFieldProperties());
		}
	}
}