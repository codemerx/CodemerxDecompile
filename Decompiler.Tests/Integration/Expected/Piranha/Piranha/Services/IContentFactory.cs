using Piranha.Models;
using System;
using System.Threading.Tasks;

namespace Piranha.Services
{
	public interface IContentFactory
	{
		Task<T> CreateAsync<T>(ContentTypeBase type)
		where T : ContentBase;

		Task<object> CreateBlockAsync(string typeName);

		Task<object> CreateDynamicRegionAsync(ContentTypeBase type, string regionId, bool managerInit = false);

		Task<T> InitAsync<T>(T model, ContentTypeBase type)
		where T : ContentBase;

		Task<T> InitDynamicAsync<T>(T model, ContentTypeBase type)
		where T : IDynamicContent;

		Task<T> InitDynamicManagerAsync<T>(T model, ContentTypeBase type)
		where T : IDynamicContent;

		Task<object> InitFieldAsync(object field, bool managerInit = false);

		Task<T> InitManagerAsync<T>(T model, ContentTypeBase type)
		where T : ContentBase;
	}
}