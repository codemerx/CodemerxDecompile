using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Assets;
using Squidex.Domain.Apps.Entities.Assets.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Assets.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class UpsertAssetDto
	{
		[FromQuery]
		public bool Duplicate
		{
			get;
			set;
		}

		public IFormFile File
		{
			get;
			set;
		}

		[FromQuery]
		public DomainId ParentId
		{
			get;
			set;
		}

		public UpsertAssetDto()
		{
		}

		public static UpsertAsset ToCommand(AssetTusFile file)
		{
			UpsertAssetDto.u003cu003ec__DisplayClass12_0 variable = new UpsertAssetDto.u003cu003ec__DisplayClass12_0();
			string str;
			string str1;
			string str2;
			bool flag;
			variable.file = file;
			UpsertAsset upsertAsset = new UpsertAsset();
			upsertAsset.set_File(variable.file);
			UpsertAsset upsertAsset1 = upsertAsset;
			if (UpsertAssetDto.u003cToCommandu003eg__TryGetStringu007c12_0("id", out str, ref variable))
			{
				upsertAsset1.set_AssetId(DomainId.Create(str));
			}
			if (UpsertAssetDto.u003cToCommandu003eg__TryGetStringu007c12_0("parentId", out str1, ref variable))
			{
				upsertAsset1.set_ParentId(new DomainId?(DomainId.Create(str1)));
			}
			if (UpsertAssetDto.u003cToCommandu003eg__TryGetStringu007c12_0("duplicate", out str2, ref variable) && bool.TryParse(str2, out flag))
			{
				upsertAsset1.set_Duplicate(flag);
			}
			return upsertAsset1;
		}

		public UpsertAsset ToCommand(DomainId id, AssetFile file)
		{
			UpsertAsset upsertAsset = new UpsertAsset();
			upsertAsset.set_File(file);
			upsertAsset.set_AssetId(id);
			return SimpleMapper.Map<UpsertAssetDto, UpsertAsset>(this, upsertAsset);
		}
	}
}