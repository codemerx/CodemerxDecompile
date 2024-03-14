using Piranha;
using Piranha.Cache;
using Piranha.Models;
using Piranha.Repositories;
using Piranha.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Piranha.Services
{
	public class PostTypeService : IPostTypeService
	{
		private readonly IPostTypeRepository _repo;

		private readonly ICache _cache;

		public PostTypeService(IPostTypeRepository repo, ICache cache)
		{
			this._repo = repo;
			if (App.CacheLevel > CacheLevel.Minimal)
			{
				this._cache = cache;
			}
		}

		public async Task DeleteAsync(string id)
		{
			ConfiguredTaskAwaitable<PostType> configuredTaskAwaitable = this._repo.GetById(id).ConfigureAwait(false);
			PostType postType = await configuredTaskAwaitable;
			if (postType != null)
			{
				await this.DeleteAsync(postType).ConfigureAwait(false);
			}
		}

		public async Task DeleteAsync(PostType model)
		{
			App.Hooks.OnBeforeDelete<PostType>(model);
			ConfiguredTaskAwaitable configuredTaskAwaitable = this._repo.Delete(model.Id).ConfigureAwait(false);
			await configuredTaskAwaitable;
			App.Hooks.OnAfterDelete<PostType>(model);
			ICache cache = this._cache;
			if (cache != null)
			{
				cache.Remove("Piranha_PostTypes");
			}
			else
			{
			}
		}

		public Task<IEnumerable<PostType>> GetAllAsync()
		{
			return this.GetTypes();
		}

		public async Task<PostType> GetByIdAsync(string id)
		{
			ConfiguredTaskAwaitable<IEnumerable<PostType>> configuredTaskAwaitable = this.GetTypes().ConfigureAwait(false);
			PostType postType = await configuredTaskAwaitable.FirstOrDefault<PostType>((PostType t) => t.Id == id);
			return postType;
		}

		private async Task<IEnumerable<PostType>> GetTypes()
		{
			IEnumerable<PostType> postTypes;
			ICache cache = this._cache;
			if (cache != null)
			{
				postTypes = cache.Get<IEnumerable<PostType>>("Piranha_PostTypes");
			}
			else
			{
				postTypes = null;
			}
			IEnumerable<PostType> postTypes1 = postTypes;
			if (postTypes1 == null)
			{
				ConfiguredTaskAwaitable<IEnumerable<PostType>> configuredTaskAwaitable = this._repo.GetAll().ConfigureAwait(false);
				postTypes1 = await configuredTaskAwaitable;
				ICache cache1 = this._cache;
				if (cache1 != null)
				{
					cache1.Set<IEnumerable<PostType>>("Piranha_PostTypes", postTypes1);
				}
				else
				{
				}
			}
			return postTypes1;
		}

		public async Task SaveAsync(PostType model)
		{
			Validator.ValidateObject(model, new ValidationContext(model), true);
			App.Hooks.OnBeforeSave<PostType>(model);
			await this._repo.Save(model).ConfigureAwait(false);
			App.Hooks.OnAfterSave<PostType>(model);
			ICache cache = this._cache;
			if (cache != null)
			{
				cache.Remove("Piranha_PostTypes");
			}
			else
			{
			}
		}
	}
}