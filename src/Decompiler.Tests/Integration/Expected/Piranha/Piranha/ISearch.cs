using Piranha.Models;
using System.Threading.Tasks;

namespace Piranha
{
	public interface ISearch
	{
		Task DeletePageAsync(PageBase page);

		Task DeletePostAsync(PostBase post);

		Task SavePageAsync(PageBase page);

		Task SavePostAsync(PostBase post);
	}
}