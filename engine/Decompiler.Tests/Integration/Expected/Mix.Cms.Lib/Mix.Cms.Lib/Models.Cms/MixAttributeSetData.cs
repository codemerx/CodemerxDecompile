using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Cms
{
	public class MixAttributeSetData
	{
		public virtual MixAttributeSet AttributeSet
		{
			get;
			set;
		}

		public int AttributeSetId
		{
			get;
			set;
		}

		public string AttributeSetName
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

		public MixAttributeSetData()
		{
		}
	}
}