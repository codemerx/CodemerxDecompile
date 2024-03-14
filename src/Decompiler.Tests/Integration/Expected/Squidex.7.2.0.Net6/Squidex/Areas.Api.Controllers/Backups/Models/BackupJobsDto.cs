using Squidex.Areas.Api.Controllers.Backups;
using Squidex.Domain.Apps.Entities.Backup;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Backups.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class BackupJobsDto : Resource
	{
		[LocalizedRequired]
		public BackupJobDto[] Items
		{
			get;
			set;
		}

		public BackupJobsDto()
		{
		}

		private BackupJobsDto CreateLinks(Resources resources)
		{
			var variable = new { app = resources.get_App() };
			base.AddSelfLink(resources.Url<BackupsController>((BackupsController x) => "GetBackups", variable));
			if (resources.get_CanCreateBackup())
			{
				base.AddPostLink("create", resources.Url<BackupsController>((BackupsController x) => "PostBackup", variable), null);
			}
			return this;
		}

		public static BackupJobsDto FromDomain(IEnumerable<IBackupJob> backups, Resources resources)
		{
			return (new BackupJobsDto()
			{
				Items = (
					from x in backups
					select BackupJobDto.FromDomain(x, resources)).ToArray<BackupJobDto>()
			}).CreateLinks(resources);
		}
	}
}