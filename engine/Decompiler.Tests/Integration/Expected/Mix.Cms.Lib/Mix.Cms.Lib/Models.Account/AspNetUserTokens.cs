using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Account
{
	public class AspNetUserTokens
	{
		public string LoginProvider
		{
			get;
			set;
		}

		public string Name
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

		public string Value
		{
			get;
			set;
		}

		public AspNetUserTokens()
		{
			base();
			return;
		}
	}
}