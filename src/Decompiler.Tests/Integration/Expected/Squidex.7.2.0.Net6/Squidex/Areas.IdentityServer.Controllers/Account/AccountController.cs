using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Squidex.Areas.IdentityServer.Controllers;
using Squidex.Config;
using Squidex.Domain.Users;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Security;
using Squidex.Infrastructure.Translations;
using Squidex.Shared.Identity;
using Squidex.Shared.Users;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Squidex.Areas.IdentityServer.Controllers.Account
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AccountController : IdentityServerController
	{
		private readonly IUserService userService;

		private readonly MyIdentityOptions identityOptions;

		public AccountController(IUserService userService, IOptions<MyIdentityOptions> identityOptions)
		{
			this.identityOptions = identityOptions.get_Value();
			this.userService = userService;
		}

		[HttpGet]
		[Route("account/accessdenied/")]
		public IActionResult AccessDenied()
		{
			return this.View();
		}

		[HttpGet]
		[Route("account/consent/")]
		public IActionResult Consent([Nullable(2)] string returnUrl = null)
		{
			return this.View(new ConsentVM()
			{
				PrivacyUrl = this.identityOptions.PrivacyUrl,
				ReturnUrl = returnUrl
			});
		}

		[HttpPost]
		[Route("account/consent/")]
		public async Task<IActionResult> Consent(ConsentModel model, [Nullable(2)] string returnUrl = null)
		{
			IActionResult actionResult;
			if (!model.ConsentToCookies)
			{
				base.get_ModelState().AddModelError("ConsentToCookies", T.Get("users.consent.needed", null));
			}
			if (!model.ConsentToPersonalInformation)
			{
				base.get_ModelState().AddModelError("ConsentToPersonalInformation", T.Get("users.consent.needed", null));
			}
			if (base.get_ModelState().get_IsValid())
			{
				IUser async = await this.userService.GetAsync(base.get_User(), base.get_HttpContext().get_RequestAborted());
				if (async == null)
				{
					throw new DomainException(T.Get("users.userNotFound", null), null);
				}
				UserValues userValue = new UserValues();
				userValue.set_Consent(new bool?(true));
				userValue.set_ConsentForEmails(new bool?(model.ConsentToAutomatedEmails));
				UserValues userValue1 = userValue;
				await this.userService.UpdateAsync(async.get_Id(), userValue1, false, base.get_HttpContext().get_RequestAborted());
				actionResult = base.RedirectToReturnUrl(returnUrl);
			}
			else
			{
				ConsentVM consentVM = new ConsentVM()
				{
					PrivacyUrl = this.identityOptions.PrivacyUrl,
					ReturnUrl = returnUrl
				};
				actionResult = this.View(consentVM);
			}
			return actionResult;
		}

		[HttpPost]
		[Route("account/external/")]
		public IActionResult External(string provider, [Nullable(2)] string returnUrl = null)
		{
			string str = UrlHelperExtensions.Action(base.get_Url(), "ExternalCallback", new { returnUrl = returnUrl });
			AuthenticationProperties authenticationProperty = base.SignInManager.ConfigureExternalAuthenticationProperties(provider, str, null);
			return this.Challenge(authenticationProperty, new string[] { provider });
		}

		[HttpGet]
		[Route("account/external-callback/")]
		public async Task<IActionResult> ExternalCallback([Nullable(2)] string returnUrl = null)
		{
			IActionResult action;
			IUser user;
			ExternalLoginInfo externalLoginInfoWithDisplayNameAsync = await Squidex.Areas.IdentityServer.Controllers.Extensions.GetExternalLoginInfoWithDisplayNameAsync(base.SignInManager, null);
			if (externalLoginInfoWithDisplayNameAsync != null)
			{
				Microsoft.AspNetCore.Identity.SignInResult signInResult = await base.SignInManager.ExternalLoginSignInAsync(externalLoginInfoWithDisplayNameAsync.get_LoginProvider(), externalLoginInfoWithDisplayNameAsync.get_ProviderKey(), true);
				if (signInResult.get_Succeeded() || !signInResult.get_IsLockedOut())
				{
					bool succeeded = signInResult.get_Succeeded();
					bool item2 = false;
					user = null;
					if (!succeeded)
					{
						string email = Squidex.Infrastructure.Security.Extensions.GetEmail(externalLoginInfoWithDisplayNameAsync.get_Principal());
						if (string.IsNullOrWhiteSpace(email))
						{
							throw new DomainException(T.Get("users.noEmailAddress", null), null);
						}
						user = await this.userService.FindByEmailAsync(email, base.get_HttpContext().get_RequestAborted());
						bool flag = user != null;
						if (flag)
						{
							flag = await this.HasLoginAsync(user);
						}
						if (flag)
						{
							user = null;
						}
						if (user == null)
						{
							UserValues userValue = new UserValues();
							userValue.set_CustomClaims(SquidexClaimsExtensions.GetSquidexClaims(externalLoginInfoWithDisplayNameAsync.get_Principal().Claims).ToList<Claim>());
							UserValues userValue1 = userValue;
							bool lockAutomatically = this.identityOptions.LockAutomatically;
							user = await this.userService.CreateAsync(email, userValue1, lockAutomatically, base.get_HttpContext().get_RequestAborted());
						}
						if (user != null)
						{
							await this.userService.AddLoginAsync(user.get_Id(), externalLoginInfoWithDisplayNameAsync, base.get_HttpContext().get_RequestAborted());
							ValueTuple<bool, bool> valueTuple = await this.LoginAsync(externalLoginInfoWithDisplayNameAsync);
							succeeded = valueTuple.Item1;
							item2 = valueTuple.Item2;
						}
						email = null;
					}
					else
					{
						user = await this.userService.FindByLoginAsync(externalLoginInfoWithDisplayNameAsync.get_LoginProvider(), externalLoginInfoWithDisplayNameAsync.get_ProviderKey(), base.get_HttpContext().get_RequestAborted());
					}
					if (item2)
					{
						action = this.View("LockedOut");
					}
					else if (!succeeded)
					{
						action = this.RedirectToAction("Login");
					}
					else if (user == null || SquidexClaimsExtensions.HasConsent(user.get_Claims()) || this.identityOptions.NoConsent)
					{
						action = base.RedirectToReturnUrl(returnUrl);
					}
					else
					{
						action = this.RedirectToAction("Consent", new { returnUrl = returnUrl });
					}
				}
				else
				{
					action = this.View("LockedOut");
				}
			}
			else
			{
				action = this.RedirectToAction("Login");
			}
			externalLoginInfoWithDisplayNameAsync = null;
			user = null;
			return action;
		}

		[HttpGet]
		[Route("account/forbidden/")]
		public IActionResult Forbidden()
		{
			throw new DomainForbiddenException(T.Get("users.userLocked", null), null);
		}

		private async Task<bool> HasLoginAsync(IUser user)
		{
			bool count;
			if (!await this.userService.HasPasswordAsync(user, base.get_HttpContext().get_RequestAborted()))
			{
				IList<UserLoginInfo> loginsAsync = await this.userService.GetLoginsAsync(user, base.get_HttpContext().get_RequestAborted());
				count = loginsAsync.Count > 0;
			}
			else
			{
				count = true;
			}
			return count;
		}

		[HttpGet]
		[Route("account/lockedout/")]
		public IActionResult LockedOut()
		{
			return this.View();
		}

		[ClearCookies]
		[HttpGet]
		[Route("account/login/")]
		public Task<IActionResult> Login([Nullable(2)] string returnUrl = null)
		{
			return this.LoginViewAsync(returnUrl, true, false);
		}

		[HttpPost]
		[Route("account/login/")]
		public async Task<IActionResult> Login(LoginModel model, [Nullable(2)] string returnUrl = null)
		{
			IActionResult actionResult;
			if (base.get_ModelState().get_IsValid())
			{
				Microsoft.AspNetCore.Identity.SignInResult signInResult = await base.SignInManager.PasswordSignInAsync(model.Email, model.Password, true, true);
				if (!signInResult.get_Succeeded() && signInResult.get_IsLockedOut())
				{
					actionResult = this.View("LockedOut");
				}
				else if (signInResult.get_Succeeded())
				{
					actionResult = base.RedirectToReturnUrl(returnUrl);
				}
				else
				{
					actionResult = await this.LoginViewAsync(returnUrl, true, true);
				}
			}
			else
			{
				actionResult = await this.LoginViewAsync(returnUrl, true, true);
			}
			return actionResult;
		}

		[return: Nullable(new byte[] { 1, 0 })]
		[return: TupleElementNames(new string[] { "Success", "Locked" })]
		private async Task<ValueTuple<bool, bool>> LoginAsync(UserLoginInfo externalLogin)
		{
			Microsoft.AspNetCore.Identity.SignInResult signInResult = await base.SignInManager.ExternalLoginSignInAsync(externalLogin.get_LoginProvider(), externalLogin.get_ProviderKey(), true);
			return new ValueTuple<bool, bool>(signInResult.get_Succeeded(), signInResult.get_IsLockedOut());
		}

		[HttpGet]
		[Route("account/error/")]
		public IActionResult LoginError()
		{
			throw new InvalidOperationException();
		}

		private async Task<IActionResult> LoginViewAsync([Nullable(2)] string returnUrl, bool isLogin, bool isFailed)
		{
			IActionResult actionResult;
			bool allowPasswordAuth = this.identityOptions.AllowPasswordAuth;
			List<ExternalProvider> externalProvidersAsync = await Squidex.Areas.IdentityServer.Controllers.Extensions.GetExternalProvidersAsync(base.SignInManager);
			if (externalProvidersAsync.Count != 1 || allowPasswordAuth)
			{
				LoginVM loginVM = new LoginVM()
				{
					ExternalProviders = externalProvidersAsync,
					IsFailed = isFailed,
					IsLogin = isLogin,
					HasPasswordAuth = allowPasswordAuth,
					ReturnUrl = returnUrl
				};
				actionResult = this.View("Login", loginVM);
			}
			else
			{
				string authenticationScheme = externalProvidersAsync[0].AuthenticationScheme;
				string str = UrlHelperExtensions.Action(base.get_Url(), "ExternalCallback");
				AuthenticationProperties authenticationProperty = base.SignInManager.ConfigureExternalAuthenticationProperties(authenticationScheme, str, null);
				string[] strArrays = new string[] { authenticationScheme };
				actionResult = this.Challenge(authenticationProperty, strArrays);
			}
			return actionResult;
		}

		[HttpGet]
		[Route("account/logout/")]
		public async Task<IActionResult> Logout(string logoutId)
		{
			await base.SignInManager.SignOutAsync();
			return this.Redirect("~/../");
		}

		[HttpGet]
		[Route("account/logout-completed/")]
		public IActionResult LogoutCompleted()
		{
			return this.View();
		}

		[HttpGet]
		[Route("account/logout-redirect/")]
		public async Task<IActionResult> LogoutRedirect()
		{
			await base.SignInManager.SignOutAsync();
			return this.RedirectToAction("LogoutCompleted");
		}

		[HttpGet]
		[Route("account/signup/")]
		public Task<IActionResult> Signup([Nullable(2)] string returnUrl = null)
		{
			return this.LoginViewAsync(returnUrl, false, false);
		}
	}
}