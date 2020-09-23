using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Account;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixThemes;
using Mix.Cms.Messenger.Models.Data;
using Mix.Domain.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.Services
{
	public class InitCmsService
	{
		public InitCmsService()
		{
			base();
			return;
		}

		public static async Task<RepositoryResponse<bool>> InitAttributeSetsAsync(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<InitCmsService.u003cInitAttributeSetsAsyncu003ed__3>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public static async Task<RepositoryResponse<bool>> InitCms(string siteName, InitCulture culture)
		{
			V_0.siteName = siteName;
			V_0.culture = culture;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<InitCmsService.u003cInitCmsu003ed__1>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public static async Task<RepositoryResponse<bool>> InitConfigurationsAsync(string siteName, string specifiCulture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.siteName = siteName;
			V_0.specifiCulture = specifiCulture;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<InitCmsService.u003cInitConfigurationsAsyncu003ed__2>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		protected static bool InitCultures(InitCulture culture, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0 = true;
			try
			{
				if (context.get_MixCulture().Count<MixCulture>() == 0)
				{
					stackVariable5 = new MixCulture();
					stackVariable5.set_Id(1);
					stackVariable5.set_Specificulture(culture.get_Specificulture());
					stackVariable5.set_FullName(culture.get_FullName());
					stackVariable5.set_Description(culture.get_Description());
					stackVariable5.set_Icon(culture.get_Icon());
					stackVariable5.set_Alias(culture.get_Alias());
					stackVariable5.set_Status(2.ToString());
					stackVariable5.set_CreatedDateTime(DateTime.get_UtcNow());
					context.Entry<MixCulture>(stackVariable5).set_State(4);
					dummyVar0 = context.SaveChanges();
				}
			}
			catch
			{
				dummyVar1 = exception_0;
				V_0 = false;
			}
			return V_0;
		}

		public async Task<RepositoryResponse<bool>> InitLanguagesAsync(string specificulture, List<MixLanguage> languages, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.specificulture = specificulture;
			V_0.languages = languages;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<InitCmsService.u003cInitLanguagesAsyncu003ed__4>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		protected static void InitPages(string culture, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0 = JObject.Parse(FileRepository.get_Instance().GetFile("pages.json", "data", true, "{}").get_Content()).get_Item("data").ToObject<List<MixPage>>().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_1.set_Specificulture(culture);
					V_1.set_SeoTitle(V_1.get_Title().ToLower());
					V_1.set_SeoName(SeoHelper.GetSEOString(V_1.get_Title(), '-'));
					V_1.set_SeoDescription(V_1.get_Title().ToLower());
					V_1.set_SeoKeywords(V_1.get_Title().ToLower());
					V_1.set_CreatedDateTime(DateTime.get_UtcNow());
					V_1.set_CreatedBy("SuperAdmin");
					context.Entry<MixPage>(V_1).set_State(4);
					stackVariable43 = new MixUrlAlias();
					stackVariable43.set_Id(V_1.get_Id());
					stackVariable43.set_SourceId(V_1.get_Id().ToString());
					stackVariable43.set_Type(0);
					stackVariable43.set_Specificulture(culture);
					stackVariable43.set_CreatedDateTime(DateTime.get_UtcNow());
					stackVariable43.set_Alias(V_1.get_Title().ToLower());
					stackVariable43.set_Status(2.ToString());
					context.Entry<MixUrlAlias>(stackVariable43).set_State(4);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		public async Task<RepositoryResponse<bool>> InitThemesAsync(string siteName, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.siteName = siteName;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<InitCmsService.u003cInitThemesAsyncu003ed__5>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}