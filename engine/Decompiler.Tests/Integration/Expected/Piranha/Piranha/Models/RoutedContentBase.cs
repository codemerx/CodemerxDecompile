using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public abstract class RoutedContentBase : ContentBase
	{
		public bool IsPublished
		{
			get
			{
				if (!this.get_Published().get_HasValue())
				{
					return false;
				}
				V_0 = this.get_Published();
				return DateTime.op_LessThanOrEqual(V_0.get_Value(), DateTime.get_Now());
			}
		}

		[StringLength(0x100)]
		public string MetaDescription
		{
			get;
			set;
		}

		[StringLength(128)]
		public string MetaKeywords
		{
			get;
			set;
		}

		public string Permalink
		{
			get;
			set;
		}

		public DateTime? Published
		{
			get;
			set;
		}

		[StringLength(0x100)]
		public string Route
		{
			get;
			set;
		}

		[StringLength(128)]
		public string Slug
		{
			get;
			set;
		}

		protected RoutedContentBase()
		{
			base();
			return;
		}
	}
}