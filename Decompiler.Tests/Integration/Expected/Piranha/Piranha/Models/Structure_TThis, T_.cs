using System;
using System.Collections.Generic;

namespace Piranha.Models
{
	[Serializable]
	public abstract class Structure<TThis, T> : List<T>
	where TThis : Structure<TThis, T>
	where T : StructureItem<TThis, T>
	{
		protected Structure()
		{
			base();
			return;
		}

		public IList<T> GetBreadcrumb(Guid? id)
		{
			if (!id.get_HasValue())
			{
				return new List<T>();
			}
			return this.GetBreadcrumbRecursive(this, id.get_Value());
		}

		private IList<T> GetBreadcrumbRecursive(IList<T> items, Guid id)
		{
			V_0 = items.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!Guid.op_Equality(V_1.get_Id(), id))
					{
						V_2 = this.GetBreadcrumbRecursive(V_1.get_Items(), id);
						if (V_2 == null)
						{
							continue;
						}
						V_2.Insert(0, V_1);
						V_3 = V_2;
						goto Label1;
					}
					else
					{
						stackVariable24 = new List<T>();
						stackVariable24.Add(V_1);
						V_3 = stackVariable24;
						goto Label1;
					}
				}
				goto Label0;
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
		Label1:
			return V_3;
		Label0:
			return null;
		}

		public TThis GetPartial(Guid? id, bool includeRootNode = false)
		{
			if (!id.get_HasValue())
			{
				return (TThis)this;
			}
			return this.GetPartialRecursive(this, id.get_Value(), includeRootNode);
		}

		private TThis GetPartialRecursive(IList<T> items, Guid id, bool includeRootNode)
		{
			V_0 = items.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!Guid.op_Equality(V_1.get_Id(), id))
					{
						V_2 = this.GetPartialRecursive(V_1.get_Items(), id, includeRootNode);
						if (V_2 == null)
						{
							continue;
						}
						V_3 = V_2;
						goto Label1;
					}
					else
					{
						if (!includeRootNode)
						{
							V_3 = V_1.get_Items();
							goto Label1;
						}
						else
						{
							stackVariable27 = Activator.CreateInstance<TThis>();
							stackVariable27.Add(V_1);
							V_3 = stackVariable27;
							goto Label1;
						}
					}
				}
				goto Label0;
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
		Label1:
			return V_3;
		Label0:
			V_4 = default(TThis);
			return V_4;
		}
	}
}