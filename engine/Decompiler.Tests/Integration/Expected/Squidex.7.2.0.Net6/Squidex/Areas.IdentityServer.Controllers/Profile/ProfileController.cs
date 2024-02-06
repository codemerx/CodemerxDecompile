using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Squidex.Areas.IdentityServer.Controllers;
using Squidex.Assets;
using Squidex.Config;
using Squidex.Domain.Users;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Tasks;
using Squidex.Infrastructure.Translations;
using Squidex.Infrastructure.Validation;
using Squidex.Shared.Identity;
using Squidex.Shared.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Areas.IdentityServer.Controllers.Profile
{
	[Authorize]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class ProfileController : IdentityServerController
	{
		private readonly IUserPictureStore userPictureStore;

		private readonly IUserService userService;

		private readonly IAssetThumbnailGenerator assetThumbnailGenerator;

		private readonly MyIdentityOptions identityOptions;

		public ProfileController(IOptions<MyIdentityOptions> identityOptions, IUserPictureStore userPictureStore, IUserService userService, IAssetThumbnailGenerator assetThumbnailGenerator)
		{
			this.identityOptions = identityOptions.get_Value();
			this.userPictureStore = userPictureStore;
			this.userService = userService;
			this.assetThumbnailGenerator = assetThumbnailGenerator;
		}

		[HttpPost]
		[Route("account/profile/login-add/")]
		public async Task<IActionResult> AddLogin(string provider)
		{
			await AuthenticationHttpContextExtensions.SignOutAsync(base.get_HttpContext(), IdentityConstants.ExternalScheme);
			string userId = this.userService.GetUserId(base.get_User(), base.get_HttpContext().get_RequestAborted());
			string str = UrlHelperExtensions.Action(base.get_Url(), "AddLoginCallback");
			AuthenticationProperties authenticationProperty = base.SignInManager.ConfigureExternalAuthenticationProperties(provider, str, userId);
			return this.Challenge(authenticationProperty, new string[] { provider });
		}

		private async Task AddLoginAsync(string id, CancellationToken ct)
		{
			ExternalLoginInfo externalLoginInfoWithDisplayNameAsync = await Extensions.GetExternalLoginInfoWithDisplayNameAsync(base.SignInManager, id);
			await this.userService.AddLoginAsync(id, externalLoginInfoWithDisplayNameAsync, ct);
		}

		[HttpGet]
		[Route("account/profile/login-add-callback/")]
		public Task<IActionResult> AddLoginCallback()
		{
			return this.MakeChangeAsync<None>((string id, CancellationToken ct) => this.AddLoginAsync(id, ct), T.Get("users.profile.addLoginDone", null), None.Value);
		}

		[HttpPost]
		[Route("account/profile/password-change/")]
		public Task<IActionResult> ChangePassword(ChangePasswordModel model)
		{
			return this.MakeChangeAsync<ChangePasswordModel>((string id, CancellationToken ct) => this.userService.SetPasswordAsync(id, model.Password, model.OldPassword, ct), T.Get("users.profile.changePasswordDone", null), model);
		}

		[HttpPost]
		[Route("account/profile/generate-client-secret/")]
		public Task<IActionResult> GenerateClientSecret()
		{
			return this.MakeChangeAsync<None>((string id, CancellationToken ct) => this.GenerateClientSecretAsync(id, ct), T.Get("users.profile.generateClientDone", null), None.Value);
		}

		private async Task GenerateClientSecretAsync(string id, CancellationToken ct)
		{
			UserValues userValue = new UserValues();
			userValue.set_ClientSecret(RandomHash.New());
			UserValues userValue1 = userValue;
			await this.userService.UpdateAsync(id, userValue1, false, ct);
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		private async Task<ProfileVM> GetVM<TModel>(IUser user, TModel model = null, string errorMessage = null, string successMessage = null)
		where TModel : class
		{
			if (user == null)
			{
				throw new DomainException(T.Get("users.userNotFound", null), null);
			}
			ValueTuple<List<ExternalProvider>, bool, IList<UserLoginInfo>> valueTuple = await AsyncHelper.WhenAll<List<ExternalProvider>, bool, IList<UserLoginInfo>>(Extensions.GetExternalProvidersAsync(base.SignInManager), this.userService.HasPasswordAsync(user, base.get_HttpContext().get_RequestAborted()), this.userService.GetLoginsAsync(user, base.get_HttpContext().get_RequestAborted()));
			List<ExternalProvider> item1 = valueTuple.Item1;
			bool item2 = valueTuple.Item2;
			IList<UserLoginInfo> item3 = valueTuple.Item3;
			ProfileVM profileVM = new ProfileVM()
			{
				Id = user.get_Id(),
				ClientSecret = SquidexClaimsExtensions.ClientSecret(user.get_Claims()),
				Email = user.get_Email(),
				ErrorMessage = errorMessage,
				ExternalLogins = item3,
				ExternalProviders = item1,
				DisplayName = SquidexClaimsExtensions.DisplayName(user.get_Claims()),
				HasPassword = item2,
				HasPasswordAuth = this.identityOptions.AllowPasswordAuth,
				IsHidden = SquidexClaimsExtensions.IsHidden(user.get_Claims()),
				SuccessMessage = successMessage
			};
			ProfileVM profileVM1 = profileVM;
			if (model != null)
			{
				SimpleMapper.Map<TModel, ProfileVM>(model, profileVM1);
			}
			ProfileVM profileVM2 = profileVM1;
			if (profileVM2.Properties == null)
			{
				List<UserProperty> list = SquidexClaimsExtensions.GetCustomProperties(user.get_Claims()).Select<ValueTuple<string, string>, UserProperty>(new Func<ValueTuple<string, string>, UserProperty>(UserProperty.FromTuple)).ToList<UserProperty>();
				profileVM2.Properties = list;
			}
			return profileVM1;
		}

		private async Task<IActionResult> MakeChangeAsync<TModel>(Func<string, CancellationToken, Task> action, string successMessage, [Nullable(2)] TModel model = null)
		where TModel : class
		{
			IActionResult actionResult;
			string message;
			IUser async = await this.userService.GetAsync(base.get_User(), base.get_HttpContext().get_RequestAborted());
			if (async == null)
			{
				actionResult = this.NotFound();
			}
			else if (base.get_ModelState().get_IsValid())
			{
				try
				{
					await action(async.get_Id(), base.get_HttpContext().get_RequestAborted());
					await base.SignInManager.SignInAsync((IdentityUser)async.get_Identity(), true, null);
					actionResult = this.RedirectToAction("Profile", new { successMessage = successMessage });
					async = null;
					return actionResult;
				}
				catch (ValidationException validationException)
				{
					message = validationException.Message;
				}
				catch (Exception exception)
				{
					message = T.Get("users.errorHappened", null);
				}
				ProfileVM vM = await this.GetVM<TModel>(async, model, message, null);
				actionResult = this.View("Profile", vM);
			}
			else
			{
				ProfileVM profileVM = await this.GetVM<TModel>(async, model, null, null);
				actionResult = this.View("Profile", profileVM);
			}
			async = null;
			return actionResult;
		}

		[HttpGet]
		[Route("account/profile/")]
		public async Task<IActionResult> Profile([Nullable(2)] string successMessage = null)
		{
			IUser async = await this.userService.GetAsync(base.get_User(), base.get_HttpContext().get_RequestAborted());
			IActionResult actionResult = this.View(await this.GetVM<None>(async, null, null, successMessage));
			return actionResult;
		}

		[HttpPost]
		[Route("account/profile/login-remove/")]
		public Task<IActionResult> RemoveLogin(RemoveLoginModel model)
		{
			return this.MakeChangeAsync<RemoveLoginModel>((string id, CancellationToken ct) => this.userService.RemoveLoginAsync(id, model.LoginProvider, model.ProviderKey, ct), T.Get("users.profile.removeLoginDone", null), model);
		}

		[HttpPost]
		[Route("account/profile/password-set/")]
		public Task<IActionResult> SetPassword(SetPasswordModel model)
		{
			return this.MakeChangeAsync<SetPasswordModel>((string id, CancellationToken ct) => this.userService.SetPasswordAsync(id, model.Password, null, ct), T.Get("users.profile.setPasswordDone", null), model);
		}

		private async Task UpdatePictureAsync(List<IFormFile> files, string id, CancellationToken ct)
		{
			if (files.Count != 1)
			{
				throw new ValidationException(T.Get("validation.onlyOneFile", null), null);
			}
			await this.UploadResizedAsync(files[0], id, ct);
			UserValues userValue = new UserValues();
			userValue.set_PictureUrl("store");
			UserValues userValue1 = userValue;
			await this.userService.UpdateAsync(id, userValue1, false, ct);
		}

		[HttpPost]
		[Route("account/profile/update/")]
		public Task<IActionResult> UpdateProfile(ChangeProfileModel model)
		{
			return this.MakeChangeAsync<ChangeProfileModel>((string id, CancellationToken ct) => this.userService.UpdateAsync(id, model.ToValues(), false, ct), T.Get("users.profile.updateProfileDone", null), model);
		}

		[HttpPost]
		[Route("account/profile/properties/")]
		public Task<IActionResult> UpdateProperties(ChangePropertiesModel model)
		{
			return this.MakeChangeAsync<ChangePropertiesModel>((string id, CancellationToken ct) => this.userService.UpdateAsync(id, model.ToValues(), false, ct), T.Get("users.profile.updatePropertiesDone", null), model);
		}

		[HttpPost]
		[Route("account/profile/upload-picture/")]
		public Task<IActionResult> UploadPicture(List<IFormFile> file)
		{
			return this.MakeChangeAsync<None>((string id, CancellationToken ct) => this.UpdatePictureAsync(file, id, ct), T.Get("users.profile.uploadPictureDone", null), None.Value);
		}

		private async Task UploadResizedAsync(IFormFile file, string id, CancellationToken ct)
		{
			// 
			// Current member / type: System.Threading.Tasks.Task Squidex.Areas.IdentityServer.Controllers.Profile.ProfileController::UploadResizedAsync(Microsoft.AspNetCore.Http.IFormFile,System.String,System.Threading.CancellationToken)
			// Exception in: System.Threading.Tasks.Task UploadResizedAsync(Microsoft.AspNetCore.Http.IFormFile,System.String,System.Threading.CancellationToken)
			// GoTo misplaced.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}
	}
}