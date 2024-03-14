using Squidex.Domain.Apps.Entities.Assets.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Assets.Models
{
	public sealed class MoveAssetFolderDto
	{
		public DomainId ParentId
		{
			get;
			set;
		}

		public MoveAssetFolderDto()
		{
		}

		[NullableContext(1)]
		public MoveAssetFolder ToCommand(DomainId id)
		{
			MoveAssetFolder moveAssetFolder = new MoveAssetFolder();
			moveAssetFolder.set_AssetFolderId(id);
			return SimpleMapper.Map<MoveAssetFolderDto, MoveAssetFolder>(this, moveAssetFolder);
		}
	}
}