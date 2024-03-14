using Piranha.Extend;
using Piranha.Extend.Fields;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public abstract class PageBase : RoutedContentBase, IBlockContent, IMeta, ICommentModel
	{
		public IList<Block> Blocks { get; set; } = new List<Block>();

		public int CloseCommentsAfterDays
		{
			get;
			set;
		}

		public int CommentCount
		{
			get;
			set;
		}

		public bool EnableComments
		{
			get;
			set;
		}

		public string Excerpt
		{
			get;
			set;
		}

		public bool IsCommentsOpen
		{
			get
			{
				if (!this.EnableComments || !base.Published.HasValue)
				{
					return false;
				}
				if (this.CloseCommentsAfterDays == 0)
				{
					return true;
				}
				DateTime value = base.Published.Value;
				return value.AddDays((double)this.CloseCommentsAfterDays) > DateTime.Now;
			}
		}

		public bool IsHidden
		{
			get;
			set;
		}

		[StringLength(128)]
		public string NavigationTitle
		{
			get;
			set;
		}

		public Guid? OriginalPageId
		{
			get;
			set;
		}

		public Guid? ParentId
		{
			get;
			set;
		}

		public ImageField PrimaryImage { get; set; } = new ImageField();

		public Piranha.Models.RedirectType RedirectType
		{
			get;
			set;
		}

		[StringLength(0x100)]
		public string RedirectUrl
		{
			get;
			set;
		}

		public Guid SiteId
		{
			get;
			set;
		}

		public int SortOrder
		{
			get;
			set;
		}

		protected PageBase()
		{
		}
	}
}