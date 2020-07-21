using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Cms
{
	public class MixMedia
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

		public string Extension
		{
			get;
			set;
		}

		public string FileFolder
		{
			get;
			set;
		}

		public string FileName
		{
			get;
			set;
		}

		public string FileProperties
		{
			get;
			set;
		}

		public long FileSize
		{
			get;
			set;
		}

		public string FileType
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

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixPostMedia> MixPostMedia
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

		public string TargetUrl
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public MixMedia()
		{
			base();
			this.set_MixPostMedia(new HashSet<Mix.Cms.Lib.Models.Cms.MixPostMedia>());
			return;
		}
	}
}