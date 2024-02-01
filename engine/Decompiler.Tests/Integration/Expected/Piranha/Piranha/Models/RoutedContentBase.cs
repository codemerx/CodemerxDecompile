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
				if (!this.Published.HasValue)
				{
					return false;
				}
				return this.Published.Value <= DateTime.Now;
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
		}
	}
}