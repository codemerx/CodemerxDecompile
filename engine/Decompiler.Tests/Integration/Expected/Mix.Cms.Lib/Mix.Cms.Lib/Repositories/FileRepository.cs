using Microsoft.AspNetCore.Http;
using Mix.Cms.Lib;
using Mix.Cms.Lib.ViewModels;
using Mix.Domain.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.Repositories
{
	public class FileRepository
	{
		private static volatile FileRepository instance;

		private readonly static object syncRoot;

		public string CurrentDirectory
		{
			get;
			set;
		}

		public static FileRepository Instance
		{
			get
			{
				if (FileRepository.instance == null)
				{
					V_0 = FileRepository.syncRoot;
					V_1 = false;
					try
					{
						Monitor.Enter(V_0, ref V_1);
						if (FileRepository.instance == null)
						{
							FileRepository.instance = new FileRepository();
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
				return FileRepository.instance;
			}
			set
			{
				FileRepository.instance = value;
				return;
			}
		}

		static FileRepository()
		{
			FileRepository.syncRoot = new object();
			return;
		}

		private FileRepository()
		{
			base();
			this.set_CurrentDirectory(Environment.get_CurrentDirectory());
			return;
		}

		public bool CopyDirectory(string srcPath, string desPath)
		{
			if (!string.op_Inequality(srcPath.ToLower(), desPath.ToLower()) || !Directory.Exists(srcPath))
			{
				return true;
			}
			V_0 = Directory.GetDirectories(srcPath, "*", 1);
			V_1 = 0;
			while (V_1 < (int)V_0.Length)
			{
				dummyVar0 = Directory.CreateDirectory(V_0[V_1].Replace(srcPath, desPath));
				V_1 = V_1 + 1;
			}
			V_0 = Directory.GetFiles(srcPath, "*.*", 1);
			V_1 = 0;
			while (V_1 < (int)V_0.Length)
			{
				stackVariable38 = V_0[V_1];
				File.Copy(stackVariable38, stackVariable38.Replace(srcPath, desPath), true);
				V_1 = V_1 + 1;
			}
			return true;
		}

		public bool CopyWebDirectory(string srcPath, string desPath)
		{
			if (!string.op_Inequality(srcPath, desPath))
			{
				return true;
			}
			V_0 = Directory.GetDirectories(string.Concat("wwwroot/", srcPath), "*", 1);
			V_1 = 0;
			while (V_1 < (int)V_0.Length)
			{
				dummyVar0 = Directory.CreateDirectory(V_0[V_1].Replace(srcPath, desPath));
				V_1 = V_1 + 1;
			}
			V_0 = Directory.GetFiles(string.Concat("wwwroot/", srcPath), "*.*", 1);
			V_1 = 0;
			while (V_1 < (int)V_0.Length)
			{
				stackVariable38 = V_0[V_1];
				File.Copy(stackVariable38, stackVariable38.Replace(srcPath, desPath), true);
				V_1 = V_1 + 1;
			}
			return true;
		}

		public void CreateDirectoryIfNotExist(string fullPath)
		{
			if (!string.IsNullOrEmpty(fullPath) && !Directory.Exists(fullPath))
			{
				dummyVar0 = Directory.CreateDirectory(fullPath);
			}
			return;
		}

		public bool DeleteFile(string name, string extension, string FileFolder)
		{
			stackVariable1 = new string[2];
			stackVariable1[0] = "Content/Uploads";
			stackVariable1[1] = FileFolder;
			V_0 = CommonHelper.GetFullPath(stackVariable1);
			V_1 = string.Format("{0}/{1}{2}", V_0, name, extension);
			if (File.Exists(V_1))
			{
				dummyVar0 = CommonHelper.RemoveFile(V_1);
			}
			return true;
		}

		public bool DeleteFile(string fullPath)
		{
			if (File.Exists(fullPath))
			{
				dummyVar0 = CommonHelper.RemoveFile(fullPath);
			}
			return true;
		}

		public bool DeleteFolder(string folderPath)
		{
			if (!Directory.Exists(folderPath))
			{
				return false;
			}
			Directory.Delete(folderPath, true);
			return true;
		}

		public bool DeleteWebFile(string filename, string folder)
		{
			stackVariable1 = new string[4];
			stackVariable1[0] = "wwwroot";
			stackVariable1[1] = "content";
			stackVariable1[2] = folder;
			stackVariable1[3] = filename;
			V_0 = CommonHelper.GetFullPath(stackVariable1);
			if (File.Exists(V_0))
			{
				dummyVar0 = CommonHelper.RemoveFile(V_0);
			}
			return true;
		}

		public bool DeleteWebFile(string filePath)
		{
			stackVariable1 = new string[2];
			stackVariable1[0] = "wwwroot";
			stackVariable1[1] = filePath;
			V_0 = CommonHelper.GetFullPath(stackVariable1);
			if (File.Exists(V_0))
			{
				dummyVar0 = CommonHelper.RemoveFile(V_0);
			}
			return true;
		}

		public bool DeleteWebFile(string name, string extension, string FileFolder)
		{
			stackVariable2 = new object[4];
			stackVariable2[0] = "wwwroot";
			stackVariable2[1] = FileFolder;
			stackVariable2[2] = name;
			stackVariable2[3] = extension;
			V_0 = string.Format("{0}/{1}/{2}{3}", stackVariable2);
			if (File.Exists(V_0))
			{
				dummyVar0 = CommonHelper.RemoveFile(V_0);
			}
			return true;
		}

		public bool DeleteWebFolder(string folderPath)
		{
			stackVariable1 = new string[2];
			stackVariable1[0] = "wwwroot";
			stackVariable1[1] = folderPath;
			V_0 = CommonHelper.GetFullPath(stackVariable1);
			if (Directory.Exists(V_0))
			{
				Directory.Delete(V_0, true);
			}
			return true;
		}

		public bool EmptyFolder(string folderPath)
		{
			dummyVar0 = this.DeleteFolder(folderPath);
			this.CreateDirectoryIfNotExist(folderPath);
			return true;
		}

		public FileViewModel GetFile(string FilePath, List<FileViewModel> Files, string FileFolder)
		{
			V_0 = new FileRepository.u003cu003ec__DisplayClass10_0();
			V_0.FilePath = FilePath;
			stackVariable7 = Files.Find(new Predicate<FileViewModel>(V_0.u003cGetFileu003eb__0));
			if (stackVariable7 == null)
			{
				dummyVar0 = stackVariable7;
				stackVariable7 = new FileViewModel();
				stackVariable7.set_FileFolder(FileFolder);
			}
			return stackVariable7;
		}

		public FileViewModel GetFile(string name, string ext, string FileFolder, bool isCreate = false, string defaultContent = "")
		{
			V_0 = null;
			V_1 = Path.Combine(this.get_CurrentDirectory(), FileFolder, string.Format("{0}{1}", name, ext));
			V_2 = new FileInfo(V_1);
			if (!V_2.get_Exists())
			{
				if (isCreate)
				{
					this.CreateDirectoryIfNotExist(FileFolder);
					dummyVar1 = V_2.Create();
					stackVariable20 = new FileViewModel();
					stackVariable20.set_FileFolder(FileFolder);
					stackVariable20.set_Filename(name);
					stackVariable20.set_Extension(ext);
					stackVariable20.set_Content(defaultContent);
					V_0 = stackVariable20;
					dummyVar2 = this.SaveFile(V_0);
				}
			}
			else
			{
				try
				{
					V_3 = File.Open(V_1, 3, 1, 1);
					try
					{
						V_4 = new StreamReader(V_3);
						try
						{
							stackVariable35 = new FileViewModel();
							stackVariable35.set_FileFolder(FileFolder);
							stackVariable35.set_Filename(name);
							stackVariable35.set_Extension(ext);
							stackVariable35.set_Content(V_4.ReadToEnd());
							V_0 = stackVariable35;
						}
						finally
						{
							if (V_4 != null)
							{
								((IDisposable)V_4).Dispose();
							}
						}
					}
					finally
					{
						if (V_3 != null)
						{
							((IDisposable)V_3).Dispose();
						}
					}
				}
				catch
				{
					dummyVar0 = exception_0;
				}
			}
			stackVariable14 = V_0;
			if (stackVariable14 == null)
			{
				dummyVar3 = stackVariable14;
				stackVariable14 = new FileViewModel();
				stackVariable14.set_FileFolder(FileFolder);
			}
			return stackVariable14;
		}

		public FileViewModel GetFile(string fullname, string FileFolder, bool isCreate = false, string defaultContent = "")
		{
			V_0 = fullname.Split('.', 0);
			if ((int)V_0.Length < 2)
			{
				stackVariable8 = new FileViewModel();
				stackVariable8.set_FileFolder(FileFolder);
				return stackVariable8;
			}
			return this.GetFile(fullname.Substring(0, fullname.LastIndexOf('.')), string.Concat(".", V_0[(int)V_0.Length - 1]), FileFolder, isCreate, defaultContent);
		}

		public List<FileViewModel> GetFiles(string fullPath)
		{
			this.CreateDirectoryIfNotExist(fullPath);
			V_0 = new List<FileViewModel>();
			V_1 = Directory.GetDirectories(fullPath, "*", 1);
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				stackVariable15 = new DirectoryInfo(V_1[V_2]);
				V_3 = stackVariable15.get_Name();
				stackVariable17 = stackVariable15.GetFiles();
				stackVariable18 = FileRepository.u003cu003ec.u003cu003e9__30_0;
				if (stackVariable18 == null)
				{
					dummyVar0 = stackVariable18;
					stackVariable18 = new Func<FileInfo, DateTime>(FileRepository.u003cu003ec.u003cu003e9.u003cGetFilesu003eb__30_0);
					FileRepository.u003cu003ec.u003cu003e9__30_0 = stackVariable18;
				}
				V_4 = ((IEnumerable<FileInfo>)stackVariable17).OrderByDescending<FileInfo, DateTime>(stackVariable18).GetEnumerator();
				try
				{
					while (V_4.MoveNext())
					{
						V_5 = V_4.get_Current();
						V_6 = new FileViewModel();
						V_6.set_FolderName(V_3);
						stackVariable31 = new string[2];
						stackVariable31[0] = fullPath;
						stackVariable31[1] = V_3;
						V_6.set_FileFolder(CommonHelper.GetFullPath(stackVariable31));
						V_6.set_Filename(V_5.get_Name().Substring(0, V_5.get_Name().LastIndexOf('.')));
						V_6.set_Extension(V_5.get_Extension());
						V_0.Add(V_6);
					}
				}
				finally
				{
					if (V_4 != null)
					{
						V_4.Dispose();
					}
				}
				V_2 = V_2 + 1;
			}
			return V_0;
		}

		public List<FileViewModel> GetFiles(MixEnums.FileFolder FileFolder)
		{
			return this.GetUploadFiles(FileFolder.ToString());
		}

		public List<FileViewModel> GetFilesWithContent(string fullPath)
		{
			this.CreateDirectoryIfNotExist(fullPath);
			V_0 = new List<FileViewModel>();
			V_1 = Directory.GetDirectories(fullPath, "*", 1);
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				stackVariable15 = new DirectoryInfo(V_1[V_2]);
				V_3 = stackVariable15.get_Name();
				stackVariable17 = stackVariable15.GetFiles();
				stackVariable18 = FileRepository.u003cu003ec.u003cu003e9__29_0;
				if (stackVariable18 == null)
				{
					dummyVar0 = stackVariable18;
					stackVariable18 = new Func<FileInfo, DateTime>(FileRepository.u003cu003ec.u003cu003e9.u003cGetFilesWithContentu003eb__29_0);
					FileRepository.u003cu003ec.u003cu003e9__29_0 = stackVariable18;
				}
				V_4 = ((IEnumerable<FileInfo>)stackVariable17).OrderByDescending<FileInfo, DateTime>(stackVariable18).GetEnumerator();
				try
				{
					while (V_4.MoveNext())
					{
						V_5 = V_4.get_Current();
						V_6 = V_5.OpenText();
						try
						{
							V_7 = new FileViewModel();
							V_7.set_FolderName(V_3);
							stackVariable33 = new string[2];
							stackVariable33[0] = fullPath;
							stackVariable33[1] = V_3;
							V_7.set_FileFolder(CommonHelper.GetFullPath(stackVariable33));
							V_7.set_Filename(V_5.get_Name().Substring(0, V_5.get_Name().LastIndexOf('.')));
							V_7.set_Extension(V_5.get_Extension());
							V_7.set_Content(V_6.ReadToEnd());
							V_0.Add(V_7);
						}
						finally
						{
							if (V_6 != null)
							{
								((IDisposable)V_6).Dispose();
							}
						}
					}
				}
				finally
				{
					if (V_4 != null)
					{
						V_4.Dispose();
					}
				}
				V_2 = V_2 + 1;
			}
			return V_0;
		}

		public List<string> GetTopDirectories(string folder)
		{
			V_0 = new List<string>();
			if (Directory.Exists(folder))
			{
				V_1 = Directory.GetDirectories(folder, "*", 0);
				V_2 = 0;
				while (V_2 < (int)V_1.Length)
				{
					V_0.Add((new DirectoryInfo(V_1[V_2])).get_Name());
					V_2 = V_2 + 1;
				}
			}
			return V_0;
		}

		public List<FileViewModel> GetTopFiles(string folder)
		{
			V_0 = new List<FileViewModel>();
			if (Directory.Exists(folder))
			{
				stackVariable5 = new DirectoryInfo(folder);
				V_1 = stackVariable5.get_Name();
				stackVariable7 = stackVariable5.GetFiles();
				stackVariable8 = FileRepository.u003cu003ec.u003cu003e9__28_0;
				if (stackVariable8 == null)
				{
					dummyVar0 = stackVariable8;
					stackVariable8 = new Func<FileInfo, DateTime>(FileRepository.u003cu003ec.u003cu003e9.u003cGetTopFilesu003eb__28_0);
					FileRepository.u003cu003ec.u003cu003e9__28_0 = stackVariable8;
				}
				V_2 = ((IEnumerable<FileInfo>)stackVariable7).OrderByDescending<FileInfo, DateTime>(stackVariable8).GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						stackVariable15 = V_0;
						stackVariable16 = new FileViewModel();
						stackVariable16.set_FolderName(V_1);
						stackVariable16.set_FileFolder(folder);
						stackVariable20 = V_3.get_Name();
						if (V_3.get_Name().LastIndexOf('.') >= 0)
						{
							stackVariable30 = V_3.get_Name().LastIndexOf('.');
						}
						else
						{
							stackVariable30 = 0;
						}
						stackVariable16.set_Filename(stackVariable20.Substring(0, stackVariable30));
						stackVariable16.set_Extension(V_3.get_Extension());
						stackVariable15.Add(stackVariable16);
					}
				}
				finally
				{
					if (V_2 != null)
					{
						V_2.Dispose();
					}
				}
			}
			return V_0;
		}

		public FileViewModel GetUploadFile(string name, string ext, string FileFolder)
		{
			V_0 = null;
			stackVariable2 = new string[2];
			stackVariable2[0] = "Content/Uploads";
			stackVariable2[1] = FileFolder;
			V_1 = CommonHelper.GetFullPath(stackVariable2);
			V_2 = new FileInfo(string.Format("{0}/{1}.{2}", V_1, name, ext));
			try
			{
				V_3 = V_2.OpenText();
				try
				{
					stackVariable16 = new FileViewModel();
					stackVariable16.set_FileFolder(FileFolder);
					stackVariable16.set_Filename(V_2.get_Name().Substring(0, V_2.get_Name().LastIndexOf('.')));
					stackVariable16.set_Extension(V_2.get_Extension().Remove(0, 1));
					stackVariable16.set_Content(V_3.ReadToEnd());
					V_0 = stackVariable16;
				}
				finally
				{
					if (V_3 != null)
					{
						((IDisposable)V_3).Dispose();
					}
				}
			}
			catch
			{
				dummyVar0 = exception_0;
			}
			stackVariable33 = V_0;
			if (stackVariable33 == null)
			{
				dummyVar1 = stackVariable33;
				stackVariable33 = new FileViewModel();
				stackVariable33.set_FileFolder(FileFolder);
			}
			return stackVariable33;
		}

		public List<FileViewModel> GetUploadFiles(string folder)
		{
			stackVariable1 = new string[2];
			stackVariable1[0] = "Content/Uploads";
			stackVariable1[1] = folder;
			V_0 = CommonHelper.GetFullPath(stackVariable1);
			this.CreateDirectoryIfNotExist(V_0);
			stackVariable11 = (new DirectoryInfo(V_0)).GetFiles();
			V_1 = new List<FileViewModel>();
			stackVariable13 = FileRepository.u003cu003ec.u003cu003e9__26_0;
			if (stackVariable13 == null)
			{
				dummyVar0 = stackVariable13;
				stackVariable13 = new Func<FileInfo, DateTime>(FileRepository.u003cu003ec.u003cu003e9.u003cGetUploadFilesu003eb__26_0);
				FileRepository.u003cu003ec.u003cu003e9__26_0 = stackVariable13;
			}
			V_2 = ((IEnumerable<FileInfo>)stackVariable11).OrderByDescending<FileInfo, DateTime>(stackVariable13).GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					V_4 = V_3.OpenText();
					try
					{
						stackVariable23 = new FileViewModel();
						stackVariable23.set_FileFolder(folder);
						stackVariable23.set_Filename(V_3.get_Name().Substring(0, V_3.get_Name().LastIndexOf('.')));
						stackVariable23.set_Extension(V_3.get_Extension());
						stackVariable23.set_Content(V_4.ReadToEnd());
						V_1.Add(stackVariable23);
					}
					finally
					{
						if (V_4 != null)
						{
							((IDisposable)V_4).Dispose();
						}
					}
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			return V_1;
		}

		public FileViewModel GetWebFile(string filename, string folder)
		{
			stackVariable4 = string.Concat("wwwroot/", folder, "/", filename);
			V_0 = string.Concat("wwwroot/content/", folder);
			V_1 = new FileInfo(stackVariable4);
			V_2 = null;
			try
			{
				V_3 = new DirectoryInfo(V_0);
				V_4 = V_1.OpenText();
				try
				{
					stackVariable14 = new FileViewModel();
					stackVariable14.set_FolderName(V_3.get_Name());
					stackVariable14.set_FileFolder(folder);
					stackVariable14.set_Filename(V_1.get_Name().Substring(0, V_1.get_Name().LastIndexOf('.')));
					stackVariable14.set_Extension(V_1.get_Extension());
					stackVariable14.set_Content(V_4.ReadToEnd());
					V_2 = stackVariable14;
				}
				finally
				{
					if (V_4 != null)
					{
						((IDisposable)V_4).Dispose();
					}
				}
			}
			catch
			{
				dummyVar0 = exception_0;
			}
			stackVariable30 = V_2;
			if (stackVariable30 == null)
			{
				dummyVar1 = stackVariable30;
				stackVariable30 = new FileViewModel();
				stackVariable30.set_FileFolder(folder);
			}
			return stackVariable30;
		}

		public List<FileViewModel> GetWebFiles(string folder)
		{
			stackVariable1 = new string[3];
			stackVariable1[0] = "wwwroot";
			stackVariable1[1] = "content";
			stackVariable1[2] = folder;
			V_0 = CommonHelper.GetFullPath(stackVariable1);
			this.CreateDirectoryIfNotExist(V_0);
			V_1 = new List<FileViewModel>();
			V_2 = Directory.GetDirectories(V_0, "*", 1);
			V_3 = 0;
			while (V_3 < (int)V_2.Length)
			{
				V_4 = new DirectoryInfo(V_2[V_3]);
				V_5 = V_4.ToString().Replace("\\", "/").Replace("wwwroot", string.Empty);
				stackVariable34 = V_4.GetFiles();
				stackVariable35 = FileRepository.u003cu003ec.u003cu003e9__31_0;
				if (stackVariable35 == null)
				{
					dummyVar0 = stackVariable35;
					stackVariable35 = new Func<FileInfo, DateTime>(FileRepository.u003cu003ec.u003cu003e9.u003cGetWebFilesu003eb__31_0);
					FileRepository.u003cu003ec.u003cu003e9__31_0 = stackVariable35;
				}
				V_6 = ((IEnumerable<FileInfo>)stackVariable34).OrderByDescending<FileInfo, DateTime>(stackVariable35).GetEnumerator();
				try
				{
					while (V_6.MoveNext())
					{
						V_7 = V_6.get_Current();
						stackVariable42 = V_1;
						stackVariable43 = new FileViewModel();
						stackVariable43.set_FolderName(V_4.get_Name());
						stackVariable43.set_FileFolder(V_5);
						if (V_7.get_Name().LastIndexOf('.') >= 0)
						{
							stackVariable59 = V_7.get_Name().Substring(0, V_7.get_Name().LastIndexOf('.'));
						}
						else
						{
							stackVariable59 = V_7.get_Name();
						}
						stackVariable43.set_Filename(stackVariable59);
						stackVariable43.set_Extension(V_7.get_Extension());
						stackVariable42.Add(stackVariable43);
					}
				}
				finally
				{
					if (V_6 != null)
					{
						V_6.Dispose();
					}
				}
				V_3 = V_3 + 1;
			}
			return V_1;
		}

		public RepositoryResponse<FileViewModel> SaveFile(IFormFile file, string filename, string folder)
		{
			V_0 = new RepositoryResponse<FileViewModel>();
			try
			{
				if (file.get_Length() <= (long)0)
				{
					V_0.set_IsSucceed(false);
					V_0.get_Errors().Add("File not found");
				}
				else
				{
					this.CreateDirectoryIfNotExist(folder);
					stackVariable14 = new string[2];
					stackVariable14[0] = folder;
					stackVariable14[1] = filename;
					V_1 = CommonHelper.GetFullPath(stackVariable14);
					if (File.Exists(V_1))
					{
						dummyVar0 = this.DeleteFile(V_1);
					}
					V_2 = new FileStream(V_1, 2);
					try
					{
						file.CopyTo(V_2);
					}
					finally
					{
						if (V_2 != null)
						{
							((IDisposable)V_2).Dispose();
						}
					}
					V_0.set_IsSucceed(true);
					stackVariable30 = new FileViewModel();
					stackVariable30.set_Filename(filename.Substring(0, file.get_FileName().LastIndexOf('.')));
					stackVariable30.set_Extension(filename.Substring(file.get_FileName().LastIndexOf('.')));
					stackVariable30.set_FileFolder(folder);
					V_0.set_Data(stackVariable30);
				}
			}
			catch (Exception exception_0)
			{
				V_3 = exception_0;
				V_0.set_IsSucceed(false);
				V_0.set_Exception(V_3);
				V_0.get_Errors().Add(V_3.get_Message());
			}
			return V_0;
		}

		public bool SaveFile(FileViewModel file)
		{
			try
			{
				if (string.IsNullOrEmpty(file.get_Filename()))
				{
					V_2 = false;
				}
				else
				{
					this.CreateDirectoryIfNotExist(file.get_FileFolder());
					V_0 = string.Concat(file.get_Filename(), file.get_Extension());
					if (!string.IsNullOrEmpty(file.get_FileFolder()))
					{
						stackVariable49 = new string[2];
						stackVariable49[0] = file.get_FileFolder();
						stackVariable49[1] = V_0;
						V_0 = CommonHelper.GetFullPath(stackVariable49);
					}
					if (File.Exists(V_0))
					{
						dummyVar0 = this.DeleteFile(V_0);
					}
					if (string.IsNullOrEmpty(file.get_Content()))
					{
						V_3 = Convert.FromBase64String(file.get_FileStream().Split(',', 0)[1]);
						V_4 = File.Create(V_0);
						try
						{
							V_4.Write(V_3, 0, (int)V_3.Length);
							V_2 = true;
						}
						finally
						{
							if (V_4 != null)
							{
								((IDisposable)V_4).Dispose();
							}
						}
					}
					else
					{
						V_1 = File.CreateText(V_0);
						try
						{
							V_1.WriteLine(file.get_Content());
							V_1.Dispose();
							V_2 = true;
						}
						finally
						{
							if (V_1 != null)
							{
								((IDisposable)V_1).Dispose();
							}
						}
					}
				}
			}
			catch
			{
				dummyVar1 = exception_0;
				V_2 = false;
			}
			return V_2;
		}

		public bool SaveWebFile(FileViewModel file)
		{
			try
			{
				stackVariable1 = new string[2];
				stackVariable1[0] = "wwwroot";
				stackVariable1[1] = file.get_FileFolder();
				V_0 = CommonHelper.GetFullPath(stackVariable1);
				if (string.IsNullOrEmpty(file.get_Filename()))
				{
					V_3 = false;
				}
				else
				{
					this.CreateDirectoryIfNotExist(V_0);
					stackVariable16 = new string[2];
					stackVariable16[0] = V_0;
					stackVariable16[1] = string.Concat(file.get_Filename(), file.get_Extension());
					V_1 = CommonHelper.GetFullPath(stackVariable16);
					if (File.Exists(V_1))
					{
						dummyVar0 = this.DeleteFile(V_1);
					}
					if (string.IsNullOrEmpty(file.get_Content()))
					{
						if (file.get_FileStream().IndexOf(',') >= 0)
						{
							stackVariable42 = file.get_FileStream().Split(',', 0)[1];
						}
						else
						{
							stackVariable42 = file.get_FileStream();
						}
						V_4 = stackVariable42;
						if (string.IsNullOrEmpty(ImageResizer.getContentType(V_1)))
						{
							V_5 = Convert.FromBase64String(V_4);
							V_6 = File.Create(V_1);
							try
							{
								V_6.Write(V_5, 0, (int)V_5.Length);
								V_3 = true;
							}
							finally
							{
								if (V_6 != null)
								{
									((IDisposable)V_6).Dispose();
								}
							}
						}
						else
						{
							V_3 = ImageResizer.ResizeImage(MixService.GetConfig<int>("ImageSize"), V_4, V_1);
						}
					}
					else
					{
						V_2 = File.CreateText(V_1);
						try
						{
							V_2.WriteLine(file.get_Content());
							V_3 = true;
						}
						finally
						{
							if (V_2 != null)
							{
								((IDisposable)V_2).Dispose();
							}
						}
					}
				}
			}
			catch
			{
				dummyVar1 = exception_0;
				V_3 = false;
			}
			return V_3;
		}

		public RepositoryResponse<FileViewModel> SaveWebFile(IFormFile file, string filename, string folder)
		{
			try
			{
				V_0 = string.Concat("wwwroot/", folder);
				V_1 = this.SaveFile(file, filename, V_0);
			}
			catch
			{
				dummyVar0 = exception_0;
				V_1 = null;
			}
			return V_1;
		}

		public void UnZipFile(string filePath, string webFolder)
		{
			try
			{
				ZipFile.ExtractToDirectory(filePath, webFolder, true);
			}
			catch
			{
				dummyVar0 = exception_0;
			}
			return;
		}

		public bool UnZipFile(FileViewModel file)
		{
			stackVariable1 = new string[3];
			stackVariable1[0] = "wwwroot";
			stackVariable1[1] = file.get_FileFolder();
			stackVariable1[2] = string.Concat(file.get_Filename(), file.get_Extension());
			V_0 = CommonHelper.GetFullPath(stackVariable1);
			stackVariable15 = new string[2];
			stackVariable15[0] = "wwwroot";
			stackVariable15[1] = file.get_FileFolder();
			V_1 = CommonHelper.GetFullPath(stackVariable15);
			try
			{
				ZipFile.ExtractToDirectory(V_0, V_1);
				V_2 = true;
			}
			catch
			{
				dummyVar0 = exception_0;
				V_2 = false;
			}
			return V_2;
		}

		public string ZipFolder(string tmpPath, string outputPath, string fileName)
		{
			try
			{
				stackVariable1 = new string[5];
				stackVariable1[0] = "wwwroot/";
				stackVariable1[1] = outputPath;
				stackVariable1[2] = "/";
				stackVariable1[3] = fileName;
				stackVariable1[4] = ".zip";
				V_0 = string.Concat(stackVariable1);
				V_1 = string.Concat(outputPath, "/", fileName, ".zip");
				if (!Directory.Exists(tmpPath))
				{
					V_2 = string.Empty;
				}
				else
				{
					if (File.Exists(V_0))
					{
						File.Delete(V_0);
					}
					ZipFile.CreateFromDirectory(tmpPath, V_0);
					dummyVar0 = this.DeleteFolder(tmpPath);
					V_2 = V_1;
				}
			}
			catch (Exception exception_0)
			{
				V_2 = exception_0.get_Message();
			}
			return V_2;
		}
	}
}