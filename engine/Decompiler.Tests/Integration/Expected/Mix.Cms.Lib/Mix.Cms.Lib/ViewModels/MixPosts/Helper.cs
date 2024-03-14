using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Common.Helper;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Mix.Heart.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixPosts
{
	public class Helper
	{
		public Helper()
		{
		}

		public static async Task<RepositoryResponse<PaginationModel<TView>>> GetModelistByMeta<TView>(string metaName, string metaValue, string culture, string orderByPropertyName, MixHeartEnums.DisplayDirection direction, int? pageSize, int? pageIndex, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		where TView : ViewModelBase<MixCmsContext, MixPost, TView>
		{
			Helper.u003cGetModelistByMetau003ed__0<TView> variable = new Helper.u003cGetModelistByMetau003ed__0<TView>();
			variable.metaName = metaName;
			variable.metaValue = metaValue;
			variable.culture = culture;
			variable.orderByPropertyName = orderByPropertyName;
			variable.direction = direction;
			variable.pageSize = pageSize;
			variable.pageIndex = pageIndex;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<PaginationModel<TView>>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Helper.u003cGetModelistByMetau003ed__0<TView>>(ref variable);
			return variable.u003cu003et__builder.Task;
		}
	}
}