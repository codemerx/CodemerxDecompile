using Newtonsoft.Json;
using Piranha.Cache;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Extend.Serializers;
using Piranha.Models;
using Piranha.Runtime;
using Piranha.Security;
using Piranha.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Piranha
{
	public sealed class App
	{
		private readonly static App Instance;

		private readonly static object _mutex;

		private static volatile bool _isInitialized;

		private readonly AppBlockList _blocks;

		private readonly AppFieldList _fields;

		private readonly AppModuleList _modules;

		private readonly MediaManager _mediaTypes;

		private readonly SerializerManager _serializers;

		private readonly HookManager _hooks;

		private readonly PermissionManager _permissions;

		private Piranha.Cache.CacheLevel _cacheLevel = Piranha.Cache.CacheLevel.Full;

		private IMarkdown _markdown;

		private readonly ContentTypeList<PageType> _pageTypes;

		private readonly ContentTypeList<PostType> _postTypes;

		private readonly ContentTypeList<SiteType> _siteTypes;

		public static AppBlockList Blocks
		{
			get
			{
				return App.Instance._blocks;
			}
		}

		public static Piranha.Cache.CacheLevel CacheLevel
		{
			get
			{
				return App.Instance._cacheLevel;
			}
			set
			{
				App.Instance._cacheLevel = value;
			}
		}

		public static AppFieldList Fields
		{
			get
			{
				return App.Instance._fields;
			}
		}

		public static HookManager Hooks
		{
			get
			{
				return App.Instance._hooks;
			}
		}

		public static IMarkdown Markdown
		{
			get
			{
				return App.Instance._markdown;
			}
		}

		public static MediaManager MediaTypes
		{
			get
			{
				return App.Instance._mediaTypes;
			}
		}

		public static AppModuleList Modules
		{
			get
			{
				return App.Instance._modules;
			}
		}

		public static ContentTypeList<PageType> PageTypes
		{
			get
			{
				return App.Instance._pageTypes;
			}
		}

		public static PermissionManager Permissions
		{
			get
			{
				return App.Instance._permissions;
			}
		}

		public static ContentTypeList<PostType> PostTypes
		{
			get
			{
				return App.Instance._postTypes;
			}
		}

		public static BindingFlags PropertyBindings
		{
			get
			{
				return BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;
			}
		}

		public static SerializerManager Serializers
		{
			get
			{
				return App.Instance._serializers;
			}
		}

		public static ContentTypeList<SiteType> SiteTypes
		{
			get
			{
				return App.Instance._siteTypes;
			}
		}

		static App()
		{
			App._mutex = new Object();
			App.Instance = new App();
			bool? nullable = null;
			App.Instance._mediaTypes.Documents.Add(".pdf", "application/pdf", nullable);
			nullable = null;
			App.Instance._mediaTypes.Images.Add(".jpg", "image/jpeg", nullable);
			nullable = null;
			App.Instance._mediaTypes.Images.Add(".jpeg", "image/jpeg", nullable);
			nullable = null;
			App.Instance._mediaTypes.Images.Add(".png", "image/png", nullable);
			nullable = null;
			App.Instance._mediaTypes.Videos.Add(".mp4", "video/mp4", nullable);
			nullable = null;
			App.Instance._mediaTypes.Audio.Add(".mp3", "audio/mpeg", nullable);
			nullable = null;
			App.Instance._mediaTypes.Audio.Add(".wav", "audio/wav", nullable);
			App.Instance._fields.Register<AudioField>();
			App.Instance._fields.Register<CheckBoxField>();
			App.Instance._fields.Register<DateField>();
			App.Instance._fields.Register<DocumentField>();
			App.Instance._fields.Register<HtmlField>();
			App.Instance._fields.Register<ImageField>();
			App.Instance._fields.Register<MarkdownField>();
			App.Instance._fields.Register<MediaField>();
			App.Instance._fields.Register<NumberField>();
			App.Instance._fields.Register<PageField>();
			App.Instance._fields.Register<PostField>();
			App.Instance._fields.Register<ReadonlyField>();
			App.Instance._fields.Register<StringField>();
			App.Instance._fields.Register<TextField>();
			App.Instance._fields.Register<VideoField>();
			App.Instance._fields.RegisterSelect<ImageAspect>();
			App.Instance._blocks.Register<AudioBlock>();
			App.Instance._blocks.Register<ColumnBlock>();
			App.Instance._blocks.Register<HtmlBlock>();
			App.Instance._blocks.Register<HtmlColumnBlock>();
			App.Instance._blocks.Register<ImageBlock>();
			App.Instance._blocks.Register<ImageGalleryBlock>();
			App.Instance._blocks.Register<PageBlock>();
			App.Instance._blocks.Register<PostBlock>();
			App.Instance._blocks.Register<QuoteBlock>();
			App.Instance._blocks.Register<SeparatorBlock>();
			App.Instance._blocks.Register<TextBlock>();
			App.Instance._blocks.Register<VideoBlock>();
			App.Instance._serializers.Register<CheckBoxField>(new CheckBoxFieldSerializer<CheckBoxField>());
			App.Instance._serializers.Register<DateField>(new DateFieldSerializer());
			App.Instance._serializers.Register<DocumentField>(new DocumentFieldSerializer());
			App.Instance._serializers.Register<HtmlField>(new StringFieldSerializer<HtmlField>());
			App.Instance._serializers.Register<MarkdownField>(new StringFieldSerializer<MarkdownField>());
			App.Instance._serializers.Register<MediaField>(new MediaFieldSerializer());
			App.Instance._serializers.Register<NumberField>(new IntegerFieldSerializer<NumberField>());
			App.Instance._serializers.Register<PageField>(new PageFieldSerializer());
			App.Instance._serializers.Register<PostField>(new PostFieldSerializer());
			App.Instance._serializers.Register<StringField>(new StringFieldSerializer<StringField>());
			App.Instance._serializers.Register<TextField>(new StringFieldSerializer<TextField>());
			App.Instance._serializers.Register<ImageField>(new ImageFieldSerializer());
			App.Instance._serializers.Register<VideoField>(new VideoFieldSerializer());
			App.Instance._serializers.Register<AudioField>(new AudioFieldSerializer());
			App.Instance._markdown = new DefaultMarkdown();
			App.Instance._permissions["Core"].Add(new PermissionItem()
			{
				Name = "PiranhaPagePreview",
				Title = "Page Preview",
				IsInternal = true
			});
			App.Instance._permissions["Core"].Add(new PermissionItem()
			{
				Name = "PiranhaPostPreview",
				Title = "Post Preview",
				IsInternal = true
			});
		}

		private App()
		{
			this._blocks = new AppBlockList();
			this._fields = new AppFieldList();
			this._modules = new AppModuleList();
			this._mediaTypes = new MediaManager();
			this._serializers = new SerializerManager();
			this._hooks = new HookManager();
			this._permissions = new PermissionManager();
			this._pageTypes = new ContentTypeList<PageType>();
			this._postTypes = new ContentTypeList<PostType>();
			this._siteTypes = new ContentTypeList<SiteType>();
		}

		public static object DeserializeObject(string value, Type type)
		{
			ISerializer item = App.Instance._serializers[type];
			if (item != null)
			{
				return item.Deserialize(value);
			}
			return JsonConvert.DeserializeObject(value, type);
		}

		public static void Init(IApi api)
		{
			App.Instance.InitApp(api);
		}

		private void InitApp(IApi api)
		{
			if (!App._isInitialized)
			{
				lock (App._mutex)
				{
					if (!App._isInitialized)
					{
						ContentTypeList<PageType> contentTypeList = this._pageTypes;
						TaskAwaiter<IEnumerable<PageType>> awaiter = api.PageTypes.GetAllAsync().GetAwaiter();
						contentTypeList.Init(awaiter.GetResult());
						ContentTypeList<PostType> contentTypeList1 = this._postTypes;
						TaskAwaiter<IEnumerable<PostType>> taskAwaiter = api.PostTypes.GetAllAsync().GetAwaiter();
						contentTypeList1.Init(taskAwaiter.GetResult());
						ContentTypeList<SiteType> contentTypeList2 = this._siteTypes;
						TaskAwaiter<IEnumerable<SiteType>> awaiter1 = api.SiteTypes.GetAllAsync().GetAwaiter();
						contentTypeList2.Init(awaiter1.GetResult());
						foreach (AppModule _module in this._modules)
						{
							_module.Instance.Init();
						}
						App._isInitialized = true;
					}
				}
			}
		}

		public static string SerializeObject(object obj, Type type)
		{
			ISerializer item = App.Instance._serializers[type];
			if (item == null)
			{
				return JsonConvert.SerializeObject(obj);
			}
			return item.Serialize(obj);
		}
	}
}