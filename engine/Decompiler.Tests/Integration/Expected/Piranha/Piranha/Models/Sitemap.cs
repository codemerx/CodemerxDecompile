using System;

namespace Piranha.Models
{
	[Serializable]
	public class Sitemap : Structure<Sitemap, SitemapItem>
	{
		public Sitemap()
		{
			base();
			return;
		}
	}
}