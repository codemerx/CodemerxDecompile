using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSetDatas
{
	public class MenuItem
	{
		[JsonProperty("classes")]
		public string Classes
		{
			get;
			set;
		}

		[JsonIgnore]
		[JsonProperty("data")]
		public JObject Data
		{
			get;
			set;
		}

		[JsonProperty("description")]
		public string Description
		{
			get;
			set;
		}

		[JsonProperty("icon")]
		public string Icon
		{
			get;
			set;
		}

		[JsonProperty("id")]
		public string Id
		{
			get;
			set;
		}

		[JsonProperty("isActive")]
		public bool IsActive
		{
			get;
			set;
		}

		[JsonProperty("menu_items")]
		public List<MenuItem> MenuItems
		{
			get;
			set;
		}

		[JsonProperty("target")]
		public string Target
		{
			get;
			set;
		}

		[JsonProperty("title")]
		public string Title
		{
			get;
			set;
		}

		[JsonProperty("type")]
		public string Type
		{
			get;
			set;
		}

		[JsonProperty("uri")]
		public string Uri
		{
			get;
			set;
		}

		public MenuItem()
		{
			base();
			return;
		}

		public T Property<T>(string fieldName)
		{
			if (this.get_Data() == null)
			{
				V_1 = default(T);
				return V_1;
			}
			V_0 = this.get_Data().GetValue(fieldName);
			if (V_0 != null)
			{
				return Newtonsoft.Json.Linq.Extensions.Value<T>(V_0);
			}
			V_1 = default(T);
			return V_1;
		}
	}
}