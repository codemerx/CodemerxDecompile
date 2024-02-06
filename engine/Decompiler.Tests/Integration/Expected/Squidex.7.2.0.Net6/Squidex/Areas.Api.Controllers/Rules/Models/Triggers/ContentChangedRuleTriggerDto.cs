using Squidex.Areas.Api.Controllers.Rules.Models;
using Squidex.Domain.Apps.Core.Rules;
using Squidex.Domain.Apps.Core.Rules.Triggers;
using Squidex.Infrastructure.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Rules.Models.Triggers
{
	public sealed class ContentChangedRuleTriggerDto : RuleTriggerDto
	{
		public bool HandleAll
		{
			get;
			set;
		}

		[Nullable(new byte[] { 2, 1 })]
		public ContentChangedRuleTriggerSchemaDto[] Schemas
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public ContentChangedRuleTriggerDto()
		{
		}

		[NullableContext(1)]
		public override RuleTrigger ToTrigger()
		{
			ReadonlyList<ContentChangedTriggerSchemaV2> readonlyList;
			ContentChangedRuleTriggerSchemaDto[] schemas = this.Schemas;
			if (schemas != null)
			{
				readonlyList = ReadonlyList.ToReadonlyList<ContentChangedTriggerSchemaV2>(
					from x in (IEnumerable<ContentChangedRuleTriggerSchemaDto>)schemas
					select x.ToTrigger());
			}
			else
			{
				readonlyList = null;
			}
			ReadonlyList<ContentChangedTriggerSchemaV2> readonlyList1 = readonlyList;
			ContentChangedTriggerV2 contentChangedTriggerV2 = new ContentChangedTriggerV2();
			contentChangedTriggerV2.set_HandleAll(this.HandleAll);
			contentChangedTriggerV2.set_Schemas(readonlyList1);
			return contentChangedTriggerV2;
		}
	}
}