using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Account;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Repositories;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels;
using Mix.Common.Helper;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Mix.Identity.Models.AccountViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.Account
{
	public class UserInfoViewModel : ViewModelBase<MixCmsContext, MixCmsUser, UserInfoViewModel>
	{
		[JsonProperty("address")]
		public string Address
		{
			get;
			set;
		}

		[JsonProperty("avatar")]
		public string Avatar
		{
			get;
			set;
		}

		[JsonProperty("avatarUrl")]
		public string AvatarUrl
		{
			get
			{
				if (this.Avatar == null || this.Avatar.IndexOf("http") != -1 || this.Avatar[0] == '/')
				{
					return this.Avatar;
				}
				return CommonHelper.GetFullPath(new string[] { this.Domain, this.Avatar });
			}
		}

		[JsonProperty("changePassword")]
		public ChangePasswordViewModel ChangePassword
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

		[JsonProperty("detailsUrl")]
		public string DetailsUrl
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

		[JsonProperty("email")]
		public string Email
		{
			get;
			set;
		}

		[JsonProperty("firstName")]
		public string FirstName
		{
			get;
			set;
		}

		[JsonProperty("id")]
		public string Id
		{
			get;
			set;
		}

		[JsonProperty("isChangePassword")]
		public bool IsChangePassword
		{
			get;
			set;
		}

		[JsonProperty("lastName")]
		public string LastName
		{
			get;
			set;
		}

		[JsonProperty("mediaFile")]
		public FileViewModel MediaFile { get; set; } = new FileViewModel();

		[JsonProperty("middleName")]
		public string MiddleName
		{
			get;
			set;
		}

		[JsonProperty("phoneNumber")]
		public string PhoneNumber
		{
			get;
			set;
		}

		[JsonProperty("resetPassword")]
		public ResetPasswordViewModel ResetPassword
		{
			get;
			set;
		}

		[JsonProperty("username")]
		public string Username
		{
			get;
			set;
		}

		[JsonProperty("userRoles")]
		public List<UserRoleViewModel> UserRoles { get; set; } = new List<UserRoleViewModel>();

		public UserInfoViewModel()
		{
		}

		public UserInfoViewModel(MixCmsUser model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.UserRoles = ViewModelBase<MixCmsAccountContext, AspNetUserRoles, UserRoleViewModel>.Repository.GetModelListBy((AspNetUserRoles ur) => ur.UserId == this.Id, null, null).get_Data();
			this.ResetPassword = new ResetPasswordViewModel();
		}

		public override MixCmsUser ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.MediaFile.FileStream != null)
			{
				FileViewModel mediaFile = this.MediaFile;
				string[] str = new string[] { "Content/Uploads", null };
				str[1] = DateTime.UtcNow.ToString("MMM-yyyy");
				mediaFile.FileFolder = CommonHelper.GetFullPath(str);
				if (!FileRepository.Instance.SaveWebFile(this.MediaFile))
				{
					base.set_IsValid(false);
				}
				else
				{
					this.Avatar = this.MediaFile.FullPath;
				}
			}
			return base.ParseModel(_context, _transaction);
		}
	}
}