using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Controllers.Account
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class ConsentVM
	{
		public string PrivacyUrl
		{
			get;
			set;
		}

		public string ReturnUrl
		{
			get;
			set;
		}

		public ConsentVM()
		{
		}
	}
}