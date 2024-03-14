using Piranha;
using Piranha.Extend;
using Piranha.Models;
using Piranha.Services;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Page", Shorthand="Page", Component="page-field")]
	public class PageField : IField, IEquatable<PageField>
	{
		public bool HasValue
		{
			get
			{
				return this.Page != null;
			}
		}

		public Guid? Id
		{
			get;
			set;
		}

		public PageInfo Page
		{
			get;
			private set;
		}

		public PageField()
		{
		}

		public override bool Equals(object obj)
		{
			PageField pageField = obj as PageField;
			if (pageField == null)
			{
				return false;
			}
			return this.Equals(pageField);
		}

		public virtual bool Equals(PageField obj)
		{
			if (obj == null)
			{
				return false;
			}
			Guid? id = this.Id;
			Guid? nullable = obj.Id;
			if (id.HasValue != nullable.HasValue)
			{
				return false;
			}
			if (!id.HasValue)
			{
				return true;
			}
			return id.GetValueOrDefault() == nullable.GetValueOrDefault();
		}

		public override int GetHashCode()
		{
			if (!this.Id.HasValue)
			{
				return 0;
			}
			return this.Id.GetHashCode();
		}

		public virtual Task<T> GetPageAsync<T>(IApi api)
		where T : GenericPage<T>
		{
			if (!this.Id.HasValue)
			{
				return null;
			}
			return api.Pages.GetByIdAsync<T>(this.Id.Value);
		}

		public virtual string GetTitle()
		{
			PageInfo page = this.Page;
			if (page != null)
			{
				return page.Title;
			}
			return null;
		}

		public virtual async Task Init(IApi api)
		{
			Guid? id = this.Id;
			if (id.HasValue)
			{
				IPageService pages = api.Pages;
				id = this.Id;
				ConfiguredTaskAwaitable<PageInfo> configuredTaskAwaitable = pages.GetByIdAsync<PageInfo>(id.Value).ConfigureAwait(false);
				this.Page = await configuredTaskAwaitable;
				if (this.Page == null)
				{
					id = null;
					this.Id = id;
				}
			}
		}

		public static bool operator ==(PageField field1, PageField field2)
		{
			if (field1 != null && field2 != null)
			{
				return field1.Equals(field2);
			}
			if (field1 == null && field2 == null)
			{
				return true;
			}
			return false;
		}

		public static implicit operator PageField(Guid guid)
		{
			return new PageField()
			{
				Id = new Guid?(guid)
			};
		}

		public static implicit operator PageField(PageBase page)
		{
			return new PageField()
			{
				Id = new Guid?(page.Id)
			};
		}

		public static bool operator !=(PageField field1, PageField field2)
		{
			return !(field1 == field2);
		}
	}
}