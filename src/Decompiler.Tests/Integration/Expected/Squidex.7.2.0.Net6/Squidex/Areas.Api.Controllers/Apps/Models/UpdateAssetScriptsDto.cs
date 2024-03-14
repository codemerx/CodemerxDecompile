using Squidex.Domain.Apps.Core.Assets;
using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class UpdateAssetScriptsDto
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

		public UpdateAssetScriptsDto()
		{
		}

		[NullableContext(1)]
		public ConfigureAssetScripts ToCommand()
		{
			AssetScripts assetScript = SimpleMapper.Map<UpdateAssetScriptsDto, AssetScripts>(this, new AssetScripts());
			ConfigureAssetScripts configureAssetScript = new ConfigureAssetScripts();
			configureAssetScript.set_Scripts(assetScript);
			return configureAssetScript;
		}
	}
}