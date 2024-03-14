using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using NodaTime;
using Squidex.Areas.Api.Controllers.Assets.Models;
using Squidex.Assets;
using Squidex.Domain.Apps.Core.ValidateContent;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Assets;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Assets
{
	[ApiExplorerSettings(GroupName="Assets")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AssetContentController : ApiController
	{
		private readonly IAssetFileStore assetFileStore;

		private readonly IAssetQueryService assetQuery;

		private readonly IAssetLoader assetLoader;

		private readonly IAssetThumbnailGenerator assetThumbnailGenerator;

		public AssetContentController(ICommandBus commandBus, IAssetFileStore assetFileStore, IAssetQueryService assetQuery, IAssetLoader assetLoader, IAssetThumbnailGenerator assetThumbnailGenerator) : base(commandBus)
		{
			this.assetFileStore = assetFileStore;
			this.assetQuery = assetQuery;
			this.assetLoader = assetLoader;
			this.assetThumbnailGenerator = assetThumbnailGenerator;
		}

		private async Task<IActionResult> DeliverAssetAsync(Context context, [Nullable(2)] IAssetEntity asset, AssetContentQueryDto request)
		{
			IActionResult actionResult;
			string str = null;
			AssetContentController.u003cu003ec__DisplayClass7_0 variable = null;
			AssetContentQueryDto assetContentQueryDto = request;
			IAssetEntity assetEntity = asset;
			if (assetContentQueryDto == null)
			{
				assetContentQueryDto = new AssetContentQueryDto();
			}
			if (assetEntity == null)
			{
				actionResult = this.NotFound();
			}
			else if (!assetEntity.get_IsProtected() || base.get_Resources().get_CanReadAssets())
			{
				if (assetEntity != null && assetContentQueryDto.Version > (long)-2 && assetEntity.get_Version() != assetContentQueryDto.Version)
				{
					if (context.get_App() == null)
					{
						IAssetEntity async = await this.assetLoader.GetAsync(assetEntity.get_AppId().get_Id(), assetEntity.get_Id(), assetContentQueryDto.Version, base.get_HttpContext().get_RequestAborted());
						assetEntity = async;
					}
					else
					{
						IEnrichedAssetEntity enrichedAssetEntity = await this.assetQuery.FindAsync(context, assetEntity.get_Id(), assetContentQueryDto.Version, base.get_HttpContext().get_RequestAborted());
						assetEntity = enrichedAssetEntity;
					}
				}
				if (assetEntity != null)
				{
					IHeaderDictionary headers = base.get_Response().get_Headers();
					string eTag = HeaderNames.ETag;
					long fileVersion = assetEntity.get_FileVersion();
					headers.set_Item(eTag, fileVersion.ToString(CultureInfo.InvariantCulture));
					if (assetContentQueryDto.CacheDuration > (long)0)
					{
						IHeaderDictionary headerDictionary = base.get_Response().get_Headers();
						string cacheControl = HeaderNames.CacheControl;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 1);
						defaultInterpolatedStringHandler.AppendLiteral("public,max-age=");
						defaultInterpolatedStringHandler.AppendFormatted<long>(assetContentQueryDto.CacheDuration);
						headerDictionary.set_Item(cacheControl, defaultInterpolatedStringHandler.ToStringAndClear());
					}
					ResizeOptions resizeOptions = assetContentQueryDto.ToResizeOptions(assetEntity, this.assetThumbnailGenerator, base.get_HttpContext().get_Request());
					long? nullable = null;
					FileCallback fileCallback = null;
					string mimeType = assetEntity.get_MimeType();
					if (assetEntity.get_Type() != 1 || !this.assetThumbnailGenerator.IsResizable(assetEntity.get_MimeType(), resizeOptions, ref str))
					{
						nullable = new long?(assetEntity.get_FileSize());
						fileCallback = new FileCallback(variable, async (Stream body, BytesRange range, CancellationToken ct) => await this.u003cu003e4__this.DownloadAsync(this.asset, body, null, range, ct));
					}
					else
					{
						mimeType = str;
						fileCallback = new FileCallback(variable, async (Stream body, BytesRange range, CancellationToken ct) => {
							AssetContentController.u003cu003ec__DisplayClass7_0.u003cu003cDeliverAssetAsyncu003eb__0u003ed _ = new AssetContentController.u003cu003ec__DisplayClass7_0.u003cu003cDeliverAssetAsyncu003eb__0u003ed();
							_.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
							_.u003cu003e4__this = this;
							_.body = body;
							_.range = range;
							_.ct = ct;
							_.u003cu003e1__state = -1;
							_.u003cu003et__builder.Start<AssetContentController.u003cu003ec__DisplayClass7_0.u003cu003cDeliverAssetAsyncu003eb__0u003ed>(ref _);
							return _.u003cu003et__builder.Task;
						});
					}
					FileCallbackResult fileCallbackResult = new FileCallbackResult(mimeType, fileCallback);
					long? nullable1 = nullable;
					fileVersion = (long)0;
					fileCallbackResult.set_EnableRangeProcessing(nullable1.GetValueOrDefault() > fileVersion & nullable1.HasValue);
					fileCallbackResult.set_ErrorAs404(true);
					fileCallbackResult.set_FileDownloadName(assetEntity.get_FileName());
					fileCallbackResult.set_FileSize(nullable);
					Instant lastModified = assetEntity.get_LastModified();
					fileCallbackResult.set_LastModified(new DateTimeOffset?(lastModified.ToDateTimeOffset()));
					fileCallbackResult.set_SendInline(assetContentQueryDto.Download != 1);
					actionResult = fileCallbackResult;
				}
				else
				{
					actionResult = this.NotFound();
				}
			}
			else
			{
				base.get_Response().get_Headers().set_Item(HeaderNames.CacheControl, "public,max-age=0");
				actionResult = this.StatusCode(0x193);
			}
			variable = null;
			return actionResult;
		}

		private async Task DownloadAsync(IAssetEntity asset, Stream bodyStream, [Nullable(2)] string suffix, BytesRange range, CancellationToken ct)
		{
			await this.assetFileStore.DownloadAsync(asset.get_AppId().get_Id(), asset.get_Id(), asset.get_FileVersion(), suffix, bodyStream, range, ct);
		}

		[AllowAnonymous]
		[ApiCosts(0.5)]
		[ApiPermission(new string[] {  })]
		[HttpGet]
		[Obsolete("Use overload with app name")]
		[ProducesResponseType(typeof(FileResult), 200)]
		[Route("assets/{id}/")]
		public async Task<IActionResult> GetAssetContent(DomainId id, AssetContentQueryDto request)
		{
			Context context = base.get_Context();
			Context context1 = context.Clone((ICloneBuilder b) => AssetExtensions.WithoutAssetEnrichment(b, true));
			IEnrichedAssetEntity enrichedAssetEntity = await this.assetQuery.FindGlobalAsync(context1, id, base.get_HttpContext().get_RequestAborted());
			IActionResult actionResult = await this.DeliverAssetAsync(context1, enrichedAssetEntity, request);
			context1 = null;
			return actionResult;
		}

		[AllowAnonymous]
		[ApiCosts(0.5)]
		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(FileResult), 200)]
		[Route("assets/{app}/{idOrSlug}/{*more}")]
		public async Task<IActionResult> GetAssetContentBySlug(string app, string idOrSlug, AssetContentQueryDto request, [Nullable(2)] string more = null)
		{
			Context context = base.get_Context();
			Context context1 = context.Clone((ICloneBuilder b) => AssetExtensions.WithoutAssetEnrichment(b, true));
			IEnrichedAssetEntity enrichedAssetEntity = await this.assetQuery.FindAsync(context1, DomainId.Create(idOrSlug), (long)-2, base.get_HttpContext().get_RequestAborted()) ?? await this.assetQuery.FindBySlugAsync(context1, idOrSlug, base.get_HttpContext().get_RequestAborted());
			IActionResult actionResult = await this.DeliverAssetAsync(context1, enrichedAssetEntity, request);
			context1 = null;
			return actionResult;
		}

		private async Task ResizeAsync(IAssetEntity asset, string suffix, Stream target, ResizeOptions resizeOptions, bool overwrite, CancellationToken ct)
		{
			// 
			// Current member / type: System.Threading.Tasks.Task Squidex.Areas.Api.Controllers.Assets.AssetContentController::ResizeAsync(Squidex.Domain.Apps.Entities.Assets.IAssetEntity,System.String,System.IO.Stream,Squidex.Assets.ResizeOptions,System.Boolean,System.Threading.CancellationToken)
			// Exception in: System.Threading.Tasks.Task ResizeAsync(Squidex.Domain.Apps.Entities.Assets.IAssetEntity,System.String,System.IO.Stream,Squidex.Assets.ResizeOptions,System.Boolean,System.Threading.CancellationToken)
			// GoTo misplaced.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}
	}
}