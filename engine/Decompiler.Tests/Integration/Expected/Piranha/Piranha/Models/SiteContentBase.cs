using System;

namespace Piranha.Models
{
	[Serializable]
	public abstract class SiteContentBase : ContentBase
	{
		protected SiteContentBase()
		{
			base();
			return;
		}
	}
}