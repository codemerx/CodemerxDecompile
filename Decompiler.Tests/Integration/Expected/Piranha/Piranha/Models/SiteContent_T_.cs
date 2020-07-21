using Piranha;
using System;
using System.Threading.Tasks;

namespace Piranha.Models
{
	[Serializable]
	public class SiteContent<T> : SiteContentBase
	where T : SiteContent<T>
	{
		public SiteContent()
		{
			base();
			return;
		}

		public static Task<T> CreateAsync(IApi api, string typeId = null)
		{
			return api.get_Sites().CreateContentAsync<T>(typeId);
		}
	}
}