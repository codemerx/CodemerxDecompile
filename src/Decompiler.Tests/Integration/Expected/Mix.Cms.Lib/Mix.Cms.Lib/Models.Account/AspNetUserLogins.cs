using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Account
{
	public class AspNetUserLogins
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

		public string LoginProvider
		{
			get;
			set;
		}

		public string ProviderDisplayName
		{
			get;
			set;
		}

		public string ProviderKey
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

		public AspNetUserLogins()
		{
		}
	}
}