using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Controllers.Account
{
	public sealed class ConsentModel
	{
		public bool ConsentToAutomatedEmails
		{
			get;
			set;
		}

		public bool ConsentToCookies
		{
			get;
			set;
		}

		public bool ConsentToPersonalInformation
		{
			get;
			set;
		}

		public ConsentModel()
		{
		}
	}
}