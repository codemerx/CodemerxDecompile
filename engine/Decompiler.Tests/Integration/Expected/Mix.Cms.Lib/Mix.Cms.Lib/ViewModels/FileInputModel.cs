using Microsoft.AspNetCore.Http;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels
{
	public class FileInputModel
	{
		public IFormFile FileToUpload
		{
			get;
			set;
		}

		public FileInputModel()
		{
		}
	}
}