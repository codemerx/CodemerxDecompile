using Squidex.Areas.Api.Controllers.Assets;
using Squidex.Domain.Apps.Entities.Assets;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Assets.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AssetFoldersDto : Resource
	{
		[LocalizedRequired]
		public AssetFolderDto[] Items
		{
			get;
			set;
		}

		[LocalizedRequired]
		public AssetFolderDto[] Path
		{
			get;
			set;
		}

		public long Total
		{
			get;
			set;
		}

		public AssetFoldersDto()
		{
		}

		private AssetFoldersDto CreateLinks(Resources resources)
		{
			var variable = new { app = resources.get_App() };
			base.AddSelfLink(resources.Url<AssetFoldersController>((AssetFoldersController x) => "GetAssetFolders", variable));
			if (resources.get_CanUpdateAsset())
			{
				base.AddPostLink("create", resources.Url<AssetFoldersController>((AssetFoldersController x) => "PostAssetFolder", variable), null);
			}
			return this;
		}

		public static AssetFoldersDto FromDomain(IResultList<IAssetFolderEntity> assetFolders, IEnumerable<IAssetFolderEntity> path, Resources resources)
		{
			AssetFoldersDto assetFoldersDto = new AssetFoldersDto()
			{
				Total = assetFolders.get_Total(),
				Items = (
					from x in assetFolders
					select AssetFolderDto.FromDomain(x, resources)).ToArray<AssetFolderDto>(),
				Path = (
					from x in path
					select AssetFolderDto.FromDomain(x, resources)).ToArray<AssetFolderDto>()
			};
			return assetFoldersDto.CreateLinks(resources);
		}
	}
}