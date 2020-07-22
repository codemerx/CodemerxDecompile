using Piranha;
using Piranha.Extend;
using Piranha.Models;
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
				return this.get_Media() != null;
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
			base();
			return;
		}

		public override bool Equals(object obj)
		{
			V_0 = (T)(obj as T);
			if (V_0 == null)
			{
				return false;
			}
			return this.Equals(V_0);
		}

		public virtual bool Equals(T obj)
		{
			if (MediaFieldBase<T>.op_Equality(obj, null))
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

		public virtual string GetTitle()
		{
			if (this.get_Media() == null)
			{
				return null;
			}
			if (String.IsNullOrWhiteSpace(this.get_Media().get_Title()))
			{
				return this.get_Media().get_Filename();
			}
			return String.Format("{0} ({1})", this.get_Media().get_Title(), this.get_Media().get_Filename());
		}

		public virtual async Task Init(IApi api)
		{
			V_0.u003cu003e4__this = this;
			V_0.api = api;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<MediaFieldBase<T>.u003cInitu003ed__11>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
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
			return !MediaFieldBase<T>.op_Equality(field1, field2);
		}
	}
}