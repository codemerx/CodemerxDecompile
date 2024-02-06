using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Controllers.Profile
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class UserProperty
	{
		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string Value
		{
			get;
			set;
		}

		public UserProperty()
		{
		}

		public static UserProperty FromTuple([TupleElementNames(new string[] { "Name", "Value" })][Nullable(new byte[] { 0, 1, 1 })] ValueTuple<string, string> value)
		{
			return new UserProperty()
			{
				Name = value.Item1,
				Value = value.Item2
			};
		}

		[return: Nullable(new byte[] { 0, 1, 1 })]
		[return: TupleElementNames(new string[] { "Name", "Value" })]
		public ValueTuple<string, string> ToTuple()
		{
			return new ValueTuple<string, string>(this.Name, this.Value);
		}
	}
}