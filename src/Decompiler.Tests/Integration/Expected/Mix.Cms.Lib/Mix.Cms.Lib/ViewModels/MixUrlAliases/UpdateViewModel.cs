using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixCultures;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
		}

		public UpdateViewModel(MixUrlAlias model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.Cultures = this.LoadCultures(this.Specificulture, _context, _transaction);
		}

		private List<SupportedCulture> LoadCultures(string initCulture = null, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<List<SystemCultureViewModel>> modelList = ViewModelBase<MixCmsContext, MixCulture, SystemCultureViewModel>.Repository.GetModelList(_context, _transaction);
			List<SupportedCulture> supportedCultures = new List<SupportedCulture>();
			if (modelList.get_IsSucceed())
			{
				foreach (SystemCultureViewModel datum in modelList.get_Data())
				{
					List<SupportedCulture> supportedCultures1 = supportedCultures;
					SupportedCulture supportedCulture = new SupportedCulture();
					supportedCulture.set_Icon(datum.Icon);
					supportedCulture.set_Specificulture(datum.Specificulture);
					supportedCulture.set_Alias(datum.Alias);
					supportedCulture.set_FullName(datum.FullName);
					supportedCulture.set_Description(datum.FullName);
					supportedCulture.set_Id(datum.Id);
					supportedCulture.set_Lcid(datum.Lcid);
					supportedCulture.set_IsSupported((datum.Specificulture == initCulture ? true : _context.MixUrlAlias.Any<MixUrlAlias>((MixUrlAlias p) => p.Id == this.Id && p.Specificulture == datum.Specificulture)));
					supportedCultures1.Add(supportedCulture);
				}
			}
			return supportedCultures;
		}

		public override MixUrlAlias ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.Id == 0)
			{
				this.Id = ViewModelBase<MixCmsContext, MixUrlAlias, Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>.Repository.Max((MixUrlAlias c) => c.Id, null, null).get_Data() + 1;
				this.CreatedDateTime = DateTime.UtcNow;
				this.Cultures = this.Cultures ?? this.LoadCultures(this.Specificulture, _context, _transaction);
				this.Cultures.ForEach((SupportedCulture c) => c.set_IsSupported(true));
			}
			return base.ParseModel(_context, _transaction);
		}

		public override void Validate(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			base.Validate(_context, _transaction);
			if (base.get_IsValid() && ViewModelBase<MixCmsContext, MixUrlAlias, Mix.Cms.Lib.ViewModels.MixUrlAliases.UpdateViewModel>.Repository.CheckIsExists((MixUrlAlias o) => {
				if (!(o.Alias == this.Alias) || !(o.Specificulture == this.Specificulture))
				{
					return false;
				}
				return o.Id != this.Id;
			}, null, null))
			{
				base.get_Errors().Add("Alias Existed");
				base.set_IsValid(false);
			}
		}
	}
}