using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Config
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class MyIdentityOptions
	{
		public string[] AdminApps
		{
			get;
			set;
		}

		public string AdminClientId
		{
			get;
			set;
		}

		public string AdminClientSecret
		{
			get;
			set;
		}

		public string AdminEmail
		{
			get;
			set;
		}

		public string AdminPassword
		{
			get;
			set;
		}

		public bool AdminRecreate
		{
			get;
			set;
		}

		public bool AllowPasswordAuth
		{
			get;
			set;
		}

		public string AuthorityUrl
		{
			get;
			set;
		}

		public string GithubClient
		{
			get;
			set;
		}

		public string GithubSecret
		{
			get;
			set;
		}

		public string GoogleClient
		{
			get;
			set;
		}

		public string GoogleSecret
		{
			get;
			set;
		}

		public bool LockAutomatically
		{
			get;
			set;
		}

		public string MicrosoftClient
		{
			get;
			set;
		}

		public string MicrosoftSecret
		{
			get;
			set;
		}

		public string MicrosoftTenant
		{
			get;
			set;
		}

		public bool MultipleDomains
		{
			get;
			set;
		}

		public bool NoConsent
		{
			get;
			set;
		}

		public string OidcAuthority
		{
			get;
			set;
		}

		public string OidcClient
		{
			get;
			set;
		}

		public bool OidcGetClaimsFromUserInfoEndpoint
		{
			get;
			set;
		}

		public string OidcMetadataAddress
		{
			get;
			set;
		}

		public string OidcName
		{
			get;
			set;
		}

		public string OidcOnSignoutRedirectUrl
		{
			get;
			set;
		}

		public string OidcResponseType
		{
			get;
			set;
		}

		public string OidcRoleClaimType
		{
			get;
			set;
		}

		public Dictionary<string, string[]> OidcRoleMapping
		{
			get;
			set;
		}

		public string[] OidcScopes
		{
			get;
			set;
		}

		public string OidcSecret
		{
			get;
			set;
		}

		public string PrivacyUrl
		{
			get;
			set;
		}

		public bool RequiresHttps
		{
			get;
			set;
		}

		public bool ShowPII
		{
			get;
			set;
		}

		public bool SuppressXFrameOptionsHeader
		{
			get;
			set;
		}

		public MyIdentityOptions()
		{
		}

		public bool IsAdminClientConfigured()
		{
			if (string.IsNullOrWhiteSpace(this.AdminClientId))
			{
				return false;
			}
			return !string.IsNullOrWhiteSpace(this.AdminClientSecret);
		}

		public bool IsAdminConfigured()
		{
			if (string.IsNullOrWhiteSpace(this.AdminEmail))
			{
				return false;
			}
			return !string.IsNullOrWhiteSpace(this.AdminPassword);
		}

		public bool IsGithubAuthConfigured()
		{
			if (string.IsNullOrWhiteSpace(this.GithubClient))
			{
				return false;
			}
			return !string.IsNullOrWhiteSpace(this.GithubSecret);
		}

		public bool IsGoogleAuthConfigured()
		{
			if (string.IsNullOrWhiteSpace(this.GoogleClient))
			{
				return false;
			}
			return !string.IsNullOrWhiteSpace(this.GoogleSecret);
		}

		public bool IsMicrosoftAuthConfigured()
		{
			if (string.IsNullOrWhiteSpace(this.MicrosoftClient))
			{
				return false;
			}
			return !string.IsNullOrWhiteSpace(this.MicrosoftSecret);
		}

		public bool IsOidcConfigured()
		{
			if (string.IsNullOrWhiteSpace(this.OidcAuthority))
			{
				return false;
			}
			return !string.IsNullOrWhiteSpace(this.OidcClient);
		}
	}
}