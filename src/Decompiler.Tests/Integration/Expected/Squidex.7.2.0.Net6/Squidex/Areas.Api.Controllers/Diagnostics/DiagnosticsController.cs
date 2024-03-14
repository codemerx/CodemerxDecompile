using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.Diagnostics;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Diagnostics
{
	[ApiExplorerSettings(GroupName="Diagnostics")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class DiagnosticsController : ApiController
	{
		private readonly Diagnoser dumper;

		public DiagnosticsController(ICommandBus commandBus, Diagnoser dumper) : base(commandBus)
		{
			this.dumper = dumper;
		}

		[ApiPermissionOrAnonymous(new string[] { "squidex.admin.*" })]
		[HttpGet]
		[Route("diagnostics/dump")]
		public async Task<IActionResult> GetDump()
		{
			IActionResult actionResult;
			if (await this.dumper.CreateDumpAsync(base.get_HttpContext().get_RequestAborted()))
			{
				actionResult = this.NoContent();
			}
			else
			{
				actionResult = this.StatusCode(0x1f5);
			}
			return actionResult;
		}

		[ApiPermissionOrAnonymous(new string[] { "squidex.admin.*" })]
		[HttpGet]
		[Route("diagnostics/gcdump")]
		public async Task<IActionResult> GetGCDump()
		{
			IActionResult actionResult;
			if (await this.dumper.CreateGCDumpAsync(base.get_HttpContext().get_RequestAborted()))
			{
				actionResult = this.NoContent();
			}
			else
			{
				actionResult = this.StatusCode(0x1f5);
			}
			return actionResult;
		}
	}
}