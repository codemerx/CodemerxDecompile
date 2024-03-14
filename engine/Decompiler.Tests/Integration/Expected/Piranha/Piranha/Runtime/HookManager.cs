using Piranha;
using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Piranha.Runtime
{
	public sealed class HookManager
	{
		private readonly Dictionary<Type, object> _onLoad = new Dictionary<Type, object>();

		private readonly Dictionary<Type, object> _onBeforeSave = new Dictionary<Type, object>();

		private readonly Dictionary<Type, object> _onValidate = new Dictionary<Type, object>();

		private readonly Dictionary<Type, object> _onAfterSave = new Dictionary<Type, object>();

		private readonly Dictionary<Type, object> _onBeforeDelete = new Dictionary<Type, object>();

		private readonly Dictionary<Type, object> _onAfterDelete = new Dictionary<Type, object>();

		public HookManager.ServiceHooks<Piranha.Models.Alias> Alias { get; } = new HookManager.ServiceHooks<Piranha.Models.Alias>();

		public HookManager.ValidationServiceHooks<Comment> Comments { get; } = new HookManager.ValidationServiceHooks<Comment>();

		public HookManager.ServiceHooks<Piranha.Models.Media> Media { get; } = new HookManager.ServiceHooks<Piranha.Models.Media>();

		public HookManager.ServiceHooks<Piranha.Models.MediaFolder> MediaFolder { get; } = new HookManager.ServiceHooks<Piranha.Models.MediaFolder>();

		public HookManager.SitemapDelegate OnGenerateSitemap
		{
			get;
			set;
		}

		public HookManager.SlugDelegate OnGenerateSlug
		{
			get;
			set;
		}

		public HookManager.ServiceHooks<PageBase> Pages { get; } = new HookManager.ServiceHooks<PageBase>();

		public HookManager.ServiceHooks<Piranha.Models.Param> Param { get; } = new HookManager.ServiceHooks<Piranha.Models.Param>();

		public HookManager.ServiceHooks<PostBase> Posts { get; } = new HookManager.ServiceHooks<PostBase>();

		public HookManager.ServiceHooks<Piranha.Models.Site> Site { get; } = new HookManager.ServiceHooks<Piranha.Models.Site>();

		public HookManager.ServiceHooks<SiteContentBase> SiteContent { get; } = new HookManager.ServiceHooks<SiteContentBase>();

		public HookManager.SitemapServiceHooks Sitemap { get; } = new HookManager.SitemapServiceHooks();

		public HookManager()
		{
		}

		internal void Clear<T>()
		{
			if (this._onLoad.ContainsKey(typeof(T)))
			{
				this._onLoad.Remove(typeof(T));
			}
			if (this._onBeforeSave.ContainsKey(typeof(T)))
			{
				this._onBeforeSave.Remove(typeof(T));
			}
			if (this._onValidate.ContainsKey(typeof(T)))
			{
				this._onValidate.Remove(typeof(T));
			}
			if (this._onAfterSave.ContainsKey(typeof(T)))
			{
				this._onAfterSave.Remove(typeof(T));
			}
			if (this._onBeforeDelete.ContainsKey(typeof(T)))
			{
				this._onBeforeDelete.Remove(typeof(T));
			}
			if (this._onAfterDelete.ContainsKey(typeof(T)))
			{
				this._onAfterDelete.Remove(typeof(T));
			}
		}

		public void OnAfterDelete<T>(T model)
		{
			if (this._onAfterDelete.ContainsKey(typeof(T)))
			{
				foreach (HookManager.ModelDelegate<T> item in (List<HookManager.ModelDelegate<T>>)this._onAfterDelete[typeof(T)])
				{
					item(model);
				}
			}
		}

		public void OnAfterSave<T>(T model)
		{
			if (this._onAfterSave.ContainsKey(typeof(T)))
			{
				foreach (HookManager.ModelDelegate<T> item in (List<HookManager.ModelDelegate<T>>)this._onAfterSave[typeof(T)])
				{
					item(model);
				}
			}
		}

		public void OnBeforeDelete<T>(T model)
		{
			if (this._onBeforeDelete.ContainsKey(typeof(T)))
			{
				foreach (HookManager.ModelDelegate<T> item in (List<HookManager.ModelDelegate<T>>)this._onBeforeDelete[typeof(T)])
				{
					item(model);
				}
			}
		}

		public void OnBeforeSave<T>(T model)
		{
			if (this._onBeforeSave.ContainsKey(typeof(T)))
			{
				foreach (HookManager.ModelDelegate<T> item in (List<HookManager.ModelDelegate<T>>)this._onBeforeSave[typeof(T)])
				{
					item(model);
				}
			}
		}

		public void OnLoad<T>(T model)
		{
			if (this._onLoad.ContainsKey(typeof(T)))
			{
				foreach (HookManager.ModelDelegate<T> item in (List<HookManager.ModelDelegate<T>>)this._onLoad[typeof(T)])
				{
					item(model);
				}
			}
		}

		public void OnValidate<T>(T model)
		{
			if (this._onValidate.ContainsKey(typeof(T)))
			{
				foreach (HookManager.ModelDelegate<T> item in (List<HookManager.ModelDelegate<T>>)this._onValidate[typeof(T)])
				{
					item(model);
				}
			}
		}

		internal void RegisterOnAfterDelete<T>(HookManager.ModelDelegate<T> hook)
		{
			if (!this._onAfterDelete.ContainsKey(typeof(T)))
			{
				this._onAfterDelete[typeof(T)] = new List<HookManager.ModelDelegate<T>>();
			}
			((List<HookManager.ModelDelegate<T>>)this._onAfterDelete[typeof(T)]).Add(hook);
		}

		internal void RegisterOnAfterSave<T>(HookManager.ModelDelegate<T> hook)
		{
			if (!this._onAfterSave.ContainsKey(typeof(T)))
			{
				this._onAfterSave[typeof(T)] = new List<HookManager.ModelDelegate<T>>();
			}
			((List<HookManager.ModelDelegate<T>>)this._onAfterSave[typeof(T)]).Add(hook);
		}

		internal void RegisterOnBeforeDelete<T>(HookManager.ModelDelegate<T> hook)
		{
			if (!this._onBeforeDelete.ContainsKey(typeof(T)))
			{
				this._onBeforeDelete[typeof(T)] = new List<HookManager.ModelDelegate<T>>();
			}
			((List<HookManager.ModelDelegate<T>>)this._onBeforeDelete[typeof(T)]).Add(hook);
		}

		internal void RegisterOnBeforeSave<T>(HookManager.ModelDelegate<T> hook)
		{
			if (!this._onBeforeSave.ContainsKey(typeof(T)))
			{
				this._onBeforeSave[typeof(T)] = new List<HookManager.ModelDelegate<T>>();
			}
			((List<HookManager.ModelDelegate<T>>)this._onBeforeSave[typeof(T)]).Add(hook);
		}

		internal void RegisterOnLoad<T>(HookManager.ModelDelegate<T> hook)
		{
			if (!this._onLoad.ContainsKey(typeof(T)))
			{
				this._onLoad[typeof(T)] = new List<HookManager.ModelDelegate<T>>();
			}
			((List<HookManager.ModelDelegate<T>>)this._onLoad[typeof(T)]).Add(hook);
		}

		internal void RegisterOnValidate<T>(HookManager.ModelDelegate<T> hook)
		{
			if (!this._onValidate.ContainsKey(typeof(T)))
			{
				this._onValidate[typeof(T)] = new List<HookManager.ModelDelegate<T>>();
			}
			((List<HookManager.ModelDelegate<T>>)this._onValidate[typeof(T)]).Add(hook);
		}

		public delegate void ModelDelegate<T>(T model);

		public class ServiceHooks<T>
		{
			public ServiceHooks()
			{
			}

			public void Clear()
			{
				App.Hooks.Clear<T>();
			}

			public void RegisterOnAfterDelete(HookManager.ModelDelegate<T> hook)
			{
				App.Hooks.RegisterOnAfterDelete<T>(hook);
			}

			public void RegisterOnAfterSave(HookManager.ModelDelegate<T> hook)
			{
				App.Hooks.RegisterOnAfterSave<T>(hook);
			}

			public void RegisterOnBeforeDelete(HookManager.ModelDelegate<T> hook)
			{
				App.Hooks.RegisterOnBeforeDelete<T>(hook);
			}

			public void RegisterOnBeforeSave(HookManager.ModelDelegate<T> hook)
			{
				App.Hooks.RegisterOnBeforeSave<T>(hook);
			}

			public void RegisterOnLoad(HookManager.ModelDelegate<T> hook)
			{
				App.Hooks.RegisterOnLoad<T>(hook);
			}
		}

		public delegate Piranha.Models.Sitemap SitemapDelegate(Piranha.Models.Sitemap sitemap);

		public class SitemapServiceHooks
		{
			public SitemapServiceHooks()
			{
			}

			public void RegisterOnInvalidate(HookManager.ModelDelegate<Piranha.Models.Sitemap> hook)
			{
				App.Hooks.RegisterOnBeforeDelete<Piranha.Models.Sitemap>(hook);
			}

			public void RegisterOnLoad(HookManager.ModelDelegate<Piranha.Models.Sitemap> hook)
			{
				App.Hooks.RegisterOnLoad<Piranha.Models.Sitemap>(hook);
			}
		}

		public delegate string SlugDelegate(string str);

		public class ValidationServiceHooks<T> : HookManager.ServiceHooks<T>
		{
			public ValidationServiceHooks()
			{
			}

			public void RegisterOnValidate(HookManager.ModelDelegate<T> hook)
			{
				App.Hooks.RegisterOnLoad<T>(hook);
			}
		}
	}
}