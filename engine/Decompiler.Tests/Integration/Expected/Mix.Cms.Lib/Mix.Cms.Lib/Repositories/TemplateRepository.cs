using Mix.Cms.Lib.ViewModels;
using Mix.Common.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Repositories
{
	public class TemplateRepository
	{
		private static volatile TemplateRepository instance;

		private readonly static object syncRoot;

		public static TemplateRepository Instance
		{
			get
			{
				if (TemplateRepository.instance == null)
				{
					lock (TemplateRepository.syncRoot)
					{
						if (TemplateRepository.instance == null)
						{
							TemplateRepository.instance = new TemplateRepository();
						}
					}
				}
				return TemplateRepository.instance;
			}
		}

		static TemplateRepository()
		{
			TemplateRepository.syncRoot = new object();
		}

		private TemplateRepository()
		{
		}

		public bool DeleteTemplate(string name, string templateFolder)
		{
			string fullPath = CommonHelper.GetFullPath(new string[] { templateFolder, string.Concat(name, ".cshtml") });
			if (File.Exists(fullPath))
			{
				CommonHelper.RemoveFile(fullPath);
			}
			return true;
		}

		public TemplateViewModel GetTemplate(string templatePath, List<TemplateViewModel> templates, string templateFolder)
		{
			return templates.Find((TemplateViewModel v) => {
				if (string.IsNullOrEmpty(templatePath))
				{
					return false;
				}
				return v.Filename == templatePath.Replace("\\", "/").Split('/', StringSplitOptions.None)[1];
			}) ?? new TemplateViewModel()
			{
				FileFolder = templateFolder
			};
		}

		public TemplateViewModel GetTemplate(string name, string templateFolder)
		{
			FileInfo fileInfo = (new DirectoryInfo(templateFolder)).GetFiles(name).FirstOrDefault<FileInfo>();
			TemplateViewModel templateViewModel = null;
			if (fileInfo != null)
			{
				using (StreamReader streamReader = fileInfo.OpenText())
				{
					templateViewModel = new TemplateViewModel()
					{
						FileFolder = templateFolder,
						Filename = fileInfo.Name,
						Extension = fileInfo.Extension,
						Content = streamReader.ReadToEnd()
					};
				}
			}
			return templateViewModel ?? new TemplateViewModel()
			{
				FileFolder = templateFolder
			};
		}

		public List<TemplateViewModel> GetTemplates(string folder)
		{
			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}
			FileInfo[] files = (new DirectoryInfo(folder)).GetFiles(string.Format("*{0}", ".cshtml"));
			List<TemplateViewModel> templateViewModels = new List<TemplateViewModel>();
			FileInfo[] fileInfoArray = files;
			for (int i = 0; i < (int)fileInfoArray.Length; i++)
			{
				FileInfo fileInfo = fileInfoArray[i];
				using (StreamReader streamReader = fileInfo.OpenText())
				{
					templateViewModels.Add(new TemplateViewModel()
					{
						FileFolder = folder,
						Filename = fileInfo.Name,
						Extension = ".cshtml",
						Content = streamReader.ReadToEnd()
					});
				}
			}
			return templateViewModels;
		}

		public bool SaveTemplate(TemplateViewModel file)
		{
			bool flag;
			try
			{
				if (string.IsNullOrEmpty(file.Filename))
				{
					flag = false;
				}
				else
				{
					if (!Directory.Exists(file.FileFolder))
					{
						Directory.CreateDirectory(file.FileFolder);
					}
					using (StreamWriter streamWriter = File.CreateText(CommonHelper.GetFullPath(new string[] { file.FileFolder, string.Concat(file.Filename, file.Extension) })))
					{
						streamWriter.WriteLine(file.Content);
						flag = true;
					}
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}
	}
}