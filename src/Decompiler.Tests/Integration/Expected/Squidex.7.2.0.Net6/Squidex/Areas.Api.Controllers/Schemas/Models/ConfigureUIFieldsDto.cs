using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Entities.Schemas.Commands;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class ConfigureUIFieldsDto
	{
		public FieldNames FieldsInLists
		{
			get;
			set;
		}

		public FieldNames FieldsInReferences
		{
			get;
			set;
		}

		public ConfigureUIFieldsDto()
		{
		}

		[NullableContext(1)]
		public ConfigureUIFields ToCommand()
		{
			return SimpleMapper.Map<ConfigureUIFieldsDto, ConfigureUIFields>(this, new ConfigureUIFields());
		}
	}
}