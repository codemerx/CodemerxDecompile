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
using System.Runtime.ExceptionServices;
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
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 2);
				defaultInterpolatedStringHandler.AppendFormatted<DomainId>(this.u003cu003e4__this.get_App().get_Id());
				defaultInterpolatedStringHandler.AppendLiteral("_");
				defaultInterpolatedStringHandler.AppendFormatted(this.etag);
				defaultInterpolatedStringHandler.AppendLiteral("_Resized");
				string stringAndClear = defaultInterpolatedStringHandler.ToStringAndClear();
				int num = 0;
				try
				{
					await this.u003cu003e4__this.assetStore.DownloadAsync(stringAndClear, body, new BytesRange(), ct);
				}
				catch (AssetNotFoundException assetNotFoundException)
				{
					num = 1;
				}
				if (num == 1)
				{
					await this.u003cu003e4__this.ResizeAsync(stringAndClear, this.u003cu003e4__this.get_App().get_Image().get_MimeType(), body, ct);
				}
				stringAndClear = null;
			});
			FileCallbackResult fileCallbackResult = new FileCallbackResult(base.get_App().get_Image().get_MimeType(), fileCallback);
			fileCallbackResult.set_ErrorAs404(true);
			return fileCallbackResult;
		}

		private async Task ResizeAsync(string resizedAsset, string mimeType, Stream target, CancellationToken ct)
		{
			int num;
			object obj;
			ValueTask valueTask;
			Exception exception;
			object obj1;
			TempAssetFile tempAssetFile;
			TempAssetFile tempAssetFile1;
			ResizeOptions resizeOption;
			Stream stream;
			object obj2;
			int num1;
			int num2;
			Stream stream1;
			object obj3;
			int num3;
			using (Activity activity = Telemetry.Activities.StartActivity("Resize", ActivityKind.Internal))
			{
				tempAssetFile = new TempAssetFile(resizedAsset, mimeType, (long)0);
				object obj4 = null;
				int num4 = 0;
				try
				{
					tempAssetFile1 = new TempAssetFile(resizedAsset, mimeType, (long)0);
					object obj5 = null;
					int num5 = 0;
					try
					{
						ResizeOptions resizeOption1 = new ResizeOptions();
						resizeOption1.set_TargetWidth(new int?(50));
						resizeOption1.set_TargetHeight(new int?(50));
						resizeOption = resizeOption1;
						using (Activity activity1 = Telemetry.Activities.StartActivity("Read", ActivityKind.Internal))
						{
							stream = tempAssetFile.OpenWrite();
							obj2 = null;
							num1 = 0;
							try
							{
								await this.appImageStore.DownloadAsync(base.get_App().get_Id(), stream, ct);
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
										await this.assetThumbnailGenerator.CreateThumbnailAsync(stream, mimeType, stream1, resizeOption, ct);
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
									IAssetStore assetStore = this.assetStore;
									string str = resizedAsset;
									Stream stream2 = stream;
									CancellationToken cancellationToken = new CancellationToken();
									await assetStore.UploadAsync(str, stream2, false, cancellationToken);
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
					resizeOption = null;
					return;
				}
			}
			activity = null;
			tempAssetFile = null;
			tempAssetFile1 = null;
			resizeOption = null;
			activity = null;
			tempAssetFile = null;
			tempAssetFile1 = null;
			resizeOption = null;
		}
	}
}