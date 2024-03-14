using Squidex.Areas.Api.Controllers.Assets;
using Squidex.Domain.Apps.Entities.Assets;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Assets.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AssetsDto : Resource
	{
		[LocalizedRequired]
		public AssetDto[] Items
		{
			get;
			set;
		}

		public long Total
		{
			get;
			set;
		}

		public AssetsDto()
		{
		}

		private AssetsDto CreateLinks(Resources resources)
		{
			var variable = new { app = resources.get_App() };
			base.AddSelfLink(resources.Url<AssetsController>((AssetsController x) => "GetAssets", variable));
			if (resources.get_CanCreateAsset())
			{
				base.AddPostLink("create", resources.Url<AssetsController>((AssetsController x) => "PostAsset", variable), null);
			}
			if (resources.get_CanUpdateAsset())
			{
				var variable1 = new { app = variable.app, name = "tag" };
				base.AddPutLink("tags/rename", resources.Url<AssetsController>((AssetsController x) => "PutTag", variable1), null);
			}
			base.AddGetLink("tags", resources.Url<AssetsController>((AssetsController x) => "GetTags", variable), null);
			return this;
		}

		public static AssetsDto FromDomain(IResultList<IEnrichedAssetEntity> assets, Resources resources)
		{
			return (new AssetsDto()
			{
				Total = assets.get_Total(),
				Items = (
					from x in assets
					select AssetDto.FromDomain(x, resources, false)).ToArray<AssetDto>()
			}).CreateLinks(resources);
		}
	}
}