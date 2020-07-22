using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels
{
	public class FileViewModel
	{
		private string _fullPath;

		private string _webPath;

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
				stackVariable2 = new string[2];
				stackVariable2[0] = this.get_FileFolder();
				stackVariable2[1] = string.Concat(this.get_Filename(), this.get_Extension());
				this._fullPath = CommonHelper.GetFullPath(stackVariable2);
				return this._fullPath;
			}
			set
			{
				this._fullPath = value;
				return;
			}
		}

		[JsonProperty("webPath")]
		public string WebPath
		{
			get
			{
				this._webPath = this.get_FullPath().Replace("wwwroot", string.Empty);
				return this._webPath;
			}
			set
			{
				this._webPath = value;
				return;
			}
		}

		public FileViewModel()
		{
			this._fullPath = string.Empty;
			this._webPath = string.Empty;
			base();
			return;
		}

		public FileViewModel(IFormFile file, string folder)
		{
			this._fullPath = string.Empty;
			this._webPath = string.Empty;
			base();
			this.set_Filename(file.get_FileName().Substring(0, file.get_FileName().LastIndexOf('.')));
			this.set_Extension(file.get_FileName().Substring(file.get_FileName().LastIndexOf('.')));
			this.set_FileFolder(folder);
			return;
		}
	}
}