using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixThemes
{
	public class ReadViewModel : ViewModelBase<MixCmsContext, MixTheme, ReadViewModel>
	{
		[JsonProperty("asset")]
		public IFormFile Asset
		{
			get;
			set;
		}

		[JsonProperty("assetFolder")]
		public string AssetFolder
		{
			get
			{
				return string.Concat("wwwroot/content/templates/", this.get_Name(), "/assets");
			}
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

		[JsonProperty("domain")]
		public string Domain
		{
			get
			{
				return MixService.GetConfig<string>("Domain");
			}
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
				if (string.IsNullOrEmpty(this.get_Image()) || this.get_Image().IndexOf("http") != -1 || this.get_Image().get_Chars(0) == '/')
				{
					return this.get_Image();
				}
				stackVariable16 = new string[2];
				stackVariable16[0] = this.get_Domain();
				stackVariable16[1] = this.get_Image();
				return CommonHelper.GetFullPath(stackVariable16);
			}
		}

		[JsonProperty("isActived")]
		public bool IsActived
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

		[JsonProperty("name")]
		public string Name
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

		[JsonProperty("status")]
		public MixEnums.MixContentStatus Status
		{
			get;
			set;
		}

		[JsonProperty("templateFolder")]
		public string TemplateFolder
		{
			get
			{
				return string.Concat("Views/Shared/Templates/", this.get_Name());
			}
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
				if (this.get_Thumbnail() == null || this.get_Thumbnail().IndexOf("http") != -1 || this.get_Thumbnail().get_Chars(0) == '/')
				{
					if (!string.IsNullOrEmpty(this.get_Thumbnail()))
					{
						return this.get_Thumbnail();
					}
					return this.get_ImageUrl();
				}
				stackVariable20 = new string[2];
				stackVariable20[0] = this.get_Domain();
				stackVariable20[1] = this.get_Thumbnail();
				return CommonHelper.GetFullPath(stackVariable20);
			}
		}

		[JsonProperty("title")]
		public string Title
		{
			get;
			set;
		}

		public string UploadsFolder
		{
			get
			{
				return string.Concat("wwwroot/content/templates/", this.get_Name(), "/uploads");
			}
		}

		public ReadViewModel()
		{
			base();
			return;
		}

		public ReadViewModel(MixTheme model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}
	}
}