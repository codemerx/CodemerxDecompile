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

		public IList<ContentTypeEditor> CustomEditors { get; set; } = new List<ContentTypeEditor>();

		[Required]
		[StringLength(64)]
		public string Id
		{
			get;
			set;
		}

		public IList<RegionType> Regions { get; set; } = new List<RegionType>();

		public IList<ContentTypeRoute> Routes { get; set; } = new List<ContentTypeRoute>();

		public string Title
		{
			get;
			set;
		}

		protected ContentTypeBase()
		{
		}
	}
}