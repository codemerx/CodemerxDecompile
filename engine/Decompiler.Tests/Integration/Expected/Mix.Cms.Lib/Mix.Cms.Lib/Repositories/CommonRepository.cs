using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixCultures;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
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
					V_0 = CommonRepository.syncRoot;
					V_1 = false;
					try
					{
						Monitor.Enter(V_0, ref V_1);
						if (CommonRepository.instance == null)
						{
							CommonRepository.instance = new CommonRepository();
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
				return CommonRepository.instance;
			}
		}

		static CommonRepository()
		{
			CommonRepository.syncRoot = new object();
			return;
		}

		private CommonRepository()
		{
			base();
			return;
		}

		public List<SupportedCulture> LoadCultures(string initCulture = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = ViewModelBase<MixCmsContext, MixCulture, SystemCultureViewModel>.Repository.GetModelList(_context, _transaction);
			V_1 = new List<SupportedCulture>();
			if (V_0.get_IsSucceed())
			{
				V_2 = V_0.get_Data().GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						stackVariable16 = new SupportedCulture();
						stackVariable16.set_Icon(V_3.get_Icon());
						stackVariable16.set_Specificulture(V_3.get_Specificulture());
						stackVariable16.set_Alias(V_3.get_Alias());
						stackVariable16.set_FullName(V_3.get_FullName());
						stackVariable16.set_Description(V_3.get_FullName());
						stackVariable16.set_Id(V_3.get_Id());
						stackVariable16.set_Lcid(V_3.get_Lcid());
						stackVariable16.set_IsSupported(string.op_Equality(V_3.get_Specificulture(), initCulture));
						V_1.Add(stackVariable16);
					}
				}
				finally
				{
					V_2.Dispose();
				}
			}
			return V_1;
		}
	}
}