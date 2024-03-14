using Squidex.Domain.Apps.Entities.Assets.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Assets.Models
{
	public sealed class MoveAssetDto
	{
		public DomainId ParentId
		{
			get;
			set;
		}

		public MoveAssetDto()
		{
		}

		[NullableContext(1)]
		public MoveAsset ToCommand(DomainId id)
		{
			MoveAsset moveAsset = new MoveAsset();
			moveAsset.set_AssetId(id);
			return SimpleMapper.Map<MoveAssetDto, MoveAsset>(this, moveAsset);
		}
	}
}