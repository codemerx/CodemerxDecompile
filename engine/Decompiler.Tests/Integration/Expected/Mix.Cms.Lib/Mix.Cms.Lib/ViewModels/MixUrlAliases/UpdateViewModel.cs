using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixCultures;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixUrlAliases
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixUrlAlias, Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>
	{
		[JsonProperty("alias")]
		[Required]
		public string Alias
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

		[JsonProperty("description")]
		public string Description
		{
			get;
			set;
		}

		[JsonProperty("id")]
		public int Id
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

		[JsonProperty("sourceId")]
		public string SourceId
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

		[JsonProperty("type")]
		public MixEnums.UrlAliasType Type
		{
			get;
			set;
		}

		public UpdateViewModel()
		{
			base();
			return;
		}

		public UpdateViewModel(MixUrlAlias model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.set_Cultures(this.LoadCultures(this.get_Specificulture(), _context, _transaction));
			return;
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
						V_3 = new Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel.u003cu003ec__DisplayClass57_0();
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
							stackVariable58 = _context.get_MixUrlAlias();
							V_5 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
							// Current member / type: System.Collections.Generic.List`1<Mix.Domain.Core.Models.SupportedCulture> Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel::LoadCultures(System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
							// Exception in: System.Collections.Generic.List<Mix.Domain.Core.Models.SupportedCulture> LoadCultures(System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
							// Specified method is not supported.
							// 
							// mailto: JustDecompilePublicFeedback@telerik.com


		public override MixUrlAlias ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.get_Id() == 0)
			{
				stackVariable7 = ViewModelBase<MixCmsContext, MixUrlAlias, Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>.Repository;
				V_0 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
				// Current member / type: Mix.Cms.Lib.Models.Cms.MixUrlAlias Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel::ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: Mix.Cms.Lib.Models.Cms.MixUrlAlias ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public override void Validate(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			this.Validate(_context, _transaction);
			if (this.get_IsValid() && ViewModelBase<MixCmsContext, MixUrlAlias, Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>.Repository.CheckIsExists(new Func<MixUrlAlias, bool>(this.u003cValidateu003eb__56_0), null, null))
			{
				this.get_Errors().Add("Alias Existed");
				this.set_IsValid(false);
			}
			return;
		}
	}
}