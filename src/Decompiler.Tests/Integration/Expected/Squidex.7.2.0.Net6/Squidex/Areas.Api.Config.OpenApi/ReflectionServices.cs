using Namotion.Reflection;
using NJsonSchema.Generation;
using Squidex.Infrastructure.Collections;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Config.OpenApi
{
	[Nullable(0)]
	[NullableContext(1)]
	public class ReflectionServices : DefaultReflectionService
	{
		public ReflectionServices()
		{
		}

		protected override bool IsArrayType(ContextualType contextualType)
		{
			if (contextualType.get_Type().IsGenericType && contextualType.get_Type().GetGenericTypeDefinition() == typeof(ReadonlyList))
			{
				return true;
			}
			return base.IsArrayType(contextualType);
		}

		protected override bool IsDictionaryType(ContextualType contextualType)
		{
			if (contextualType.get_Type().IsGenericType && contextualType.get_Type().GetGenericTypeDefinition() == typeof(ReadonlyDictionary))
			{
				return true;
			}
			return base.IsDictionaryType(contextualType);
		}
	}
}