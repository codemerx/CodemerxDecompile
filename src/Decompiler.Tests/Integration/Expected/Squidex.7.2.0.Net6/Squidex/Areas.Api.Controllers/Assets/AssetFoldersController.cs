using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Squidex.Areas.Api.Controllers.Assets.Models;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Assets;
using Squidex.Domain.Apps.Entities.Assets.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.Tasks;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Assets
{
	[ApiExplorerSettings(GroupName="Assets")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AssetFoldersController : ApiController
	{
		private readonly IAssetQueryService assetQuery;

		public AssetFoldersController(ICommandBus commandBus, IAssetQueryService assetQuery) : base(commandBus)
		{
			this.assetQuery = assetQuery;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.update" })]
		[HttpDelete]
		[Route("apps/{app}/assets/folders/{id}/", Order=-1)]
		public async Task<IActionResult> DeleteAssetFolder(string app, DomainId id)
		{
			DeleteAssetFolder deleteAssetFolder = new DeleteAssetFolder();
			deleteAssetFolder.set_AssetFolderId(id);
			DeleteAssetFolder deleteAssetFolder1 = deleteAssetFolder;
			await base.get_CommandBus().PublishAsync(deleteAssetFolder1, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(AssetFoldersDto), 200)]
		[Route("apps/{app}/assets/folders", Order=-1)]
		public async Task<IActionResult> GetAssetFolders(string app, [FromQuery] DomainId parentId, [FromQuery] AssetFolderScope scope = 0)
		{
			ValueTuple<IResultList<IAssetFolderEntity>, IReadOnlyList<IAssetFolderEntity>> valueTuple = await AsyncHelper.WhenAll<IResultList<IAssetFolderEntity>, IReadOnlyList<IAssetFolderEntity>>(this.GetAssetFoldersAsync(parentId, scope), this.GetAssetPathAsync(parentId, scope));
			IResultList<IAssetFolderEntity> item1 = valueTuple.Item1;
			IReadOnlyList<IAssetFolderEntity> item2 = valueTuple.Item2;
			Deferred deferred = Deferred.Response(() => AssetFoldersDto.FromDomain(item1, item2, base.get_Resources()));
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, ETagExtensions.ToEtag<IAssetFolderEntity>(item1));
			IActionResult actionResult = this.Ok(deferred);
			return actionResult;
		}

		private Task<IResultList<IAssetFolderEntity>> GetAssetFoldersAsync(DomainId parentId, AssetFolderScope scope)
		{
			if (scope == AssetFolderScope.Path)
			{
				return Task.FromResult<IResultList<IAssetFolderEntity>>(ResultList.Empty<IAssetFolderEntity>());
			}
			return this.assetQuery.QueryAssetFoldersAsync(base.get_Context(), parentId, base.get_HttpContext().get_RequestAborted());
		}

		private Task<IReadOnlyList<IAssetFolderEntity>> GetAssetPathAsync(DomainId parentId, AssetFolderScope scope)
		{
			if (scope == AssetFolderScope.Items)
			{
				return Task.FromResult<IReadOnlyList<IAssetFolderEntity>>(ReadonlyList.Empty<IAssetFolderEntity>());
			}
			return this.assetQuery.FindAssetFolderAsync(base.get_Context().get_App().get_Id(), parentId, base.get_HttpContext().get_RequestAborted());
		}

		private async Task<AssetFolderDto> InvokeCommandAsync(ICommand command)
		{
			CommandContext commandContext = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted());
			AssetFolderDto assetFolderDto = AssetFolderDto.FromDomain(commandContext.Result<IAssetFolderEntity>(), base.get_Resources());
			return assetFolderDto;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.update" })]
		[AssetRequestSizeLimit]
		[HttpPost]
		[ProducesResponseType(typeof(AssetFolderDto), 201)]
		[Route("apps/{app}/assets/folders", Order=-1)]
		public async Task<IActionResult> PostAssetFolder(string app, [FromBody] CreateAssetFolderDto request)
		{
			AssetFolderDto assetFolderDto = await this.InvokeCommandAsync(request.ToCommand());
			IActionResult actionResult = this.CreatedAtAction("GetAssetFolders", new { parentId = request.ParentId, app = app }, assetFolderDto);
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.update" })]
		[AssetRequestSizeLimit]
		[HttpPut]
		[ProducesResponseType(typeof(AssetFolderDto), 200)]
		[Route("apps/{app}/assets/folders/{id}/", Order=-1)]
		public async Task<IActionResult> PutAssetFolder(string app, DomainId id, [FromBody] RenameAssetFolderDto request)
		{
			RenameAssetFolder command = request.ToCommand(id);
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.assets.update" })]
		[AssetRequestSizeLimit]
		[HttpPut]
		[ProducesResponseType(typeof(AssetFolderDto), 200)]
		[Route("apps/{app}/assets/folders/{id}/parent", Order=-1)]
		public async Task<IActionResult> PutAssetFolderParent(string app, DomainId id, [FromBody] MoveAssetFolderDto request)
		{
			MoveAssetFolder command = request.ToCommand(id);
			return this.Ok(await this.InvokeCommandAsync(command));
		}
	}
}