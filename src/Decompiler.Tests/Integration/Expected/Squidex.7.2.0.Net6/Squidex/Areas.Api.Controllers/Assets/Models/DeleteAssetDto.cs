using Microsoft.AspNetCore.Mvc;
using Squidex.Domain.Apps.Entities.Assets.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Assets.Models
{
	public sealed class DeleteAssetDto
	{
		[FromQuery]
		public bool CheckReferrers
		{
			get;
			set;
		}

		[FromQuery]
		public bool Permanent
		{
			get;
			set;
		}

		public DeleteAssetDto()
		{
		}

		[NullableContext(1)]
		public DeleteAsset ToCommand(DomainId id)
		{
			DeleteAsset deleteAsset = new DeleteAsset();
			deleteAsset.set_AssetId(id);
			return SimpleMapper.Map<DeleteAssetDto, DeleteAsset>(this, deleteAsset);
		}
	}
}