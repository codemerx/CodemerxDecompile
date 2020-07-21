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
				if (!String.IsNullOrWhiteSpace(this.get_NavigationTitle()))
				{
					return this.get_NavigationTitle();
				}
				return this.get_Title();
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

		public IList<string> Permissions
		{
			get;
			set;
		}

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
			this.u003cPermissionsu003ek__BackingField = new List<string>();
			base();
			this.set_Items(new Sitemap());
			return;
		}

		public bool HasChild(Guid id)
		{
			V_0 = this.get_Items().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!Guid.op_Equality(V_1.get_Id(), id) && !V_1.HasChild(id))
					{
						continue;
					}
					V_2 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return V_2;
		Label0:
			return false;
		}
	}
}