using Piranha;
using Piranha.Models;
using Piranha.Repositories;
using Piranha.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

		private async Task<IEnumerable<Media>> _getFast(IEnumerable<Guid> ids)
		{
			IEnumerable<Media> medias;
			Guid[] array = ids as Guid[];
			if (array == null)
			{
				array = ids.ToArray<Guid>();
			}
			Guid[] guidArray = array;
			medias = (this._cache != null ? 
				from c in guidArray
				select this._cache.Get<Media>(c.ToString()) : Enumerable.Empty<Media>());
			Media[] mediaArray = (
				from c in medias
				where c != null
				select c).ToArray<Media>();
			Guid[] guidArray1 = guidArray;
			Media[] mediaArray1 = mediaArray;
			Guid[] array1 = ((IEnumerable<Guid>)guidArray1).Except<Guid>(
				from c in (IEnumerable<Media>)mediaArray1
				select c.Id).ToArray<Guid>();
			IEnumerable<Media> medias1 = mediaArray;
			IEnumerable<Media> byId = await this._repo.GetById(array1);
			IEnumerable<Media> medias2 = medias1.Concat<Media>(byId.Select<Media, Media>((Media c) => {
				this.OnLoad(c);
				return c;
			}));
			medias1 = null;
			IEnumerable<Media> array2 = (
				from m in medias2
				orderby m.Filename
				select m).ToArray<Media>();
			return array2;
		}

		static MediaService()
		{
			MediaService.ScaleMutex = new Object();
		}

		public MediaService(IMediaRepository repo, IParamService paramService, IStorage storage, IImageProcessor processor = null, ICache cache = null)
		{
			this._repo = repo;
			this._paramService = paramService;
			this._storage = storage;
			this._processor = processor;
			this._cache = cache;
		}

		public Task<int> CountFolderItemsAsync(Guid? folderId = null)
		{
			return this._repo.CountAll(folderId);
		}

		public async Task DeleteAsync(Guid id)
		{
			ConfiguredTaskAwaitable<bool> configuredTaskAwaitable;
			ConfiguredTaskAwaitable<Media> configuredTaskAwaitable1 = this.GetByIdAsync(id).ConfigureAwait(false);
			Media medium = await configuredTaskAwaitable1;
			if (medium != null)
			{
				ConfiguredTaskAwaitable<IStorageSession> configuredTaskAwaitable2 = this._storage.OpenAsync().ConfigureAwait(false);
				using (IStorageSession storageSession = await configuredTaskAwaitable2)
				{
					if (medium.Versions.Count > 0)
					{
						foreach (MediaVersion version in medium.Versions)
						{
							configuredTaskAwaitable = storageSession.DeleteAsync(medium, this.GetResourceName(medium, new int?(version.Width), version.Height, version.FileExtension)).ConfigureAwait(false);
							await configuredTaskAwaitable;
						}
					}
					App.Hooks.OnBeforeDelete<Media>(medium);
					await this._repo.Delete(id).ConfigureAwait(false);
					configuredTaskAwaitable = storageSession.DeleteAsync(medium, medium.Filename).ConfigureAwait(false);
					await configuredTaskAwaitable;
					App.Hooks.OnAfterDelete<Media>(medium);
				}
				storageSession = null;
				this.RemoveFromCache(medium);
				this.RemoveStructureFromCache();
			}
		}

		public Task DeleteAsync(Media model)
		{
			return this.DeleteAsync(model.Id);
		}

		public async Task DeleteFolderAsync(Guid id)
		{
			ConfiguredTaskAwaitable<MediaFolder> configuredTaskAwaitable = this.GetFolderByIdAsync(id).ConfigureAwait(false);
			MediaFolder mediaFolder = await configuredTaskAwaitable;
			if (mediaFolder != null)
			{
				App.Hooks.OnBeforeDelete<MediaFolder>(mediaFolder);
				await this._repo.DeleteFolder(id).ConfigureAwait(false);
				App.Hooks.OnAfterDelete<MediaFolder>(mediaFolder);
				this.RemoveFromCache(mediaFolder);
			}
		}

		public Task DeleteFolderAsync(MediaFolder model)
		{
			return this.DeleteFolderAsync(model.Id);
		}

		public string EnsureVersion(Guid id, int width, int? height = null)
		{
			TaskAwaiter<string> awaiter = this.EnsureVersionAsync(id, width, height).GetAwaiter();
			return awaiter.GetResult();
		}

		public async Task<string> EnsureVersionAsync(Guid id, int width, int? height = null)
		{
			string str;
			string str1;
			if (this._processor != null)
			{
				ConfiguredTaskAwaitable<Media> configuredTaskAwaitable = this.GetByIdAsync(id).ConfigureAwait(false);
				Media medium = await configuredTaskAwaitable;
				if (medium == null)
				{
					str1 = null;
				}
				else
				{
					ConfiguredTaskAwaitable<string> configuredTaskAwaitable1 = this.EnsureVersionAsync(medium, width, height).ConfigureAwait(false);
					str1 = await configuredTaskAwaitable1;
				}
				str = str1;
			}
			else
			{
				str = null;
			}
			return str;
		}

		public async Task<string> EnsureVersionAsync(Media media, int width, int? height = null)
		{
			string publicUrl;
			int? nullable1;
			IEnumerable<MediaVersion> hasValue;
			string str;
			if (App.MediaTypes.GetItem(media.Filename).AllowProcessing)
			{
				nullable1 = media.Width;
				int value = width;
				if (nullable1.GetValueOrDefault() == value & nullable1.HasValue)
				{
					if (height.HasValue)
					{
						nullable1 = media.Height;
						value = height.Value;
						if (!(nullable1.GetValueOrDefault() == value & nullable1.HasValue))
						{
							goto Label1;
						}
					}
					nullable1 = null;
					int? nullable2 = nullable1;
					nullable1 = null;
					publicUrl = this.GetPublicUrl(media, nullable2, nullable1, null);
					return publicUrl;
				}
			Label1:
				IEnumerable<MediaVersion> versions = 
					from v in media.Versions
					where v.Width == width
					select v;
				if (height.HasValue)
				{
					hasValue = versions.Where<MediaVersion>((MediaVersion v) => {
						int? nullable3 = v.Height;
						int? nullable = height;
						return nullable3.GetValueOrDefault() == nullable.GetValueOrDefault() & nullable3.HasValue == nullable.HasValue;
					});
				}
				else
				{
					IEnumerable<MediaVersion> mediaVersions = versions;
					hasValue = 
						from v in mediaVersions
						where !v.Height.HasValue
						select v;
				}
				versions = hasValue;
				MediaVersion mediaVersion = versions.FirstOrDefault<MediaVersion>();
				if (mediaVersion == null)
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						ConfiguredTaskAwaitable<IStorageSession> configuredTaskAwaitable = this._storage.OpenAsync().ConfigureAwait(false);
						using (IStorageSession storageSession = await configuredTaskAwaitable)
						{
							ConfiguredTaskAwaitable<bool> configuredTaskAwaitable1 = storageSession.GetAsync(media, media.Filename, memoryStream).ConfigureAwait(false);
							if (await configuredTaskAwaitable1)
							{
								memoryStream.Position = (long)0;
								using (MemoryStream memoryStream1 = new MemoryStream())
								{
									if (!height.HasValue)
									{
										this._processor.Scale(memoryStream, memoryStream1, width);
									}
									else
									{
										this._processor.CropScale(memoryStream, memoryStream1, width, height.Value);
									}
									memoryStream1.Position = (long)0;
									bool flag = false;
									lock (MediaService.ScaleMutex)
									{
										mediaVersion = versions.FirstOrDefault<MediaVersion>();
										if (mediaVersion == null)
										{
											FileInfo fileInfo = new FileInfo(media.Filename);
											MediaVersion mediaVersion1 = new MediaVersion()
											{
												Id = Guid.NewGuid(),
												Size = memoryStream1.Length,
												Width = width,
												Height = height,
												FileExtension = fileInfo.Extension
											};
											mediaVersion = mediaVersion1;
											media.Versions.Add(mediaVersion);
											this._repo.Save(media).Wait();
											this.RemoveFromCache(media);
											flag = true;
										}
									}
									if (!flag)
									{
										publicUrl = this.GetPublicUrl(media, new int?(width), height, mediaVersion.FileExtension);
									}
									else
									{
										ConfiguredTaskAwaitable<string> configuredTaskAwaitable2 = storageSession.PutAsync(media, this.GetResourceName(media, new int?(width), height, null), media.ContentType, memoryStream1).ConfigureAwait(false);
										publicUrl = await configuredTaskAwaitable2;
									}
								}
							}
							else
							{
								publicUrl = null;
							}
						}
					}
				}
				else
				{
					nullable1 = media.Width;
					value = width;
					if (nullable1.GetValueOrDefault() == value & nullable1.HasValue)
					{
						if (height.HasValue)
						{
							nullable1 = media.Height;
							value = height.Value;
							if (nullable1.GetValueOrDefault() != value | !nullable1.HasValue)
							{
								goto Label3;
							}
						}
						nullable1 = null;
						int? nullable4 = nullable1;
						nullable1 = null;
						str = this.GetPublicUrl(media, nullable4, nullable1, null);
						goto Label2;
					}
				Label3:
					str = this.GetPublicUrl(media, new int?(width), height, mediaVersion.FileExtension);
				Label2:
					publicUrl = str;
				}
			}
			else
			{
				nullable1 = null;
				int? nullable5 = nullable1;
				nullable1 = null;
				publicUrl = this.GetPublicUrl(media, nullable5, nullable1, null);
			}
			return publicUrl;
		}

		public Task<IEnumerable<Media>> GetAllByFolderIdAsync(Guid? folderId = null)
		{
			return this._repo.GetAll(folderId).ContinueWith<Task<IEnumerable<Media>>>((Task<IEnumerable<Guid>> t) => this._getFast(t.Result.ToArray<Guid>())).Unwrap<IEnumerable<Media>>();
		}

		public async Task<IEnumerable<MediaFolder>> GetAllFoldersAsync(Guid? folderId = null)
		{
			List<MediaFolder> mediaFolders = new List<MediaFolder>();
			foreach (Guid guid in await this._repo.GetAllFolders(folderId).ConfigureAwait(false))
			{
				ConfiguredTaskAwaitable<MediaFolder> configuredTaskAwaitable = this.GetFolderByIdAsync(guid).ConfigureAwait(false);
				MediaFolder mediaFolder = await configuredTaskAwaitable;
				if (mediaFolder == null)
				{
					continue;
				}
				mediaFolders.Add(mediaFolder);
			}
			return mediaFolders;
		}

		public async Task<Media> GetByIdAsync(Guid id)
		{
			Media medium;
			ICache cache = this._cache;
			if (cache != null)
			{
				medium = cache.Get<Media>(id.ToString());
			}
			else
			{
				medium = null;
			}
			Media medium1 = medium;
			if (medium1 == null)
			{
				ConfiguredTaskAwaitable<Media> configuredTaskAwaitable = this._repo.GetById(id).ConfigureAwait(false);
				medium1 = await configuredTaskAwaitable;
				this.OnLoad(medium1);
			}
			return medium1;
		}

		public Task<IEnumerable<Media>> GetByIdAsync(params Guid[] ids)
		{
			return this._repo.GetById(ids);
		}

		public async Task<MediaFolder> GetFolderByIdAsync(Guid id)
		{
			MediaFolder mediaFolder;
			ICache cache = this._cache;
			if (cache != null)
			{
				mediaFolder = cache.Get<MediaFolder>(id.ToString());
			}
			else
			{
				mediaFolder = null;
			}
			MediaFolder mediaFolder1 = mediaFolder;
			if (mediaFolder1 == null)
			{
				ConfiguredTaskAwaitable<MediaFolder> configuredTaskAwaitable = this._repo.GetFolderById(id).ConfigureAwait(false);
				mediaFolder1 = await configuredTaskAwaitable;
				this.OnFolderLoad(mediaFolder1);
			}
			return mediaFolder1;
		}

		private string GetPublicUrl(Media media, int? width = null, int? height = null, string extension = null)
		{
			string str;
			string resourceName = this.GetResourceName(media, width, height, extension);
			using (Config config = new Config(this._paramService))
			{
				string mediaCDN = config.MediaCDN;
				str = (String.IsNullOrWhiteSpace(mediaCDN) ? this._storage.GetPublicUrl(media, resourceName) : String.Concat(mediaCDN, this._storage.GetResourceName(media, resourceName)));
			}
			return str;
		}

		private string GetResourceName(Media media, int? width = null, int? height = null, string extension = null)
		{
			FileInfo fileInfo = new FileInfo(media.Filename);
			StringBuilder stringBuilder = new StringBuilder();
			if (!width.HasValue)
			{
				stringBuilder.Append(fileInfo.Name.Replace(fileInfo.Extension, ""));
			}
			else
			{
				stringBuilder.Append(fileInfo.Name.Replace(fileInfo.Extension, "_"));
				stringBuilder.Append(width);
				if (height.HasValue)
				{
					stringBuilder.Append("x");
					stringBuilder.Append(height.Value);
				}
			}
			if (!String.IsNullOrEmpty(extension))
			{
				stringBuilder.Append(extension);
			}
			else
			{
				stringBuilder.Append(fileInfo.Extension);
			}
			return stringBuilder.ToString();
		}

		public async Task<MediaStructure> GetStructureAsync()
		{
			MediaStructure mediaStructure;
			ICache cache = this._cache;
			if (cache != null)
			{
				mediaStructure = cache.Get<MediaStructure>("MediaStructure");
			}
			else
			{
				mediaStructure = null;
			}
			MediaStructure mediaStructure1 = mediaStructure;
			if (mediaStructure1 == null)
			{
				ConfiguredTaskAwaitable<MediaStructure> configuredTaskAwaitable = this._repo.GetStructure().ConfigureAwait(false);
				mediaStructure1 = await configuredTaskAwaitable;
				if (mediaStructure1 != null)
				{
					ICache cache1 = this._cache;
					if (cache1 != null)
					{
						cache1.Set<MediaStructure>("MediaStructure", mediaStructure1);
					}
					else
					{
					}
				}
			}
			return mediaStructure1;
		}

		public async Task MoveAsync(Media model, Guid? folderId)
		{
			ConfiguredTaskAwaitable configuredTaskAwaitable = this._repo.Move(model, folderId).ConfigureAwait(false);
			await configuredTaskAwaitable;
			this.RemoveFromCache(model);
			this.RemoveStructureFromCache();
		}

		private void OnFolderLoad(MediaFolder model)
		{
			if (model != null)
			{
				App.Hooks.OnLoad<MediaFolder>(model);
				ICache cache = this._cache;
				if (cache == null)
				{
					return;
				}
				cache.Set<MediaFolder>(model.Id.ToString(), model);
			}
		}

		private void OnLoad(Media model)
		{
			if (model != null)
			{
				int? nullable = null;
				int? nullable1 = nullable;
				nullable = null;
				model.PublicUrl = this.GetPublicUrl(model, nullable1, nullable, null);
				foreach (string metaProperty in App.MediaTypes.MetaProperties)
				{
					if (model.Properties.Any<KeyValuePair<string, string>>((KeyValuePair<string, string> p) => p.Key == metaProperty))
					{
						continue;
					}
					model.Properties.Add(metaProperty, null);
				}
				App.Hooks.OnLoad<Media>(model);
				ICache cache = this._cache;
				if (cache == null)
				{
					return;
				}
				cache.Set<Media>(model.Id.ToString(), model);
			}
		}

		private void RemoveFromCache(Media model)
		{
			ICache cache = this._cache;
			if (cache == null)
			{
				return;
			}
			cache.Remove(model.Id.ToString());
		}

		private void RemoveFromCache(MediaFolder model)
		{
			if (this._cache != null)
			{
				this._cache.Remove(model.Id.ToString());
				this.RemoveStructureFromCache();
			}
		}

		private void RemoveStructureFromCache()
		{
			ICache cache = this._cache;
			if (cache == null)
			{
				return;
			}
			cache.Remove("MediaStructure");
		}

		public async Task SaveAsync(Media model)
		{
			if (await this.GetByIdAsync(model.Id) == null)
			{
				throw new FileNotFoundException("You can only update meta data for an existing media object");
			}
			Validator.ValidateObject(model, new ValidationContext(model), true);
			App.Hooks.OnBeforeSave<Media>(model);
			await this._repo.Save(model).ConfigureAwait(false);
			App.Hooks.OnAfterSave<Media>(model);
			this.RemoveFromCache(model);
			this.RemoveStructureFromCache();
		}

		public async Task SaveAsync(MediaContent content)
		{
			ConfiguredTaskAwaitable<IStorageSession> configuredTaskAwaitable;
			ConfiguredTaskAwaitable<bool> configuredTaskAwaitable1;
			byte[] data;
			int num;
			int num1;
			ConfiguredTaskAwaitable<string> configuredTaskAwaitable2;
			Guid guid;
			if (!App.MediaTypes.IsSupported(content.Filename))
			{
				throw new ValidationException("Filetype not supported.");
			}
			Media folderId = null;
			if (content.Id.HasValue)
			{
				Guid? id = content.Id;
				ConfiguredTaskAwaitable<Media> configuredTaskAwaitable3 = this.GetByIdAsync(id.Value).ConfigureAwait(false);
				folderId = await configuredTaskAwaitable3;
			}
			if (folderId != null)
			{
				configuredTaskAwaitable = this._storage.OpenAsync().ConfigureAwait(false);
				using (IStorageSession storageSession = await configuredTaskAwaitable)
				{
					if (folderId.Versions.Count > 0)
					{
						foreach (MediaVersion version in folderId.Versions)
						{
							configuredTaskAwaitable1 = storageSession.DeleteAsync(folderId, this.GetResourceName(folderId, new int?(version.Width), version.Height, version.FileExtension)).ConfigureAwait(false);
							await configuredTaskAwaitable1;
						}
						folderId.Versions.Clear();
					}
					int? nullable = null;
					int? nullable1 = nullable;
					nullable = null;
					configuredTaskAwaitable1 = storageSession.DeleteAsync(folderId, this.GetResourceName(folderId, nullable1, nullable, null)).ConfigureAwait(false);
					await configuredTaskAwaitable1;
				}
				storageSession = null;
			}
			else
			{
				Media medium = new Media();
				guid = (folderId != null || content.Id.HasValue ? content.Id.Value : Guid.NewGuid());
				medium.Id = guid;
				medium.Created = DateTime.Now;
				folderId = medium;
				content.Id = new Guid?(folderId.Id);
			}
			MediaManager.MediaTypeItem item = App.MediaTypes.GetItem(content.Filename);
			folderId.Filename = content.Filename.Replace(" ", "_");
			folderId.FolderId = content.FolderId;
			folderId.Type = App.MediaTypes.GetMediaType(content.Filename);
			folderId.ContentType = item.ContentType;
			folderId.LastModified = DateTime.Now;
			if (this._processor != null && item.AllowProcessing && folderId.Type == MediaType.Image)
			{
				if (!(content is BinaryMediaContent))
				{
					BinaryReader binaryReader = new BinaryReader(((StreamMediaContent)content).Data);
					data = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
					((StreamMediaContent)content).Data.Position = (long)0;
				}
				else
				{
					data = ((BinaryMediaContent)content).Data;
				}
				this._processor.GetSize(data, out num, out num1);
				folderId.Width = new int?(num);
				folderId.Height = new int?(num1);
			}
			configuredTaskAwaitable = this._storage.OpenAsync().ConfigureAwait(false);
			using (storageSession = await configuredTaskAwaitable)
			{
				if (content is BinaryMediaContent)
				{
					BinaryMediaContent binaryMediaContent = (BinaryMediaContent)content;
					folderId.Size = (long)((int)binaryMediaContent.Data.Length);
					configuredTaskAwaitable2 = storageSession.PutAsync(folderId, folderId.Filename, folderId.ContentType, binaryMediaContent.Data).ConfigureAwait(false);
					await configuredTaskAwaitable2;
				}
				else if (content is StreamMediaContent)
				{
					StreamMediaContent streamMediaContent = (StreamMediaContent)content;
					Stream stream = streamMediaContent.Data;
					folderId.Size = streamMediaContent.Data.Length;
					configuredTaskAwaitable2 = storageSession.PutAsync(folderId, folderId.Filename, folderId.ContentType, stream).ConfigureAwait(false);
					await configuredTaskAwaitable2;
				}
			}
			storageSession = null;
			App.Hooks.OnBeforeSave<Media>(folderId);
			await this._repo.Save(folderId).ConfigureAwait(false);
			App.Hooks.OnAfterSave<Media>(folderId);
			this.RemoveFromCache(folderId);
			this.RemoveStructureFromCache();
		}

		public async Task SaveFolderAsync(MediaFolder model)
		{
			if (model.Id == Guid.Empty)
			{
				model.Id = Guid.NewGuid();
			}
			Validator.ValidateObject(model, new ValidationContext(model), true);
			App.Hooks.OnBeforeSave<MediaFolder>(model);
			await this._repo.SaveFolder(model).ConfigureAwait(false);
			App.Hooks.OnAfterSave<MediaFolder>(model);
			this.RemoveFromCache(model);
			this.RemoveStructureFromCache();
		}
	}
}