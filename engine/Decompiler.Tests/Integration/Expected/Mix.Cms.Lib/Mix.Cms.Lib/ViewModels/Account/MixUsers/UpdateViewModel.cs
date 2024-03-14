using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Account;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Repositories;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.Account;
using Mix.Common.Helper;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.Account.MixUsers
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixCmsUser, UpdateViewModel>
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

		[JsonProperty("confirmPassword")]
		public string ConfirmPassword
		{
			get;
			set;
		}

		[JsonProperty("createdby")]
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

		[JsonProperty("lastModified")]
		public DateTime? LastModified
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

		[JsonProperty("modifiedBy")]
		public string ModifiedBy
		{
			get;
			set;
		}

		[JsonProperty("password")]
		public string Password
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
		public MixEnums.MixUserStatus Status
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
		public List<NavUserRoleViewModel> UserRoles
		{
			get;
			set;
		}

		public UpdateViewModel()
		{
		}

		public UpdateViewModel(MixCmsUser model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.UserRoles = this.GetRoleNavs();
		}

		public List<NavUserRoleViewModel> GetRoleNavs()
		{
			List<NavUserRoleViewModel> list;
			using (MixCmsAccountContext mixCmsAccountContext = new MixCmsAccountContext())
			{
				list = (
					from p in EntityFrameworkQueryableExtensions.Include<AspNetRoles, ICollection<AspNetUserRoles>>(mixCmsAccountContext.AspNetRoles, (AspNetRoles cp) => cp.AspNetUserRoles).ToList<AspNetRoles>()
					select new NavUserRoleViewModel()
					{
						UserId = this.Id,
						RoleId = p.Id,
						Specificulture = this.Specificulture,
						Description = p.Name,
						IsActived = mixCmsAccountContext.AspNetUserRoles.Any<AspNetUserRoles>((AspNetUserRoles m) => m.UserId == this.Id && m.RoleId == p.Id)
					} into m
					orderby m.Priority
					select m).ToList<NavUserRoleViewModel>();
			}
			return list;
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