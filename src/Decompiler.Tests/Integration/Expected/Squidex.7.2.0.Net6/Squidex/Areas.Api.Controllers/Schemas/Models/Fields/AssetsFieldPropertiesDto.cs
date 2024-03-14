using Squidex.Areas.Api.Controllers.Schemas.Models;
using Squidex.Domain.Apps.Core.Assets;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models.Fields
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class AssetsFieldPropertiesDto : FieldPropertiesDto
	{
		public bool AllowDuplicates
		{
			get;
			set;
		}

		[Nullable(new byte[] { 2, 1 })]
		public ReadonlyList<string> AllowedExtensions
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public int? AspectHeight
		{
			get;
			set;
		}

		public int? AspectWidth
		{
			get;
			set;
		}

		[Nullable(new byte[] { 2, 1 })]
		public ReadonlyList<string> DefaultValue
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		[Nullable(new byte[] { 2, 2, 1 })]
		public LocalizedValue<ReadonlyList<string>> DefaultValues
		{
			[return: Nullable(new byte[] { 2, 2, 1 })]
			get;
			set;
		}

		public AssetType? ExpectedType
		{
			get;
			set;
		}

		public string FolderId
		{
			get;
			set;
		}

		public int? MaxHeight
		{
			get;
			set;
		}

		public int? MaxItems
		{
			get;
			set;
		}

		public int? MaxSize
		{
			get;
			set;
		}

		public int? MaxWidth
		{
			get;
			set;
		}

		public int? MinHeight
		{
			get;
			set;
		}

		public int? MinItems
		{
			get;
			set;
		}

		public int? MinSize
		{
			get;
			set;
		}

		public int? MinWidth
		{
			get;
			set;
		}

		[Obsolete("Use 'expectedType' field now")]
		public bool MustBeImage
		{
			get
			{
				AssetType? expectedType = this.ExpectedType;
				return expectedType.GetValueOrDefault() == 1 & expectedType.HasValue;
			}
			set
			{
				this.ExpectedType = (value ? new AssetType?(1) : this.ExpectedType);
			}
		}

		public AssetPreviewMode PreviewMode
		{
			get;
			set;
		}

		public bool ResolveFirst
		{
			get;
			set;
		}

		[Obsolete("Use 'resolveFirst' field now")]
		public bool ResolveImage
		{
			get
			{
				return this.ResolveFirst;
			}
			set
			{
				this.ResolveFirst = value;
			}
		}

		public AssetsFieldPropertiesDto()
		{
		}

		[NullableContext(1)]
		public override FieldProperties ToProperties()
		{
			return SimpleMapper.Map<AssetsFieldPropertiesDto, AssetsFieldProperties>(this, new AssetsFieldProperties());
		}
	}
}