using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Areas.Api.Controllers.Users.Models;
using Squidex.Domain.Users;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.Translations;
using Squidex.Shared.Users;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Users
{
	[ApiExplorerSettings(GroupName="UserManagement")]
	[ApiModelValidation(true)]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class UserManagementController : ApiController
	{
		private readonly IUserService userService;

		public UserManagementController(ICommandBus commandBus, IUserService userService) : base(commandBus)
		{
			this.userService = userService;
		}

		[ApiPermission(new string[] { "squidex.admin.users.unlock" })]
		[HttpDelete]
		[ProducesResponseType(204)]
		[Route("user-management/{id}/")]
		public async Task<IActionResult> DeleteUser(string id)
		{
			if (Extensions.IsUser(this, id))
			{
				throw new DomainForbiddenException(T.Get("users.deleteYourselfError", null), null);
			}
			await this.userService.DeleteAsync(id, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiPermission(new string[] { "squidex.admin.users.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(UserDto), 200)]
		[Route("user-management/{id}/")]
		public async Task<IActionResult> GetUser(string id)
		{
			IActionResult actionResult;
			IUser user = await this.userService.FindByIdAsync(id, base.get_HttpContext().get_RequestAborted());
			if (user != null)
			{
				UserDto userDto = UserDto.FromDomain(user, base.get_Resources());
				actionResult = this.Ok(userDto);
			}
			else
			{
				actionResult = this.NotFound();
			}
			return actionResult;
		}

		[ApiPermission(new string[] { "squidex.admin.users.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(UsersDto), 200)]
		[Route("user-management/")]
		public async Task<IActionResult> GetUsers([Nullable(2)][FromQuery] string query = null, [FromQuery] int skip = 0, [FromQuery] int take = 10)
		{
			IResultList<IUser> resultList = await this.userService.QueryAsync(query, take, skip, base.get_HttpContext().get_RequestAborted());
			UsersDto usersDto = UsersDto.FromDomain(resultList, resultList.get_Total(), base.get_Resources());
			return this.Ok(usersDto);
		}

		[ApiPermission(new string[] { "squidex.admin.users.lock" })]
		[HttpPut]
		[ProducesResponseType(typeof(UserDto), 200)]
		[Route("user-management/{id}/lock/")]
		public async Task<IActionResult> LockUser(string id)
		{
			if (Extensions.IsUser(this, id))
			{
				throw new DomainForbiddenException(T.Get("users.lockYourselfError", null), null);
			}
			IUser user = await this.userService.LockAsync(id, base.get_HttpContext().get_RequestAborted());
			UserDto userDto = UserDto.FromDomain(user, base.get_Resources());
			return this.Ok(userDto);
		}

		[ApiPermission(new string[] { "squidex.admin.users.create" })]
		[HttpPost]
		[ProducesResponseType(typeof(UserDto), 201)]
		[Route("user-management/")]
		public async Task<IActionResult> PostUser([FromBody] CreateUserDto request)
		{
			IUser user = await this.userService.CreateAsync(request.Email, request.ToValues(), false, base.get_HttpContext().get_RequestAborted());
			UserDto userDto = UserDto.FromDomain(user, base.get_Resources());
			IActionResult actionResult = this.CreatedAtAction("GetUser", new { id = user.get_Id() }, userDto);
			return actionResult;
		}

		[ApiPermission(new string[] { "squidex.admin.users.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(UserDto), 200)]
		[Route("user-management/{id}/")]
		public async Task<IActionResult> PutUser(string id, [FromBody] UpdateUserDto request)
		{
			IUser user = await this.userService.UpdateAsync(id, request.ToValues(), false, base.get_HttpContext().get_RequestAborted());
			UserDto userDto = UserDto.FromDomain(user, base.get_Resources());
			return this.Ok(userDto);
		}

		[ApiPermission(new string[] { "squidex.admin.users.unlock" })]
		[HttpPut]
		[ProducesResponseType(typeof(UserDto), 200)]
		[Route("user-management/{id}/unlock/")]
		public async Task<IActionResult> UnlockUser(string id)
		{
			if (Extensions.IsUser(this, id))
			{
				throw new DomainForbiddenException(T.Get("users.unlockYourselfError", null), null);
			}
			IUser user = await this.userService.UnlockAsync(id, base.get_HttpContext().get_RequestAborted());
			UserDto userDto = UserDto.FromDomain(user, base.get_Resources());
			return this.Ok(userDto);
		}
	}
}