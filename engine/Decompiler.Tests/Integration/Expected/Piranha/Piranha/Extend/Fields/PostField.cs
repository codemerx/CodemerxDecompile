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
	[FieldType(Name="Post", Shorthand="Post", Component="post-field")]
	public class PostField : IField, IEquatable<PostField>
	{
		public bool HasValue
		{
			get
			{
				return this.Post != null;
			}
		}

		public Guid? Id
		{
			get;
			set;
		}

		public PostInfo Post
		{
			get;
			private set;
		}

		public PostField()
		{
		}

		public override bool Equals(object obj)
		{
			PostField postField = obj as PostField;
			if (postField == null)
			{
				return false;
			}
			return this.Equals(postField);
		}

		public virtual bool Equals(PostField obj)
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

		public virtual Task<T> GetPostAsync<T>(IApi api)
		where T : Post<T>
		{
			if (!this.Id.HasValue)
			{
				return null;
			}
			return api.Posts.GetByIdAsync<T>(this.Id.Value);
		}

		public virtual string GetTitle()
		{
			PostInfo post = this.Post;
			if (post != null)
			{
				return post.Title;
			}
			return null;
		}

		public virtual async Task Init(IApi api)
		{
			Guid? id = this.Id;
			if (id.HasValue)
			{
				IPostService posts = api.Posts;
				id = this.Id;
				ConfiguredTaskAwaitable<PostInfo> configuredTaskAwaitable = posts.GetByIdAsync<PostInfo>(id.Value).ConfigureAwait(false);
				this.Post = await configuredTaskAwaitable;
				if (this.Post == null)
				{
					id = null;
					this.Id = id;
				}
			}
		}

		public static bool operator ==(PostField field1, PostField field2)
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

		public static implicit operator PostField(Guid guid)
		{
			return new PostField()
			{
				Id = new Guid?(guid)
			};
		}

		public static implicit operator PostField(PostBase post)
		{
			return new PostField()
			{
				Id = new Guid?(post.Id)
			};
		}

		public static bool operator !=(PostField field1, PostField field2)
		{
			return !(field1 == field2);
		}
	}
}