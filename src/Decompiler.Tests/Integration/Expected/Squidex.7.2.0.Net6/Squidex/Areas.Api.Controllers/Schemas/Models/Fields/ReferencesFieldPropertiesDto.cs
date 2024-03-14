using Squidex.Areas.Api.Controllers.Schemas.Models;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models.Fields
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class ReferencesFieldPropertiesDto : FieldPropertiesDto
	{
		public bool AllowDuplicates
		{
			get;
			set;
		}

		[Nullable(new byte[] { 2, 1 })]
		public ReadonlyList<string> DefaultValue
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		[Nullable(new byte[] { 2, 2, 1 })]
		public LocalizedValue<ReadonlyList<string>> DefaultValues
		{
			[return: Nullable(new byte[] { 2, 2, 1 })]
			get;
			set;
		}

		public ReferencesFieldEditor Editor
		{
			get;
			set;
		}

		public int? MaxItems
		{
			get;
			set;
		}

		public int? MinItems
		{
			get;
			set;
		}

		public bool MustBePublished
		{
			get;
			set;
		}

		public bool ResolveReference
		{
			get;
			set;
		}

		public ReadonlyList<DomainId> SchemaIds
		{
			get;
			set;
		}

		public ReferencesFieldPropertiesDto()
		{
		}

		[NullableContext(1)]
		public override FieldProperties ToProperties()
		{
			return SimpleMapper.Map<ReferencesFieldPropertiesDto, ReferencesFieldProperties>(this, new ReferencesFieldProperties());
		}
	}
}