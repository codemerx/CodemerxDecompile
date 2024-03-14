using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Cms
{
	public class MixPortalPageNavigation
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

		public string Description
		{
			get;
			set;
		}

		public int Id
		{
			get;
			set;
		}

		public virtual MixPortalPage IdNavigation
		{
			get;
			set;
		}

		public string Image
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

		public int PageId
		{
			get;
			set;
		}

		public virtual MixPortalPage Parent
		{
			get;
			set;
		}

		public int ParentId
		{
			get;
			set;
		}

		public int Priority
		{
			get;
			set;
		}

		public string Status
		{
			get;
			set;
		}

		public MixPortalPageNavigation()
		{
		}
	}
}