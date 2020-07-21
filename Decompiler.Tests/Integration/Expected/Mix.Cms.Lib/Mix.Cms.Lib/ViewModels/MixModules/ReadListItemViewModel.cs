using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Domain.Core.Models;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixModules
{
	public class ReadListItemViewModel : ViewModelBase<MixCmsContext, MixModule, ReadListItemViewModel>
	{
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

		[JsonProperty("detailsUrl")]
		public string DetailsUrl
		{
			get
			{
				return string.Format("/module/{0}/{1}/{2}", this.get_Specificulture(), this.get_Id(), SeoHelper.GetSEOString(this.get_Title(), '-'));
			}
		}

		[JsonProperty("edmTemplate")]
		public string EdmTemplate
		{
			get;
			set;
		}

		[JsonProperty("fields")]
		public string Fields
		{
			get;
			set;
		}

		[JsonProperty("formTemplate")]
		public string FormTemplate
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

		[JsonProperty("image")]
		public string Image
		{
			get;
			set;
		}

		[JsonProperty("imageUrl")]
		public string ImageUrl
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.get_Image()) || this.get_Image().IndexOf("http") != -1 || this.get_Image().get_Chars(0) == '/')
				{
					return this.get_Image();
				}
				return string.Concat(MixService.GetConfig<string>("Domain"), "/", this.get_Image());
			}
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

		[JsonProperty("name")]
		public string Name
		{
			get;
			set;
		}

		[JsonProperty("pageSize")]
		public int? PageSize
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

		[JsonProperty("specificulture")]
		public string Specificulture
		{
			get;
			set;
		}

		[JsonConverter(typeof(StringEnumConverter))]
		[JsonProperty("status")]
		public MixEnums.MixContentStatus Status
		{
			get;
			set;
		}

		[JsonProperty("template")]
		public string Template
		{
			get;
			set;
		}

		[JsonProperty("thumbnail")]
		public string Thumbnail
		{
			get;
			set;
		}

		[JsonProperty("thumbnailUrl")]
		public string ThumbnailUrl
		{
			get
			{
				if (this.get_Thumbnail() != null && this.get_Thumbnail().IndexOf("http") == -1 && this.get_Thumbnail().get_Chars(0) != '/')
				{
					return string.Concat(MixService.GetConfig<string>("Domain"), "/", this.get_Thumbnail());
				}
				if (!string.IsNullOrEmpty(this.get_Thumbnail()))
				{
					return this.get_Thumbnail();
				}
				return this.get_ImageUrl();
			}
		}

		[JsonProperty("title")]
		public string Title
		{
			get;
			set;
		}

		[JsonProperty("type")]
		public MixEnums.MixModuleType Type
		{
			get;
			set;
		}

		public ReadListItemViewModel()
		{
			base();
			return;
		}

		public ReadListItemViewModel(MixModule model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			return;
		}

		public override Task<bool> ExpandViewAsync(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			return this.ExpandViewAsync(_context, _transaction);
		}
	}
}