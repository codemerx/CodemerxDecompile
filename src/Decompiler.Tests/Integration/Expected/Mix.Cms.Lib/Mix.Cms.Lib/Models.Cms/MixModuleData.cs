using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Cms
{
	public class MixModuleData
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

		public string Fields
		{
			get;
			set;
		}

		public string Id
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

		public virtual Mix.Cms.Lib.Models.Cms.MixPage MixPage
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

		public int? PageId
		{
			get;
			set;
		}

		public int? PostId
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

		public string Value
		{
			get;
			set;
		}

		public MixModuleData()
		{
		}
	}
}