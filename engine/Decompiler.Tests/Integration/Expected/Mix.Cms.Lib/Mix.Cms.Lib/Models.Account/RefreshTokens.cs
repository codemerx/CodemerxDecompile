using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Account
{
	public class RefreshTokens
	{
		public string ClientId
		{
			get;
			set;
		}

		public string Email
		{
			get;
			set;
		}

		public DateTime ExpiresUtc
		{
			get;
			set;
		}

		public string Id
		{
			get;
			set;
		}

		public DateTime IssuedUtc
		{
			get;
			set;
		}

		public string Username
		{
			get;
			set;
		}

		public RefreshTokens()
		{
			base();
			return;
		}
	}
}