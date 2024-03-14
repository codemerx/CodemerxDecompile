using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.IdentityServer.Controllers
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class Extensions
	{
		public static async Task<ExternalLoginInfo> GetExternalLoginInfoWithDisplayNameAsync(SignInManager<IdentityUser> signInManager, [Nullable(2)] string expectedXsrf = null)
		{
			ExternalLoginInfo externalLoginInfo;
			ExternalLoginInfo externalLoginInfoAsync = await signInManager.GetExternalLoginInfoAsync(expectedXsrf);
			if (externalLoginInfoAsync != null)
			{
				string email = Squidex.Infrastructure.Security.Extensions.GetEmail(externalLoginInfoAsync.get_Principal());
				if (!string.IsNullOrWhiteSpace(email))
				{
					externalLoginInfoAsync.set_ProviderDisplayName(email);
					externalLoginInfo = externalLoginInfoAsync;
				}
				else
				{
					Squidex.Infrastructure.ThrowHelper.InvalidOperationException("External provider does not provide email claim.");
					externalLoginInfo = null;
				}
			}
			else
			{
				Squidex.Infrastructure.ThrowHelper.InvalidOperationException("Request from external provider cannot be handled.");
				externalLoginInfo = null;
			}
			return externalLoginInfo;
		}

		public static async Task<List<ExternalProvider>> GetExternalProvidersAsync(SignInManager<IdentityUser> signInManager)
		{
			IEnumerable<AuthenticationScheme> externalAuthenticationSchemesAsync = await signInManager.GetExternalAuthenticationSchemesAsync();
			IEnumerable<AuthenticationScheme> name = 
				from x in externalAuthenticationSchemesAsync
				where x.get_Name() != "OpenIdConnect"
				select x;
			IEnumerable<AuthenticationScheme> authenticationSchemes = 
				from x in name
				where x.get_Name() != "API"
				select x;
			List<ExternalProvider> list = (
				from x in authenticationSchemes
				select new ExternalProvider(x.get_Name(), x.get_DisplayName() ?? x.get_Name())).ToList<ExternalProvider>();
			return list;
		}
	}
}