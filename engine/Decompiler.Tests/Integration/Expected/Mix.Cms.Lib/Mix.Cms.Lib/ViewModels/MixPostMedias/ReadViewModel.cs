using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixMedias;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixPostMedias
{
	public class ReadViewModel : ViewModelBase<MixCmsContext, MixPostMedia, Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel>
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

		public UpdateViewModel Media
		{
			get;
			set;
		}

		[JsonProperty("mediaId")]
		public int MediaId
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

		[JsonProperty("postId")]
		public int PostId
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

		public ReadViewModel(MixPostMedia model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public ReadViewModel()
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<UpdateViewModel> singleModel = ViewModelBase<MixCmsContext, MixMedia, UpdateViewModel>.Repository.GetSingleModel((MixMedia p) => p.Id == this.MediaId && p.Specificulture == this.Specificulture, _context, _transaction);
			if (singleModel.get_IsSucceed())
			{
				this.Media = singleModel.get_Data();
			}
		}

		public override MixPostMedia ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.Id == 0)
			{
				this.Id = ViewModelBase<MixCmsContext, MixPostMedia, Mix.Cms.Lib.ViewModels.MixPostMedias.ReadViewModel>.Repository.Max((MixPostMedia m) => m.Id, _context, _transaction).get_Data() + 1;
			}
			return base.ParseModel(_context, _transaction);
		}

		public override RepositoryResponse<bool> SaveSubModels(MixPostMedia parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			RepositoryResponse<UpdateViewModel> repositoryResponse2 = this.Media.SaveModel(false, _context, _transaction);
			if (!repositoryResponse2.get_IsSucceed())
			{
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse1.set_Exception(repositoryResponse2.get_Exception());
				repositoryResponse1.set_Errors(repositoryResponse2.get_Errors());
			}
			return repositoryResponse1;
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixPostMedia parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			RepositoryResponse<UpdateViewModel> repositoryResponse2 = await this.Media.SaveModelAsync(false, _context, _transaction);
			if (!repositoryResponse2.get_IsSucceed())
			{
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse1.set_Exception(repositoryResponse2.get_Exception());
				repositoryResponse1.set_Errors(repositoryResponse2.get_Errors());
			}
			return repositoryResponse1;
		}
	}
}