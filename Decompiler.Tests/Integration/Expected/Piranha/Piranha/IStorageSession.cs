using Piranha.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Piranha
{
	public interface IStorageSession : IDisposable
	{
		Task<bool> DeleteAsync(Media media, string filename);

		Task<bool> GetAsync(Media media, string filename, Stream stream);

		Task<string> PutAsync(Media media, string filename, string contentType, Stream stream);

		Task<string> PutAsync(Media media, string filename, string contentType, byte[] bytes);
	}
}