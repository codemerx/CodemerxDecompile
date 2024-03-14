using Piranha.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Piranha.Runtime
{
	public class MediaManager
	{
		public MediaManager.MediaTypeList Audio { get; set; } = new MediaManager.MediaTypeList(false);

		public MediaManager.MediaTypeList Documents { get; set; } = new MediaManager.MediaTypeList(false);

		public MediaManager.MediaTypeList Images { get; set; } = new MediaManager.MediaTypeList(true);

		public IList<string> MetaProperties { get; set; } = new List<string>();

		public MediaManager.MediaTypeList Resources { get; set; } = new MediaManager.MediaTypeList(false);

		public MediaManager.MediaTypeList Videos { get; set; } = new MediaManager.MediaTypeList(false);

		public MediaManager()
		{
		}

		public string GetContentType(string filename)
		{
			string lower = Path.GetExtension(filename).ToLower();
			MediaManager.MediaTypeItem mediaTypeItem = null;
			MediaManager.MediaTypeItem mediaTypeItem1 = this.Documents.SingleOrDefault<MediaManager.MediaTypeItem>((MediaManager.MediaTypeItem t) => t.Extension == lower);
			mediaTypeItem = mediaTypeItem1;
			if (mediaTypeItem1 != null)
			{
				return mediaTypeItem.ContentType;
			}
			MediaManager.MediaTypeItem mediaTypeItem2 = this.Images.SingleOrDefault<MediaManager.MediaTypeItem>((MediaManager.MediaTypeItem t) => t.Extension == lower);
			mediaTypeItem = mediaTypeItem2;
			if (mediaTypeItem2 != null)
			{
				return mediaTypeItem.ContentType;
			}
			MediaManager.MediaTypeItem mediaTypeItem3 = this.Videos.SingleOrDefault<MediaManager.MediaTypeItem>((MediaManager.MediaTypeItem t) => t.Extension == lower);
			mediaTypeItem = mediaTypeItem3;
			if (mediaTypeItem3 != null)
			{
				return mediaTypeItem.ContentType;
			}
			MediaManager.MediaTypeItem mediaTypeItem4 = this.Audio.SingleOrDefault<MediaManager.MediaTypeItem>((MediaManager.MediaTypeItem t) => t.Extension == lower);
			mediaTypeItem = mediaTypeItem4;
			if (mediaTypeItem4 != null)
			{
				return mediaTypeItem.ContentType;
			}
			MediaManager.MediaTypeItem mediaTypeItem5 = this.Resources.SingleOrDefault<MediaManager.MediaTypeItem>((MediaManager.MediaTypeItem t) => t.Extension == lower);
			mediaTypeItem = mediaTypeItem5;
			if (mediaTypeItem5 == null)
			{
				return "application/octet-stream";
			}
			return mediaTypeItem.ContentType;
		}

		public MediaManager.MediaTypeItem GetItem(string filename)
		{
			string lower = Path.GetExtension(filename).ToLower();
			MediaManager.MediaTypeItem mediaTypeItem = null;
			MediaManager.MediaTypeItem mediaTypeItem1 = this.Documents.SingleOrDefault<MediaManager.MediaTypeItem>((MediaManager.MediaTypeItem t) => t.Extension == lower);
			mediaTypeItem = mediaTypeItem1;
			if (mediaTypeItem1 != null)
			{
				return mediaTypeItem;
			}
			MediaManager.MediaTypeItem mediaTypeItem2 = this.Images.SingleOrDefault<MediaManager.MediaTypeItem>((MediaManager.MediaTypeItem t) => t.Extension == lower);
			mediaTypeItem = mediaTypeItem2;
			if (mediaTypeItem2 != null)
			{
				return mediaTypeItem;
			}
			MediaManager.MediaTypeItem mediaTypeItem3 = this.Videos.SingleOrDefault<MediaManager.MediaTypeItem>((MediaManager.MediaTypeItem t) => t.Extension == lower);
			mediaTypeItem = mediaTypeItem3;
			if (mediaTypeItem3 != null)
			{
				return mediaTypeItem;
			}
			MediaManager.MediaTypeItem mediaTypeItem4 = this.Audio.SingleOrDefault<MediaManager.MediaTypeItem>((MediaManager.MediaTypeItem t) => t.Extension == lower);
			mediaTypeItem = mediaTypeItem4;
			if (mediaTypeItem4 != null)
			{
				return mediaTypeItem;
			}
			MediaManager.MediaTypeItem mediaTypeItem5 = this.Resources.SingleOrDefault<MediaManager.MediaTypeItem>((MediaManager.MediaTypeItem t) => t.Extension == lower);
			mediaTypeItem = mediaTypeItem5;
			if (mediaTypeItem5 != null)
			{
				return mediaTypeItem;
			}
			return null;
		}

		public MediaType GetMediaType(string filename)
		{
			string extension = Path.GetExtension(filename);
			if (this.Documents.ContainsExtension(extension))
			{
				return MediaType.Document;
			}
			if (this.Images.ContainsExtension(extension))
			{
				return MediaType.Image;
			}
			if (this.Videos.ContainsExtension(extension))
			{
				return MediaType.Video;
			}
			if (this.Audio.ContainsExtension(extension))
			{
				return MediaType.Audio;
			}
			if (this.Resources.ContainsExtension(extension))
			{
				return MediaType.Resource;
			}
			return MediaType.Unknown;
		}

		public bool IsSupported(string filename)
		{
			string extension = Path.GetExtension(filename);
			if (this.Documents.ContainsExtension(extension) || this.Images.ContainsExtension(extension) || this.Videos.ContainsExtension(extension) || this.Audio.ContainsExtension(extension))
			{
				return true;
			}
			return this.Resources.ContainsExtension(extension);
		}

		public class MediaTypeItem
		{
			public bool AllowProcessing
			{
				get;
				set;
			}

			public string ContentType
			{
				get;
				set;
			}

			public string Extension
			{
				get;
				set;
			}

			public MediaTypeItem()
			{
			}
		}

		public class MediaTypeList : List<MediaManager.MediaTypeItem>
		{
			private readonly bool _allowProcessing;

			public MediaTypeList(bool allowProcessing = false)
			{
				this._allowProcessing = allowProcessing;
			}

			public void Add(string extension, string contentType, bool? allowProcessing = null)
			{
				base.Add(new MediaManager.MediaTypeItem()
				{
					Extension = extension.ToLower(),
					ContentType = contentType,
					AllowProcessing = (allowProcessing.HasValue ? allowProcessing.Value : this._allowProcessing)
				});
			}

			public bool ContainsExtension(string extension)
			{
				return this.Any<MediaManager.MediaTypeItem>((MediaManager.MediaTypeItem t) => t.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase));
			}
		}
	}
}