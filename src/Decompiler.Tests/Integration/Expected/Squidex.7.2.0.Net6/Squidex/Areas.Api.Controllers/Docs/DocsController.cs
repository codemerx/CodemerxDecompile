using Microsoft.AspNetCore.Mvc;
using Squidex.Areas.Api.Controllers;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Docs
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class DocsController : ApiController
	{
		public DocsController(ICommandBus commandBus) : base(commandBus)
		{
		}

		[HttpGet]
		[Route("docs/")]
		public IActionResult Docs()
		{
			return this.View("Docs", new DocsVM()
			{
				Specification = "~/api/swagger/v1/swagger.json"
			});
		}
	}
}