using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Squidex.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Squidex.Config.Authentication
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class OidcHandler : OpenIdConnectEvents
	{
		private readonly MyIdentityOptions options;

		public OidcHandler(MyIdentityOptions options)
		{
			this.options = options;
		}

		public override Task RedirectToIdentityProviderForSignOut(RedirectContext context)
		{
			if (string.IsNullOrEmpty(this.options.OidcOnSignoutRedirectUrl))
			{
				return base.RedirectToIdentityProviderForSignOut(context);
			}
			string oidcOnSignoutRedirectUrl = this.options.OidcOnSignoutRedirectUrl;
			context.get_Response().Redirect(oidcOnSignoutRedirectUrl);
			context.HandleResponse();
			return Task.CompletedTask;
		}

		public override Task TokenValidated(TokenValidatedContext context)
		{
			bool count;
			ClaimsIdentity identity = (ClaimsIdentity)context.get_Principal().Identity;
			if (!string.IsNullOrWhiteSpace(this.options.OidcRoleClaimType))
			{
				Dictionary<string, string[]> oidcRoleMapping = this.options.OidcRoleMapping;
				if (oidcRoleMapping != null)
				{
					count = oidcRoleMapping.Count >= 0;
				}
				else
				{
					count = false;
				}
				if (count)
				{
					IEnumerable<string> strs = (
						from r in this.options.OidcRoleMapping
						where identity.HasClaim(this.options.OidcRoleClaimType, r.Key)
						select r.Value).SelectMany<string[], string>((string[] r) => r).Distinct<string>();
					foreach (string str in strs)
					{
						identity.AddClaim(new Claim("urn:squidex:permissions", str));
					}
				}
			}
			return base.TokenValidated(context);
		}
	}
}