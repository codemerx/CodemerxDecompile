using Microsoft.Extensions.Options;
using Squidex.Areas.Api.Controllers.News;
using Squidex.Areas.Api.Controllers.News.Models;
using Squidex.ClientLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.News.Service
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class FeaturesService
	{
		private const int FeatureVersion = 21;

		private readonly QueryContext flatten = QueryContext.Default.Flatten(true);

		private readonly IContentsClient<FeaturesService.NewsEntity, FeatureDto> client;

		public FeaturesService(IOptions<MyNewsOptions> options)
		{
			if (options.get_Value().IsConfigured())
			{
				SquidexOptions squidexOption = new SquidexOptions();
				squidexOption.set_AppName(options.get_Value().AppName);
				squidexOption.set_ClientId(options.get_Value().ClientId);
				squidexOption.set_ClientSecret(options.get_Value().ClientSecret);
				squidexOption.set_Url("https://cloud.squidex.io");
				this.client = (new SquidexClientManager(squidexOption)).CreateContentsClient<FeaturesService.NewsEntity, FeatureDto>("feature-news");
			}
		}

		public async Task<FeaturesDto> GetFeaturesAsync(int version = 0, CancellationToken ct = null)
		{
			FeaturesService.u003cGetFeaturesAsyncu003ed__5 variable = new FeaturesService.u003cGetFeaturesAsyncu003ed__5();
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<FeaturesDto>.Create();
			variable.u003cu003e4__this = this;
			variable.version = version;
			variable.ct = ct;
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<FeaturesService.u003cGetFeaturesAsyncu003ed__5>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		[Nullable(new byte[] { 0, 1 })]
		public sealed class NewsEntity : Content<FeatureDto>
		{
			public NewsEntity()
			{
			}
		}
	}
}