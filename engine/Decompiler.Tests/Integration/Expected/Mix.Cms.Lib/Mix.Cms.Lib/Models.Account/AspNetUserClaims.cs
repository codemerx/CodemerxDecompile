using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Account
{
	public class AspNetUserClaims
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

		public AspNetUserClaims()
		{
		}
	}
}