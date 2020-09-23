using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Extensions.Features;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace OrchardCore.Environment.Shell.Builders
{
	public class FeatureAwareServiceCollection : IServiceCollection, ICollection<ServiceDescriptor>, IEnumerable<ServiceDescriptor>, IEnumerable, IList<ServiceDescriptor>
	{
		private readonly IServiceCollection _innerServiceCollection;

		private readonly Dictionary<IFeatureInfo, ServiceCollection> _featureServiceCollections;

		private ServiceCollection _currentFeatureServiceCollection;

		public int Count
		{
			get
			{
				return this._innerServiceCollection.get_Count();
			}
		}

		public IDictionary<IFeatureInfo, ServiceCollection> FeatureCollections
		{
			get
			{
				return this._featureServiceCollections;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this._innerServiceCollection.get_IsReadOnly();
			}
		}

		public ServiceDescriptor this[int index]
		{
			get
			{
				return this._innerServiceCollection.get_Item(index);
			}
			set
			{
				this._innerServiceCollection.set_Item(index, value);
				return;
			}
		}

		public FeatureAwareServiceCollection(IServiceCollection innerServiceCollection)
		{
			this._featureServiceCollections = new Dictionary<IFeatureInfo, ServiceCollection>();
			base();
			this._innerServiceCollection = innerServiceCollection;
			return;
		}

		public void Clear()
		{
			this._innerServiceCollection.Clear();
			this._featureServiceCollections.Clear();
			return;
		}

		public bool Contains(ServiceDescriptor item)
		{
			return this._innerServiceCollection.Contains(item);
		}

		public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
		{
			this._innerServiceCollection.CopyTo(array, arrayIndex);
			return;
		}

		public IEnumerator<ServiceDescriptor> GetEnumerator()
		{
			return this._innerServiceCollection.GetEnumerator();
		}

		public int IndexOf(ServiceDescriptor item)
		{
			return this._innerServiceCollection.IndexOf(item);
		}

		public void Insert(int index, ServiceDescriptor item)
		{
			this._innerServiceCollection.Insert(index, item);
			stackVariable5 = this._currentFeatureServiceCollection;
			if (stackVariable5 == null)
			{
				dummyVar0 = stackVariable5;
				return;
			}
			dummyVar1 = ServiceCollectionDescriptorExtensions.Add(stackVariable5, item);
			return;
		}

		public bool Remove(ServiceDescriptor item)
		{
			return this._innerServiceCollection.Remove(item);
		}

		public void RemoveAt(int index)
		{
			this._innerServiceCollection.RemoveAt(index);
			return;
		}

		public void SetCurrentFeature(IFeatureInfo feature)
		{
			if (!this._featureServiceCollections.TryGetValue(feature, out this._currentFeatureServiceCollection))
			{
				stackVariable7 = this._featureServiceCollections;
				stackVariable10 = new ServiceCollection();
				V_0 = stackVariable10;
				this._currentFeatureServiceCollection = stackVariable10;
				stackVariable7.Add(feature, V_0);
			}
			return;
		}

		void System.Collections.Generic.ICollection<Microsoft.Extensions.DependencyInjection.ServiceDescriptor>.Add(ServiceDescriptor item)
		{
			this._innerServiceCollection.Add(item);
			stackVariable4 = this._currentFeatureServiceCollection;
			if (stackVariable4 == null)
			{
				dummyVar0 = stackVariable4;
				return;
			}
			dummyVar1 = ServiceCollectionDescriptorExtensions.Add(stackVariable4, item);
			return;
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}