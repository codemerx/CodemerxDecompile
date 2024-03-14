using OrchardCore.Environment.Extensions;
using System;
using System.Runtime.CompilerServices;

namespace OrchardCore.Environment.Extensions.Features
{
	public class FeatureInfo : IFeatureInfo
	{
		public string Category
		{
			get;
		}

		public bool DefaultTenantOnly
		{
			get;
		}

		public string[] Dependencies
		{
			get;
		}

		public string Description
		{
			get;
		}

		public IExtensionInfo Extension
		{
			get;
		}

		public string Id
		{
			get;
		}

		public bool IsAlwaysEnabled
		{
			get;
		}

		public string Name
		{
			get;
		}

		public int Priority
		{
			get;
		}

		public FeatureInfo(string id, string name, int priority, string category, string description, IExtensionInfo extension, string[] dependencies, bool defaultTenantOnly, bool isAlwaysEnabled)
		{
			this.Id = id;
			this.Name = name;
			this.Priority = priority;
			this.Category = category;
			this.Description = description;
			this.Extension = extension;
			this.Dependencies = dependencies;
			this.DefaultTenantOnly = defaultTenantOnly;
			this.IsAlwaysEnabled = isAlwaysEnabled;
		}
	}
}