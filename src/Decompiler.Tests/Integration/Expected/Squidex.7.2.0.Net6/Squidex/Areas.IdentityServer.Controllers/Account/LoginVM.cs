using Squidex.Areas.IdentityServer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Controllers.Account
{
	[Nullable(0)]
	[NullableContext(1)]
	public class LoginVM
	{
		public IReadOnlyList<ExternalProvider> ExternalProviders
		{
			get;
			set;
		}

		public bool HasExternalLogin
		{
			get
			{
				return this.ExternalProviders.Any<ExternalProvider>();
			}
		}

		public bool HasPasswordAuth
		{
			get;
			set;
		}

		public bool IsFailed
		{
			get;
			set;
		}

		public bool IsLogin
		{
			get;
			set;
		}

		[Nullable(2)]
		public string ReturnUrl
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		public LoginVM()
		{
		}
	}
}