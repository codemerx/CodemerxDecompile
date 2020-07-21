using Mix.Cms.Lib;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Cms
{
	public class MixAttributeSetReference
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

		public string ModifiedBy
		{
			get;
			set;
		}

		public int ParentId
		{
			get;
			set;
		}

		public MixEnums.MixAttributeSetDataType ParentType
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

		public MixAttributeSetReference()
		{
			base();
			return;
		}
	}
}