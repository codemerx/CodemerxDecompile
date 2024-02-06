using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Entities.Schemas.Commands;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class FieldRuleDto
	{
		[LocalizedRequired]
		public FieldRuleAction Action
		{
			get;
			set;
		}

		[Nullable(2)]
		public string Condition
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		[LocalizedRequired]
		public string Field
		{
			get;
			set;
		}

		public FieldRuleDto()
		{
		}

		public static FieldRuleDto FromDomain(FieldRule fieldRule)
		{
			return SimpleMapper.Map<FieldRule, FieldRuleDto>(fieldRule, new FieldRuleDto());
		}

		public FieldRuleCommand ToCommand()
		{
			return SimpleMapper.Map<FieldRuleDto, FieldRuleCommand>(this, new FieldRuleCommand());
		}
	}
}