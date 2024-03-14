using NodaTime;
using Squidex.Areas.Api.Controllers.Assets;
using Squidex.Domain.Apps.Core.Assets;
using Squidex.Domain.Apps.Core.ValidateContent;
using Squidex.Domain.Apps.Entities.Assets;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Squidex.Areas.Api.Controllers.Assets.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AssetDto : Resource
	{
		public Instant Created
		{
			get;
			set;
		}

		[LocalizedRequired]
		public RefToken CreatedBy
		{
			get;
			set;
		}

		[Nullable(2)]
		public string EditToken
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		[Nullable(2)]
		public string FileHash
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		[LocalizedRequired]
		public string FileName
		{
			get;
			set;
		}

		public long FileSize
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string FileType
		{
			get;
			set;
		}

		public long FileVersion
		{
			get;
			set;
		}

		public DomainId Id
		{
			get;
			set;
		}

		[Obsolete("Use 'type' field now.")]
		public bool IsImage
		{
			get
			{
				return this.Type == 1;
			}
		}

		public bool IsProtected
		{
			get;
			set;
		}

		public Instant LastModified
		{
			get;
			set;
		}

		[LocalizedRequired]
		public RefToken LastModifiedBy
		{
			get;
			set;
		}

		[JsonPropertyName("_meta")]
		[Nullable(2)]
		public AssetMeta Meta
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		[LocalizedRequired]
		public AssetMetadata Metadata
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string MetadataText
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string MimeType
		{
			get;
			set;
		}

		public DomainId ParentId
		{
			get;
			set;
		}

		[Obsolete("Use 'metadata' field now.")]
		public int? PixelHeight
		{
			get
			{
				return this.Metadata.GetPixelHeight();
			}
		}

		[Obsolete("Use 'metadata' field now.")]
		public int? PixelWidth
		{
			get
			{
				return this.Metadata.GetPixelWidth();
			}
		}

		[LocalizedRequired]
		public string Slug
		{
			get;
			set;
		}

		[LocalizedRequired]
		[Nullable(new byte[] { 2, 1 })]
		public HashSet<string> Tags
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public AssetType Type
		{
			get;
			set;
		}

		public long Version
		{
			get;
			set;
		}

		public AssetDto()
		{
		}

		private AssetDto CreateLinks(Resources resources)
		{
			string app = resources.get_App();
			var variable = new { app = app, id = this.Id };
			base.AddSelfLink(resources.Url<AssetsController>((AssetsController x) => "GetAsset", variable));
			if (resources.get_CanUpdateAsset())
			{
				base.AddPutLink("update", resources.Url<AssetsController>((AssetsController x) => "PutAsset", variable), null);
			}
			if (resources.get_CanUpdateAsset())
			{
				base.AddPutLink("move", resources.Url<AssetsController>((AssetsController x) => "PutAssetParent", variable), null);
			}
			if (resources.get_CanUploadAsset())
			{
				base.AddPutLink("upload", resources.Url<AssetsController>((AssetsController x) => "PutAssetContent", variable), null);
			}
			if (resources.get_CanDeleteAsset())
			{
				base.AddDeleteLink("delete", resources.Url<AssetsController>((AssetsController x) => "DeleteAsset", variable), null);
			}
			if (string.IsNullOrWhiteSpace(this.Slug))
			{
				var variable1 = new { app = app, idOrSlug = this.Id };
				base.AddGetLink("content", resources.Url<AssetContentController>((AssetContentController x) => "GetAssetContentBySlug", variable1), null);
			}
			else
			{
				var variable2 = new { app = app, idOrSlug = this.Id, more = this.Slug };
				base.AddGetLink("content", resources.Url<AssetContentController>((AssetContentController x) => "GetAssetContentBySlug", variable2), null);
				var variable3 = new { app = app, idOrSlug = this.Slug };
				base.AddGetLink("content/slug", resources.Url<AssetContentController>((AssetContentController x) => "GetAssetContentBySlug", variable3), null);
			}
			return this;
		}

		public static AssetDto FromDomain(IEnrichedAssetEntity asset, Resources resources, bool isDuplicate = false)
		{
			AssetDto tagNames = SimpleMapper.Map<IEnrichedAssetEntity, AssetDto>(asset, new AssetDto());
			tagNames.Tags = asset.get_TagNames();
			if (isDuplicate)
			{
				tagNames.Meta = new AssetMeta()
				{
					IsDuplicate = "true"
				};
			}
			tagNames.FileType = Squidex.Infrastructure.FileExtensions.FileType(asset.get_FileName());
			return tagNames.CreateLinks(resources);
		}
	}
}