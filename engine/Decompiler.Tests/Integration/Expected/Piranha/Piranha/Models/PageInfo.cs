using System;

namespace Piranha.Models
{
	[Serializable]
	public class PageInfo : PageBase, IContentInfo
	{
		public PageInfo()
		{
			base();
			return;
		}
	}
}