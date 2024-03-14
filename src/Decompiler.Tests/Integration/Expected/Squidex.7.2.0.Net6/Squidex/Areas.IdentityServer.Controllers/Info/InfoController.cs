using Microsoft.AspNetCore.Mvc;
using Squidex.Areas.IdentityServer.Controllers;
using Squidex.Shared.Identity;
using System;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Squidex.Areas.IdentityServer.Controllers.Info
{
	public sealed class InfoController : IdentityServerController
	{
		public InfoController()
		{
		}

		[HttpGet]
		[NullableContext(1)]
		[Route("info")]
		public IActionResult Info()
		{
			string str = SquidexClaimsExtensions.DisplayName(base.get_User().Claims);
			return this.Ok(new { displayName = str });
		}
	}
}