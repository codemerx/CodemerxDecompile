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
	public class BulkUpdateAssetsJobDto
	{
		public long ExpectedVersion { get; set; } = (long)-2;

		public string FileName
		{
			get;
			set;
		}

		public DomainId Id
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

		public DomainId ParentId
		{
			get;
			set;
		}

		public bool Permanent
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

		public BulkUpdateAssetType Type
		{
			get;
			set;
		}

		public BulkUpdateAssetsJobDto()
		{
		}

		[NullableContext(1)]
		public BulkUpdateJob ToJob()
		{
			return SimpleMapper.Map<BulkUpdateAssetsJobDto, BulkUpdateJob>(this, new BulkUpdateJob());
		}
	}
}