using System;
using System.Collections;
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
		}

		public IList<T> GetBreadcrumb(Guid? id)
		{
			if (!id.HasValue)
			{
				return new List<T>();
			}
			return this.GetBreadcrumbRecursive(this, id.Value);
		}

		private IList<T> GetBreadcrumbRecursive(IList<T> items, Guid id)
		{
			IList<T> ts;
			using (IEnumerator<T> enumerator = items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					if (current.Id != id)
					{
						IList<T> breadcrumbRecursive = this.GetBreadcrumbRecursive(current.Items, id);
						if (breadcrumbRecursive == null)
						{
							continue;
						}
						breadcrumbRecursive.Insert(0, current);
						ts = breadcrumbRecursive;
						return ts;
					}
					else
					{
						ts = new List<T>()
						{
							current
						};
						return ts;
					}
				}
				return null;
			}
			return ts;
		}

		public TThis GetPartial(Guid? id, bool includeRootNode = false)
		{
			if (!id.HasValue)
			{
				return (TThis)this;
			}
			return this.GetPartialRecursive(this, id.Value, includeRootNode);
		}

		private TThis GetPartialRecursive(IList<T> items, Guid id, bool includeRootNode)
		{
			TThis tThi;
			using (IEnumerator<T> enumerator = items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					if (current.Id != id)
					{
						TThis partialRecursive = this.GetPartialRecursive(current.Items, id, includeRootNode);
						if (partialRecursive == null)
						{
							continue;
						}
						tThi = partialRecursive;
						return tThi;
					}
					else if (!includeRootNode)
					{
						tThi = current.Items;
						return tThi;
					}
					else
					{
						TThis tThi1 = Activator.CreateInstance<TThis>();
						tThi1.Add(current);
						tThi = tThi1;
						return tThi;
					}
				}
				return default(TThis);
			}
			return tThi;
		}
	}
}