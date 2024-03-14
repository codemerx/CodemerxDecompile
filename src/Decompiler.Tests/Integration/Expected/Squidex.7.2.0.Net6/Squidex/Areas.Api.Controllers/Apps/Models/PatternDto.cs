using Squidex.Domain.Apps.Core.Apps;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class PatternDto
	{
		[Nullable(2)]
		public string Message
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string Regex
		{
			get;
			set;
		}

		public PatternDto()
		{
		}

		public static PatternDto FromPattern(Pattern pattern)
		{
			return SimpleMapper.Map<Pattern, PatternDto>(pattern, new PatternDto());
		}

		public Pattern ToPattern()
		{
			Pattern pattern = new Pattern(this.Name, this.Regex);
			pattern.set_Message(this.Message);
			return pattern;
		}
	}
}