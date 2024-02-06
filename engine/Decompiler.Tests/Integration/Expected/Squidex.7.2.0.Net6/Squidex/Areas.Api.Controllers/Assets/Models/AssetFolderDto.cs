using Squidex.Areas.Api.Controllers.Assets;
using Squidex.Domain.Apps.Entities.Assets;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Assets.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AssetFolderDto : Resource
	{
		[LocalizedRequired]
		public string FolderName
		{
			get;
			set;
		}

		public DomainId Id
		{
			get;
			set;
		}

		public DomainId ParentId
		{
			get;
			set;
		}

		public long Version
		{
			get;
			set;
		}

		public AssetFolderDto()
		{
		}

		private AssetFolderDto CreateLinks(Resources resources)
		{
			var variable = new { app = resources.get_App(), id = this.Id };
			if (resources.get_CanUpdateAsset())
			{
				base.AddPutLink("update", resources.Url<AssetFoldersController>((AssetFoldersController x) => "PutAssetFolder", variable), null);
				base.AddPutLink("move", resources.Url<AssetFoldersController>((AssetFoldersController x) => "PutAssetFolderParent", variable), null);
			}
			if (resources.get_CanUpdateAsset())
			{
				base.AddDeleteLink("delete", resources.Url<AssetFoldersController>((AssetFoldersController x) => "DeleteAssetFolder", variable), null);
			}
			return this;
		}

		public static AssetFolderDto FromDomain(IAssetFolderEntity asset, Resources resources)
		{
			return SimpleMapper.Map<IAssetFolderEntity, AssetFolderDto>(asset, new AssetFolderDto()).CreateLinks(resources);
		}
	}
}