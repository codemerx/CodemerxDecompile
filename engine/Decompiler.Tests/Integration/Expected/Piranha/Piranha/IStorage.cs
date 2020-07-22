using Piranha.Models;
using System;
using System.Threading.Tasks;

namespace Piranha
{
	public interface IStorage
	{
		string GetPublicUrl(Media media, string filename);

		string GetResourceName(Media media, string filename);

		Task<IStorageSession> OpenAsync();
	}
}