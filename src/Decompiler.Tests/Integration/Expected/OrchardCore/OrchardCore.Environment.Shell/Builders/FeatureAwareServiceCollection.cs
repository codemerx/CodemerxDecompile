using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

		private readonly Dictionary<IFeatureInfo, ServiceCollection> _featureServiceCollections = new Dictionary<IFeatureInfo, ServiceCollection>();

		private ServiceCollection _currentFeatureServiceCollection;

		public int Count
		{
			get
			{
				return this._innerServiceCollection.Count;
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
				return this._innerServiceCollection.IsReadOnly;
			}
		}

		public ServiceDescriptor this[int index]
		{
			get
			{
				return this._innerServiceCollection[index];
			}
			set
			{
				this._innerServiceCollection[index] = value;
			}
		}

		public FeatureAwareServiceCollection(IServiceCollection innerServiceCollection)
		{
			this._innerServiceCollection = innerServiceCollection;
		}

		public void Clear()
		{
			this._innerServiceCollection.Clear();
			this._featureServiceCollections.Clear();
		}

		public bool Contains(ServiceDescriptor item)
		{
			return this._innerServiceCollection.Contains(item);
		}

		public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
		{
			this._innerServiceCollection.CopyTo(array, arrayIndex);
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
			ServiceCollection serviceCollection = this._currentFeatureServiceCollection;
			if (serviceCollection == null)
			{
				return;
			}
			ServiceCollectionDescriptorExtensions.Add(serviceCollection, item);
		}

		public bool Remove(ServiceDescriptor item)
		{
			return this._innerServiceCollection.Remove(item);
		}

		public void RemoveAt(int index)
		{
			this._innerServiceCollection.RemoveAt(index);
		}

		public void SetCurrentFeature(IFeatureInfo feature)
		{
			if (!this._featureServiceCollections.TryGetValue(feature, out this._currentFeatureServiceCollection))
			{
				Dictionary<IFeatureInfo, ServiceCollection> featureInfos = this._featureServiceCollections;
				ServiceCollection serviceCollection = new ServiceCollection();
				ServiceCollection serviceCollection1 = serviceCollection;
				this._currentFeatureServiceCollection = serviceCollection;
				featureInfos.Add(feature, serviceCollection1);
			}
		}

		void System.Collections.Generic.ICollection<Microsoft.Extensions.DependencyInjection.ServiceDescriptor>.Add(ServiceDescriptor item)
		{
			this._innerServiceCollection.Add(item);
			ServiceCollection serviceCollection = this._currentFeatureServiceCollection;
			if (serviceCollection == null)
			{
				return;
			}
			ServiceCollectionDescriptorExtensions.Add(serviceCollection, item);
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}