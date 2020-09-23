using Mix.Cms.Lib.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
					V_0 = TemplateRepository.syncRoot;
					V_1 = false;
					try
					{
						Monitor.Enter(V_0, ref V_1);
						if (TemplateRepository.instance == null)
						{
							TemplateRepository.instance = new TemplateRepository();
						}
					}
					finally
					{
						if (V_1)
						{
							Monitor.Exit(V_0);
						}
					}
				}
				return TemplateRepository.instance;
			}
		}

		static TemplateRepository()
		{
			TemplateRepository.syncRoot = new object();
			return;
		}

		private TemplateRepository()
		{
			base();
			return;
		}

		public bool DeleteTemplate(string name, string templateFolder)
		{
			stackVariable1 = new string[2];
			stackVariable1[0] = templateFolder;
			stackVariable1[1] = string.Concat(name, ".cshtml");
			V_0 = CommonHelper.GetFullPath(stackVariable1);
			if (File.Exists(V_0))
			{
				dummyVar0 = CommonHelper.RemoveFile(V_0);
			}
			return true;
		}

		public TemplateViewModel GetTemplate(string templatePath, List<TemplateViewModel> templates, string templateFolder)
		{
			V_0 = new TemplateRepository.u003cu003ec__DisplayClass5_0();
			V_0.templatePath = templatePath;
			stackVariable7 = templates.Find(new Predicate<TemplateViewModel>(V_0.u003cGetTemplateu003eb__0));
			if (stackVariable7 == null)
			{
				dummyVar0 = stackVariable7;
				stackVariable7 = new TemplateViewModel();
				stackVariable7.set_FileFolder(templateFolder);
			}
			return stackVariable7;
		}

		public TemplateViewModel GetTemplate(string name, string templateFolder)
		{
			V_0 = (new DirectoryInfo(templateFolder)).GetFiles(name).FirstOrDefault<FileInfo>();
			V_1 = null;
			if (V_0 != null)
			{
				V_2 = V_0.OpenText();
				try
				{
					stackVariable11 = new TemplateViewModel();
					stackVariable11.set_FileFolder(templateFolder);
					stackVariable11.set_Filename(V_0.get_Name());
					stackVariable11.set_Extension(V_0.get_Extension());
					stackVariable11.set_Content(V_2.ReadToEnd());
					V_1 = stackVariable11;
				}
				finally
				{
					if (V_2 != null)
					{
						((IDisposable)V_2).Dispose();
					}
				}
			}
			stackVariable7 = V_1;
			if (stackVariable7 == null)
			{
				dummyVar0 = stackVariable7;
				stackVariable7 = new TemplateViewModel();
				stackVariable7.set_FileFolder(templateFolder);
			}
			return stackVariable7;
		}

		public List<TemplateViewModel> GetTemplates(string folder)
		{
			if (!Directory.Exists(folder))
			{
				dummyVar0 = Directory.CreateDirectory(folder);
			}
			stackVariable7 = (new DirectoryInfo(folder)).GetFiles(string.Format("*{0}", ".cshtml"));
			V_0 = new List<TemplateViewModel>();
			V_1 = stackVariable7;
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				V_3 = V_1[V_2];
				V_4 = V_3.OpenText();
				try
				{
					stackVariable20 = new TemplateViewModel();
					stackVariable20.set_FileFolder(folder);
					stackVariable20.set_Filename(V_3.get_Name());
					stackVariable20.set_Extension(".cshtml");
					stackVariable20.set_Content(V_4.ReadToEnd());
					V_0.Add(stackVariable20);
				}
				finally
				{
					if (V_4 != null)
					{
						((IDisposable)V_4).Dispose();
					}
				}
				V_2 = V_2 + 1;
			}
			return V_0;
		}

		public bool SaveTemplate(TemplateViewModel file)
		{
			try
			{
				if (string.IsNullOrEmpty(file.get_Filename()))
				{
					V_1 = false;
				}
				else
				{
					if (!Directory.Exists(file.get_FileFolder()))
					{
						dummyVar0 = Directory.CreateDirectory(file.get_FileFolder());
					}
					stackVariable9 = new string[2];
					stackVariable9[0] = file.get_FileFolder();
					stackVariable9[1] = string.Concat(file.get_Filename(), file.get_Extension());
					V_0 = File.CreateText(CommonHelper.GetFullPath(stackVariable9));
					try
					{
						V_0.WriteLine(file.get_Content());
						V_1 = true;
					}
					finally
					{
						if (V_0 != null)
						{
							((IDisposable)V_0).Dispose();
						}
					}
				}
			}
			catch
			{
				dummyVar1 = exception_0;
				V_1 = false;
			}
			return V_1;
		}
	}
}