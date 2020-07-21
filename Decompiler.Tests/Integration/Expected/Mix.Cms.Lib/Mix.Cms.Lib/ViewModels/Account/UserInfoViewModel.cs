using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels;
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
				if (this.get_Avatar() == null || this.get_Avatar().IndexOf("http") != -1 || this.get_Avatar().get_Chars(0) == '/')
				{
					return this.get_Avatar();
				}
				stackVariable15 = new string[2];
				stackVariable15[0] = this.get_Domain();
				stackVariable15[1] = this.get_Avatar();
				return CommonHelper.GetFullPath(stackVariable15);
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
		public FileViewModel MediaFile
		{
			get;
			set;
		}

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
		public List<UserRoleViewModel> UserRoles
		{
			get;
			set;
		}

		public UserInfoViewModel()
		{
			this.u003cUserRolesu003ek__BackingField = new List<UserRoleViewModel>();
			this.u003cMediaFileu003ek__BackingField = new FileViewModel();
			base();
			return;
		}

		public UserInfoViewModel(MixCmsUser model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cUserRolesu003ek__BackingField = new List<UserRoleViewModel>();
			this.u003cMediaFileu003ek__BackingField = new FileViewModel();
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable1 = ViewModelBase<MixCmsAccountContext, AspNetUserRoles, UserRoleViewModel>.Repository;
			V_0 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.Account.UserInfoViewModel::ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public override MixCmsUser ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.get_MediaFile().get_FileStream() != null)
			{
				stackVariable8 = this.get_MediaFile();
				stackVariable10 = new string[2];
				stackVariable10[0] = "Content/Uploads";
				V_0 = DateTime.get_UtcNow();
				stackVariable10[1] = V_0.ToString("MMM-yyyy");
				stackVariable8.set_FileFolder(CommonHelper.GetFullPath(stackVariable10));
				if (!FileRepository.get_Instance().SaveWebFile(this.get_MediaFile()))
				{
					this.set_IsValid(false);
				}
				else
				{
					this.set_Avatar(this.get_MediaFile().get_FullPath());
				}
			}
			return this.ParseModel(_context, _transaction);
		}
	}
}