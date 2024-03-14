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
		public IList<Block> Blocks { get; set; } = new List<Block>();

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

		public bool EnableComments { get; set; } = true;

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

		public IList<Taxonomy> Tags { get; set; } = new List<Taxonomy>();

		protected PostBase()
		{
		}
	}
}