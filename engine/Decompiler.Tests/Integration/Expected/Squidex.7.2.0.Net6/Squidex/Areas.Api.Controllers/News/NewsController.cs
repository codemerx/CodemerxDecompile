using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Areas.Api.Controllers.News.Models;
using Squidex.Areas.Api.Controllers.News.Service;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.News
{
	[ApiExplorerSettings(GroupName="News")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class NewsController : ApiController
	{
		private readonly FeaturesService featuresService;

		public NewsController(ICommandBus commandBus, FeaturesService featuresService) : base(commandBus)
		{
			this.featuresService = featuresService;
		}

		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(FeaturesDto), 200)]
		[Route("news/features/")]
		public async Task<IActionResult> GetNews([FromQuery] int version = 0)
		{
			IActionResult actionResult = this.Ok(await this.featuresService.GetFeaturesAsync(version, base.get_HttpContext().get_RequestAborted()));
			return actionResult;
		}
	}
}