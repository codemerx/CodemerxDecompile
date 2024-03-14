using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Cms
{
	public class MixCache
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

		public DateTime? ExpiredDateTime
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

		public string Value
		{
			get;
			set;
		}

		public MixCache()
		{
		}
	}
}