using Squidex.Domain.Apps.Core.Rules;
using Squidex.Web.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Squidex.Areas.Api.Controllers.Rules.Models
{
	[JsonInheritanceConverter(typeof(RuleTriggerDto), "triggerType")]
	[KnownType("Subtypes")]
	[Nullable(0)]
	[NullableContext(1)]
	public abstract class RuleTriggerDto
	{
		protected RuleTriggerDto()
		{
		}

		public static Type[] Subtypes()
		{
			Type type = typeof(RuleTriggerDto);
			Type type1 = type;
			return type.Assembly.GetTypes().Where<Type>(new Func<Type, bool>(type1.IsAssignableFrom)).ToArray<Type>();
		}

		public abstract RuleTrigger ToTrigger();
	}
}