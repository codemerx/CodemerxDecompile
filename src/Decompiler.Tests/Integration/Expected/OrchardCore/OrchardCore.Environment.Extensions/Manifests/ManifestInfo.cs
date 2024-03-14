using OrchardCore.Environment.Extensions;
using OrchardCore.Modules.Manifest;
using System;
using System.Collections.Generic;

namespace OrchardCore.Environment.Extensions.Manifests
{
	public class ManifestInfo : IManifestInfo
	{
		private readonly ModuleAttribute _moduleInfo;

		private Lazy<System.Version> _version;

		public string Author
		{
			get
			{
				return this._moduleInfo.get_Author();
			}
		}

		public string Description
		{
			get
			{
				return this._moduleInfo.get_Description();
			}
		}

		public bool Exists
		{
			get
			{
				return this._moduleInfo.get_Exists();
			}
		}

		public ModuleAttribute ModuleInfo
		{
			get
			{
				return this._moduleInfo;
			}
		}

		public string Name
		{
			get
			{
				return this._moduleInfo.get_Name() ?? this._moduleInfo.get_Id();
			}
		}

		public IEnumerable<string> Tags
		{
			get
			{
				return this._moduleInfo.get_Tags();
			}
		}

		public string Type
		{
			get
			{
				return this._moduleInfo.get_Type();
			}
		}

		public System.Version Version
		{
			get
			{
				return this._version.Value;
			}
		}

		public string Website
		{
			get
			{
				return this._moduleInfo.get_Website();
			}
		}

		public ManifestInfo(ModuleAttribute moduleInfo)
		{
			this._moduleInfo = moduleInfo;
			this._version = new Lazy<System.Version>(new Func<System.Version>(this.ParseVersion));
		}

		private System.Version ParseVersion()
		{
			System.Version version;
			if (System.Version.TryParse(this._moduleInfo.get_Version(), out version))
			{
				return version;
			}
			return new System.Version(0, 0);
		}
	}
}