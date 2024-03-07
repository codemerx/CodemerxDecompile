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
using System.Runtime.ExceptionServices;
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
			string str1 = null;
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
					if (assetEntity.get_Type() != 1 || !this.assetThumbnailGenerator.IsResizable(assetEntity.get_MimeType(), resizeOptions, ref str1))
					{
						nullable = new long?(assetEntity.get_FileSize());
						fileCallback = new FileCallback(variable, async (Stream body, BytesRange range, CancellationToken ct) => await this.u003cu003e4__this.DownloadAsync(this.asset, body, null, range, ct));
					}
					else
					{
						mimeType = str1;
						fileCallback = new FileCallback(variable, async (Stream body, BytesRange range, CancellationToken ct) => {
							string str = this.resizeOptions.ToString();
							if (!this.request.Force)
							{
								int num = 0;
								try
								{
									await this.u003cu003e4__this.DownloadAsync(this.asset, body, str, range, ct);
								}
								catch (AssetNotFoundException assetNotFoundException)
								{
									num = 1;
								}
								if (num == 1)
								{
									await this.u003cu003e4__this.ResizeAsync(this.asset, str, body, this.resizeOptions, false, ct);
								}
							}
							else
							{
								using (Activity activity = Telemetry.Activities.StartActivity("Resize", ActivityKind.Internal))
								{
									await this.u003cu003e4__this.ResizeAsync(this.asset, str, body, this.resizeOptions, true, ct);
								}
							}
							str = null;
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
			int num;
			CancellationToken cancellationToken;
			object obj;
			ValueTask valueTask;
			Exception exception;
			object obj1;
			TempAssetFile tempAssetFile;
			TempAssetFile tempAssetFile1;
			Stream stream;
			object obj2;
			int num1;
			int num2;
			Stream stream1;
			object obj3;
			int num3;
			using (Activity activity = Telemetry.Activities.StartActivity("Resize", ActivityKind.Internal))
			{
				tempAssetFile = new TempAssetFile(asset.get_FileName(), asset.get_MimeType(), (long)0);
				object obj4 = null;
				int num4 = 0;
				try
				{
					tempAssetFile1 = new TempAssetFile(asset.get_FileName(), asset.get_MimeType(), (long)0);
					object obj5 = null;
					int num5 = 0;
					try
					{
						using (Activity activity1 = Telemetry.Activities.StartActivity("Read", ActivityKind.Internal))
						{
							stream = tempAssetFile.OpenWrite();
							obj2 = null;
							num1 = 0;
							try
							{
								IAssetFileStore assetFileStore = this.assetFileStore;
								DomainId id = asset.get_AppId().get_Id();
								DomainId domainId = asset.get_Id();
								long fileVersion = asset.get_FileVersion();
								Stream stream2 = stream;
								BytesRange bytesRange = new BytesRange();
								cancellationToken = new CancellationToken();
								await assetFileStore.DownloadAsync(id, domainId, fileVersion, null, stream2, bytesRange, cancellationToken);
							}
							catch
							{
								obj = obj6;
								obj2 = obj;
							}
							if (stream != null)
							{
								valueTask = ((IAsyncDisposable)stream).DisposeAsync();
								await valueTask;
							}
							obj = obj2;
							if (obj != null)
							{
								exception = obj as Exception;
								if (exception == null)
								{
									throw obj;
								}
								ExceptionDispatchInfo.Capture(exception).Throw();
							}
							obj2 = null;
							stream = null;
						}
						activity1 = null;
						using (activity1 = Telemetry.Activities.StartActivity("Resize", ActivityKind.Internal))
						{
							num1 = 0;
							try
							{
								stream = tempAssetFile.OpenRead();
								obj2 = null;
								num2 = 0;
								try
								{
									stream1 = tempAssetFile1.OpenWrite();
									obj3 = null;
									num3 = 0;
									try
									{
										IAssetThumbnailGenerator assetThumbnailGenerator = this.assetThumbnailGenerator;
										Stream stream3 = stream;
										string mimeType = asset.get_MimeType();
										Stream stream4 = stream1;
										ResizeOptions resizeOption = resizeOptions;
										cancellationToken = new CancellationToken();
										await assetThumbnailGenerator.CreateThumbnailAsync(stream3, mimeType, stream4, resizeOption, cancellationToken);
									}
									catch
									{
										obj = obj7;
										obj3 = obj;
									}
									if (stream1 != null)
									{
										valueTask = ((IAsyncDisposable)stream1).DisposeAsync();
										await valueTask;
									}
									obj = obj3;
									if (obj != null)
									{
										exception = obj as Exception;
										if (exception == null)
										{
											throw obj;
										}
										ExceptionDispatchInfo.Capture(exception).Throw();
									}
									obj3 = null;
									stream1 = null;
								}
								catch
								{
									obj = obj8;
									obj2 = obj;
								}
								if (stream != null)
								{
									valueTask = ((IAsyncDisposable)stream).DisposeAsync();
									await valueTask;
								}
								obj = obj2;
								if (obj != null)
								{
									exception = obj as Exception;
									if (exception == null)
									{
										throw obj;
									}
									ExceptionDispatchInfo.Capture(exception).Throw();
								}
								obj2 = null;
								stream = null;
							}
							catch
							{
								obj = obj9;
								num1 = 1;
							}
							if (num1 == 1)
							{
								stream = tempAssetFile.OpenRead();
								obj2 = null;
								num2 = 0;
								try
								{
									stream1 = tempAssetFile1.OpenWrite();
									obj3 = null;
									num3 = 0;
									try
									{
										await stream.CopyToAsync(stream1);
									}
									catch
									{
										obj1 = obj10;
										obj3 = obj1;
									}
									if (stream1 != null)
									{
										valueTask = ((IAsyncDisposable)stream1).DisposeAsync();
										await valueTask;
									}
									obj1 = obj3;
									if (obj1 != null)
									{
										exception = obj1 as Exception;
										if (exception == null)
										{
											throw obj1;
										}
										ExceptionDispatchInfo.Capture(exception).Throw();
									}
									obj3 = null;
									stream1 = null;
								}
								catch
								{
									obj1 = obj11;
									obj2 = obj1;
								}
								if (stream != null)
								{
									valueTask = ((IAsyncDisposable)stream).DisposeAsync();
									await valueTask;
								}
								obj1 = obj2;
								if (obj1 != null)
								{
									exception = obj1 as Exception;
									if (exception == null)
									{
										throw obj1;
									}
									ExceptionDispatchInfo.Capture(exception).Throw();
								}
								obj2 = null;
								stream = null;
							}
						}
						activity1 = null;
						using (activity1 = Telemetry.Activities.StartActivity("Save", ActivityKind.Internal))
						{
							try
							{
								stream = tempAssetFile1.OpenRead();
								obj2 = null;
								num1 = 0;
								try
								{
									IAssetFileStore assetFileStore1 = this.assetFileStore;
									DomainId id1 = asset.get_AppId().get_Id();
									DomainId domainId1 = asset.get_Id();
									long fileVersion1 = asset.get_FileVersion();
									string str = suffix;
									Stream stream5 = stream;
									bool flag = overwrite;
									cancellationToken = new CancellationToken();
									await assetFileStore1.UploadAsync(id1, domainId1, fileVersion1, str, stream5, flag, cancellationToken);
								}
								catch
								{
									obj1 = obj12;
									obj2 = obj1;
								}
								if (stream != null)
								{
									valueTask = ((IAsyncDisposable)stream).DisposeAsync();
									await valueTask;
								}
								obj1 = obj2;
								if (obj1 != null)
								{
									exception = obj1 as Exception;
									if (exception == null)
									{
										throw obj1;
									}
									ExceptionDispatchInfo.Capture(exception).Throw();
								}
								obj2 = null;
								stream = null;
							}
							catch (AssetAlreadyExistsException assetAlreadyExistsException)
							{
								goto Label1;
							}
						}
						activity1 = null;
						using (activity1 = Telemetry.Activities.StartActivity("Write", ActivityKind.Internal))
						{
							stream = tempAssetFile1.OpenRead();
							obj2 = null;
							num1 = 0;
							try
							{
								await stream.CopyToAsync(target, ct);
							}
							catch
							{
								obj1 = obj13;
								obj2 = obj1;
							}
							if (stream != null)
							{
								valueTask = ((IAsyncDisposable)stream).DisposeAsync();
								await valueTask;
							}
							obj1 = obj2;
							if (obj1 != null)
							{
								exception = obj1 as Exception;
								if (exception == null)
								{
									throw obj1;
								}
								ExceptionDispatchInfo.Capture(exception).Throw();
							}
							obj2 = null;
							stream = null;
						}
						activity1 = null;
					Label1:
						num5 = 1;
					}
					catch
					{
						obj1 = obj14;
						obj5 = obj1;
					}
					if (tempAssetFile1 != null)
					{
						await tempAssetFile1.DisposeAsync();
					}
					obj1 = obj5;
					if (obj1 != null)
					{
						exception = obj1 as Exception;
						if (exception == null)
						{
							throw obj1;
						}
						ExceptionDispatchInfo.Capture(exception).Throw();
					}
					if (num5 == 1)
					{
						num4 = 1;
					}
					else
					{
						obj5 = null;
					}
				}
				catch
				{
					obj1 = obj15;
					obj4 = obj1;
				}
				if (tempAssetFile != null)
				{
					await tempAssetFile.DisposeAsync();
				}
				obj1 = obj4;
				if (obj1 != null)
				{
					exception = obj1 as Exception;
					if (exception == null)
					{
						throw obj1;
					}
					ExceptionDispatchInfo.Capture(exception).Throw();
				}
				if (num4 != 1)
				{
					obj4 = null;
				}
				else
				{
					activity = null;
					tempAssetFile = null;
					tempAssetFile1 = null;
					return;
				}
			}
			activity = null;
			tempAssetFile = null;
			tempAssetFile1 = null;
			activity = null;
			tempAssetFile = null;
			tempAssetFile1 = null;
		}
	}
}