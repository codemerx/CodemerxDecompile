using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public abstract class ContentTypeBase
	{
		[StringLength(0xff)]
		public string CLRType
		{
			get;
			set;
		}

		public IList<ContentTypeEditor> CustomEditors
		{
			get;
			set;
		}

		[Required]
		[StringLength(64)]
		public string Id
		{
			get;
			set;
		}

		public IList<RegionType> Regions
		{
			get;
			set;
		}

		public IList<ContentTypeRoute> Routes
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		protected ContentTypeBase()
		{
			this.u003cRegionsu003ek__BackingField = new List<RegionType>();
			this.u003cRoutesu003ek__BackingField = new List<ContentTypeRoute>();
			this.u003cCustomEditorsu003ek__BackingField = new List<ContentTypeEditor>();
			base();
			return;
		}
	}
}