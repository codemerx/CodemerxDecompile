using Mix.Cms.Lib;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels
{
	public class DataValueViewModel
	{
		[JsonProperty("dataType")]
		public MixEnums.MixDataType DataType { get; set; } = MixEnums.MixDataType.Text;

		[JsonProperty("name")]
		public string Name
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

		public DataValueViewModel()
		{
		}
	}
}