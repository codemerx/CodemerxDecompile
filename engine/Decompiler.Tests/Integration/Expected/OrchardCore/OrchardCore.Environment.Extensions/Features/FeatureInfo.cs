using OrchardCore.Environment.Extensions;
using System;
using System.Runtime.CompilerServices;

namespace OrchardCore.Environment.Extensions.Features
{
	public class FeatureInfo : IFeatureInfo
	{
		public string Category
		{
			get
			{
				return this.u003cCategoryu003ek__BackingField;
			}
		}

		public bool DefaultTenantOnly
		{
			get
			{
				return this.u003cDefaultTenantOnlyu003ek__BackingField;
			}
		}

		public string[] Dependencies
		{
			get
			{
				return this.u003cDependenciesu003ek__BackingField;
			}
		}

		public string Description
		{
			get
			{
				return this.u003cDescriptionu003ek__BackingField;
			}
		}

		public IExtensionInfo Extension
		{
			get
			{
				return this.u003cExtensionu003ek__BackingField;
			}
		}

		public string Id
		{
			get
			{
				return this.u003cIdu003ek__BackingField;
			}
		}

		public bool IsAlwaysEnabled
		{
			get
			{
				return this.u003cIsAlwaysEnabledu003ek__BackingField;
			}
		}

		public string Name
		{
			get
			{
				return this.u003cNameu003ek__BackingField;
			}
		}

		public int Priority
		{
			get
			{
				return this.u003cPriorityu003ek__BackingField;
			}
		}

		public FeatureInfo(string id, string name, int priority, string category, string description, IExtensionInfo extension, string[] dependencies, bool defaultTenantOnly, bool isAlwaysEnabled)
		{
			base();
			this.u003cIdu003ek__BackingField = id;
			this.u003cNameu003ek__BackingField = name;
			this.u003cPriorityu003ek__BackingField = priority;
			this.u003cCategoryu003ek__BackingField = category;
			this.u003cDescriptionu003ek__BackingField = description;
			this.u003cExtensionu003ek__BackingField = extension;
			this.u003cDependenciesu003ek__BackingField = dependencies;
			this.u003cDefaultTenantOnlyu003ek__BackingField = defaultTenantOnly;
			this.u003cIsAlwaysEnabledu003ek__BackingField = isAlwaysEnabled;
			return;
		}
	}
}