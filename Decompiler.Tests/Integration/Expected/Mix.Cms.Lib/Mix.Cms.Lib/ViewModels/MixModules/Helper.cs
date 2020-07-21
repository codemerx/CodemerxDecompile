using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixCultures;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixModules
{
	public class Helper
	{
		public Helper()
		{
			base();
			return;
		}

		public static RepositoryResponse<Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel> GetBy(Expression<Func<MixModule, bool>> predicate, string postId = null, string productId = null, int pageId = 0, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.UpdateViewModel>.Repository.GetSingleModel(predicate, _context, _transaction);
			if (V_0.get_IsSucceed())
			{
				V_0.get_Data().set_PostId(postId);
				V_0.get_Data().set_PageId(pageId);
			}
			return V_0;
		}

		public static async Task<RepositoryResponse<bool>> Import(List<MixModule> arrModule, string destCulture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.arrModule = arrModule;
			V_0.destCulture = destCulture;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Helper.u003cImportu003ed__0>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public static List<SupportedCulture> LoadCultures(int id, string initCulture = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = new Helper.u003cu003ec__DisplayClass1_0();
			V_0.id = id;
			V_1 = ViewModelBase<MixCmsContext, MixCulture, SystemCultureViewModel>.Repository.GetModelList(_context, _transaction);
			V_2 = new List<SupportedCulture>();
			if (V_1.get_IsSucceed())
			{
				V_3 = V_1.get_Data().GetEnumerator();
				try
				{
					while (V_3.MoveNext())
					{
						V_4 = new Helper.u003cu003ec__DisplayClass1_1();
						V_4.CSu0024u003cu003e8__locals1 = V_0;
						V_4.culture = V_3.get_Current();
						stackVariable22 = V_2;
						V_5 = new SupportedCulture();
						V_5.set_Icon(V_4.culture.get_Icon());
						V_5.set_Specificulture(V_4.culture.get_Specificulture());
						V_5.set_Alias(V_4.culture.get_Alias());
						V_5.set_FullName(V_4.culture.get_FullName());
						V_5.set_Description(V_4.culture.get_FullName());
						V_5.set_Id(V_4.culture.get_Id());
						V_5.set_Lcid(V_4.culture.get_Lcid());
						stackVariable52 = V_5;
						if (string.op_Equality(V_4.culture.get_Specificulture(), initCulture))
						{
							stackVariable58 = true;
						}
						else
						{
							stackVariable61 = _context.get_MixModule();
							V_6 = Expression.Parameter(Type.GetTypeFromHandle(// 
							// Current member / type: System.Collections.Generic.List`1<Mix.Domain.Core.Models.SupportedCulture> Mix.Cms.Lib.ViewModels.MixModules.Helper::LoadCultures(System.Int32,System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
							// Exception in: System.Collections.Generic.List<Mix.Domain.Core.Models.SupportedCulture> LoadCultures(System.Int32,System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
							// Specified method is not supported.
							// 
							// mailto: JustDecompilePublicFeedback@telerik.com

	}
}