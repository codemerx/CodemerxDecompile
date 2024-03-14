using Piranha;
using Piranha.Services;
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
		}

		public static Task<T> CreateAsync(IApi api, string typeId = null)
		{
			return api.Posts.CreateAsync<T>(typeId);
		}
	}
}