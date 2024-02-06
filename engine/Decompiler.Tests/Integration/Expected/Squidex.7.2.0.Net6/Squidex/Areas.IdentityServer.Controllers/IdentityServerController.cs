using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Hosting;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Controllers
{
	[Area("IdentityServer")]
	[Nullable(0)]
	[NullableContext(1)]
	[Route("/identity-server")]
	public abstract class IdentityServerController : Controller
	{
		public SignInManager<IdentityUser> SignInManager
		{
			get
			{
				return ServiceProviderServiceExtensions.GetRequiredService<SignInManager<IdentityUser>>(base.get_HttpContext().get_RequestServices());
			}
		}

		protected IdentityServerController()
		{
		}

		protected IActionResult RedirectToReturnUrl([Nullable(2)] string returnUrl)
		{
			if (string.IsNullOrWhiteSpace(returnUrl))
			{
				return this.Redirect("~/../");
			}
			if (ServiceProviderServiceExtensions.GetRequiredService<IUrlGenerator>(base.get_HttpContext().get_RequestServices()).IsAllowedHost(returnUrl))
			{
				return this.Redirect(returnUrl);
			}
			return this.Redirect("~/../");
		}
	}
}