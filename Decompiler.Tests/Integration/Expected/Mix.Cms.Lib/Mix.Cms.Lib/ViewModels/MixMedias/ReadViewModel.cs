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
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixMedias
{
	public class ReadViewModel : ViewModelBase<MixCmsContext, MixMedia, Mix.Cms.Lib.ViewModels.MixMedias.ReadViewModel>
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
				return MixService.GetConfig<string>("Domain");
			}
		}

		[JsonProperty("extension")]
		[Required(ErrorMessage="Please choose File")]
		public string Extension
		{
			get;
			set;
		}

		[JsonProperty("fileFolder")]
		public string FileFolder
		{
			get;
			set;
		}

		[JsonProperty("fileName")]
		public string FileName
		{
			get;
			set;
		}

		[JsonProperty("filePath")]
		public string FilePath
		{
			get
			{
				if (string.IsNullOrEmpty(this.get_FileName()) || !string.IsNullOrEmpty(this.get_TargetUrl()))
				{
					return this.get_TargetUrl();
				}
				if (this.get_FileFolder().IndexOf("http") > 0)
				{
					return string.Concat(this.get_FileFolder(), "/", this.get_FileName(), this.get_Extension());
				}
				stackVariable22 = new string[5];
				stackVariable22[0] = "/";
				stackVariable22[1] = this.get_FileFolder();
				stackVariable22[2] = "/";
				stackVariable22[3] = this.get_FileName();
				stackVariable22[4] = this.get_Extension();
				return string.Concat(stackVariable22);
			}
		}

		[JsonProperty("fileSize")]
		public int FileSize
		{
			get;
			set;
		}

		[JsonProperty("fileType")]
		public string FileType
		{
			get;
			set;
		}

		[JsonProperty("fullPath")]
		public string FullPath
		{
			get
			{
				if (string.IsNullOrEmpty(this.get_FileName()) || !string.IsNullOrEmpty(this.get_TargetUrl()))
				{
					return this.get_TargetUrl();
				}
				if (this.get_FileFolder().IndexOf("http") > 0)
				{
					return string.Concat(this.get_FileFolder(), "/", this.get_FileName(), this.get_Extension());
				}
				stackVariable22 = new string[6];
				stackVariable22[0] = this.get_Domain();
				stackVariable22[1] = "/";
				stackVariable22[2] = this.get_FileFolder();
				stackVariable22[3] = "/";
				stackVariable22[4] = this.get_FileName();
				stackVariable22[5] = this.get_Extension();
				return string.Concat(stackVariable22);
			}
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

		[JsonProperty("mediaFile")]
		public FileViewModel MediaFile
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

		[JsonProperty("source")]
		public string Source
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

		[JsonProperty("tags")]
		public string Tags
		{
			get;
			set;
		}

		[JsonProperty("targetUrl")]
		public string TargetUrl
		{
			get;
			set;
		}

		[JsonProperty("title")]
		public string Title
		{
			get;
			set;
		}

		public ReadViewModel()
		{
			base();
			return;
		}

		public ReadViewModel(MixMedia model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
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
						V_3 = new Mix.Cms.Lib.ViewModels.MixMedias.ReadViewModel.u003cu003ec__DisplayClass80_0();
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
							stackVariable58 = _context.get_MixMedia();
							V_5 = Expression.Parameter(Type.GetTypeFromHandle(// 
							// Current member / type: System.Collections.Generic.List`1<Mix.Domain.Core.Models.SupportedCulture> Mix.Cms.Lib.ViewModels.MixMedias.ReadViewModel::LoadCultures(System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
							// Exception in: System.Collections.Generic.List<Mix.Domain.Core.Models.SupportedCulture> LoadCultures(System.String,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
							// Specified method is not supported.
							// 
							// mailto: JustDecompilePublicFeedback@telerik.com

	}
}