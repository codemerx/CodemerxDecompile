using Mix.Cms.Lib.Models.Cms;
using Newtonsoft.Json;
using System;
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
			V_0 = new DashboardViewModel.u003cu003ec__DisplayClass20_0();
			V_0.culture = culture;
			base();
			V_1 = new MixCmsContext();
			try
			{
				stackVariable7 = V_1.get_MixPage();
				V_2 = Expression.Parameter(Type.GetTypeFromHandle(// 
				// Current member / type: System.Void Mix.Cms.Lib.ViewModels.DashboardViewModel::.ctor(System.String)
				// Exception in: System.Void .ctor(System.String)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com

	}
}