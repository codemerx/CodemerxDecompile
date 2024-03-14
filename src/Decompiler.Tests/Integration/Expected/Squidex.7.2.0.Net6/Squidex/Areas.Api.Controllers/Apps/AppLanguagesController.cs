using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Squidex.Areas.Api.Controllers.Apps.Models;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Apps
{
	[ApiExplorerSettings(GroupName="Apps")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AppLanguagesController : ApiController
	{
		public AppLanguagesController(ICommandBus commandBus) : base(commandBus)
		{
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.languages.delete" })]
		[HttpDelete]
		[ProducesResponseType(typeof(AppLanguagesDto), 200)]
		[Route("apps/{app}/languages/{language}/")]
		public async Task<IActionResult> DeleteLanguage(string app, string language)
		{
			RemoveLanguage removeLanguage = new RemoveLanguage();
			removeLanguage.set_Language(AppLanguagesController.ParseLanguage(language));
			return this.Ok(await this.InvokeCommandAsync(removeLanguage));
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.languages.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(AppLanguagesDto), 200)]
		[Route("apps/{app}/languages/")]
		public IActionResult GetLanguages(string app)
		{
			Deferred deferred = Deferred.Response(() => this.GetResponse(base.get_App()));
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, ETagExtensions.ToEtag<IAppEntity>(base.get_App()));
			return this.Ok(deferred);
		}

		private AppLanguagesDto GetResponse(IAppEntity result)
		{
			return AppLanguagesDto.FromDomain(result, base.get_Resources());
		}

		private async Task<AppLanguagesDto> InvokeCommandAsync(ICommand command)
		{
			CommandContext commandContext = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted());
			return this.GetResponse(commandContext.Result<IAppEntity>());
		}

		private static Language ParseLanguage(string language)
		{
			Language language1;
			try
			{
				language1 = Language.GetLanguage(language);
			}
			catch (NotSupportedException notSupportedException)
			{
				throw new DomainObjectNotFoundException(language, null);
			}
			return language1;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.languages.create" })]
		[HttpPost]
		[ProducesResponseType(typeof(AppLanguagesDto), 201)]
		[Route("apps/{app}/languages/")]
		public async Task<IActionResult> PostLanguage(string app, [FromBody] AddLanguageDto request)
		{
			AppLanguagesDto appLanguagesDto = await this.InvokeCommandAsync(request.ToCommand());
			IActionResult actionResult = this.CreatedAtAction("GetLanguages", new { app = app }, appLanguagesDto);
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.languages.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(AppLanguagesDto), 200)]
		[Route("apps/{app}/languages/{language}/")]
		public async Task<IActionResult> PutLanguage(string app, string language, [FromBody] UpdateLanguageDto request)
		{
			UpdateLanguage command = request.ToCommand(AppLanguagesController.ParseLanguage(language));
			return this.Ok(await this.InvokeCommandAsync(command));
		}
	}
}