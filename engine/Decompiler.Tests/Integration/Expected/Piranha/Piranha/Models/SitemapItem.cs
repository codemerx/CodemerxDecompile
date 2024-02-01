using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public class SitemapItem : StructureItem<Sitemap, SitemapItem>
	{
		public DateTime Created
		{
			get;
			set;
		}

		public bool IsHidden
		{
			get;
			set;
		}

		public DateTime LastModified
		{
			get;
			set;
		}

		public string MenuTitle
		{
			get
			{
				if (!String.IsNullOrWhiteSpace(this.NavigationTitle))
				{
					return this.NavigationTitle;
				}
				return this.Title;
			}
		}

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

		public string PageTypeName
		{
			get;
			set;
		}

		public Guid? ParentId
		{
			get;
			set;
		}

		public string Permalink
		{
			get;
			set;
		}

		public IList<string> Permissions { get; set; } = new List<string>();

		public DateTime? Published
		{
			get;
			set;
		}

		public int SortOrder
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public SitemapItem()
		{
			base.Items = new Sitemap();
		}

		public bool HasChild(Guid id)
		{
			bool flag;
			List<SitemapItem>.Enumerator enumerator = base.Items.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SitemapItem current = enumerator.Current;
					if (!(current.Id == id) && !current.HasChild(id))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}
	}
}