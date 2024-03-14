using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels
{
	public class FilePageViewModel
	{
		[JsonProperty("directories")]
		public List<string> Directories
		{
			get;
			set;
		}

		[JsonProperty("files")]
		public List<FileViewModel> Files
		{
			get;
			set;
		}

		public FilePageViewModel()
		{
		}
	}
}