using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Areas.Api.Controllers.Translations.Models;
using Squidex.Infrastructure.Commands;
using Squidex.Text.Translations;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Translations
{
	[ApiExplorerSettings(GroupName="Translations")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class TranslationsController : ApiController
	{
		private readonly ITranslator translator;

		public TranslationsController(ICommandBus commandBus, ITranslator translator) : base(commandBus)
		{
			this.translator = translator;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.translate" })]
		[HttpPost]
		[ProducesResponseType(typeof(TranslationDto), 200)]
		[Route("apps/{app}/translations/")]
		public async Task<IActionResult> PostTranslation(string app, [FromBody] TranslateDto request)
		{
			TranslationResult translationResult = await this.translator.TranslateAsync(request.Text, request.TargetLanguage, request.SourceLanguage, base.get_HttpContext().get_RequestAborted());
			return this.Ok(TranslationDto.FromDomain(translationResult));
		}
	}
}