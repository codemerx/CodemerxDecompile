using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels
{
	public class TemplateViewModel
	{
		public string Content
		{
			get;
			set;
		}

		public string Extension
		{
			get;
			set;
		}

		public string FileFolder
		{
			get;
			set;
		}

		[Required]
		public string Filename
		{
			get;
			set;
		}

		public string FileStream
		{
			get;
			set;
		}

		public string Scripts
		{
			get;
			set;
		}

		public string Styles
		{
			get;
			set;
		}

		public TemplateViewModel()
		{
			base();
			return;
		}
	}
}