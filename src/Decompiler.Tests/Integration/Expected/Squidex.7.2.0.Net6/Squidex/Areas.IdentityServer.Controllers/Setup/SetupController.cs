using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Squidex.Areas.Api.Controllers.UI;
using Squidex.Areas.IdentityServer.Controllers;
using Squidex.Assets;
using Squidex.Config;
using Squidex.Domain.Users;
using Squidex.Hosting;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Translations;
using Squidex.Infrastructure.Validation;
using Squidex.Shared.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.IdentityServer.Controllers.Setup
{
	[Nullable(0)]
	[NullableContext(1)]
	public class SetupController : IdentityServerController
	{
		private readonly IAssetStore assetStore;

		private readonly IUrlGenerator urlGenerator;

		private readonly IUserService userService;

		private readonly MyUIOptions uiOptions;

		private readonly MyIdentityOptions identityOptions;

		public SetupController(IAssetStore assetStore, IOptions<MyUIOptions> uiOptions, IOptions<MyIdentityOptions> identityOptions, IUrlGenerator urlGenerator, IUserService userService)
		{
			this.assetStore = assetStore;
			this.identityOptions = identityOptions.get_Value();
			this.uiOptions = uiOptions.get_Value();
			this.urlGenerator = urlGenerator;
			this.userService = userService;
		}

		private string GetCurrentUrl()
		{
			HttpRequest request = base.get_HttpContext().get_Request();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 3);
			defaultInterpolatedStringHandler.AppendFormatted(request.get_Scheme());
			defaultInterpolatedStringHandler.AppendLiteral("://");
			defaultInterpolatedStringHandler.AppendFormatted<HostString>(request.get_Host());
			defaultInterpolatedStringHandler.AppendFormatted<PathString>(request.get_PathBase());
			string stringAndClear = defaultInterpolatedStringHandler.ToStringAndClear();
			if (stringAndClear.EndsWith("/identity-server", StringComparison.Ordinal))
			{
				string str = stringAndClear;
				int length = "/identity-server".Length;
				stringAndClear = str.Substring(0, str.Length - length);
			}
			return stringAndClear.TrimEnd('/');
		}

		private async Task<SetupVM> GetVM<TModel>([Nullable(2)] TModel model = null, [Nullable(2)] string errorMessage = null)
		where TModel : class
		{
			List<ExternalProvider> externalProvidersAsync = await Extensions.GetExternalProvidersAsync(base.SignInManager);
			SetupVM setupVM = new SetupVM()
			{
				BaseUrlConfigured = this.urlGenerator.BuildUrl(string.Empty, false),
				BaseUrlCurrent = this.GetCurrentUrl(),
				ErrorMessage = errorMessage,
				EverybodyCanCreateApps = !this.uiOptions.OnlyAdminsCanCreateApps,
				EverybodyCanCreateTeams = !this.uiOptions.OnlyAdminsCanCreateTeams,
				IsValidHttps = base.get_HttpContext().get_Request().get_IsHttps(),
				IsAssetStoreFile = this.assetStore is FolderAssetStore,
				IsAssetStoreFtp = this.assetStore is FTPAssetStore,
				HasExternalLogin = externalProvidersAsync.Any<ExternalProvider>(),
				HasPasswordAuth = this.identityOptions.AllowPasswordAuth
			};
			SetupVM setupVM1 = setupVM;
			if (model != null)
			{
				SimpleMapper.Map<TModel, SetupVM>(model, setupVM1);
			}
			return setupVM1;
		}

		[HttpGet]
		[Route("setup/")]
		public async Task<IActionResult> Setup()
		{
			IActionResult returnUrl;
			if (await this.userService.IsEmptyAsync(base.get_HttpContext().get_RequestAborted()))
			{
				SetupVM vM = await this.GetVM<None>(None.Value, null);
				returnUrl = this.View("Setup", vM);
			}
			else
			{
				returnUrl = base.RedirectToReturnUrl(null);
			}
			return returnUrl;
		}

		[HttpPost]
		[Route("setup/")]
		public async Task<IActionResult> Setup(CreateUserModel model)
		{
			IActionResult returnUrl;
			string message;
			if (!await this.userService.IsEmptyAsync(base.get_HttpContext().get_RequestAborted()))
			{
				returnUrl = base.RedirectToReturnUrl(null);
			}
			else if (base.get_ModelState().get_IsValid())
			{
				try
				{
					IUserService userService = this.userService;
					string email = model.Email;
					UserValues userValue = new UserValues();
					userValue.set_Password(model.Password);
					IUser user = await userService.CreateAsync(email, userValue, false, base.get_HttpContext().get_RequestAborted());
					await base.SignInManager.SignInAsync((IdentityUser)user.get_Identity(), true, null);
					returnUrl = base.RedirectToReturnUrl(null);
					return returnUrl;
				}
				catch (ValidationException validationException)
				{
					message = validationException.Message;
				}
				catch (Exception exception)
				{
					message = T.Get("users.errorHappened", null);
				}
				SetupVM vM = await this.GetVM<CreateUserModel>(model, message);
				returnUrl = this.View("Setup", vM);
			}
			else
			{
				SetupVM setupVM = await this.GetVM<CreateUserModel>(model, null);
				returnUrl = this.View("Profile", setupVM);
			}
			return returnUrl;
		}
	}
}