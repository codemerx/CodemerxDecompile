using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Assets;
using Squidex.Domain.Apps.Entities.Assets.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Assets.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class CreateAssetDto
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
		public DomainId? Id
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

		public CreateAssetDto()
		{
		}

		public CreateAsset ToCommand(AssetFile file)
		{
			CreateAsset createAsset = new CreateAsset();
			createAsset.set_File(file);
			CreateAsset createAsset1 = SimpleMapper.Map<CreateAssetDto, CreateAsset>(this, createAsset);
			if (this.Id.HasValue)
			{
				DomainId value = this.Id.Value;
				DomainId domainId = new DomainId();
				if (value != domainId)
				{
					domainId = this.Id.Value;
					if (!string.IsNullOrWhiteSpace(domainId.ToString()))
					{
						createAsset1.set_AssetId(this.Id.Value);
					}
				}
			}
			return createAsset1;
		}
	}
}