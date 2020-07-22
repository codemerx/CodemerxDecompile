using Piranha.Extend;
using Piranha.Extend.Fields;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public abstract class PostBase : RoutedContentBase, IBlockContent, IMeta, ICommentModel
	{
		public IList<Block> Blocks
		{
			get;
			set;
		}

		[Required]
		public Guid BlogId
		{
			get;
			set;
		}

		[Required]
		public Taxonomy Category
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

		public IList<Taxonomy> Tags
		{
			get;
			set;
		}

		protected PostBase()
		{
			this.u003cPrimaryImageu003ek__BackingField = new ImageField();
			this.u003cTagsu003ek__BackingField = new List<Taxonomy>();
			this.u003cBlocksu003ek__BackingField = new List<Block>();
			this.u003cEnableCommentsu003ek__BackingField = true;
			base();
			return;
		}
	}
}