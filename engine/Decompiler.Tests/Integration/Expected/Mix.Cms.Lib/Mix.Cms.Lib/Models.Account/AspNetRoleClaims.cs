using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Account
{
	public class AspNetRoleClaims
	{
		public string ClaimType
		{
			get;
			set;
		}

		public string ClaimValue
		{
			get;
			set;
		}

		public int Id
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

		public AspNetRoleClaims()
		{
		}
	}
}