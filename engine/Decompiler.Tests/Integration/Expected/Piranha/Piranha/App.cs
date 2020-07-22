using Piranha.Cache;
using Piranha.Extend;
using Piranha.Models;
using Piranha.Runtime;
using Piranha.Security;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

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

		private Piranha.Cache.CacheLevel _cacheLevel;

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
				return;
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
				return 21;
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
			V_0 = null;
			App.Instance._mediaTypes.get_Documents().Add(".pdf", "application/pdf", V_0);
			V_0 = null;
			App.Instance._mediaTypes.get_Images().Add(".jpg", "image/jpeg", V_0);
			V_0 = null;
			App.Instance._mediaTypes.get_Images().Add(".jpeg", "image/jpeg", V_0);
			V_0 = null;
			App.Instance._mediaTypes.get_Images().Add(".png", "image/png", V_0);
			V_0 = null;
			App.Instance._mediaTypes.get_Videos().Add(".mp4", "video/mp4", V_0);
			V_0 = null;
			App.Instance._mediaTypes.get_Audio().Add(".mp3", "audio/mpeg", V_0);
			V_0 = null;
			App.Instance._mediaTypes.get_Audio().Add(".wav", "audio/wav", V_0);
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
			stackVariable154 = App.Instance._permissions.get_Item("Core");
			stackVariable155 = new PermissionItem();
			stackVariable155.set_Name("PiranhaPagePreview");
			stackVariable155.set_Title("Page Preview");
			stackVariable155.set_IsInternal(true);
			stackVariable154.Add(stackVariable155);
			stackVariable162 = App.Instance._permissions.get_Item("Core");
			stackVariable163 = new PermissionItem();
			stackVariable163.set_Name("PiranhaPostPreview");
			stackVariable163.set_Title("Post Preview");
			stackVariable163.set_IsInternal(true);
			stackVariable162.Add(stackVariable163);
			return;
		}

		private App()
		{
			this._cacheLevel = 3;
			base();
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
			return;
		}

		public static object DeserializeObject(string value, Type type)
		{
			V_0 = App.Instance._serializers.get_Item(type);
			if (V_0 != null)
			{
				return V_0.Deserialize(value);
			}
			return JsonConvert.DeserializeObject(value, type);
		}

		public static void Init(IApi api)
		{
			App.Instance.InitApp(api);
			return;
		}

		private void InitApp(IApi api)
		{
			if (!App._isInitialized)
			{
				V_0 = App._mutex;
				V_1 = false;
				try
				{
					Monitor.Enter(V_0, ref V_1);
					if (!App._isInitialized)
					{
						stackVariable7 = this._pageTypes;
						V_2 = api.get_PageTypes().GetAllAsync().GetAwaiter();
						stackVariable7.Init(V_2.GetResult());
						stackVariable15 = this._postTypes;
						V_3 = api.get_PostTypes().GetAllAsync().GetAwaiter();
						stackVariable15.Init(V_3.GetResult());
						stackVariable23 = this._siteTypes;
						V_4 = api.get_SiteTypes().GetAllAsync().GetAwaiter();
						stackVariable23.Init(V_4.GetResult());
						V_5 = this._modules.GetEnumerator();
						try
						{
							while (V_5.MoveNext())
							{
								V_5.get_Current().get_Instance().Init();
							}
						}
						finally
						{
							if (V_5 != null)
							{
								V_5.Dispose();
							}
						}
						App._isInitialized = true;
					}
				}
				finally
				{
					if (V_1)
					{
						Monitor.Exit(V_0);
					}
				}
			}
			return;
		}

		public static string SerializeObject(object obj, Type type)
		{
			V_0 = App.Instance._serializers.get_Item(type);
			if (V_0 == null)
			{
				return JsonConvert.SerializeObject(obj);
			}
			return V_0.Serialize(obj);
		}
	}
}