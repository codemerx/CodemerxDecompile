using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Squidex.Areas.Api.Controllers.Users.Models;
using Squidex.Assets;
using Squidex.Domain.Users;
using Squidex.Infrastructure.Commands;
using Squidex.Shared.Identity;
using Squidex.Shared.Users;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Users
{
	[ApiExplorerSettings(GroupName="Users")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class UsersController : ApiController
	{
		private readonly static byte[] AvatarBytes;

		private readonly IHttpClientFactory httpClientFactory;

		private readonly IUserPictureStore userPictureStore;

		private readonly IUserResolver userResolver;

		private readonly ILogger<UsersController> log;

		static UsersController()
		{
			using (Stream manifestResourceStream = typeof(UsersController).Assembly.GetManifestResourceStream("Squidex.Areas.Api.Controllers.Users.Assets.Avatar.png"))
			{
				UsersController.AvatarBytes = new byte[checked((IntPtr)manifestResourceStream.Length)];
				manifestResourceStream.Read(UsersController.AvatarBytes, 0, (int)UsersController.AvatarBytes.Length);
			}
		}

		public UsersController(ICommandBus commandBus, IHttpClientFactory httpClientFactory, IUserPictureStore userPictureStore, IUserResolver userResolver, ILogger<UsersController> log) : base(commandBus)
		{
			this.httpClientFactory = httpClientFactory;
			this.userPictureStore = userPictureStore;
			this.userResolver = userResolver;
			this.log = log;
		}

		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(UserDto), 200)]
		[Route("users/{id}/")]
		public async Task<IActionResult> GetUser(string id)
		{
			IActionResult actionResult;
			try
			{
				IUser user = await this.userResolver.FindByIdAsync(id, base.get_HttpContext().get_RequestAborted());
				if (user != null)
				{
					UserDto userDto = UserDto.FromDomain(user, base.get_Resources());
					actionResult = this.Ok(userDto);
					return actionResult;
				}
			}
			catch (Exception exception)
			{
				LoggerExtensions.LogError(this.log, exception, "Failed to return user, returning empty results.", Array.Empty<object>());
			}
			actionResult = this.NotFound();
			return actionResult;
		}

		[HttpGet]
		[ProducesResponseType(typeof(FileResult), 200)]
		[ResponseCache(Duration=0x12c)]
		[Route("users/{id}/picture/")]
		public async Task<IActionResult> GetUserPicture(string id)
		{
			IActionResult fileCallbackResult;
			string str;
			string tag;
			UsersController.u003cu003ec__DisplayClass10_0 variable;
			try
			{
				IUser user = await this.userResolver.FindByIdAsync(id, base.get_HttpContext().get_RequestAborted());
				IUser user1 = user;
				if (user1 != null)
				{
					if (!SquidexClaimsExtensions.IsPictureUrlStored(user1.get_Claims()))
					{
						using (HttpClient httpClient = HttpClientFactoryExtensions.CreateClient(this.httpClientFactory))
						{
							string str1 = SquidexClaimsExtensions.PictureNormalizedUrl(user1.get_Claims());
							if (!string.IsNullOrWhiteSpace(str1))
							{
								HttpResponseMessage async = await httpClient.GetAsync(str1, HttpCompletionOption.ResponseHeadersRead, base.get_HttpContext().get_RequestAborted());
								if (!async.IsSuccessStatusCode)
								{
									async = null;
								}
								else
								{
									MediaTypeHeaderValue contentType = async.Content.Headers.ContentType;
									if (contentType != null)
									{
										str = contentType.ToString();
									}
									else
									{
										str = null;
									}
									string str2 = str;
									Stream stream = await async.Content.ReadAsStreamAsync(base.get_HttpContext().get_RequestAborted());
									System.Net.Http.Headers.EntityTagHeaderValue eTag = async.Headers.ETag;
									FileStreamResult fileStreamResult = new FileStreamResult(stream, str2);
									if (eTag != null)
									{
										tag = eTag.Tag;
									}
									else
									{
										tag = null;
									}
									if (!string.IsNullOrWhiteSpace(tag))
									{
										fileStreamResult.set_EntityTag(new Microsoft.Net.Http.Headers.EntityTagHeaderValue(eTag.Tag, eTag.IsWeak));
									}
									fileCallbackResult = fileStreamResult;
									variable = null;
									return fileCallbackResult;
								}
							}
						}
						httpClient = null;
					}
					else
					{
						FileCallback fileCallback = new FileCallback(variable, async (Stream body, BytesRange range, CancellationToken ct) => {
							int num = 0;
							try
							{
								await this.u003cu003e4__this.userPictureStore.DownloadAsync(this.entity.get_Id(), body, ct);
							}
							catch
							{
								num = 1;
							}
							if (num == 1)
							{
								await body.WriteAsync(UsersController.AvatarBytes, ct);
							}
						});
						fileCallbackResult = new FileCallbackResult("image/png", fileCallback);
						variable = null;
						return fileCallbackResult;
					}
				}
			}
			catch (Exception exception)
			{
				LoggerExtensions.LogError(this.log, exception, "Failed to return user picture, returning fallback image.", Array.Empty<object>());
			}
			fileCallbackResult = new FileStreamResult(new MemoryStream(UsersController.AvatarBytes), "image/png");
			variable = null;
			return fileCallbackResult;
		}

		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(ResourcesDto), 200)]
		[Route("")]
		public IActionResult GetUserResources()
		{
			return this.Ok(ResourcesDto.FromDomain(base.get_Resources()));
		}

		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(UserDto[]), 200)]
		[Route("users/")]
		public async Task<IActionResult> GetUsers(string query)
		{
			IActionResult actionResult;
			try
			{
				List<IUser> users = await this.userResolver.QueryByEmailAsync(query, base.get_HttpContext().get_RequestAborted());
				UserDto[] array = (
					from x in users
					select UserDto.FromDomain(x, base.get_Resources())).ToArray<UserDto>();
				actionResult = this.Ok(array);
				return actionResult;
			}
			catch (Exception exception)
			{
				LoggerExtensions.LogError(this.log, exception, "Failed to return users, returning empty results.", Array.Empty<object>());
			}
			actionResult = this.Ok(Array.Empty<UserDto>());
			return actionResult;
		}
	}
}