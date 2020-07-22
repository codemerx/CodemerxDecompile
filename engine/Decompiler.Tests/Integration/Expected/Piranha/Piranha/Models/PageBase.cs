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
		public IList<Block> Blocks
		{
			get;
			set;
		}

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
				if (!this.get_EnableComments() || !this.get_Published().get_HasValue())
				{
					return false;
				}
				if (this.get_CloseCommentsAfterDays() == 0)
				{
					return true;
				}
				V_1 = this.get_Published().get_Value();
				return DateTime.op_GreaterThan(V_1.AddDays((double)this.get_CloseCommentsAfterDays()), DateTime.get_Now());
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

		public ImageField PrimaryImage
		{
			get;
			set;
		}

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
			this.u003cPrimaryImageu003ek__BackingField = new ImageField();
			this.u003cBlocksu003ek__BackingField = new List<Block>();
			base();
			return;
		}
	}
}