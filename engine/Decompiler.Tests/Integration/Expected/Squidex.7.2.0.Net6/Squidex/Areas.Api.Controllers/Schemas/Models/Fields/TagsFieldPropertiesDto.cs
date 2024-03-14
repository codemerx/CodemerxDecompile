using Squidex.Areas.Api.Controllers.Schemas.Models;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models.Fields
{
	public sealed class TagsFieldPropertiesDto : FieldPropertiesDto
	{
		[Nullable(new byte[] { 2, 1 })]
		public ReadonlyList<string> AllowedValues
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public bool CreateEnum
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

		public TagsFieldEditor Editor
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

		public TagsFieldPropertiesDto()
		{
		}

		[NullableContext(1)]
		public override FieldProperties ToProperties()
		{
			return SimpleMapper.Map<TagsFieldPropertiesDto, TagsFieldProperties>(this, new TagsFieldProperties());
		}
	}
}