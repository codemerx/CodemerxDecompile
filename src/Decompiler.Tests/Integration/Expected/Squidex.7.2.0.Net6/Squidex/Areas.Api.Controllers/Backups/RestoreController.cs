using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Areas.Api.Controllers.Backups.Models;
using Squidex.Domain.Apps.Entities.Backup;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.Security;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Backups
{
	[ApiExplorerSettings(GroupName="Backups")]
	[ApiModelValidation(true)]
	[Nullable(0)]
	[NullableContext(1)]
	public class RestoreController : ApiController
	{
		private readonly IBackupService backupService;

		public RestoreController(ICommandBus commandBus, IBackupService backupService) : base(commandBus)
		{
			this.backupService = backupService;
		}

		[ApiPermission(new string[] { "squidex.admin.restore" })]
		[HttpGet]
		[ProducesResponseType(typeof(RestoreJobDto), 200)]
		[Route("apps/restore/")]
		public async Task<IActionResult> GetRestoreJob()
		{
			IActionResult actionResult;
			IRestoreJob restoreAsync = await this.backupService.GetRestoreAsync(base.get_HttpContext().get_RequestAborted());
			if (restoreAsync != null)
			{
				actionResult = this.Ok(RestoreJobDto.FromDomain(restoreAsync));
			}
			else
			{
				actionResult = this.NotFound();
			}
			return actionResult;
		}

		[ApiPermission(new string[] { "squidex.admin.restore" })]
		[HttpPost]
		[Route("apps/restore/")]
		public async Task<IActionResult> PostRestoreJob([FromBody] RestoreRequestDto request)
		{
			await this.backupService.StartRestoreAsync(Squidex.Infrastructure.Security.Extensions.Token(base.get_User()), request.Url, request.Name, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}
	}
}