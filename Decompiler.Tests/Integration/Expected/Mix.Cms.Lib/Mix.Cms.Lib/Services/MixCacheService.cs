using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels;
using Mix.Domain.Data.Repository;
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
					V_0 = MixCacheService.syncRoot;
					V_1 = false;
					try
					{
						Monitor.Enter(V_0, ref V_1);
						if (MixCacheService.instance == null)
						{
							MixCacheService.instance = new MixCacheService();
						}
					}
					finally
					{
						if (V_1)
						{
							Monitor.Exit(V_0);
						}
					}
				}
				return MixCacheService.instance;
			}
		}

		static MixCacheService()
		{
			MixCacheService.syncRoot = new object();
			return;
		}

		public MixCacheService()
		{
			base();
			this.Repository = DefaultModelRepository<MixCmsContext, MixCache>.get_Instance();
			return;
		}

		public static Task<T> GetAsync<T>(string key)
		{
			V_0 = FileRepository.get_Instance().GetFile(key, ".json", "cache", false, "{}");
			if (V_0 == null || string.IsNullOrEmpty(V_0.get_Content()))
			{
				V_1 = default(T);
				return Task.FromResult<T>(V_1);
			}
			return Task.FromResult<T>(JObject.Parse(V_0.get_Content()).ToObject<T>());
		}

		public static Task RemoveCacheAsync()
		{
			return Task.FromResult<bool>(FileRepository.get_Instance().EmptyFolder("cache"));
		}
	}
}