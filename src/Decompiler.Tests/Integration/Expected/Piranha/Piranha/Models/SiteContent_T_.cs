using Piranha;
using Piranha.Services;
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
		}

		public static Task<T> CreateAsync(IApi api, string typeId = null)
		{
			return api.Sites.CreateContentAsync<T>(typeId);
		}
	}
}