using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Account
{
	public class Clients
	{
		public bool Active
		{
			get;
			set;
		}

		public string AllowedOrigin
		{
			get;
			set;
		}

		public int ApplicationType
		{
			get;
			set;
		}

		public string Id
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public int RefreshTokenLifeTime
		{
			get;
			set;
		}

		public string Secret
		{
			get;
			set;
		}

		public Clients()
		{
			base();
			return;
		}
	}
}