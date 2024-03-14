using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Areas.Api.Controllers.Backups.Models;
using Squidex.Domain.Apps.Entities.Backup;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.Security;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Backups
{
	[ApiExplorerSettings(GroupName="Backups")]
	[Nullable(0)]
	[NullableContext(1)]
	public class BackupsController : ApiController
	{
		private readonly IBackupService backupService;

		public BackupsController(ICommandBus commandBus, IBackupService backupService) : base(commandBus)
		{
			this.backupService = backupService;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.backups.delete" })]
		[HttpDelete]
		[ProducesResponseType(typeof(BackupJobDto[]), 200)]
		[Route("apps/{app}/backups/{id}")]
		public async Task<IActionResult> DeleteBackup(string app, DomainId id)
		{
			await this.backupService.DeleteBackupAsync(base.get_AppId(), id, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.backups.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(BackupJobsDto), 200)]
		[Route("apps/{app}/backups/")]
		public async Task<IActionResult> GetBackups(string app)
		{
			List<IBackupJob> backupsAsync = await this.backupService.GetBackupsAsync(base.get_AppId(), base.get_HttpContext().get_RequestAborted());
			BackupJobsDto backupJobsDto = BackupJobsDto.FromDomain(backupsAsync, base.get_Resources());
			return this.Ok(backupJobsDto);
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.backups.create" })]
		[HttpPost]
		[ProducesResponseType(typeof(BackupJobDto[]), 200)]
		[Route("apps/{app}/backups/")]
		public async Task<IActionResult> PostBackup(string app)
		{
			await this.backupService.StartBackupAsync(base.get_App().get_Id(), Squidex.Infrastructure.Security.Extensions.Token(base.get_User()), base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}
	}
}