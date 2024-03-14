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
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			FeaturesDto featuresDto = new FeaturesDto()
			{
				Version = version
			};
			if (this.client != null && version < 21)
			{
				try
				{
					ContentQuery contentQuery = new ContentQuery();
					if (version != 0)
					{
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 2);
						defaultInterpolatedStringHandler.AppendLiteral("data/version/iv le ");
						defaultInterpolatedStringHandler.AppendFormatted<int>(21);
						defaultInterpolatedStringHandler.AppendLiteral(" and data/version/iv gt ");
						defaultInterpolatedStringHandler.AppendFormatted<int>(version);
						contentQuery.set_Filter(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 1);
						defaultInterpolatedStringHandler.AppendLiteral("data/version/iv eq ");
						defaultInterpolatedStringHandler.AppendFormatted<int>(21);
						contentQuery.set_Filter(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					ContentsResult<FeaturesService.NewsEntity, FeatureDto> async = await this.client.GetAsync(contentQuery, this.flatten, ct);
					List<FeatureDto> features = featuresDto.Features;
					List<FeaturesService.NewsEntity> items = async.get_Items();
					features.AddRange((
						from x in items
						select x.get_Data()).ToList<FeatureDto>());
					if (async.get_Items().Count > 0)
					{
						FeaturesDto featuresDto1 = featuresDto;
						List<FeaturesService.NewsEntity> newsEntities = async.get_Items();
						featuresDto1.Version = newsEntities.Max<FeaturesService.NewsEntity>((FeaturesService.NewsEntity x) => x.get_Version());
					}
				}
				catch
				{
				}
			}
			FeaturesDto featuresDto2 = featuresDto;
			featuresDto = null;
			return featuresDto2;
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