using Squidex.Areas.Api.Controllers.Rules.Models;
using Squidex.Domain.Apps.Core.Rules;
using Squidex.Domain.Apps.Core.Rules.Triggers;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Rules.Models.Triggers
{
	[Nullable(0)]
	[NullableContext(2)]
	public class CommentRuleTriggerDto : RuleTriggerDto
	{
		public string Condition
		{
			get;
			set;
		}

		public CommentRuleTriggerDto()
		{
		}

		[NullableContext(1)]
		public override RuleTrigger ToTrigger()
		{
			return SimpleMapper.Map<CommentRuleTriggerDto, CommentTrigger>(this, new CommentTrigger());
		}
	}
}