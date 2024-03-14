using Mix.Cms.Lib.Models.Cms;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels
{
	public class DashboardViewModel
	{
		[JsonProperty("totalModule")]
		public int TotalModule
		{
			get;
			set;
		}

		[JsonProperty("totalPage")]
		public int TotalPage
		{
			get;
			set;
		}

		[JsonProperty("totalPost")]
		public int TotalPost
		{
			get;
			set;
		}

		[JsonProperty("totalProduct")]
		public int TotalProduct
		{
			get;
			set;
		}

		[JsonProperty("totalUser")]
		public int TotalUser
		{
			get;
			set;
		}

		public DashboardViewModel(string culture)
		{
			using (MixCmsContext mixCmsContext = new MixCmsContext())
			{
				this.TotalPage = mixCmsContext.MixPage.Count<MixPage>((MixPage p) => p.Specificulture == culture);
				this.TotalPost = mixCmsContext.MixPost.Count<MixPost>((MixPost p) => p.Specificulture == culture);
				this.TotalUser = mixCmsContext.MixCmsUser.Count<MixCmsUser>();
			}
		}
	}
}