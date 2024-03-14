using Microsoft.AspNetCore.Identity;
using Squidex.Areas.IdentityServer.Controllers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Controllers.Profile
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class ProfileVM
	{
		[Nullable(2)]
		public string ClientSecret
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		public string DisplayName
		{
			get;
			set;
		}

		public string Email
		{
			get;
			set;
		}

		[Nullable(2)]
		public string ErrorMessage
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		public IList<UserLoginInfo> ExternalLogins
		{
			get;
			set;
		}

		public IList<ExternalProvider> ExternalProviders
		{
			get;
			set;
		}

		public bool HasPassword
		{
			get;
			set;
		}

		public bool HasPasswordAuth
		{
			get;
			set;
		}

		public string Id
		{
			get;
			set;
		}

		public bool IsHidden
		{
			get;
			set;
		}

		public List<UserProperty> Properties
		{
			get;
			set;
		}

		[Nullable(2)]
		public string SuccessMessage
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		public ProfileVM()
		{
		}
	}
}