using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Account
{
	public class AspNetUsers
	{
		public int AccessFailedCount
		{
			get;
			set;
		}

		public virtual ICollection<AspNetUserClaims> AspNetUserClaimsApplicationUser
		{
			get;
			set;
		}

		public virtual ICollection<AspNetUserClaims> AspNetUserClaimsUser
		{
			get;
			set;
		}

		public virtual ICollection<AspNetUserLogins> AspNetUserLoginsApplicationUser
		{
			get;
			set;
		}

		public virtual ICollection<AspNetUserLogins> AspNetUserLoginsUser
		{
			get;
			set;
		}

		public virtual ICollection<AspNetUserRoles> AspNetUserRolesApplicationUser
		{
			get;
			set;
		}

		public virtual ICollection<AspNetUserRoles> AspNetUserRolesUser
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Account.AspNetUserTokens> AspNetUserTokens
		{
			get;
			set;
		}

		public string Avatar
		{
			get;
			set;
		}

		public string ConcurrencyStamp
		{
			get;
			set;
		}

		public int CountryId
		{
			get;
			set;
		}

		public string Culture
		{
			get;
			set;
		}

		public DateTime? Dob
		{
			get;
			set;
		}

		public string Email
		{
			get;
			set;
		}

		public bool EmailConfirmed
		{
			get;
			set;
		}

		public string FirstName
		{
			get;
			set;
		}

		public string Gender
		{
			get;
			set;
		}

		public string Id
		{
			get;
			set;
		}

		public bool IsActived
		{
			get;
			set;
		}

		public DateTime JoinDate
		{
			get;
			set;
		}

		public DateTime LastModified
		{
			get;
			set;
		}

		public string LastName
		{
			get;
			set;
		}

		public bool LockoutEnabled
		{
			get;
			set;
		}

		public DateTime? LockoutEnd
		{
			get;
			set;
		}

		public string ModifiedBy
		{
			get;
			set;
		}

		public string NickName
		{
			get;
			set;
		}

		public string NormalizedEmail
		{
			get;
			set;
		}

		public string NormalizedUserName
		{
			get;
			set;
		}

		public string PasswordHash
		{
			get;
			set;
		}

		public string PhoneNumber
		{
			get;
			set;
		}

		public bool PhoneNumberConfirmed
		{
			get;
			set;
		}

		public string RegisterType
		{
			get;
			set;
		}

		public string SecurityStamp
		{
			get;
			set;
		}

		public bool TwoFactorEnabled
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public AspNetUsers()
		{
			base();
			this.set_AspNetUserClaimsApplicationUser(new HashSet<AspNetUserClaims>());
			this.set_AspNetUserClaimsUser(new HashSet<AspNetUserClaims>());
			this.set_AspNetUserLoginsApplicationUser(new HashSet<AspNetUserLogins>());
			this.set_AspNetUserLoginsUser(new HashSet<AspNetUserLogins>());
			this.set_AspNetUserRolesApplicationUser(new HashSet<AspNetUserRoles>());
			this.set_AspNetUserRolesUser(new HashSet<AspNetUserRoles>());
			this.set_AspNetUserTokens(new HashSet<Mix.Cms.Lib.Models.Account.AspNetUserTokens>());
			return;
		}
	}
}