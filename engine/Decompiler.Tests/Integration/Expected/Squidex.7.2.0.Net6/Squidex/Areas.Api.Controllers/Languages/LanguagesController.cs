using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Squidex.Areas.Api.Controllers;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Languages
{
	[ApiExplorerSettings(GroupName="Languages")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class LanguagesController : ApiController
	{
		public LanguagesController(ICommandBus commandBus) : base(commandBus)
		{
		}

		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(LanguageDto[]), 200)]
		[Route("languages/")]
		public IActionResult GetLanguages()
		{
			Deferred deferred = Deferred.Response(() => Language.get_AllLanguages().Select<Language, LanguageDto>(new Func<Language, LanguageDto>(LanguageDto.FromDomain)).ToArray<LanguageDto>());
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, "1");
			return this.Ok(deferred);
		}
	}
}