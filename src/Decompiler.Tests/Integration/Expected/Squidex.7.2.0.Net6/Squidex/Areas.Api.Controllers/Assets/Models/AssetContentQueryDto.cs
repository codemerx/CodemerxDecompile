using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Squidex.Assets;
using Squidex.Domain.Apps.Core.Assets;
using Squidex.Domain.Apps.Core.ValidateContent;
using Squidex.Domain.Apps.Entities.Assets;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Assets.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class AssetContentQueryDto
	{
		[FromQuery(Name="auto")]
		public bool Auto
		{
			get;
			set;
		}

		[FromQuery(Name="bg")]
		public string Background
		{
			get;
			set;
		}

		[FromQuery(Name="cache")]
		public long CacheDuration
		{
			get;
			set;
		}

		[FromQuery(Name="download")]
		public int Download
		{
			get;
			set;
		}

		[FromQuery(Name="focusX")]
		public float? FocusX
		{
			get;
			set;
		}

		[FromQuery(Name="focusY")]
		public float? FocusY
		{
			get;
			set;
		}

		[FromQuery(Name="force")]
		public bool Force
		{
			get;
			set;
		}

		[FromQuery(Name="format")]
		public ImageFormat? Format
		{
			get;
			set;
		}

		[FromQuery(Name="height")]
		public int? Height
		{
			get;
			set;
		}

		[FromQuery(Name="nofocus")]
		public bool IgnoreFocus
		{
			get;
			set;
		}

		[FromQuery(Name="mode")]
		public ResizeMode? Mode
		{
			get;
			set;
		}

		[FromQuery(Name="quality")]
		public int? Quality
		{
			get;
			set;
		}

		[FromQuery(Name="version")]
		public long Version { get; set; } = (long)-2;

		[FromQuery(Name="width")]
		public int? Width
		{
			get;
			set;
		}

		public AssetContentQueryDto()
		{
		}

		[NullableContext(0)]
		private ValueTuple<float?, float?> GetFocusPoint([Nullable(1)] IAssetEntity asset)
		{
			float? focusX;
			if (this.IgnoreFocus)
			{
				focusX = null;
				float? nullable = focusX;
				focusX = null;
				return new ValueTuple<float?, float?>(nullable, focusX);
			}
			focusX = this.FocusX;
			if (focusX.HasValue)
			{
				focusX = this.FocusY;
				if (focusX.HasValue)
				{
					focusX = this.FocusX;
					float? nullable1 = new float?(focusX.Value);
					focusX = this.FocusY;
					return new ValueTuple<float?, float?>(nullable1, new float?(focusX.Value));
				}
			}
			float? focusX1 = asset.get_Metadata().GetFocusX();
			float? focusY = asset.get_Metadata().GetFocusY();
			if (focusX1.HasValue && focusY.HasValue)
			{
				return new ValueTuple<float?, float?>(new float?(focusX1.Value), new float?(focusY.Value));
			}
			focusX = null;
			float? nullable2 = focusX;
			focusX = null;
			return new ValueTuple<float?, float?>(nullable2, focusX);
		}

		[NullableContext(1)]
		private ImageFormat? GetFormat(IAssetEntity asset, IAssetThumbnailGenerator assetThumbnailGenerator, HttpRequest request)
		{
			AssetContentQueryDto.u003cu003ec__DisplayClass57_0 variable = new AssetContentQueryDto.u003cu003ec__DisplayClass57_0();
			variable.asset = asset;
			variable.request = request;
			variable.assetThumbnailGenerator = assetThumbnailGenerator;
			if (this.Format.HasValue || !this.Auto)
			{
				return this.Format;
			}
			if (AssetContentQueryDto.u003cGetFormatu003eg__Acceptsu007c57_0("image/webp", ref variable))
			{
				return new ImageFormat?(7);
			}
			return this.Format;
		}

		[NullableContext(1)]
		public ResizeOptions ToResizeOptions(IAssetEntity asset, IAssetThumbnailGenerator assetThumbnailGenerator, HttpRequest request)
		{
			Guard.NotNull<IAssetEntity>(asset, "asset");
			ResizeOptions resizeOption = SimpleMapper.Map<AssetContentQueryDto, ResizeOptions>(this, new ResizeOptions());
			ValueTuple<float?, float?> focusPoint = this.GetFocusPoint(asset);
			float? item1 = focusPoint.Item1;
			float? item2 = focusPoint.Item2;
			resizeOption.set_FocusX(item1);
			resizeOption.set_FocusY(item2);
			resizeOption.set_TargetWidth(this.Width);
			resizeOption.set_TargetHeight(this.Height);
			resizeOption.set_Format(this.GetFormat(asset, assetThumbnailGenerator, request));
			return resizeOption;
		}
	}
}