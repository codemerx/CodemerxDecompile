using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Piranha.Runtime
{
	public class MediaManager
	{
		public MediaManager.MediaTypeList Audio
		{
			get;
			set;
		}

		public MediaManager.MediaTypeList Documents
		{
			get;
			set;
		}

		public MediaManager.MediaTypeList Images
		{
			get;
			set;
		}

		public IList<string> MetaProperties
		{
			get;
			set;
		}

		public MediaManager.MediaTypeList Resources
		{
			get;
			set;
		}

		public MediaManager.MediaTypeList Videos
		{
			get;
			set;
		}

		public MediaManager()
		{
			this.u003cDocumentsu003ek__BackingField = new MediaManager.MediaTypeList(false);
			this.u003cImagesu003ek__BackingField = new MediaManager.MediaTypeList(true);
			this.u003cVideosu003ek__BackingField = new MediaManager.MediaTypeList(false);
			this.u003cAudiou003ek__BackingField = new MediaManager.MediaTypeList(false);
			this.u003cResourcesu003ek__BackingField = new MediaManager.MediaTypeList(false);
			this.u003cMetaPropertiesu003ek__BackingField = new List<string>();
			base();
			return;
		}

		public string GetContentType(string filename)
		{
			V_0 = new MediaManager.u003cu003ec__DisplayClass28_0();
			V_0.extension = Path.GetExtension(filename).ToLower();
			V_1 = null;
			stackVariable11 = this.get_Documents().SingleOrDefault<MediaManager.MediaTypeItem>(new Func<MediaManager.MediaTypeItem, bool>(V_0.u003cGetContentTypeu003eb__0));
			V_1 = stackVariable11;
			if (stackVariable11 != null)
			{
				return V_1.get_ContentType();
			}
			stackVariable17 = this.get_Images().SingleOrDefault<MediaManager.MediaTypeItem>(new Func<MediaManager.MediaTypeItem, bool>(V_0.u003cGetContentTypeu003eb__1));
			V_1 = stackVariable17;
			if (stackVariable17 != null)
			{
				return V_1.get_ContentType();
			}
			stackVariable23 = this.get_Videos().SingleOrDefault<MediaManager.MediaTypeItem>(new Func<MediaManager.MediaTypeItem, bool>(V_0.u003cGetContentTypeu003eb__2));
			V_1 = stackVariable23;
			if (stackVariable23 != null)
			{
				return V_1.get_ContentType();
			}
			stackVariable29 = this.get_Audio().SingleOrDefault<MediaManager.MediaTypeItem>(new Func<MediaManager.MediaTypeItem, bool>(V_0.u003cGetContentTypeu003eb__3));
			V_1 = stackVariable29;
			if (stackVariable29 != null)
			{
				return V_1.get_ContentType();
			}
			stackVariable35 = this.get_Resources().SingleOrDefault<MediaManager.MediaTypeItem>(new Func<MediaManager.MediaTypeItem, bool>(V_0.u003cGetContentTypeu003eb__4));
			V_1 = stackVariable35;
			if (stackVariable35 == null)
			{
				return "application/octet-stream";
			}
			return V_1.get_ContentType();
		}

		public MediaManager.MediaTypeItem GetItem(string filename)
		{
			V_0 = new MediaManager.u003cu003ec__DisplayClass29_0();
			V_0.extension = Path.GetExtension(filename).ToLower();
			V_1 = null;
			stackVariable11 = this.get_Documents().SingleOrDefault<MediaManager.MediaTypeItem>(new Func<MediaManager.MediaTypeItem, bool>(V_0.u003cGetItemu003eb__0));
			V_1 = stackVariable11;
			if (stackVariable11 != null)
			{
				return V_1;
			}
			stackVariable17 = this.get_Images().SingleOrDefault<MediaManager.MediaTypeItem>(new Func<MediaManager.MediaTypeItem, bool>(V_0.u003cGetItemu003eb__1));
			V_1 = stackVariable17;
			if (stackVariable17 != null)
			{
				return V_1;
			}
			stackVariable23 = this.get_Videos().SingleOrDefault<MediaManager.MediaTypeItem>(new Func<MediaManager.MediaTypeItem, bool>(V_0.u003cGetItemu003eb__2));
			V_1 = stackVariable23;
			if (stackVariable23 != null)
			{
				return V_1;
			}
			stackVariable29 = this.get_Audio().SingleOrDefault<MediaManager.MediaTypeItem>(new Func<MediaManager.MediaTypeItem, bool>(V_0.u003cGetItemu003eb__3));
			V_1 = stackVariable29;
			if (stackVariable29 != null)
			{
				return V_1;
			}
			stackVariable35 = this.get_Resources().SingleOrDefault<MediaManager.MediaTypeItem>(new Func<MediaManager.MediaTypeItem, bool>(V_0.u003cGetItemu003eb__4));
			V_1 = stackVariable35;
			if (stackVariable35 != null)
			{
				return V_1;
			}
			return null;
		}

		public MediaType GetMediaType(string filename)
		{
			V_0 = Path.GetExtension(filename);
			if (this.get_Documents().ContainsExtension(V_0))
			{
				return 1;
			}
			if (this.get_Images().ContainsExtension(V_0))
			{
				return 2;
			}
			if (this.get_Videos().ContainsExtension(V_0))
			{
				return 3;
			}
			if (this.get_Audio().ContainsExtension(V_0))
			{
				return 4;
			}
			if (this.get_Resources().ContainsExtension(V_0))
			{
				return 5;
			}
			return 0;
		}

		public bool IsSupported(string filename)
		{
			V_0 = Path.GetExtension(filename);
			if (this.get_Documents().ContainsExtension(V_0) || this.get_Images().ContainsExtension(V_0) || this.get_Videos().ContainsExtension(V_0) || this.get_Audio().ContainsExtension(V_0))
			{
				return true;
			}
			return this.get_Resources().ContainsExtension(V_0);
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
				base();
				return;
			}
		}

		public class MediaTypeList : List<MediaManager.MediaTypeItem>
		{
			private readonly bool _allowProcessing;

			public MediaTypeList(bool allowProcessing = false)
			{
				base();
				this._allowProcessing = allowProcessing;
				return;
			}

			public void Add(string extension, string contentType, bool? allowProcessing = null)
			{
				stackVariable1 = new MediaManager.MediaTypeItem();
				stackVariable1.set_Extension(extension.ToLower());
				stackVariable1.set_ContentType(contentType);
				if (allowProcessing.get_HasValue())
				{
					stackVariable8 = allowProcessing.get_Value();
				}
				else
				{
					stackVariable8 = this._allowProcessing;
				}
				stackVariable1.set_AllowProcessing(stackVariable8);
				this.Add(stackVariable1);
				return;
			}

			public bool ContainsExtension(string extension)
			{
				V_0 = new MediaManager.MediaTypeList.u003cu003ec__DisplayClass3_0();
				V_0.extension = extension;
				return this.Any<MediaManager.MediaTypeItem>(new Func<MediaManager.MediaTypeItem, bool>(V_0.u003cContainsExtensionu003eb__0));
			}
		}
	}
}