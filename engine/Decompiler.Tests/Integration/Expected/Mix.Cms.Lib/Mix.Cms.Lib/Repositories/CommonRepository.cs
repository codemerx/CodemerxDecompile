using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixCultures;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using System;
using System.Collections.Generic;

namespace Mix.Cms.Lib.Repositories
{
	public class CommonRepository
	{
		private static volatile CommonRepository instance;

		private readonly static object syncRoot;

		public static CommonRepository Instance
		{
			get
			{
				if (CommonRepository.instance == null)
				{
					lock (CommonRepository.syncRoot)
					{
						if (CommonRepository.instance == null)
						{
							CommonRepository.instance = new CommonRepository();
						}
					}
				}
				return CommonRepository.instance;
			}
		}

		static CommonRepository()
		{
			CommonRepository.syncRoot = new object();
		}

		private CommonRepository()
		{
		}

		public List<SupportedCulture> LoadCultures(string initCulture = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<List<SystemCultureViewModel>> modelList = ViewModelBase<MixCmsContext, MixCulture, SystemCultureViewModel>.Repository.GetModelList(_context, _transaction);
			List<SupportedCulture> supportedCultures = new List<SupportedCulture>();
			if (modelList.get_IsSucceed())
			{
				foreach (SystemCultureViewModel datum in modelList.get_Data())
				{
					SupportedCulture supportedCulture = new SupportedCulture();
					supportedCulture.set_Icon(datum.Icon);
					supportedCulture.set_Specificulture(datum.Specificulture);
					supportedCulture.set_Alias(datum.Alias);
					supportedCulture.set_FullName(datum.FullName);
					supportedCulture.set_Description(datum.FullName);
					supportedCulture.set_Id(datum.Id);
					supportedCulture.set_Lcid(datum.Lcid);
					supportedCulture.set_IsSupported(datum.Specificulture == initCulture);
					supportedCultures.Add(supportedCulture);
				}
			}
			return supportedCultures;
		}
	}
}