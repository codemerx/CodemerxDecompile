using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Repositories;
using Mix.Cms.Lib.ViewModels;
using Mix.Domain.Data.Repository;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.Services
{
	public class MixCacheService
	{
		private readonly static object syncRoot;

		private static volatile MixCacheService instance;

		public DefaultModelRepository<MixCmsContext, MixCache> Repository;

		public static MixCacheService Instance
		{
			get
			{
				if (MixCacheService.instance == null)
				{
					lock (MixCacheService.syncRoot)
					{
						if (MixCacheService.instance == null)
						{
							MixCacheService.instance = new MixCacheService();
						}
					}
				}
				return MixCacheService.instance;
			}
		}

		static MixCacheService()
		{
			MixCacheService.syncRoot = new object();
		}

		public MixCacheService()
		{
			this.Repository = DefaultModelRepository<MixCmsContext, MixCache>.get_Instance();
		}

		public static Task<T> GetAsync<T>(string key)
		{
			FileViewModel file = FileRepository.Instance.GetFile(key, ".json", "cache", false, "{}");
			if (file == null || string.IsNullOrEmpty(file.Content))
			{
				return Task.FromResult<T>(default(T));
			}
			return Task.FromResult<T>(JObject.Parse(file.Content).ToObject<T>());
		}

		public static Task RemoveCacheAsync()
		{
			return Task.FromResult<bool>(FileRepository.Instance.EmptyFolder("cache"));
		}
	}
}