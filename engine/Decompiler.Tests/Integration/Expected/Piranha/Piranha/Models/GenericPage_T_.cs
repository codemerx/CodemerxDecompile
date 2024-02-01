using Piranha;
using Piranha.Services;
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
				if (base.ParentId.HasValue)
				{
					return false;
				}
				return base.SortOrder == 0;
			}
		}

		public GenericPage()
		{
		}

		public static Task<T> CreateAsync(IApi api, string typeId = null)
		{
			return api.Pages.CreateAsync<T>(typeId);
		}
	}
}