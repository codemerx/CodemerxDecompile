using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels
{
	public class ListAction<T>
	{
		[JsonProperty("action")]
		public string Action
		{
			get;
			set;
		}

		[JsonProperty("data")]
		public List<T> Data
		{
			get;
			set;
		}

		public ListAction()
		{
			base();
			return;
		}
	}
}