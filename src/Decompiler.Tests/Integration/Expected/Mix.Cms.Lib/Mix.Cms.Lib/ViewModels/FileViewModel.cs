using Microsoft.AspNetCore.Http;
using Mix.Common.Helper;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels
{
	public class FileViewModel
	{
		private string _fullPath = string.Empty;

		private string _webPath = string.Empty;

		[JsonProperty("content")]
		public string Content
		{
			get;
			set;
		}

		[JsonProperty("extension")]
		public string Extension
		{
			get;
			set;
		}

		[JsonProperty("fileFolder")]
		public string FileFolder
		{
			get;
			set;
		}

		[JsonProperty("fileName")]
		public string Filename
		{
			get;
			set;
		}

		[JsonProperty("fileStream")]
		public string FileStream
		{
			get;
			set;
		}

		[JsonProperty("folderName")]
		public string FolderName
		{
			get;
			set;
		}

		[JsonProperty("fullPath")]
		public string FullPath
		{
			get
			{
				this._fullPath = CommonHelper.GetFullPath(new string[] { this.FileFolder, string.Concat(this.Filename, this.Extension) });
				return this._fullPath;
			}
			set
			{
				this._fullPath = value;
			}
		}

		[JsonProperty("webPath")]
		public string WebPath
		{
			get
			{
				this._webPath = this.FullPath.Replace("wwwroot", string.Empty);
				return this._webPath;
			}
			set
			{
				this._webPath = value;
			}
		}

		public FileViewModel()
		{
		}

		public FileViewModel(IFormFile file, string folder)
		{
			this.Filename = file.get_FileName().Substring(0, file.get_FileName().LastIndexOf('.'));
			this.Extension = file.get_FileName().Substring(file.get_FileName().LastIndexOf('.'));
			this.FileFolder = folder;
		}
	}
}