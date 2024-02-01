using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Cms
{
	public class MixPortalPageRole
	{
		public string CreatedBy
		{
			get;
			set;
		}

		public DateTime CreatedDateTime
		{
			get;
			set;
		}

		public DateTime? LastModified
		{
			get;
			set;
		}

		public string ModifiedBy
		{
			get;
			set;
		}

		public virtual MixPortalPage Page
		{
			get;
			set;
		}

		public int PageId
		{
			get;
			set;
		}

		public int Priority
		{
			get;
			set;
		}

		public string RoleId
		{
			get;
			set;
		}

		public string Status
		{
			get;
			set;
		}

		public MixPortalPageRole()
		{
		}
	}
}