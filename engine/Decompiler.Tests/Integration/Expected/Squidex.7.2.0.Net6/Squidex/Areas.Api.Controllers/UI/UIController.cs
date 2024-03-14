using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Squidex.Areas.Api.Controllers.UI.Models;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.Json.Objects;
using Squidex.Infrastructure.Security;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.UI
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class UIController : ApiController
	{
		private readonly static Permission CreateAppPermission;

		private readonly static Permission CreateTeamPermission;

		private readonly MyUIOptions uiOptions;

		private readonly IAppUISettings appUISettings;

		static UIController()
		{
			UIController.CreateAppPermission = new Permission("squidex.admin.apps.create");
			UIController.CreateTeamPermission = new Permission("squidex.admin.teams.create");
		}

		public UIController(ICommandBus commandBus, IOptions<MyUIOptions> uiOptions, IAppUISettings appUISettings) : base(commandBus)
		{
			this.uiOptions = uiOptions.get_Value();
			this.appUISettings = appUISettings;
		}

		[ApiPermission(new string[] {  })]
		[HttpDelete]
		[Route("apps/{app}/ui/settings/{key}")]
		public async Task<IActionResult> DeleteSetting(string app, string key)
		{
			await this.appUISettings.RemoveAsync(base.get_AppId(), null, key, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiPermission(new string[] {  })]
		[HttpDelete]
		[Route("apps/{app}/ui/settings/me/{key}")]
		public async Task<IActionResult> DeleteUserSetting(string app, string key)
		{
			await this.appUISettings.RemoveAsync(base.get_AppId(), base.get_UserId(), key, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(UISettingsDto), 200)]
		[Route("ui/settings/")]
		public IActionResult GetSettings()
		{
			return this.Ok(new UISettingsDto()
			{
				CanCreateApps = (!this.uiOptions.OnlyAdminsCanCreateApps ? true : base.get_Context().get_UserPermissions().Includes(UIController.CreateAppPermission)),
				CanCreateTeams = (!this.uiOptions.OnlyAdminsCanCreateApps ? true : base.get_Context().get_UserPermissions().Includes(UIController.CreateTeamPermission))
			});
		}

		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(Dictionary<string, string>), 200)]
		[Route("apps/{app}/ui/settings/")]
		public async Task<IActionResult> GetSettings(string app)
		{
			IActionResult actionResult = this.Ok(await this.appUISettings.GetAsync(base.get_AppId(), null, base.get_HttpContext().get_RequestAborted()));
			return actionResult;
		}

		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(Dictionary<string, string>), 200)]
		[Route("apps/{app}/ui/settings/me")]
		public async Task<IActionResult> GetUserSettings(string app)
		{
			IActionResult actionResult = this.Ok(await this.appUISettings.GetAsync(base.get_AppId(), base.get_UserId(), base.get_HttpContext().get_RequestAborted()));
			return actionResult;
		}

		[ApiPermission(new string[] {  })]
		[HttpPut]
		[Route("apps/{app}/ui/settings/{key}")]
		public async Task<IActionResult> PutSetting(string app, string key, [FromBody] UpdateSettingDto request)
		{
			await this.appUISettings.SetAsync(base.get_AppId(), null, key, request.Value, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiPermission(new string[] {  })]
		[HttpPut]
		[Route("apps/{app}/ui/settings/me/{key}")]
		public async Task<IActionResult> PutUserSetting(string app, string key, [FromBody] UpdateSettingDto request)
		{
			await this.appUISettings.SetAsync(base.get_AppId(), base.get_UserId(), key, request.Value, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}
	}
}