using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Cms
{
	public class MixPage
	{
		public string Content
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

		public string CssClass
		{
			get;
			set;
		}

		public string Excerpt
		{
			get;
			set;
		}

		public string ExtraFields
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

		public string Layout
		{
			get;
			set;
		}

		public int? Level
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixModuleData> MixModuleData
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixPageModule> MixPageModule
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixPagePost> MixPagePost
		{
			get;
			set;
		}

		public string ModifiedBy
		{
			get;
			set;
		}

		public int? PageSize
		{
			get;
			set;
		}

		public int Priority
		{
			get;
			set;
		}

		public string SeoDescription
		{
			get;
			set;
		}

		public string SeoKeywords
		{
			get;
			set;
		}

		public string SeoName
		{
			get;
			set;
		}

		public string SeoTitle
		{
			get;
			set;
		}

		public string Specificulture
		{
			get;
			set;
		}

		public virtual MixCulture SpecificultureNavigation
		{
			get;
			set;
		}

		public string StaticUrl
		{
			get;
			set;
		}

		public string Status
		{
			get;
			set;
		}

		public string Tags
		{
			get;
			set;
		}

		public string Template
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public string Type
		{
			get;
			set;
		}

		public int? Views
		{
			get;
			set;
		}

		public MixPage()
		{
			base();
			this.set_MixModuleData(new HashSet<Mix.Cms.Lib.Models.Cms.MixModuleData>());
			this.set_MixPageModule(new HashSet<Mix.Cms.Lib.Models.Cms.MixPageModule>());
			this.set_MixPagePost(new HashSet<Mix.Cms.Lib.Models.Cms.MixPagePost>());
			return;
		}
	}
}