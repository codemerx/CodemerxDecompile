using Piranha;
using Piranha.Models;
using Piranha.Repositories;
using Piranha.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Piranha.Services
{
	public class MediaService : IMediaService
	{
		private readonly IMediaRepository _repo;

		private readonly IParamService _paramService;

		private readonly IStorage _storage;

		private readonly IImageProcessor _processor;

		private readonly ICache _cache;

		private readonly static object ScaleMutex;

		private const string MEDIA_STRUCTURE = "MediaStructure";

		static MediaService()
		{
			MediaService.ScaleMutex = new Object();
			return;
		}

		public MediaService(IMediaRepository repo, IParamService paramService, IStorage storage, IImageProcessor processor = null, ICache cache = null)
		{
			base();
			this._repo = repo;
			this._paramService = paramService;
			this._storage = storage;
			this._processor = processor;
			this._cache = cache;
			return;
		}

		private async Task<IEnumerable<Media>> _getFast(IEnumerable<Guid> ids)
		{
			V_0.u003cu003e4__this = this;
			V_0.ids = ids;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IEnumerable<Media>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MediaService.u003c_getFastu003ed__8>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<int> CountFolderItemsAsync(Guid? folderId = null)
		{
			return this._repo.CountAll(folderId);
		}

		public async Task DeleteAsync(Guid id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MediaService.u003cDeleteAsyncu003ed__23>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task DeleteAsync(Media model)
		{
			return this.DeleteAsync(model.get_Id());
		}

		public async Task DeleteFolderAsync(Guid id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MediaService.u003cDeleteFolderAsyncu003ed__25>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task DeleteFolderAsync(MediaFolder model)
		{
			return this.DeleteFolderAsync(model.get_Id());
		}

		public string EnsureVersion(Guid id, int width, int? height = null)
		{
			V_0 = this.EnsureVersionAsync(id, width, height).GetAwaiter();
			return V_0.GetResult();
		}

		public async Task<string> EnsureVersionAsync(Guid id, int width, int? height = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.width = width;
			V_0.height = height;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<string>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MediaService.u003cEnsureVersionAsyncu003ed__21>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<string> EnsureVersionAsync(Media media, int width, int? height = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.media = media;
			V_0.width = width;
			V_0.height = height;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<string>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MediaService.u003cEnsureVersionAsyncu003ed__22>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<IEnumerable<Media>> GetAllByFolderIdAsync(Guid? folderId = null)
		{
			return this._repo.GetAll(folderId).ContinueWith<Task<IEnumerable<Media>>>(new Func<Task<IEnumerable<Guid>>, Task<IEnumerable<Media>>>(this.u003cGetAllByFolderIdAsyncu003eb__9_0)).Unwrap<IEnumerable<Media>>();
		}

		public async Task<IEnumerable<MediaFolder>> GetAllFoldersAsync(Guid? folderId = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.folderId = folderId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IEnumerable<MediaFolder>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MediaService.u003cGetAllFoldersAsyncu003ed__11>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<Media> GetByIdAsync(Guid id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Media>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MediaService.u003cGetByIdAsyncu003ed__12>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<IEnumerable<Media>> GetByIdAsync(params Guid[] ids)
		{
			return this._repo.GetById(ids);
		}

		public async Task<MediaFolder> GetFolderByIdAsync(Guid id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<MediaFolder>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MediaService.u003cGetFolderByIdAsyncu003ed__14>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private string GetPublicUrl(Media media, int? width = null, int? height = null, string extension = null)
		{
			V_0 = this.GetResourceName(media, width, height, extension);
			V_1 = new Config(this._paramService);
			try
			{
				V_2 = V_1.get_MediaCDN();
				if (String.IsNullOrWhiteSpace(V_2))
				{
					V_3 = this._storage.GetPublicUrl(media, V_0);
				}
				else
				{
					V_3 = String.Concat(V_2, this._storage.GetResourceName(media, V_0));
				}
			}
			finally
			{
				if (V_1 != null)
				{
					((IDisposable)V_1).Dispose();
				}
			}
			return V_3;
		}

		private string GetResourceName(Media media, int? width = null, int? height = null, string extension = null)
		{
			V_0 = new FileInfo(media.get_Filename());
			V_1 = new StringBuilder();
			if (!width.get_HasValue())
			{
				dummyVar4 = V_1.Append(V_0.get_Name().Replace(V_0.get_Extension(), ""));
			}
			else
			{
				dummyVar0 = V_1.Append(V_0.get_Name().Replace(V_0.get_Extension(), "_"));
				dummyVar1 = V_1.Append(width);
				if (height.get_HasValue())
				{
					dummyVar2 = V_1.Append("x");
					dummyVar3 = V_1.Append(height.get_Value());
				}
			}
			if (!String.IsNullOrEmpty(extension))
			{
				dummyVar6 = V_1.Append(extension);
			}
			else
			{
				dummyVar5 = V_1.Append(V_0.get_Extension());
			}
			return V_1.ToString();
		}

		public async Task<MediaStructure> GetStructureAsync()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<MediaStructure>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MediaService.u003cGetStructureAsyncu003ed__15>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task MoveAsync(Media model, Guid? folderId)
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.folderId = folderId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MediaService.u003cMoveAsyncu003ed__19>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private void OnFolderLoad(MediaFolder model)
		{
			if (model != null)
			{
				App.get_Hooks().OnLoad<MediaFolder>(model);
				stackVariable4 = this._cache;
				if (stackVariable4 == null)
				{
					dummyVar0 = stackVariable4;
					return;
				}
				V_0 = model.get_Id();
				stackVariable4.Set<MediaFolder>(V_0.ToString(), model);
			}
			return;
		}

		private void OnLoad(Media model)
		{
			if (model != null)
			{
				V_0 = null;
				stackVariable5 = V_0;
				V_0 = null;
				model.set_PublicUrl(this.GetPublicUrl(model, stackVariable5, V_0, null));
				V_1 = App.get_MediaTypes().get_MetaProperties().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = new MediaService.u003cu003ec__DisplayClass27_0();
						V_2.key = V_1.get_Current();
						if (model.get_Properties().Any<KeyValuePair<string, string>>(new Func<KeyValuePair<string, string>, bool>(V_2.u003cOnLoadu003eb__0)))
						{
							continue;
						}
						model.get_Properties().Add(V_2.key, null);
					}
				}
				finally
				{
					if (V_1 != null)
					{
						V_1.Dispose();
					}
				}
				App.get_Hooks().OnLoad<Media>(model);
				stackVariable33 = this._cache;
				if (stackVariable33 == null)
				{
					dummyVar0 = stackVariable33;
					return;
				}
				V_3 = model.get_Id();
				stackVariable33.Set<Media>(V_3.ToString(), model);
			}
			return;
		}

		private void RemoveFromCache(Media model)
		{
			stackVariable1 = this._cache;
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				return;
			}
			stackVariable1.Remove(model.get_Id().ToString());
			return;
		}

		private void RemoveFromCache(MediaFolder model)
		{
			if (this._cache != null)
			{
				stackVariable3 = this._cache;
				stackVariable3.Remove(model.get_Id().ToString());
				this.RemoveStructureFromCache();
			}
			return;
		}

		private void RemoveStructureFromCache()
		{
			stackVariable1 = this._cache;
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				return;
			}
			stackVariable1.Remove("MediaStructure");
			return;
		}

		public async Task SaveAsync(Media model)
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MediaService.u003cSaveAsyncu003ed__16>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task SaveAsync(MediaContent content)
		{
			V_0.u003cu003e4__this = this;
			V_0.content = content;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MediaService.u003cSaveAsyncu003ed__17>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task SaveFolderAsync(MediaFolder model)
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MediaService.u003cSaveFolderAsyncu003ed__18>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}