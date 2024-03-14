using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Assets;
using Squidex.Domain.Apps.Entities.Backup;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Backups
{
	[ApiExplorerSettings(GroupName="Backups")]
	[Nullable(0)]
	[NullableContext(1)]
	public class BackupContentController : ApiController
	{
		private readonly IBackupArchiveStore backupArchiveStore;

		private readonly IBackupService backupservice;

		public BackupContentController(ICommandBus commandBus, IBackupArchiveStore backupArchiveStore, IBackupService backupservice) : base(commandBus)
		{
			this.backupArchiveStore = backupArchiveStore;
			this.backupservice = backupservice;
		}

		private async Task<IActionResult> GetBackupAsync(DomainId appId, string app, DomainId id)
		{
			IActionResult actionResult;
			BackupContentController.u003cu003ec__DisplayClass5_0 variable = null;
			IBackupJob backupAsync = await this.backupservice.GetBackupAsync(appId, id, base.get_HttpContext().get_RequestAborted());
			if (backupAsync == null || backupAsync.get_Status() != 2)
			{
				actionResult = this.NotFound();
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 2);
				defaultInterpolatedStringHandler.AppendLiteral("backup-");
				defaultInterpolatedStringHandler.AppendFormatted(app);
				defaultInterpolatedStringHandler.AppendLiteral("-");
				defaultInterpolatedStringHandler.AppendFormatted<Instant>(backupAsync.get_Started(), "yyyy-MM-dd_HH-mm-ss");
				defaultInterpolatedStringHandler.AppendLiteral(".zip");
				string stringAndClear = defaultInterpolatedStringHandler.ToStringAndClear();
				FileCallback fileCallback = new FileCallback(variable, (Stream body, BytesRange range, CancellationToken ct) => this.u003cu003e4__this.backupArchiveStore.DownloadAsync(this.id, body, ct));
				FileCallbackResult fileCallbackResult = new FileCallbackResult("application/zip", fileCallback);
				fileCallbackResult.set_FileDownloadName(stringAndClear);
				fileCallbackResult.set_FileSize(null);
				fileCallbackResult.set_ErrorAs404(true);
				actionResult = fileCallbackResult;
			}
			variable = null;
			return actionResult;
		}

		[AllowAnonymous]
		[ApiCosts(0)]
		[HttpGet]
		[ProducesResponseType(typeof(FileResult), 200)]
		[ResponseCache(Duration=0x278d00)]
		[Route("apps/{app}/backups/{id}")]
		public Task<IActionResult> GetBackupContent(string app, DomainId id)
		{
			return this.GetBackupAsync(base.get_AppId(), app, id);
		}

		[AllowAnonymous]
		[ApiCosts(0)]
		[HttpGet]
		[ProducesResponseType(typeof(FileResult), 200)]
		[ResponseCache(Duration=0x278d00)]
		[Route("apps/backups/{id}")]
		public Task<IActionResult> GetBackupContentV2(DomainId id, [FromQuery] DomainId appId = null, [FromQuery] string app = "")
		{
			return this.GetBackupAsync(appId, app, id);
		}
	}
}