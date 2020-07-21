using Piranha;
using Piranha.Extend;
using Piranha.Models;
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
				return this.get_Post() != null;
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
			base();
			return;
		}

		public override bool Equals(object obj)
		{
			V_0 = obj as PostField;
			if (V_0 == null)
			{
				return false;
			}
			return this.Equals(V_0);
		}

		public virtual bool Equals(PostField obj)
		{
			if (PostField.op_Equality(obj, null))
			{
				return false;
			}
			V_0 = this.get_Id();
			V_1 = obj.get_Id();
			if (V_0.get_HasValue() != V_1.get_HasValue())
			{
				return false;
			}
			if (!V_0.get_HasValue())
			{
				return true;
			}
			return Guid.op_Equality(V_0.GetValueOrDefault(), V_1.GetValueOrDefault());
		}

		public override int GetHashCode()
		{
			if (!this.get_Id().get_HasValue())
			{
				return 0;
			}
			return this.get_Id().GetHashCode();
		}

		public virtual Task<T> GetPostAsync<T>(IApi api)
		where T : Post<T>
		{
			if (!this.get_Id().get_HasValue())
			{
				return null;
			}
			stackVariable6 = api.get_Posts();
			V_0 = this.get_Id();
			return stackVariable6.GetByIdAsync<T>(V_0.get_Value());
		}

		public virtual string GetTitle()
		{
			stackVariable1 = this.get_Post();
			if (stackVariable1 != null)
			{
				return stackVariable1.get_Title();
			}
			dummyVar0 = stackVariable1;
			return null;
		}

		public virtual async Task Init(IApi api)
		{
			V_0.u003cu003e4__this = this;
			V_0.api = api;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostField.u003cInitu003ed__11>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
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
			stackVariable0 = new PostField();
			stackVariable0.set_Id(new Guid?(guid));
			return stackVariable0;
		}

		public static implicit operator PostField(PostBase post)
		{
			stackVariable0 = new PostField();
			stackVariable0.set_Id(new Guid?(post.get_Id()));
			return stackVariable0;
		}

		public static bool operator !=(PostField field1, PostField field2)
		{
			return !PostField.op_Equality(field1, field2);
		}
	}
}