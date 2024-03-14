using Squidex.Domain.Apps.Core.HandleRules;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Rules.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class RuleElementPropertyDto
	{
		[Nullable(2)]
		public string Description
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		[LocalizedRequired]
		public string Display
		{
			get;
			set;
		}

		[LocalizedRequired]
		public RuleFieldEditor Editor
		{
			get;
			set;
		}

		public bool IsFormattable
		{
			get;
			set;
		}

		public bool IsRequired
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		[Nullable(new byte[] { 2, 1 })]
		public string[] Options
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public RuleElementPropertyDto()
		{
		}
	}
}