using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Piranha.Runtime
{
	public sealed class HookManager
	{
		private readonly Dictionary<Type, object> _onLoad;

		private readonly Dictionary<Type, object> _onBeforeSave;

		private readonly Dictionary<Type, object> _onValidate;

		private readonly Dictionary<Type, object> _onAfterSave;

		private readonly Dictionary<Type, object> _onBeforeDelete;

		private readonly Dictionary<Type, object> _onAfterDelete;

		public HookManager.ServiceHooks<Piranha.Models.Alias> Alias
		{
			get
			{
				return this.u003cAliasu003ek__BackingField;
			}
		}

		public HookManager.ValidationServiceHooks<Comment> Comments
		{
			get
			{
				return this.u003cCommentsu003ek__BackingField;
			}
		}

		public HookManager.ServiceHooks<Piranha.Models.Media> Media
		{
			get
			{
				return this.u003cMediau003ek__BackingField;
			}
		}

		public HookManager.ServiceHooks<Piranha.Models.MediaFolder> MediaFolder
		{
			get
			{
				return this.u003cMediaFolderu003ek__BackingField;
			}
		}

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

		public HookManager.ServiceHooks<PageBase> Pages
		{
			get
			{
				return this.u003cPagesu003ek__BackingField;
			}
		}

		public HookManager.ServiceHooks<Piranha.Models.Param> Param
		{
			get
			{
				return this.u003cParamu003ek__BackingField;
			}
		}

		public HookManager.ServiceHooks<PostBase> Posts
		{
			get
			{
				return this.u003cPostsu003ek__BackingField;
			}
		}

		public HookManager.ServiceHooks<Piranha.Models.Site> Site
		{
			get
			{
				return this.u003cSiteu003ek__BackingField;
			}
		}

		public HookManager.ServiceHooks<SiteContentBase> SiteContent
		{
			get
			{
				return this.u003cSiteContentu003ek__BackingField;
			}
		}

		public HookManager.SitemapServiceHooks Sitemap
		{
			get
			{
				return this.u003cSitemapu003ek__BackingField;
			}
		}

		public HookManager()
		{
			this._onLoad = new Dictionary<Type, object>();
			this._onBeforeSave = new Dictionary<Type, object>();
			this._onValidate = new Dictionary<Type, object>();
			this._onAfterSave = new Dictionary<Type, object>();
			this._onBeforeDelete = new Dictionary<Type, object>();
			this._onAfterDelete = new Dictionary<Type, object>();
			this.u003cAliasu003ek__BackingField = new HookManager.ServiceHooks<Piranha.Models.Alias>();
			this.u003cCommentsu003ek__BackingField = new HookManager.ValidationServiceHooks<Comment>();
			this.u003cMediau003ek__BackingField = new HookManager.ServiceHooks<Piranha.Models.Media>();
			this.u003cMediaFolderu003ek__BackingField = new HookManager.ServiceHooks<Piranha.Models.MediaFolder>();
			this.u003cPagesu003ek__BackingField = new HookManager.ServiceHooks<PageBase>();
			this.u003cParamu003ek__BackingField = new HookManager.ServiceHooks<Piranha.Models.Param>();
			this.u003cPostsu003ek__BackingField = new HookManager.ServiceHooks<PostBase>();
			this.u003cSiteu003ek__BackingField = new HookManager.ServiceHooks<Piranha.Models.Site>();
			this.u003cSiteContentu003ek__BackingField = new HookManager.ServiceHooks<SiteContentBase>();
			this.u003cSitemapu003ek__BackingField = new HookManager.SitemapServiceHooks();
			base();
			return;
		}

		internal void Clear<T>()
		{
			if (this._onLoad.ContainsKey(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Piranha.Runtime.HookManager::Clear()
			// Exception in: System.Void Clear()
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public void OnAfterDelete<T>(T model)
		{
			if (this._onAfterDelete.ContainsKey(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Piranha.Runtime.HookManager::OnAfterDelete(T)
			// Exception in: System.Void OnAfterDelete(T)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public void OnAfterSave<T>(T model)
		{
			if (this._onAfterSave.ContainsKey(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Piranha.Runtime.HookManager::OnAfterSave(T)
			// Exception in: System.Void OnAfterSave(T)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public void OnBeforeDelete<T>(T model)
		{
			if (this._onBeforeDelete.ContainsKey(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Piranha.Runtime.HookManager::OnBeforeDelete(T)
			// Exception in: System.Void OnBeforeDelete(T)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public void OnBeforeSave<T>(T model)
		{
			if (this._onBeforeSave.ContainsKey(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Piranha.Runtime.HookManager::OnBeforeSave(T)
			// Exception in: System.Void OnBeforeSave(T)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public void OnLoad<T>(T model)
		{
			if (this._onLoad.ContainsKey(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Piranha.Runtime.HookManager::OnLoad(T)
			// Exception in: System.Void OnLoad(T)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public void OnValidate<T>(T model)
		{
			if (this._onValidate.ContainsKey(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Piranha.Runtime.HookManager::OnValidate(T)
			// Exception in: System.Void OnValidate(T)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		internal void RegisterOnAfterDelete<T>(HookManager.ModelDelegate<T> hook)
		{
			if (!this._onAfterDelete.ContainsKey(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Piranha.Runtime.HookManager::RegisterOnAfterDelete(Piranha.Runtime.HookManager/ModelDelegate`1<T>)
			// Exception in: System.Void RegisterOnAfterDelete(Piranha.Runtime.HookManager/ModelDelegate<T>)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		internal void RegisterOnAfterSave<T>(HookManager.ModelDelegate<T> hook)
		{
			if (!this._onAfterSave.ContainsKey(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Piranha.Runtime.HookManager::RegisterOnAfterSave(Piranha.Runtime.HookManager/ModelDelegate`1<T>)
			// Exception in: System.Void RegisterOnAfterSave(Piranha.Runtime.HookManager/ModelDelegate<T>)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		internal void RegisterOnBeforeDelete<T>(HookManager.ModelDelegate<T> hook)
		{
			if (!this._onBeforeDelete.ContainsKey(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Piranha.Runtime.HookManager::RegisterOnBeforeDelete(Piranha.Runtime.HookManager/ModelDelegate`1<T>)
			// Exception in: System.Void RegisterOnBeforeDelete(Piranha.Runtime.HookManager/ModelDelegate<T>)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		internal void RegisterOnBeforeSave<T>(HookManager.ModelDelegate<T> hook)
		{
			if (!this._onBeforeSave.ContainsKey(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Piranha.Runtime.HookManager::RegisterOnBeforeSave(Piranha.Runtime.HookManager/ModelDelegate`1<T>)
			// Exception in: System.Void RegisterOnBeforeSave(Piranha.Runtime.HookManager/ModelDelegate<T>)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		internal void RegisterOnLoad<T>(HookManager.ModelDelegate<T> hook)
		{
			if (!this._onLoad.ContainsKey(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Piranha.Runtime.HookManager::RegisterOnLoad(Piranha.Runtime.HookManager/ModelDelegate`1<T>)
			// Exception in: System.Void RegisterOnLoad(Piranha.Runtime.HookManager/ModelDelegate<T>)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		internal void RegisterOnValidate<T>(HookManager.ModelDelegate<T> hook)
		{
			if (!this._onValidate.ContainsKey(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Piranha.Runtime.HookManager::RegisterOnValidate(Piranha.Runtime.HookManager/ModelDelegate`1<T>)
			// Exception in: System.Void RegisterOnValidate(Piranha.Runtime.HookManager/ModelDelegate<T>)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public delegate void ModelDelegate<T>(T model);

		public class ServiceHooks<T>
		{
			public ServiceHooks()
			{
				base();
				return;
			}

			public void Clear()
			{
				App.get_Hooks().Clear<T>();
				return;
			}

			public void RegisterOnAfterDelete(HookManager.ModelDelegate<T> hook)
			{
				App.get_Hooks().RegisterOnAfterDelete<T>(hook);
				return;
			}

			public void RegisterOnAfterSave(HookManager.ModelDelegate<T> hook)
			{
				App.get_Hooks().RegisterOnAfterSave<T>(hook);
				return;
			}

			public void RegisterOnBeforeDelete(HookManager.ModelDelegate<T> hook)
			{
				App.get_Hooks().RegisterOnBeforeDelete<T>(hook);
				return;
			}

			public void RegisterOnBeforeSave(HookManager.ModelDelegate<T> hook)
			{
				App.get_Hooks().RegisterOnBeforeSave<T>(hook);
				return;
			}

			public void RegisterOnLoad(HookManager.ModelDelegate<T> hook)
			{
				App.get_Hooks().RegisterOnLoad<T>(hook);
				return;
			}
		}

		public delegate Piranha.Models.Sitemap SitemapDelegate(Piranha.Models.Sitemap sitemap);

		public class SitemapServiceHooks
		{
			public SitemapServiceHooks()
			{
				base();
				return;
			}

			public void RegisterOnInvalidate(HookManager.ModelDelegate<Piranha.Models.Sitemap> hook)
			{
				App.get_Hooks().RegisterOnBeforeDelete<Piranha.Models.Sitemap>(hook);
				return;
			}

			public void RegisterOnLoad(HookManager.ModelDelegate<Piranha.Models.Sitemap> hook)
			{
				App.get_Hooks().RegisterOnLoad<Piranha.Models.Sitemap>(hook);
				return;
			}
		}

		public delegate string SlugDelegate(string str);

		public class ValidationServiceHooks<T> : HookManager.ServiceHooks<T>
		{
			public ValidationServiceHooks()
			{
				base();
				return;
			}

			public void RegisterOnValidate(HookManager.ModelDelegate<T> hook)
			{
				App.get_Hooks().RegisterOnLoad<T>(hook);
				return;
			}
		}
	}
}