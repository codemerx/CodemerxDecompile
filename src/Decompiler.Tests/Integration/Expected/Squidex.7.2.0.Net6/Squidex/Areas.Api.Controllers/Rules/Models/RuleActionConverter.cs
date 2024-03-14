using Squidex.Domain.Apps.Core.Rules;
using Squidex.Web.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Rules.Models
{
	[Nullable(new byte[] { 0, 1 })]
	[NullableContext(1)]
	public sealed class RuleActionConverter : JsonInheritanceConverter<RuleAction>
	{
		public static IReadOnlyDictionary<string, Type> Mapping
		{
			get;
			set;
		}

		public RuleActionConverter() : base("actionType", RuleActionConverter.Mapping)
		{
		}
	}
}