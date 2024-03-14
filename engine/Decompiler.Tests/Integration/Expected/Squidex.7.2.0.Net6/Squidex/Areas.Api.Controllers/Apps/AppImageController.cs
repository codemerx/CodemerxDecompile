using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Squidex.Assets;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Apps
{
	[ApiExplorerSettings(GroupName="Apps")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AppImageController : ApiController
	{
		private readonly IAppImageStore appImageStore;

		private readonly IAssetStore assetStore;

		private readonly IAssetThumbnailGenerator assetThumbnailGenerator;

		public AppImageController(ICommandBus commandBus, IAppImageStore appImageStore, IAssetStore assetStore, IAssetThumbnailGenerator assetThumbnailGenerator) : base(commandBus)
		{
			this.appImageStore = appImageStore;
			this.assetStore = assetStore;
			this.assetThumbnailGenerator = assetThumbnailGenerator;
		}

		[AllowAnonymous]
		[ApiCosts(0)]
		[HttpGet]
		[ProducesResponseType(typeof(FileResult), 200)]
		[Route("apps/{app}/image")]
		public IActionResult GetImage(string app)
		{
			AppImageController.u003cu003ec__DisplayClass4_0 variable = null;
			if (base.get_App().get_Image() == null)
			{
				return this.NotFound();
			}
			string etag = base.get_App().get_Image().get_Etag();
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, etag);
			FileCallback fileCallback = new FileCallback(variable, async (Stream body, BytesRange range, CancellationToken ct) => {
				AppImageController.u003cu003ec__DisplayClass4_0.u003cu003cGetImageu003eb__0u003ed _ = new AppImageController.u003cu003ec__DisplayClass4_0.u003cu003cGetImageu003eb__0u003ed();
				_.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
				_.u003cu003e4__this = this;
				_.body = body;
				_.ct = ct;
				_.u003cu003e1__state = -1;
				_.u003cu003et__builder.Start<AppImageController.u003cu003ec__DisplayClass4_0.u003cu003cGetImageu003eb__0u003ed>(ref _);
				return _.u003cu003et__builder.Task;
			});
			FileCallbackResult fileCallbackResult = new FileCallbackResult(base.get_App().get_Image().get_MimeType(), fileCallback);
			fileCallbackResult.set_ErrorAs404(true);
			return fileCallbackResult;
		}

		private async Task ResizeAsync(string resizedAsset, string mimeType, Stream target, CancellationToken ct)
		{
			// 
			// Current member / type: System.Threading.Tasks.Task Squidex.Areas.Api.Controllers.Apps.AppImageController::ResizeAsync(System.String,System.String,System.IO.Stream,System.Threading.CancellationToken)
			// Exception in: System.Threading.Tasks.Task ResizeAsync(System.String,System.String,System.IO.Stream,System.Threading.CancellationToken)
			// GoTo misplaced.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}
	}
}