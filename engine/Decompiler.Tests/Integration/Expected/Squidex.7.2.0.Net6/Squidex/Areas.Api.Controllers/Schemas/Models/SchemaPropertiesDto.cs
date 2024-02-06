using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class SchemaPropertiesDto
	{
		public string ContentEditorUrl
		{
			get;
			set;
		}

		public string ContentSidebarUrl
		{
			get;
			set;
		}

		public string ContentsSidebarUrl
		{
			get;
			set;
		}

		[LocalizedStringLength(0x3e8)]
		public string Hints
		{
			get;
			set;
		}

		[LocalizedStringLength(100)]
		public string Label
		{
			get;
			set;
		}

		[Nullable(new byte[] { 2, 1 })]
		public ReadonlyList<string> Tags
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public bool ValidateOnPublish
		{
			get;
			set;
		}

		public SchemaPropertiesDto()
		{
		}
	}
}