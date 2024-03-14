using Microsoft.AspNetCore.Authentication.OAuth;
using System;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Squidex.Config.Authentication
{
	public sealed class MicrosoftHandler : OAuthEvents
	{
		public MicrosoftHandler()
		{
		}

		[NullableContext(1)]
		public override Task CreatingTicket(OAuthCreatingTicketContext context)
		{
			JsonElement jsonElement;
			JsonElement jsonElement1;
			string str = null;
			if (context.get_User().TryGetProperty("displayName", out jsonElement) && jsonElement.ValueKind == JsonValueKind.String)
			{
				str = jsonElement.GetString();
			}
			if (!string.IsNullOrEmpty(str))
			{
				ClaimsIdentity identity = context.get_Identity();
				if (identity != null)
				{
					identity.AddClaim(new Claim("urn:squidex:name", str));
				}
				else
				{
				}
			}
			string str1 = null;
			if (context.get_User().TryGetProperty("id", out jsonElement1) && jsonElement1.ValueKind == JsonValueKind.String)
			{
				str1 = jsonElement1.GetString();
			}
			if (!string.IsNullOrEmpty(str1))
			{
				string str2 = string.Concat("https://apis.live.net/v5.0/", str1, "/picture");
				ClaimsIdentity claimsIdentity = context.get_Identity();
				if (claimsIdentity != null)
				{
					claimsIdentity.AddClaim(new Claim("urn:squidex:picture", str2));
				}
				else
				{
				}
			}
			return base.CreatingTicket(context);
		}
	}
}