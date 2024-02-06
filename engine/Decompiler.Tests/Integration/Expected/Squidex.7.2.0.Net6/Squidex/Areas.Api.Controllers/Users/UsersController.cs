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
			UsersController.u003cGetUserPictureu003ed__10 variable = new UsersController.u003cGetUserPictureu003ed__10();
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<IActionResult>.Create();
			variable.u003cu003e4__this = this;
			variable.id = id;
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<UsersController.u003cGetUserPictureu003ed__10>(ref variable);
			return variable.u003cu003et__builder.Task;
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