using Mix.Cms.Lib;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels
{
	public class ExtraProperty
	{
		[JsonProperty("dataType")]
		public MixEnums.MixDataType DataType
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

		[JsonProperty("title")]
		public string Title
		{
			get;
			set;
		}

		[JsonProperty("value")]
		public string Value
		{
			get;
			set;
		}

		public ExtraProperty()
		{
			base();
			return;
		}
	}
}