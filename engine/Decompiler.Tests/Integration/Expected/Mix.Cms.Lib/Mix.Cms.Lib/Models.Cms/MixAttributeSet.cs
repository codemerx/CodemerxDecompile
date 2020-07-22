using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Models.Cms
{
	public class MixAttributeSet
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

		public bool? EdmAutoSend
		{
			get;
			set;
		}

		public string EdmFrom
		{
			get;
			set;
		}

		public string EdmSubject
		{
			get;
			set;
		}

		public string EdmTemplate
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

		public DateTime? LastModified
		{
			get;
			set;
		}

		public virtual ICollection<MixAttributeField> MixAttributeFieldAttributeSet
		{
			get;
			set;
		}

		public virtual ICollection<MixAttributeField> MixAttributeFieldReference
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixAttributeSetData> MixAttributeSetData
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixAttributeSetReference> MixAttributeSetReference
		{
			get;
			set;
		}

		public virtual ICollection<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet> MixRelatedAttributeSet
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

		public MixAttributeSet()
		{
			base();
			this.set_MixAttributeFieldAttributeSet(new HashSet<MixAttributeField>());
			this.set_MixAttributeFieldReference(new HashSet<MixAttributeField>());
			this.set_MixAttributeSetData(new HashSet<Mix.Cms.Lib.Models.Cms.MixAttributeSetData>());
			this.set_MixAttributeSetReference(new HashSet<Mix.Cms.Lib.Models.Cms.MixAttributeSetReference>());
			this.set_MixRelatedAttributeSet(new HashSet<Mix.Cms.Lib.Models.Cms.MixRelatedAttributeSet>());
			return;
		}
	}
}