using Squidex.Areas.Api.Controllers.Apps;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Infrastructure.Reflection;
using Squidex.Web;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class AssetScriptsDto : Resource
	{
		public string Annotate
		{
			get;
			set;
		}

		public string Create
		{
			get;
			set;
		}

		public string Delete
		{
			get;
			set;
		}

		public string Move
		{
			get;
			set;
		}

		public string Update
		{
			get;
			set;
		}

		public long Version
		{
			get;
			set;
		}

		public AssetScriptsDto()
		{
		}

		[NullableContext(1)]
		private AssetScriptsDto CreateLinks(Resources resources)
		{
			var variable = new { app = resources.get_App() };
			base.AddSelfLink(resources.Url<AppSettingsController>((AppSettingsController x) => "GetSettings", variable));
			if (resources.get_CanUpdateAssetsScripts())
			{
				base.AddPutLink("update", resources.Url<AppAssetsController>((AppAssetsController x) => "PutAssetScripts", variable), null);
			}
			return this;
		}

		[NullableContext(1)]
		public static AssetScriptsDto FromDomain(IAppEntity app, Resources resources)
		{
			AssetScriptsDto assetScriptsDto = SimpleMapper.Map<AssetScripts, AssetScriptsDto>(app.get_AssetScripts(), new AssetScriptsDto());
			return assetScriptsDto.CreateLinks(resources);
		}
	}
}