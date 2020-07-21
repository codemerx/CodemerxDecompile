using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Account
{
	public class AspNetRoles
	{
		public virtual ICollection<Mix.Cms.Lib.Models.Account.AspNetRoleClaims> AspNetRoleClaims
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Account.AspNetUserRoles> AspNetUserRoles
		{
			get;
			set;
		}

		public string ConcurrencyStamp
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

		public string NormalizedName
		{
			get;
			set;
		}

		public AspNetRoles()
		{
			base();
			this.set_AspNetRoleClaims(new HashSet<Mix.Cms.Lib.Models.Account.AspNetRoleClaims>());
			this.set_AspNetUserRoles(new HashSet<Mix.Cms.Lib.Models.Account.AspNetUserRoles>());
			return;
		}
	}
}