using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using System;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Squidex.Config.Authentication
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class GoogleHandler : OAuthEvents
	{
		public GoogleHandler()
		{
		}

		public override Task CreatingTicket(OAuthCreatingTicketContext context)
		{
			JsonElement jsonElement;
			JsonElement jsonElement1;
			JsonElement jsonElement2;
			string value;
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
			string str = value;
			if (!string.IsNullOrWhiteSpace(str))
			{
				ClaimsIdentity claimsIdentity = context.get_Identity();
				if (claimsIdentity != null)
				{
					claimsIdentity.AddClaim(new Claim("urn:squidex:name", str));
				}
				else
				{
				}
			}
			string str1 = null;
			if (context.get_User().TryGetProperty("picture", out jsonElement) && jsonElement.ValueKind == JsonValueKind.String)
			{
				str1 = jsonElement.GetString();
			}
			if (string.IsNullOrWhiteSpace(str1))
			{
				if (context.get_User().TryGetProperty("image", out jsonElement1) && jsonElement1.TryGetProperty("url", out jsonElement2) && jsonElement2.ValueKind == JsonValueKind.String)
				{
					str1 = jsonElement2.GetString();
				}
				if (str1 != null && str1.EndsWith("?sz=50", StringComparison.Ordinal))
				{
					string str2 = str1;
					str1 = str2.Substring(0, str2.Length - 6);
				}
			}
			if (!string.IsNullOrWhiteSpace(str1))
			{
				ClaimsIdentity identity1 = context.get_Identity();
				if (identity1 != null)
				{
					identity1.AddClaim(new Claim("urn:squidex:picture", str1));
				}
				else
				{
				}
			}
			return base.CreatingTicket(context);
		}

		public override Task RedirectToAuthorizationEndpoint(RedirectContext<OAuthOptions> context)
		{
			context.get_Response().Redirect(string.Concat(context.get_RedirectUri(), "&prompt=select_account"));
			return Task.CompletedTask;
		}
	}
}