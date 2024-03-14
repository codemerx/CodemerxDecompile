using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Account
{
	public class AspNetUserRoles
	{
		public virtual AspNetUsers ApplicationUser
		{
			get;
			set;
		}

		public string ApplicationUserId
		{
			get;
			set;
		}

		public virtual AspNetRoles Role
		{
			get;
			set;
		}

		public string RoleId
		{
			get;
			set;
		}

		public virtual AspNetUsers User
		{
			get;
			set;
		}

		public string UserId
		{
			get;
			set;
		}

		public AspNetUserRoles()
		{
		}
	}
}