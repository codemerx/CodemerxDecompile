using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Cms
{
	public class MixPost
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

		public string ExtraProperties
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

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixModuleData> MixModuleData
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixModulePost> MixModulePost
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixPagePost> MixPagePost
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixPostMedia> MixPostMedia
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixPostModule> MixPostModule
		{
			get;
			set;
		}

		public virtual ICollection<MixRelatedPost> MixRelatedPostMixPost
		{
			get;
			set;
		}

		public virtual ICollection<MixRelatedPost> MixRelatedPostS
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

		public DateTime? PublishedDateTime
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

		public string Source
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

		public string Thumbnail
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

		public MixPost()
		{
			this.MixModuleData = new HashSet<Mix.Cms.Lib.Models.Cms.MixModuleData>();
			this.MixModulePost = new HashSet<Mix.Cms.Lib.Models.Cms.MixModulePost>();
			this.MixPagePost = new HashSet<Mix.Cms.Lib.Models.Cms.MixPagePost>();
			this.MixPostMedia = new HashSet<Mix.Cms.Lib.Models.Cms.MixPostMedia>();
			this.MixPostModule = new HashSet<Mix.Cms.Lib.Models.Cms.MixPostModule>();
			this.MixRelatedPostMixPost = new HashSet<MixRelatedPost>();
			this.MixRelatedPostS = new HashSet<MixRelatedPost>();
		}
	}
}