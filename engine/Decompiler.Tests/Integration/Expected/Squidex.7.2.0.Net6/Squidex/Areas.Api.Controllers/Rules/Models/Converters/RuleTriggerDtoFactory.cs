using Squidex.Areas.Api.Controllers.Rules.Models;
using Squidex.Areas.Api.Controllers.Rules.Models.Triggers;
using Squidex.Domain.Apps.Core.Rules;
using Squidex.Domain.Apps.Core.Rules.Triggers;
using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Reflection;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Rules.Models.Converters
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class RuleTriggerDtoFactory : IRuleTriggerVisitor<RuleTriggerDto>
	{
		private readonly static RuleTriggerDtoFactory Instance;

		static RuleTriggerDtoFactory()
		{
			RuleTriggerDtoFactory.Instance = new RuleTriggerDtoFactory();
		}

		private RuleTriggerDtoFactory()
		{
		}

		public static RuleTriggerDto Create(RuleTrigger properties)
		{
			return properties.Accept<RuleTriggerDto>(RuleTriggerDtoFactory.Instance);
		}

		public RuleTriggerDto Visit(AssetChangedTriggerV2 trigger)
		{
			return SimpleMapper.Map<AssetChangedTriggerV2, AssetChangedRuleTriggerDto>(trigger, new AssetChangedRuleTriggerDto());
		}

		public RuleTriggerDto Visit(CommentTrigger trigger)
		{
			return SimpleMapper.Map<CommentTrigger, CommentRuleTriggerDto>(trigger, new CommentRuleTriggerDto());
		}

		public RuleTriggerDto Visit(ManualTrigger trigger)
		{
			return SimpleMapper.Map<ManualTrigger, ManualRuleTriggerDto>(trigger, new ManualRuleTriggerDto());
		}

		public RuleTriggerDto Visit(SchemaChangedTrigger trigger)
		{
			return SimpleMapper.Map<SchemaChangedTrigger, SchemaChangedRuleTriggerDto>(trigger, new SchemaChangedRuleTriggerDto());
		}

		public RuleTriggerDto Visit(UsageTrigger trigger)
		{
			return SimpleMapper.Map<UsageTrigger, UsageRuleTriggerDto>(trigger, new UsageRuleTriggerDto());
		}

		public RuleTriggerDto Visit(ContentChangedTriggerV2 trigger)
		{
			ContentChangedRuleTriggerSchemaDto[] array;
			ReadonlyList<ContentChangedTriggerSchemaV2> schemas = trigger.get_Schemas();
			if (schemas != null)
			{
				array = schemas.Select<ContentChangedTriggerSchemaV2, ContentChangedRuleTriggerSchemaDto>(new Func<ContentChangedTriggerSchemaV2, ContentChangedRuleTriggerSchemaDto>(ContentChangedRuleTriggerSchemaDto.FromDomain)).ToArray<ContentChangedRuleTriggerSchemaDto>();
			}
			else
			{
				array = null;
			}
			ContentChangedRuleTriggerSchemaDto[] contentChangedRuleTriggerSchemaDtoArray = array;
			return new ContentChangedRuleTriggerDto()
			{
				Schemas = contentChangedRuleTriggerSchemaDtoArray,
				HandleAll = trigger.get_HandleAll()
			};
		}
	}
}