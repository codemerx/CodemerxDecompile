using Piranha;
using System;
using System.Threading.Tasks;

namespace Piranha.Models
{
	[Serializable]
	public class Post<T> : PostBase
	where T : Post<T>
	{
		public Post()
		{
			base();
			return;
		}

		public static Task<T> CreateAsync(IApi api, string typeId = null)
		{
			return api.get_Posts().CreateAsync<T>(typeId);
		}
	}
}