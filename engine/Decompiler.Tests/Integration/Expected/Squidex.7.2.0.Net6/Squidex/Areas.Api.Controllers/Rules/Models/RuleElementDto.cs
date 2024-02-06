using Squidex.Domain.Apps.Core.HandleRules;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Rules.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class RuleElementDto
	{
		[LocalizedRequired]
		[Nullable(1)]
		public string Description
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		[LocalizedRequired]
		[Nullable(1)]
		public string Display
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		public string IconColor
		{
			get;
			set;
		}

		public string IconImage
		{
			get;
			set;
		}

		[LocalizedRequired]
		[Nullable(1)]
		public RuleElementPropertyDto[] Properties
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		public string ReadMore
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public RuleElementDto()
		{
		}

		[NullableContext(1)]
		public static RuleElementDto FromDomain(RuleActionDefinition definition)
		{
			RuleElementDto array = SimpleMapper.Map<RuleActionDefinition, RuleElementDto>(definition, new RuleElementDto());
			array.Properties = (
				from x in definition.get_Properties()
				select SimpleMapper.Map<RuleActionProperty, RuleElementPropertyDto>(x, new RuleElementPropertyDto())).ToArray<RuleElementPropertyDto>();
			return array;
		}
	}
}