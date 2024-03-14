using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Areas.Api.Controllers.Statistics.Models;
using Squidex.Assets;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Domain.Apps.Entities.Assets;
using Squidex.Domain.Apps.Entities.Billing;
using Squidex.Hosting;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.UsageTracking;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Statistics
{
	[ApiExplorerSettings(GroupName="Statistics")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class UsagesController : ApiController
	{
		private readonly IApiUsageTracker usageTracker;

		private readonly IAppLogStore usageLog;

		private readonly IUsageGate usageGate;

		private readonly IAssetUsageTracker assetStatsRepository;

		private readonly IDataProtector dataProtector;

		private readonly IUrlGenerator urlGenerator;

		public UsagesController(ICommandBus commandBus, IDataProtectionProvider dataProtection, IApiUsageTracker usageTracker, IAppLogStore usageLog, IUsageGate usageGate, IAssetUsageTracker assetStatsRepository, IUrlGenerator urlGenerator) : base(commandBus)
		{
			this.usageLog = usageLog;
			this.assetStatsRepository = assetStatsRepository;
			this.urlGenerator = urlGenerator;
			this.usageGate = usageGate;
			this.usageTracker = usageTracker;
			this.dataProtector = dataProtection.CreateProtector("LogToken");
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.usage" })]
		[HttpGet]
		[ProducesResponseType(typeof(CurrentStorageDto), 200)]
		[Route("apps/{app}/usages/storage/today/")]
		public async Task<IActionResult> GetCurrentStorageSize(string app)
		{
			long totalSizeByAppAsync = await this.assetStatsRepository.GetTotalSizeByAppAsync(base.get_AppId(), base.get_HttpContext().get_RequestAborted());
			Plan item1 = await this.usageGate.GetPlanForAppAsync(base.get_App(), base.get_HttpContext().get_RequestAborted()).Item1;
			CurrentStorageDto currentStorageDto = new CurrentStorageDto()
			{
				Size = totalSizeByAppAsync,
				MaxAllowed = item1.get_MaxAssetSize()
			};
			return this.Ok(currentStorageDto);
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.usage" })]
		[HttpGet]
		[ProducesResponseType(typeof(LogDownloadDto), 200)]
		[Route("apps/{app}/usages/log/")]
		public IActionResult GetLog(string app)
		{
			IDataProtector dataProtector = this.dataProtector;
			DomainId id = base.get_App().get_Id();
			string str = DataProtectionCommonExtensions.Protect(dataProtector, id.ToString());
			string str1 = this.urlGenerator.BuildUrl(string.Concat("/api/apps/log/", str, "/"), true);
			return this.Ok(new LogDownloadDto()
			{
				DownloadUrl = str1
			});
		}

		[ApiExplorerSettings(IgnoreApi=true)]
		[HttpGet]
		[Route("apps/log/{token}/")]
		public IActionResult GetLogFile(string token)
		{
			UsagesController.u003cu003ec__DisplayClass8_0 variable = null;
			DomainId.Create(DataProtectionCommonExtensions.Unprotect(this.dataProtector, token));
			DateTime date = DateTime.UtcNow.Date;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Usage-");
			defaultInterpolatedStringHandler.AppendFormatted<DateTime>(date, "yyy-MM-dd");
			defaultInterpolatedStringHandler.AppendLiteral(".csv");
			string stringAndClear = defaultInterpolatedStringHandler.ToStringAndClear();
			FileCallbackResult fileCallbackResult = new FileCallbackResult("text/csv", new FileCallback(variable, (Stream body, BytesRange range, CancellationToken ct) => this.u003cu003e4__this.usageLog.ReadLogAsync(this.appId, this.fileDate.AddDays(-30), this.fileDate, body, ct)));
			fileCallbackResult.set_FileDownloadName(stringAndClear);
			return fileCallbackResult;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.usage" })]
		[HttpGet]
		[ProducesResponseType(typeof(StorageUsagePerDateDto[]), 200)]
		[Route("apps/{app}/usages/storage/{fromDate}/{toDate}/")]
		public async Task<IActionResult> GetStorageSizes(string app, DateTime fromDate, DateTime toDate)
		{
			IActionResult actionResult;
			if (!(fromDate > toDate) || (toDate - fromDate).TotalDays <= 100)
			{
				IReadOnlyList<AssetStats> assetStats = await this.assetStatsRepository.QueryByAppAsync(base.get_AppId(), fromDate.Date, toDate.Date, base.get_HttpContext().get_RequestAborted());
				StorageUsagePerDateDto[] array = assetStats.Select<AssetStats, StorageUsagePerDateDto>(new Func<AssetStats, StorageUsagePerDateDto>(StorageUsagePerDateDto.FromDomain)).ToArray<StorageUsagePerDateDto>();
				actionResult = this.Ok(array);
			}
			else
			{
				actionResult = this.BadRequest();
			}
			return actionResult;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.teams.{team}.usage" })]
		[HttpGet]
		[ProducesResponseType(typeof(StorageUsagePerDateDto[]), 200)]
		[Route("teams/{team}/usages/storage/{fromDate}/{toDate}/")]
		public async Task<IActionResult> GetStorageSizesForTeam(string team, DateTime fromDate, DateTime toDate)
		{
			IActionResult actionResult;
			if (!(fromDate > toDate) || (toDate - fromDate).TotalDays <= 100)
			{
				IReadOnlyList<AssetStats> assetStats = await this.assetStatsRepository.QueryByTeamAsync(base.get_TeamId(), fromDate.Date, toDate.Date, base.get_HttpContext().get_RequestAborted());
				StorageUsagePerDateDto[] array = assetStats.Select<AssetStats, StorageUsagePerDateDto>(new Func<AssetStats, StorageUsagePerDateDto>(StorageUsagePerDateDto.FromDomain)).ToArray<StorageUsagePerDateDto>();
				actionResult = this.Ok(array);
			}
			else
			{
				actionResult = this.BadRequest();
			}
			return actionResult;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.teams.{team}.usage" })]
		[HttpGet]
		[ProducesResponseType(typeof(CurrentStorageDto), 200)]
		[Route("teams/{team}/usages/storage/today/")]
		public async Task<IActionResult> GetTeamCurrentStorageSizeForTeam(string team)
		{
			long totalSizeByTeamAsync = await this.assetStatsRepository.GetTotalSizeByTeamAsync(base.get_TeamId(), base.get_HttpContext().get_RequestAborted());
			Plan item1 = await this.usageGate.GetPlanForTeamAsync(base.get_Team(), base.get_HttpContext().get_RequestAborted()).Item1;
			CurrentStorageDto currentStorageDto = new CurrentStorageDto()
			{
				Size = totalSizeByTeamAsync,
				MaxAllowed = item1.get_MaxAssetSize()
			};
			return this.Ok(currentStorageDto);
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.usage" })]
		[HttpGet]
		[ProducesResponseType(typeof(CallsUsageDtoDto), 200)]
		[Route("apps/{app}/usages/calls/{fromDate}/{toDate}/")]
		public async Task<IActionResult> GetUsages(string app, DateTime fromDate, DateTime toDate)
		{
			IActionResult actionResult;
			ApiStatsSummary item1;
			Dictionary<string, List<ApiStats>> item2;
			if (!(fromDate > toDate) || (toDate - fromDate).TotalDays <= 100)
			{
				IApiUsageTracker apiUsageTracker = this.usageTracker;
				DomainId appId = base.get_AppId();
				ValueTuple<ApiStatsSummary, Dictionary<string, List<ApiStats>>> valueTuple = await apiUsageTracker.QueryAsync(appId.ToString(), fromDate.Date, toDate.Date, base.get_HttpContext().get_RequestAborted());
				item1 = valueTuple.Item1;
				item2 = valueTuple.Item2;
				Plan plan = await this.usageGate.GetPlanForAppAsync(base.get_App(), base.get_HttpContext().get_RequestAborted()).Item1;
				CallsUsageDtoDto callsUsageDtoDto = CallsUsageDtoDto.FromDomain(plan, item1, item2);
				actionResult = this.Ok(callsUsageDtoDto);
			}
			else
			{
				actionResult = this.BadRequest();
			}
			item1 = null;
			item2 = null;
			return actionResult;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.teams.{team}.usage" })]
		[HttpGet]
		[ProducesResponseType(typeof(CallsUsageDtoDto), 200)]
		[Route("teams/{team}/usages/calls/{fromDate}/{toDate}/")]
		public async Task<IActionResult> GetUsagesForTeam(string team, DateTime fromDate, DateTime toDate)
		{
			IActionResult actionResult;
			ApiStatsSummary item1;
			Dictionary<string, List<ApiStats>> item2;
			if (!(fromDate > toDate) || (toDate - fromDate).TotalDays <= 100)
			{
				IApiUsageTracker apiUsageTracker = this.usageTracker;
				DomainId teamId = base.get_TeamId();
				ValueTuple<ApiStatsSummary, Dictionary<string, List<ApiStats>>> valueTuple = await apiUsageTracker.QueryAsync(teamId.ToString(), fromDate.Date, toDate.Date, base.get_HttpContext().get_RequestAborted());
				item1 = valueTuple.Item1;
				item2 = valueTuple.Item2;
				Plan plan = await this.usageGate.GetPlanForTeamAsync(base.get_Team(), base.get_HttpContext().get_RequestAborted()).Item1;
				CallsUsageDtoDto callsUsageDtoDto = CallsUsageDtoDto.FromDomain(plan, item1, item2);
				actionResult = this.Ok(callsUsageDtoDto);
			}
			else
			{
				actionResult = this.BadRequest();
			}
			item1 = null;
			item2 = null;
			return actionResult;
		}
	}
}