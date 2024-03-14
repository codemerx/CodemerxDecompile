using Squidex.Domain.Apps.Core.Rules.Triggers;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Rules.Models.Triggers
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class ContentChangedRuleTriggerSchemaDto
	{
		public string Condition
		{
			get;
			set;
		}

		public DomainId SchemaId
		{
			get;
			set;
		}

		public ContentChangedRuleTriggerSchemaDto()
		{
		}

		[NullableContext(1)]
		public static ContentChangedRuleTriggerSchemaDto FromDomain(ContentChangedTriggerSchemaV2 trigger)
		{
			return SimpleMapper.Map<ContentChangedTriggerSchemaV2, ContentChangedRuleTriggerSchemaDto>(trigger, new ContentChangedRuleTriggerSchemaDto());
		}

		[NullableContext(1)]
		public ContentChangedTriggerSchemaV2 ToTrigger()
		{
			return SimpleMapper.Map<ContentChangedRuleTriggerSchemaDto, ContentChangedTriggerSchemaV2>(this, new ContentChangedTriggerSchemaV2());
		}
	}
}