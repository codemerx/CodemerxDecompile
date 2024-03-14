using Microsoft.AspNetCore.Authentication.OAuth;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Translations;
using System;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Squidex.Config.Authentication
{
	public sealed class GithubHandler : OAuthEvents
	{
		public GithubHandler()
		{
		}

		[NullableContext(1)]
		public override Task CreatingTicket(OAuthCreatingTicketContext context)
		{
			string value;
			string str;
			ClaimsIdentity identity = context.get_Identity();
			if (identity != null)
			{
				Claim claim = identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
				if (claim != null)
				{
					value = claim.Value;
				}
				else
				{
					value = null;
				}
			}
			else
			{
				value = null;
			}
			string str1 = value;
			if (!string.IsNullOrWhiteSpace(str1))
			{
				ClaimsIdentity claimsIdentity = context.get_Identity();
				if (claimsIdentity != null)
				{
					claimsIdentity.AddClaim(new Claim("urn:squidex:name", str1));
				}
				else
				{
				}
			}
			ClaimsIdentity identity1 = context.get_Identity();
			if (identity1 != null)
			{
				Claim claim1 = identity1.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
				if (claim1 != null)
				{
					str = claim1.Value;
				}
				else
				{
					str = null;
				}
			}
			else
			{
				str = null;
			}
			if (string.IsNullOrWhiteSpace(str))
			{
				throw new DomainException(T.Get("login.githubPrivateEmail", null), null);
			}
			return base.CreatingTicket(context);
		}
	}
}