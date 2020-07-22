using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Cms
{
	public class MixModule
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

		public string EdmTemplate
		{
			get;
			set;
		}

		public string Fields
		{
			get;
			set;
		}

		public string FormTemplate
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

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixPageModule> MixPageModule
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixPostModule> MixPostModule
		{
			get;
			set;
		}

		public string ModifiedBy
		{
			get;
			set;
		}

		public string Name
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

		public int Type
		{
			get;
			set;
		}

		public MixModule()
		{
			base();
			this.set_MixModuleData(new HashSet<Mix.Cms.Lib.Models.Cms.MixModuleData>());
			this.set_MixModulePost(new HashSet<Mix.Cms.Lib.Models.Cms.MixModulePost>());
			this.set_MixPageModule(new HashSet<Mix.Cms.Lib.Models.Cms.MixPageModule>());
			this.set_MixPostModule(new HashSet<Mix.Cms.Lib.Models.Cms.MixPostModule>());
			return;
		}
	}
}