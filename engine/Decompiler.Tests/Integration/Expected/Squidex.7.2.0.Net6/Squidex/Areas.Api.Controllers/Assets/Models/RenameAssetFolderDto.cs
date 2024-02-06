using Squidex.Domain.Apps.Entities.Assets.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Assets.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class RenameAssetFolderDto
	{
		[LocalizedRequired]
		public string FolderName
		{
			get;
			set;
		}

		public RenameAssetFolderDto()
		{
		}

		public RenameAssetFolder ToCommand(DomainId id)
		{
			RenameAssetFolder renameAssetFolder = new RenameAssetFolder();
			renameAssetFolder.set_AssetFolderId(id);
			return SimpleMapper.Map<RenameAssetFolderDto, RenameAssetFolder>(this, renameAssetFolder);
		}
	}
}