using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Validation;
using Squidex.Web.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[JsonInheritanceConverter(typeof(FieldPropertiesDto), "fieldType")]
	[KnownType("Subtypes")]
	[Nullable(0)]
	[NullableContext(2)]
	public abstract class FieldPropertiesDto
	{
		public string EditorUrl
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

		public bool IsHalfWidth
		{
			get;
			set;
		}

		public bool IsRequired
		{
			get;
			set;
		}

		public bool IsRequiredOnPublish
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

		[LocalizedStringLength(100)]
		public string Placeholder
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

		protected FieldPropertiesDto()
		{
		}

		[NullableContext(1)]
		public static Type[] Subtypes()
		{
			Type type = typeof(FieldPropertiesDto);
			Type type1 = type;
			return type.Assembly.GetTypes().Where<Type>(new Func<Type, bool>(type1.IsAssignableFrom)).ToArray<Type>();
		}

		[NullableContext(1)]
		public abstract FieldProperties ToProperties();
	}
}