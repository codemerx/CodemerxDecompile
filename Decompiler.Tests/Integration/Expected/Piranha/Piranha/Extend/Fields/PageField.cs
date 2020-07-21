using Piranha;
using Piranha.Extend;
using Piranha.Models;
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
				return this.get_Page() != null;
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
			base();
			return;
		}

		public override bool Equals(object obj)
		{
			V_0 = obj as PageField;
			if (V_0 == null)
			{
				return false;
			}
			return this.Equals(V_0);
		}

		public virtual bool Equals(PageField obj)
		{
			if (PageField.op_Equality(obj, null))
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

		public virtual Task<T> GetPageAsync<T>(IApi api)
		where T : GenericPage<T>
		{
			if (!this.get_Id().get_HasValue())
			{
				return null;
			}
			stackVariable6 = api.get_Pages();
			V_0 = this.get_Id();
			return stackVariable6.GetByIdAsync<T>(V_0.get_Value());
		}

		public virtual string GetTitle()
		{
			stackVariable1 = this.get_Page();
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
			V_0.u003cu003et__builder.Start<PageField.u003cInitu003ed__11>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
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
			stackVariable0 = new PageField();
			stackVariable0.set_Id(new Guid?(guid));
			return stackVariable0;
		}

		public static implicit operator PageField(PageBase page)
		{
			stackVariable0 = new PageField();
			stackVariable0.set_Id(new Guid?(page.get_Id()));
			return stackVariable0;
		}

		public static bool operator !=(PageField field1, PageField field2)
		{
			return !PageField.op_Equality(field1, field2);
		}
	}
}