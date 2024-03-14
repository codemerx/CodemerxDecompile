using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using NSwag.Annotations;
using Squidex.Areas.Api.Controllers;
using Squidex.Areas.Api.Controllers.Assets.Models;
using Squidex.Assets;
using Squidex.Domain.Apps.Core.Scripting;
using Squidex.Domain.Apps.Core.Tags;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Assets;
using Squidex.Domain.Apps.Entities.Assets.Commands;
using Squidex.Domain.Apps.Entities.Billing;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.Translations;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Assets
{
	[ApiExplorerSettings(GroupName="Assets")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AssetsController : ApiController
	{
		private readonly IUsageGate usageGate;

		private readonly IAssetQueryService assetQuery;

		private readonly IAssetUsageTracker assetUsageTracker;

		private readonly ITagService tagService;

		private readonly AssetTusRunner assetTusRunner;

		public AssetsController(ICommandBus commandBus, IUsageGate usageGate, IAssetQueryService assetQuery, IAssetUsageTracker assetUsageTracker, ITagService tagService, AssetTusRunner assetTusRunner) : base(commandBus)
		{
			this.usageGate = usageGate;
			this.assetQuery = assetQuery;
			this.assetUsageTracker = assetUsageTracker;
			this.assetTusRunner = assetTusRunner;
			this.tagService = tagService;
		}

		[ApiCosts(5)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.read" })]
		[HttpPost]
		[ProducesResponseType(typeof(BulkResultDto[]), 200)]
		[Route("apps/{app}/assets/bulk")]
		public async Task<IActionResult> BulkUpdateAssets(string app, [FromBody] BulkUpdateAssetsDto request)
		{
			BulkUpdateAssets command = request.ToCommand();
			BulkUpdateResult bulkUpdateResult = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted()).Result<BulkUpdateResult>();
			BulkResultDto[] array = (
				from x in bulkUpdateResult
				select BulkResultDto.FromDomain(x, base.get_HttpContext())).ToArray<BulkResultDto>();
			return this.Ok(array);
		}

		private async Task<AssetFile> CheckAssetFileAsync([Nullable(2)] IFormFile file)
		{
			if (file == null || base.get_Request().get_Form().get_Files().Count != 1)
			{
				throw new ValidationException(T.Get("validation.onlyOneFile", null), null);
			}
			Plan item1 = await this.usageGate.GetPlanForAppAsync(base.get_App(), base.get_HttpContext().get_RequestAborted()).Item1;
			long totalSizeByAppAsync = await this.assetUsageTracker.GetTotalSizeByAppAsync(base.get_AppId(), base.get_HttpContext().get_RequestAborted());
			if (item1.get_MaxAssetSize() > (long)0 && item1.get_MaxAssetSize() < totalSizeByAppAsync + file.get_Length())
			{
				ValidationError validationError = new ValidationError(T.Get("assets.maxSizeReached", null), Array.Empty<string>());
				throw new ValidationException(validationError, null);
			}
			AssetFile assetFile = Squidex.Web.FileExtensions.ToAssetFile(file);
			item1 = null;
			return assetFile;
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		private Q CreateQuery(string ids, string q)
		{
			Q q1 = Q.get_Empty().WithIds(ids).WithJsonQuery(q);
			QueryString queryString = base.get_Request().get_QueryString();
			return q1.WithODataQuery(queryString.ToString());
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.delete" })]
		[HttpDelete]
		[Route("apps/{app}/assets/{id}/")]
		public async Task<IActionResult> DeleteAsset(string app, DomainId id, DeleteAssetDto request)
		{
			DeleteAsset command = request.ToCommand(id);
			await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(AssetDto), 200)]
		[Route("apps/{app}/assets/{id}/")]
		public async Task<IActionResult> GetAsset(string app, DomainId id)
		{
			IActionResult actionResult;
			IEnrichedAssetEntity enrichedAssetEntity = await this.assetQuery.FindAsync(base.get_Context(), id, (long)-2, base.get_HttpContext().get_RequestAborted());
			IEnrichedAssetEntity enrichedAssetEntity1 = enrichedAssetEntity;
			if (enrichedAssetEntity1 != null)
			{
				Deferred deferred = Deferred.Response(() => AssetDto.FromDomain(enrichedAssetEntity1, base.get_Resources(), false));
				actionResult = this.Ok(deferred);
			}
			else
			{
				actionResult = this.NotFound();
			}
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(AssetsDto), 200)]
		[Route("apps/{app}/assets/")]
		public async Task<IActionResult> GetAssets(string app, [FromQuery] DomainId? parentId, [Nullable(2)][FromQuery] string ids = null, [Nullable(2)][FromQuery] string q = null)
		{
			IResultList<IEnrichedAssetEntity> resultList = await this.assetQuery.QueryAsync(base.get_Context(), parentId, this.CreateQuery(ids, q), base.get_HttpContext().get_RequestAborted());
			IResultList<IEnrichedAssetEntity> resultList1 = resultList;
			Deferred deferred = Deferred.Response(() => AssetsDto.FromDomain(resultList1, base.get_Resources()));
			IActionResult actionResult = this.Ok(deferred);
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.read" })]
		[HttpPost]
		[ProducesResponseType(typeof(AssetsDto), 200)]
		[Route("apps/{app}/assets/query")]
		public async Task<IActionResult> GetAssetsPost(string app, [FromBody] QueryDto query)
		{
			// 
			// Current member / type: System.Threading.Tasks.Task`1<Microsoft.AspNetCore.Mvc.IActionResult> Squidex.Areas.Api.Controllers.Assets.AssetsController::GetAssetsPost(System.String,Squidex.Areas.Api.Controllers.QueryDto)
			// Exception in: System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetAssetsPost(System.String,Squidex.Areas.Api.Controllers.QueryDto)
			// Object reference not set to an instance of an object.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] {  })]
		[HttpGet]
		[OpenApiIgnore]
		[Route("apps/{app}/assets/completion")]
		public IActionResult GetScriptCompletion(string app, string schema, [FromServices] ScriptingCompleter completer)
		{
			return this.Ok(completer.AssetScript());
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] {  })]
		[HttpGet]
		[OpenApiIgnore]
		[Route("apps/{app}/assets/completion/trigger")]
		public IActionResult GetScriptTriggerCompletion(string app, string schema, [FromServices] ScriptingCompleter completer)
		{
			return this.Ok(completer.AssetTrigger());
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(Dictionary<string, int>), 200)]
		[Route("apps/{app}/assets/tags")]
		public async Task<IActionResult> GetTags(string app)
		{
			TagsSet tagsAsync = await this.tagService.GetTagsAsync(base.get_AppId(), "Assets", base.get_HttpContext().get_RequestAborted());
			IHeaderDictionary headers = base.get_Response().get_Headers();
			headers.set_Item(HeaderNames.ETag, tagsAsync.get_Version().ToString(CultureInfo.InvariantCulture));
			return this.Ok(tagsAsync);
		}

		private async Task<AssetDto> InvokeCommandAsync(ICommand command)
		{
			AssetDto assetDto;
			CommandContext commandContext = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted());
			AssetDuplicate plainResult = commandContext.get_PlainResult() as AssetDuplicate;
			assetDto = (plainResult == null ? AssetDto.FromDomain(commandContext.Result<IEnrichedAssetEntity>(), base.get_Resources(), false) : AssetDto.FromDomain(plainResult.get_Asset(), base.get_Resources(), true));
			return assetDto;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.create" })]
		[AssetRequestSizeLimit]
		[HttpPost]
		[ProducesResponseType(typeof(AssetDto), 201)]
		[Route("apps/{app}/assets/")]
		public async Task<IActionResult> PostAsset(string app, CreateAssetDto request)
		{
			CreateAssetDto createAssetDto = request;
			AssetFile assetFile = await this.CheckAssetFileAsync(request.File);
			CreateAsset command = createAssetDto.ToCommand(assetFile);
			createAssetDto = null;
			AssetDto assetDto = await this.InvokeCommandAsync(command);
			IActionResult actionResult = this.CreatedAtAction("GetAsset", new { app = app, id = assetDto.Id }, assetDto);
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.create" })]
		[AssetRequestSizeLimit]
		[OpenApiIgnore]
		[ProducesResponseType(typeof(AssetDto), 201)]
		[Route("apps/{app}/assets/tus/{**fileId}")]
		public async Task<IActionResult> PostAssetTus(string app)
		{
			IActionResult actionResult;
			string str = UrlHelperExtensions.Action(base.get_Url(), null, new { app = app, fileId = null });
			ValueTuple<IActionResult, AssetTusFile> valueTuple = await this.assetTusRunner.InvokeAsync(base.get_HttpContext(), str);
			IActionResult item1 = valueTuple.Item1;
			AssetTusFile item2 = valueTuple.Item2;
			if (item2 == null)
			{
				actionResult = item1;
			}
			else
			{
				UpsertAsset command = UpsertAssetDto.ToCommand(item2);
				AssetDto assetDto = await this.InvokeCommandAsync(command);
				actionResult = this.CreatedAtAction("GetAsset", new { app = app, id = assetDto.Id }, assetDto);
			}
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.create" })]
		[AssetRequestSizeLimit]
		[HttpPost]
		[ProducesResponseType(typeof(AssetDto), 200)]
		[Route("apps/{app}/assets/{id}")]
		public async Task<IActionResult> PostUpsertAsset(string app, DomainId id, UpsertAssetDto request)
		{
			UpsertAssetDto upsertAssetDto = request;
			DomainId domainId = id;
			AssetFile assetFile = await this.CheckAssetFileAsync(request.File);
			UpsertAsset command = upsertAssetDto.ToCommand(domainId, assetFile);
			upsertAssetDto = null;
			domainId = new DomainId();
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.update" })]
		[AssetRequestSizeLimit]
		[HttpPut]
		[ProducesResponseType(typeof(AssetDto), 200)]
		[Route("apps/{app}/assets/{id}/")]
		public async Task<IActionResult> PutAsset(string app, DomainId id, [FromBody] AnnotateAssetDto request)
		{
			AnnotateAsset command = request.ToCommand(id);
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.upload" })]
		[AssetRequestSizeLimit]
		[HttpPut]
		[ProducesResponseType(typeof(AssetDto), 200)]
		[Route("apps/{app}/assets/{id}/content/")]
		public async Task<IActionResult> PutAssetContent(string app, DomainId id, IFormFile file)
		{
			UpdateAsset updateAsset = new UpdateAsset();
			UpdateAsset updateAsset1 = updateAsset;
			updateAsset1.set_File(await this.CheckAssetFileAsync(file));
			updateAsset.set_AssetId(id);
			UpdateAsset updateAsset2 = updateAsset;
			updateAsset1 = null;
			updateAsset = null;
			return this.Ok(await this.InvokeCommandAsync(updateAsset2));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.update" })]
		[AssetRequestSizeLimit]
		[HttpPut]
		[ProducesResponseType(typeof(AssetDto), 200)]
		[Route("apps/{app}/assets/{id}/parent")]
		public async Task<IActionResult> PutAssetParent(string app, DomainId id, [FromBody] MoveAssetDto request)
		{
			MoveAsset command = request.ToCommand(id);
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(Dictionary<string, int>), 200)]
		[Route("apps/{app}/assets/tags/{name}")]
		public async Task<IActionResult> PutTag(string app, string name, [FromBody] RenameTagDto request)
		{
			await this.tagService.RenameTagAsync(base.get_AppId(), "Assets", Uri.UnescapeDataString(name), request.TagName, base.get_HttpContext().get_RequestAborted());
			return await this.GetTags(app);
		}
	}
}