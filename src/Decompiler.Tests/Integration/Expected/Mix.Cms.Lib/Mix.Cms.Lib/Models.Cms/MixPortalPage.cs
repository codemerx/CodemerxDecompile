using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Cms
{
	public class MixPortalPage
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

		public string Icon
		{
			get;
			set;
		}

		public int Id
		{
			get;
			set;
		}

		public DateTime? LastModified
		{
			get;
			set;
		}

		public int Level
		{
			get;
			set;
		}

		public virtual MixPortalPageNavigation MixPortalPageNavigationIdNavigation
		{
			get;
			set;
		}

		public virtual ICollection<MixPortalPageNavigation> MixPortalPageNavigationParent
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixPortalPageRole> MixPortalPageRole
		{
			get;
			set;
		}

		public string ModifiedBy
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

		public string TextDefault
		{
			get;
			set;
		}

		public string TextKeyword
		{
			get;
			set;
		}

		public string Url
		{
			get;
			set;
		}

		public MixPortalPage()
		{
			this.MixPortalPageNavigationParent = new HashSet<MixPortalPageNavigation>();
			this.MixPortalPageRole = new HashSet<Mix.Cms.Lib.Models.Cms.MixPortalPageRole>();
		}
	}
}