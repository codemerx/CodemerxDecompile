using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using Squidex.Areas.IdentityServer.Controllers;
using Squidex.Domain.Users;
using Squidex.Shared.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.IdentityServer.Controllers.UserInfo
{
	[Nullable(0)]
	[NullableContext(1)]
	public class UserInfoController : IdentityServerController
	{
		private readonly IUserService userService;

		public UserInfoController(IUserService userService)
		{
			this.userService = userService;
		}

		[Authorize(AuthenticationSchemes="OpenIddict.Server.AspNetCore")]
		[HttpGet]
		[HttpPost]
		[Produces("application/json", new string[] {  })]
		[Route("connect/userinfo")]
		public async Task<IActionResult> UserInfo()
		{
			IActionResult actionResult;
			IUser async = await this.userService.GetAsync(base.get_User(), base.get_HttpContext().get_RequestAborted());
			if (async != null)
			{
				Dictionary<string, object> strs = new Dictionary<string, object>(StringComparer.Ordinal);
				strs["sub"] = async.get_Id();
				Dictionary<string, object> email = strs;
				if (OpenIddictExtensions.HasScope(base.get_User(), "email"))
				{
					email["email"] = async.get_Email();
					email["email_verified"] = true;
				}
				if (OpenIddictExtensions.HasScope(base.get_User(), "roles"))
				{
					email["role"] = Array.Empty<string>();
				}
				actionResult = this.Ok(email);
			}
			else
			{
				Dictionary<string, string> strs1 = new Dictionary<string, string>();
				strs1[".error"] = "invalid_token";
				strs1[".error_description"] = "The specified access token is bound to an account that no longer exists.";
				AuthenticationProperties authenticationProperty = new AuthenticationProperties(strs1);
				string[] strArrays = new string[] { "OpenIddict.Server.AspNetCore" };
				actionResult = this.Challenge(authenticationProperty, strArrays);
			}
			return actionResult;
		}
	}
}