using Microsoft.AspNetCore.Mvc;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Ping
{
	[ApiExplorerSettings(GroupName="Ping")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class PingController : ApiController
	{
		private readonly ExposedValues exposedValues;

		public PingController(ICommandBus commandBus, ExposedValues exposedValues) : base(commandBus)
		{
			this.exposedValues = exposedValues;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.ping" })]
		[HttpGet]
		[Route("ping/{app}/")]
		public IActionResult GetAppPing(string app)
		{
			return this.NoContent();
		}

		[HttpGet]
		[ProducesResponseType(typeof(ExposedValues), 200)]
		[Route("info/")]
		public IActionResult GetInfo()
		{
			return this.Ok(this.exposedValues);
		}

		[HttpGet]
		[Route("ping/")]
		public IActionResult GetPing()
		{
			return this.NoContent();
		}
	}
}