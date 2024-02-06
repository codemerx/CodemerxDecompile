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
	public sealed class StringFieldPropertiesDto : FieldPropertiesDto
	{
		[Nullable(new byte[] { 2, 1 })]
		public ReadonlyList<string> AllowedValues
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public StringContentType ContentType
		{
			get;
			set;
		}

		public bool CreateEnum
		{
			get;
			set;
		}

		public string DefaultValue
		{
			get;
			set;
		}

		public LocalizedValue<string> DefaultValues
		{
			get;
			set;
		}

		public StringFieldEditor Editor
		{
			get;
			set;
		}

		public string FolderId
		{
			get;
			set;
		}

		public bool InlineEditable
		{
			get;
			set;
		}

		public bool IsEmbeddable
		{
			get;
			set;
		}

		public bool IsUnique
		{
			get;
			set;
		}

		public int? MaxCharacters
		{
			get;
			set;
		}

		public int? MaxLength
		{
			get;
			set;
		}

		public int? MaxWords
		{
			get;
			set;
		}

		public int? MinCharacters
		{
			get;
			set;
		}

		public int? MinLength
		{
			get;
			set;
		}

		public int? MinWords
		{
			get;
			set;
		}

		public string Pattern
		{
			get;
			set;
		}

		public string PatternMessage
		{
			get;
			set;
		}

		public ReadonlyList<DomainId> SchemaIds
		{
			get;
			set;
		}

		public StringFieldPropertiesDto()
		{
		}

		[NullableContext(1)]
		public override FieldProperties ToProperties()
		{
			return SimpleMapper.Map<StringFieldPropertiesDto, StringFieldProperties>(this, new StringFieldProperties());
		}
	}
}