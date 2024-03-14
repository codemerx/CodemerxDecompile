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
	public class MediaFieldBase<T> : IField, IEquatable<T>
	where T : MediaFieldBase<T>
	{
		public bool HasValue
		{
			get
			{
				return this.Media != null;
			}
		}

		public Guid? Id
		{
			get;
			set;
		}

		public Piranha.Models.Media Media
		{
			get;
			set;
		}

		public MediaFieldBase()
		{
		}

		public override bool Equals(object obj)
		{
			T t = (T)(obj as T);
			if (t == null)
			{
				return false;
			}
			return this.Equals(t);
		}

		public virtual bool Equals(T obj)
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

		public virtual string GetTitle()
		{
			if (this.Media == null)
			{
				return null;
			}
			if (String.IsNullOrWhiteSpace(this.Media.Title))
			{
				return this.Media.Filename;
			}
			return String.Format("{0} ({1})", (object)this.Media.Title, this.Media.Filename);
		}

		public virtual async Task Init(IApi api)
		{
			Guid? id = this.Id;
			if (id.HasValue)
			{
				IMediaService media = api.Media;
				id = this.Id;
				ConfiguredTaskAwaitable<Piranha.Models.Media> configuredTaskAwaitable = media.GetByIdAsync(id.Value).ConfigureAwait(false);
				this.Media = await configuredTaskAwaitable;
				if (this.Media == null)
				{
					id = null;
					this.Id = id;
				}
			}
		}

		public static bool operator ==(MediaFieldBase<T> field1, MediaFieldBase<T> field2)
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

		public static bool operator !=(MediaFieldBase<T> field1, MediaFieldBase<T> field2)
		{
			return !(field1 == field2);
		}
	}
}