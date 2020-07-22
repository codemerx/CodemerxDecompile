using Piranha;
using System;
using System.Threading.Tasks;

namespace Piranha.Models
{
	[Serializable]
	public class GenericPage<T> : PageBase
	where T : GenericPage<T>
	{
		public bool IsStartPage
		{
			get
			{
				if (this.get_ParentId().get_HasValue())
				{
					return false;
				}
				return this.get_SortOrder() == 0;
			}
		}

		public GenericPage()
		{
			base();
			return;
		}

		public static Task<T> CreateAsync(IApi api, string typeId = null)
		{
			return api.get_Pages().CreateAsync<T>(typeId);
		}
	}
}