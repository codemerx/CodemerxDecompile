using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixCultures;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixLanguages
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixLanguage, Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel>
	{
		[JsonProperty("category")]
		public string Category
		{
			get;
			set;
		}

		[JsonProperty("createdBy")]
		public string CreatedBy
		{
			get;
			set;
		}

		[JsonProperty("createdDateTime")]
		public DateTime CreatedDateTime
		{
			get;
			set;
		}

		[JsonProperty("cultures")]
		public List<SupportedCulture> Cultures
		{
			get;
			set;
		}

		[JsonProperty("dataType")]
		public MixEnums.MixDataType DataType
		{
			get;
			set;
		}

		[JsonProperty("defaultValue")]
		public string DefaultValue
		{
			get;
			set;
		}

		[JsonProperty("description")]
		public string Description
		{
			get;
			set;
		}

		[JsonProperty("domain")]
		public string Domain
		{
			get
			{
				return MixService.GetConfig<string>("Domain", this.get_Specificulture());
			}
		}

		[JsonProperty("id")]
		public int Id
		{
			get;
			set;
		}

		[JsonProperty("keyword")]
		[Required]
		public string Keyword
		{
			get;
			set;
		}

		[JsonProperty("lastModified")]
		public DateTime? LastModified
		{
			get;
			set;
		}

		[JsonProperty("modifiedBy")]
		public string ModifiedBy
		{
			get;
			set;
		}

		[JsonProperty("priority")]
		public int Priority
		{
			get;
			set;
		}

		[JsonProperty("property")]
		public DataValueViewModel Property
		{
			get;
			set;
		}

		[JsonProperty("specificulture")]
		public string Specificulture
		{
			get;
			set;
		}

		[JsonProperty("status")]
		public MixEnums.MixContentStatus Status
		{
			get;
			set;
		}

		[JsonProperty("value")]
		public string Value
		{
			get;
			set;
		}

		public UpdateViewModel()
		{
			base();
			return;
		}

		public UpdateViewModel(MixLanguage model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.set_Cultures(this.LoadCultures(this.get_Specificulture(), _context, _transaction));
			stackVariable8 = new DataValueViewModel();
			stackVariable8.set_DataType(this.get_DataType());
			stackVariable8.set_Value(this.get_Value());
			stackVariable8.set_Name(this.get_Keyword());
			this.set_Property(stackVariable8);
			stackVariable16 = this.get_Cultures();
			stackVariable17 = Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cu003ec.u003cu003e9__74_0;
			if (stackVariable17 == null)
			{
				dummyVar0 = stackVariable17;
				stackVariable17 = new Action<SupportedCulture>(Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cu003ec.u003cu003e9.u003cExpandViewu003eb__74_0);
				Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cu003ec.u003cu003e9__74_0 = stackVariable17;
			}
			stackVariable16.ForEach(stackVariable17);
			return;
		}

		public override Task<bool> ExpandViewAsync(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.set_Cultures(this.LoadCultures(this.get_Specificulture(), _context, _transaction));
			stackVariable8 = new DataValueViewModel();
			stackVariable8.set_DataType(this.get_DataType());
			stackVariable8.set_Value(this.get_Value());
			stackVariable8.set_Name(this.get_Keyword());
			this.set_Property(stackVariable8);
			stackVariable16 = this.get_Cultures();
			stackVariable17 = Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cu003ec.u003cu003e9__70_0;
			if (stackVariable17 == null)
			{
				dummyVar0 = stackVariable17;
				stackVariable17 = new Action<SupportedCulture>(Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cu003ec.u003cu003e9.u003cExpandViewAsyncu003eb__70_0);
				Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cu003ec.u003cu003e9__70_0 = stackVariable17;
			}
			stackVariable16.ForEach(stackVariable17);
			return this.ExpandViewAsync(_context, _transaction);
		}

		public static async Task<RepositoryResponse<bool>> ImportLanguages(List<MixLanguage> arrLanguage, string destCulture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.arrLanguage = arrLanguage;
			V_0.destCulture = destCulture;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cImportLanguagesu003ed__76>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private List<SupportedCulture> LoadCultures(string initCulture = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
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
						V_3 = new Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cu003ec__DisplayClass77_0();
						V_3.u003cu003e4__this = this;
						V_3.culture = V_2.get_Current();
						stackVariable19 = V_1;
						V_4 = new SupportedCulture();
						V_4.set_Icon(V_3.culture.get_Icon());
						V_4.set_Specificulture(V_3.culture.get_Specificulture());
						V_4.set_Alias(V_3.culture.get_Alias());
						V_4.set_FullName(V_3.culture.get_FullName());
						V_4.set_Description(V_3.culture.get_FullName());
						V_4.set_Id(V_3.culture.get_Id());
						V_4.set_Lcid(V_3.culture.get_Lcid());
						stackVariable49 = V_4;
						if (string.op_Equality(V_3.culture.get_Specificulture(), initCulture))
						{
							stackVariable55 = true;
						}
						else
						{
							stackVariable58 = _context.get_MixLanguage();
							V_5 = Expression.Parameter(Type.GetTypeFromHandle(// 
							// Current member / type: System.Collections.Generic.List`1<Mix.Domain.Core.Models.SupportedCulture> Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel::LoadCultures(System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
							// Exception in: System.Collections.Generic.List<Mix.Domain.Core.Models.SupportedCulture> LoadCultures(System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
							// Specified method is not supported.
							// 
							// mailto: JustDecompilePublicFeedback@telerik.com


		public override MixLanguage ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.get_Id() == 0)
			{
				stackVariable25 = ViewModelBase<MixCmsContext, MixLanguage, Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel>.Repository;
				V_0 = Expression.Parameter(Type.GetTypeFromHandle(// 
				// Current member / type: Mix.Cms.Lib.Models.Cms.MixLanguage Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel::ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: Mix.Cms.Lib.Models.Cms.MixLanguage ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public override Task<Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel> ParseViewAsync(bool isExpand = true, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable1 = new DataValueViewModel();
			stackVariable1.set_DataType(this.get_DataType());
			stackVariable1.set_Value(this.get_Value());
			stackVariable1.set_Name(this.get_Keyword());
			this.set_Property(stackVariable1);
			return this.ParseViewAsync(isExpand, _context, _transaction);
		}

		public override async Task<RepositoryResponse<MixLanguage>> RemoveModelAsync(bool isRemoveRelatedModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.isRemoveRelatedModels = isRemoveRelatedModels;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<MixLanguage>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cRemoveModelAsyncu003ed__73>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override RepositoryResponse<bool> RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = this.get_Cultures().Where<SupportedCulture>(new Func<SupportedCulture, bool>(this.u003cRemoveRelatedModelsu003eb__75_0)).GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = new Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cu003ec__DisplayClass75_0();
					V_1.u003cu003e4__this = this;
					V_1.culture = V_0.get_Current();
					stackVariable16 = _context.get_MixLanguage();
					V_3 = Expression.Parameter(Type.GetTypeFromHandle(// 
					// Current member / type: Mix.Domain.Core.ViewModels.RepositoryResponse`1<System.Boolean> Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel::RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Exception in: Mix.Domain.Core.ViewModels.RepositoryResponse<System.Boolean> RemoveRelatedModels(Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com


		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0._context = _context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cRemoveRelatedModelsAsyncu003ed__71>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override async Task<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel>> SaveModelAsync(bool isSaveSubModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.isSaveSubModels = isSaveSubModels;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixLanguages.UpdateViewModel.u003cSaveModelAsyncu003ed__72>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}