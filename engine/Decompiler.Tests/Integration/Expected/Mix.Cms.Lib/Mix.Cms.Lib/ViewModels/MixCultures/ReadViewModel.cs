using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixConfigurations;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixCultures
{
	public class ReadViewModel : ViewModelBase<MixCmsContext, MixCulture, Mix.Cms.Lib.ViewModels.MixCultures.ReadViewModel>
	{
		[JsonProperty("alias")]
		public string Alias
		{
			get;
			set;
		}

		[JsonProperty("configurations")]
		public List<ReadMvcViewModel> Configurations
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

		[JsonProperty("description")]
		public string Description
		{
			get;
			set;
		}

		[JsonProperty("fullName")]
		public string FullName
		{
			get;
			set;
		}

		[JsonProperty("icon")]
		public string Icon
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

		[JsonProperty("lcid")]
		public string Lcid
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

		public ReadViewModel()
		{
		}

		public ReadViewModel(MixCulture model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}
	}
}