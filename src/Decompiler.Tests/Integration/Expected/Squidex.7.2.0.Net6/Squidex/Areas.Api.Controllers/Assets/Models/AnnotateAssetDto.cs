using Squidex.Domain.Apps.Core.Assets;
using Squidex.Domain.Apps.Entities.Assets.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Assets.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class AnnotateAssetDto
	{
		public string FileName
		{
			get;
			set;
		}

		public bool? IsProtected
		{
			get;
			set;
		}

		public AssetMetadata Metadata
		{
			get;
			set;
		}

		public string Slug
		{
			get;
			set;
		}

		[Nullable(new byte[] { 2, 1 })]
		public HashSet<string> Tags
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public AnnotateAssetDto()
		{
		}

		[NullableContext(1)]
		public AnnotateAsset ToCommand(DomainId id)
		{
			AnnotateAsset annotateAsset = new AnnotateAsset();
			annotateAsset.set_AssetId(id);
			return SimpleMapper.Map<AnnotateAssetDto, AnnotateAsset>(this, annotateAsset);
		}
	}
}