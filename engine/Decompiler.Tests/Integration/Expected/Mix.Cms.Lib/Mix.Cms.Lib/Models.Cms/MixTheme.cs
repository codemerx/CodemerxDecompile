using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Cms
{
	public class MixTheme
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

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixFile> MixFile
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixTemplate> MixTemplate
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

		public string PreviewUrl
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

		public MixTheme()
		{
			this.MixFile = new HashSet<Mix.Cms.Lib.Models.Cms.MixFile>();
			this.MixTemplate = new HashSet<Mix.Cms.Lib.Models.Cms.MixTemplate>();
		}
	}
}