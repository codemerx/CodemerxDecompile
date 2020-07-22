using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Mix.Heart.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixPosts
{
	public class Helper
	{
		public Helper()
		{
			base();
			return;
		}

		public static async Task<RepositoryResponse<PaginationModel<TView>>> GetModelistByMeta<TView>(string metaName, string metaValue, string culture, string orderByPropertyName, MixHeartEnums.DisplayDirection direction, int? pageSize, int? pageIndex, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		where TView : ViewModelBase<MixCmsContext, MixPost, TView>
		{
			V_0.metaName = metaName;
			V_0.metaValue = metaValue;
			V_0.culture = culture;
			V_0.orderByPropertyName = orderByPropertyName;
			V_0.direction = direction;
			V_0.pageSize = pageSize;
			V_0.pageIndex = pageIndex;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<PaginationModel<TView>>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Helper.u003cGetModelistByMetau003ed__0<TView>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}