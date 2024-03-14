using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Cms
{
	public class MixCulture
	{
		public string Alias
		{
			get;
			set;
		}

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

		public string FullName
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

		public string Lcid
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixConfiguration> MixConfiguration
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixLanguage> MixLanguage
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixModule> MixModule
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixPage> MixPage
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixPost> MixPost
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixUrlAlias> MixUrlAlias
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

		public string Specificulture
		{
			get;
			set;
		}

		public string Status
		{
			get;
			set;
		}

		public MixCulture()
		{
			this.MixConfiguration = new HashSet<Mix.Cms.Lib.Models.Cms.MixConfiguration>();
			this.MixLanguage = new HashSet<Mix.Cms.Lib.Models.Cms.MixLanguage>();
			this.MixModule = new HashSet<Mix.Cms.Lib.Models.Cms.MixModule>();
			this.MixPage = new HashSet<Mix.Cms.Lib.Models.Cms.MixPage>();
			this.MixPost = new HashSet<Mix.Cms.Lib.Models.Cms.MixPost>();
			this.MixUrlAlias = new HashSet<Mix.Cms.Lib.Models.Cms.MixUrlAlias>();
		}
	}
}