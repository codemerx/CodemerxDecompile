using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Controllers.Setup
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class SetupVM
	{
		public string BaseUrlConfigured
		{
			get;
			set;
		}

		public string BaseUrlCurrent
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

		public bool EverybodyCanCreateApps
		{
			get;
			set;
		}

		public bool EverybodyCanCreateTeams
		{
			get;
			set;
		}

		public bool HasExternalLogin
		{
			get;
			set;
		}

		public bool HasPasswordAuth
		{
			get;
			set;
		}

		public bool IsAssetStoreFile
		{
			get;
			set;
		}

		public bool IsAssetStoreFtp
		{
			get;
			set;
		}

		public bool IsValidHttps
		{
			get;
			set;
		}

		public SetupVM()
		{
		}
	}
}