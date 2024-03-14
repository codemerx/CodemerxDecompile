using NodaTime;
using Squidex.Areas.Api.Controllers.Backups;
using Squidex.Domain.Apps.Entities.Backup;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Web;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Backups.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class BackupJobDto : Resource
	{
		public int HandledAssets
		{
			get;
			set;
		}

		public int HandledEvents
		{
			get;
			set;
		}

		public DomainId Id
		{
			get;
			set;
		}

		public Instant Started
		{
			get;
			set;
		}

		public JobStatus Status
		{
			get;
			set;
		}

		public Instant? Stopped
		{
			get;
			set;
		}

		public BackupJobDto()
		{
		}

		private BackupJobDto CreateLinks(Resources resources)
		{
			if (resources.get_CanDeleteBackup())
			{
				var variable = new { app = resources.get_App(), id = this.Id };
				base.AddDeleteLink("delete", resources.Url<BackupsController>((BackupsController x) => "DeleteBackup", variable), null);
			}
			if (resources.get_CanDownloadBackup())
			{
				var variable1 = new { app = resources.get_App(), appId = resources.get_AppId(), id = this.Id };
				base.AddGetLink("download", resources.Url<BackupContentController>((BackupContentController x) => "GetBackupContentV2", variable1), null);
			}
			return this;
		}

		public static BackupJobDto FromDomain(IBackupJob backup, Resources resources)
		{
			return SimpleMapper.Map<IBackupJob, BackupJobDto>(backup, new BackupJobDto()).CreateLinks(resources);
		}
	}
}