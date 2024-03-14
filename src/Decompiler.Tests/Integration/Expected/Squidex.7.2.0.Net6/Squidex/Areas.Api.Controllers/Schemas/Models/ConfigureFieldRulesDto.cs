using Squidex.Domain.Apps.Entities.Schemas.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	public sealed class ConfigureFieldRulesDto
	{
		[Nullable(new byte[] { 2, 1 })]
		public FieldRuleDto[] FieldRules
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public ConfigureFieldRulesDto()
		{
		}

		[NullableContext(1)]
		public ConfigureFieldRules ToCommand()
		{
			FieldRuleCommand[] array;
			ConfigureFieldRules configureFieldRule = new ConfigureFieldRules();
			FieldRuleDto[] fieldRules = this.FieldRules;
			if (fieldRules != null)
			{
				array = (
					from x in (IEnumerable<FieldRuleDto>)fieldRules
					select x.ToCommand()).ToArray<FieldRuleCommand>();
			}
			else
			{
				array = null;
			}
			configureFieldRule.set_FieldRules(array);
			return configureFieldRule;
		}
	}
}