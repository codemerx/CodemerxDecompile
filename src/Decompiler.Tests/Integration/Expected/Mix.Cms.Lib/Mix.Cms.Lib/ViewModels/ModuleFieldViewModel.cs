using Mix.Cms.Lib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels
{
	public class ModuleFieldViewModel
	{
		[JsonProperty("dataType")]
		public MixEnums.MixDataType DataType
		{
			get;
			set;
		}

		[JsonProperty("defaultValue")]
		public string DefaultValue
		{
			get;
			set;
		}

		[JsonProperty("isDisplay")]
		public bool IsDisplay
		{
			get;
			set;
		}

		[JsonProperty("isGroupBy")]
		public bool IsGroupBy
		{
			get;
			set;
		}

		[JsonProperty("isRequired")]
		public bool IsRequired
		{
			get;
			set;
		}

		[JsonProperty("isSelect")]
		public bool IsSelect
		{
			get;
			set;
		}

		[JsonProperty("isUnique")]
		public bool IsUnique
		{
			get;
			set;
		}

		[JsonProperty("name")]
		public string Name
		{
			get;
			set;
		}

		[JsonProperty("options")]
		public JArray Options { get; set; } = new JArray();

		[JsonProperty("priority")]
		public int Priority
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

		[JsonProperty("width")]
		public int Width
		{
			get;
			set;
		}

		public ModuleFieldViewModel()
		{
		}
	}
}