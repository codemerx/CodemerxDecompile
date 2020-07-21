using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Cms
{
	public class MixPostModule
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

		public virtual Mix.Cms.Lib.Models.Cms.MixModule MixModule
		{
			get;
			set;
		}

		public virtual Mix.Cms.Lib.Models.Cms.MixPost MixPost
		{
			get;
			set;
		}

		public string ModifiedBy
		{
			get;
			set;
		}

		public int ModuleId
		{
			get;
			set;
		}

		public int Position
		{
			get;
			set;
		}

		public int PostId
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

		public MixPostModule()
		{
			base();
			return;
		}
	}
}