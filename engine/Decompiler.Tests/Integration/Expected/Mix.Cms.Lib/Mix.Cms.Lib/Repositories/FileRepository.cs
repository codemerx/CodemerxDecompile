using Microsoft.AspNetCore.Http;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels;
using Mix.Common.Helper;
using Mix.Domain.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
					lock (FileRepository.syncRoot)
					{
						if (FileRepository.instance == null)
						{
							FileRepository.instance = new FileRepository();
						}
					}
				}
				return FileRepository.instance;
			}
			set
			{
				FileRepository.instance = value;
			}
		}

		static FileRepository()
		{
			FileRepository.syncRoot = new object();
		}

		private FileRepository()
		{
			this.CurrentDirectory = Environment.CurrentDirectory;
		}

		public bool CopyDirectory(string srcPath, string desPath)
		{
			int i;
			if (!(srcPath.ToLower() != desPath.ToLower()) || !Directory.Exists(srcPath))
			{
				return true;
			}
			string[] directories = Directory.GetDirectories(srcPath, "*", SearchOption.AllDirectories);
			for (i = 0; i < (int)directories.Length; i++)
			{
				Directory.CreateDirectory(directories[i].Replace(srcPath, desPath));
			}
			directories = Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories);
			for (i = 0; i < (int)directories.Length; i++)
			{
				string str = directories[i];
				File.Copy(str, str.Replace(srcPath, desPath), true);
			}
			return true;
		}

		public bool CopyWebDirectory(string srcPath, string desPath)
		{
			int i;
			if (srcPath == desPath)
			{
				return true;
			}
			string[] directories = Directory.GetDirectories(string.Concat("wwwroot/", srcPath), "*", SearchOption.AllDirectories);
			for (i = 0; i < (int)directories.Length; i++)
			{
				Directory.CreateDirectory(directories[i].Replace(srcPath, desPath));
			}
			directories = Directory.GetFiles(string.Concat("wwwroot/", srcPath), "*.*", SearchOption.AllDirectories);
			for (i = 0; i < (int)directories.Length; i++)
			{
				string str = directories[i];
				File.Copy(str, str.Replace(srcPath, desPath), true);
			}
			return true;
		}

		public void CreateDirectoryIfNotExist(string fullPath)
		{
			if (!string.IsNullOrEmpty(fullPath) && !Directory.Exists(fullPath))
			{
				Directory.CreateDirectory(fullPath);
			}
		}

		public bool DeleteFile(string name, string extension, string FileFolder)
		{
			string fullPath = CommonHelper.GetFullPath(new string[] { "Content/Uploads", FileFolder });
			string str = string.Format("{0}/{1}{2}", fullPath, name, extension);
			if (File.Exists(str))
			{
				CommonHelper.RemoveFile(str);
			}
			return true;
		}

		public bool DeleteFile(string fullPath)
		{
			if (File.Exists(fullPath))
			{
				CommonHelper.RemoveFile(fullPath);
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
			string fullPath = CommonHelper.GetFullPath(new string[] { "wwwroot", "content", folder, filename });
			if (File.Exists(fullPath))
			{
				CommonHelper.RemoveFile(fullPath);
			}
			return true;
		}

		public bool DeleteWebFile(string filePath)
		{
			string fullPath = CommonHelper.GetFullPath(new string[] { "wwwroot", filePath });
			if (File.Exists(fullPath))
			{
				CommonHelper.RemoveFile(fullPath);
			}
			return true;
		}

		public bool DeleteWebFile(string name, string extension, string FileFolder)
		{
			string str = string.Format("{0}/{1}/{2}{3}", new object[] { "wwwroot", FileFolder, name, extension });
			if (File.Exists(str))
			{
				CommonHelper.RemoveFile(str);
			}
			return true;
		}

		public bool DeleteWebFolder(string folderPath)
		{
			string fullPath = CommonHelper.GetFullPath(new string[] { "wwwroot", folderPath });
			if (Directory.Exists(fullPath))
			{
				Directory.Delete(fullPath, true);
			}
			return true;
		}

		public bool EmptyFolder(string folderPath)
		{
			this.DeleteFolder(folderPath);
			this.CreateDirectoryIfNotExist(folderPath);
			return true;
		}

		public FileViewModel GetFile(string FilePath, List<FileViewModel> Files, string FileFolder)
		{
			return Files.Find((FileViewModel v) => {
				if (string.IsNullOrEmpty(FilePath))
				{
					return false;
				}
				return v.Filename == FilePath.Replace("\\", "/").Split('/', StringSplitOptions.None)[1];
			}) ?? new FileViewModel()
			{
				FileFolder = FileFolder
			};
		}

		public FileViewModel GetFile(string name, string ext, string FileFolder, bool isCreate = false, string defaultContent = "")
		{
			FileViewModel fileViewModel = null;
			string str = Path.Combine(this.CurrentDirectory, FileFolder, string.Format("{0}{1}", name, ext));
			FileInfo fileInfo = new FileInfo(str);
			if (fileInfo.Exists)
			{
				try
				{
					using (FileStream fileStream = File.Open(str, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						using (StreamReader streamReader = new StreamReader(fileStream))
						{
							fileViewModel = new FileViewModel()
							{
								FileFolder = FileFolder,
								Filename = name,
								Extension = ext,
								Content = streamReader.ReadToEnd()
							};
						}
					}
				}
				catch
				{
				}
			}
			else if (isCreate)
			{
				this.CreateDirectoryIfNotExist(FileFolder);
				fileInfo.Create();
				fileViewModel = new FileViewModel()
				{
					FileFolder = FileFolder,
					Filename = name,
					Extension = ext,
					Content = defaultContent
				};
				this.SaveFile(fileViewModel);
			}
			return fileViewModel ?? new FileViewModel()
			{
				FileFolder = FileFolder
			};
		}

		public FileViewModel GetFile(string fullname, string FileFolder, bool isCreate = false, string defaultContent = "")
		{
			string[] strArrays = fullname.Split('.', StringSplitOptions.None);
			if ((int)strArrays.Length < 2)
			{
				return new FileViewModel()
				{
					FileFolder = FileFolder
				};
			}
			return this.GetFile(fullname.Substring(0, fullname.LastIndexOf('.')), string.Concat(".", strArrays[(int)strArrays.Length - 1]), FileFolder, isCreate, defaultContent);
		}

		public List<FileViewModel> GetFiles(string fullPath)
		{
			this.CreateDirectoryIfNotExist(fullPath);
			List<FileViewModel> fileViewModels = new List<FileViewModel>();
			string[] directories = Directory.GetDirectories(fullPath, "*", SearchOption.AllDirectories);
			for (int i = 0; i < (int)directories.Length; i++)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(directories[i]);
				string name = directoryInfo.Name;
				foreach (FileInfo fileInfo in 
					from f in (IEnumerable<FileInfo>)directoryInfo.GetFiles()
					orderby f.CreationTimeUtc descending
					select f)
				{
					FileViewModel fileViewModel = new FileViewModel()
					{
						FolderName = name,
						FileFolder = CommonHelper.GetFullPath(new string[] { fullPath, name }),
						Filename = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.')),
						Extension = fileInfo.Extension
					};
					fileViewModels.Add(fileViewModel);
				}
			}
			return fileViewModels;
		}

		public List<FileViewModel> GetFiles(MixEnums.FileFolder FileFolder)
		{
			return this.GetUploadFiles(FileFolder.ToString());
		}

		public List<FileViewModel> GetFilesWithContent(string fullPath)
		{
			this.CreateDirectoryIfNotExist(fullPath);
			List<FileViewModel> fileViewModels = new List<FileViewModel>();
			string[] directories = Directory.GetDirectories(fullPath, "*", SearchOption.AllDirectories);
			for (int i = 0; i < (int)directories.Length; i++)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(directories[i]);
				string name = directoryInfo.Name;
				foreach (FileInfo fileInfo in 
					from f in (IEnumerable<FileInfo>)directoryInfo.GetFiles()
					orderby f.CreationTimeUtc descending
					select f)
				{
					using (StreamReader streamReader = fileInfo.OpenText())
					{
						FileViewModel fileViewModel = new FileViewModel()
						{
							FolderName = name,
							FileFolder = CommonHelper.GetFullPath(new string[] { fullPath, name }),
							Filename = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.')),
							Extension = fileInfo.Extension,
							Content = streamReader.ReadToEnd()
						};
						fileViewModels.Add(fileViewModel);
					}
				}
			}
			return fileViewModels;
		}

		public List<string> GetTopDirectories(string folder)
		{
			List<string> strs = new List<string>();
			if (Directory.Exists(folder))
			{
				string[] directories = Directory.GetDirectories(folder, "*", SearchOption.TopDirectoryOnly);
				for (int i = 0; i < (int)directories.Length; i++)
				{
					strs.Add((new DirectoryInfo(directories[i])).Name);
				}
			}
			return strs;
		}

		public List<FileViewModel> GetTopFiles(string folder)
		{
			List<FileViewModel> fileViewModels = new List<FileViewModel>();
			if (Directory.Exists(folder))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(folder);
				string name = directoryInfo.Name;
				foreach (FileInfo fileInfo in 
					from f in (IEnumerable<FileInfo>)directoryInfo.GetFiles()
					orderby f.CreationTimeUtc descending
					select f)
				{
					fileViewModels.Add(new FileViewModel()
					{
						FolderName = name,
						FileFolder = folder,
						Filename = fileInfo.Name.Substring(0, (fileInfo.Name.LastIndexOf('.') >= 0 ? fileInfo.Name.LastIndexOf('.') : 0)),
						Extension = fileInfo.Extension
					});
				}
			}
			return fileViewModels;
		}

		public FileViewModel GetUploadFile(string name, string ext, string FileFolder)
		{
			FileViewModel fileViewModel = null;
			string fullPath = CommonHelper.GetFullPath(new string[] { "Content/Uploads", FileFolder });
			FileInfo fileInfo = new FileInfo(string.Format("{0}/{1}.{2}", fullPath, name, ext));
			try
			{
				using (StreamReader streamReader = fileInfo.OpenText())
				{
					fileViewModel = new FileViewModel()
					{
						FileFolder = FileFolder,
						Filename = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.')),
						Extension = fileInfo.Extension.Remove(0, 1),
						Content = streamReader.ReadToEnd()
					};
				}
			}
			catch
			{
			}
			return fileViewModel ?? new FileViewModel()
			{
				FileFolder = FileFolder
			};
		}

		public List<FileViewModel> GetUploadFiles(string folder)
		{
			string fullPath = CommonHelper.GetFullPath(new string[] { "Content/Uploads", folder });
			this.CreateDirectoryIfNotExist(fullPath);
			FileInfo[] files = (new DirectoryInfo(fullPath)).GetFiles();
			List<FileViewModel> fileViewModels = new List<FileViewModel>();
			foreach (FileInfo fileInfo in 
				from f in (IEnumerable<FileInfo>)files
				orderby f.CreationTimeUtc descending
				select f)
			{
				using (StreamReader streamReader = fileInfo.OpenText())
				{
					fileViewModels.Add(new FileViewModel()
					{
						FileFolder = folder,
						Filename = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.')),
						Extension = fileInfo.Extension,
						Content = streamReader.ReadToEnd()
					});
				}
			}
			return fileViewModels;
		}

		public FileViewModel GetWebFile(string filename, string folder)
		{
			string str = string.Concat("wwwroot/", folder, "/", filename);
			string str1 = string.Concat("wwwroot/content/", folder);
			FileInfo fileInfo = new FileInfo(str);
			FileViewModel fileViewModel = null;
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(str1);
				using (StreamReader streamReader = fileInfo.OpenText())
				{
					fileViewModel = new FileViewModel()
					{
						FolderName = directoryInfo.Name,
						FileFolder = folder,
						Filename = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.')),
						Extension = fileInfo.Extension,
						Content = streamReader.ReadToEnd()
					};
				}
			}
			catch
			{
			}
			return fileViewModel ?? new FileViewModel()
			{
				FileFolder = folder
			};
		}

		public List<FileViewModel> GetWebFiles(string folder)
		{
			string fullPath = CommonHelper.GetFullPath(new string[] { "wwwroot", "content", folder });
			this.CreateDirectoryIfNotExist(fullPath);
			List<FileViewModel> fileViewModels = new List<FileViewModel>();
			string[] directories = Directory.GetDirectories(fullPath, "*", SearchOption.AllDirectories);
			for (int i = 0; i < (int)directories.Length; i++)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(directories[i]);
				string str = directoryInfo.ToString().Replace("\\", "/").Replace("wwwroot", string.Empty);
				foreach (FileInfo fileInfo in 
					from f in (IEnumerable<FileInfo>)directoryInfo.GetFiles()
					orderby f.CreationTimeUtc descending
					select f)
				{
					fileViewModels.Add(new FileViewModel()
					{
						FolderName = directoryInfo.Name,
						FileFolder = str,
						Filename = (fileInfo.Name.LastIndexOf('.') >= 0 ? fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.')) : fileInfo.Name),
						Extension = fileInfo.Extension
					});
				}
			}
			return fileViewModels;
		}

		public RepositoryResponse<FileViewModel> SaveFile(IFormFile file, string filename, string folder)
		{
			RepositoryResponse<FileViewModel> repositoryResponse = new RepositoryResponse<FileViewModel>();
			try
			{
				if (file.get_Length() <= (long)0)
				{
					repositoryResponse.set_IsSucceed(false);
					repositoryResponse.get_Errors().Add("File not found");
				}
				else
				{
					this.CreateDirectoryIfNotExist(folder);
					string fullPath = CommonHelper.GetFullPath(new string[] { folder, filename });
					if (File.Exists(fullPath))
					{
						this.DeleteFile(fullPath);
					}
					using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
					{
						file.CopyTo(fileStream);
					}
					repositoryResponse.set_IsSucceed(true);
					repositoryResponse.set_Data(new FileViewModel()
					{
						Filename = filename.Substring(0, file.get_FileName().LastIndexOf('.')),
						Extension = filename.Substring(file.get_FileName().LastIndexOf('.')),
						FileFolder = folder
					});
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				repositoryResponse.set_IsSucceed(false);
				repositoryResponse.set_Exception(exception);
				repositoryResponse.get_Errors().Add(exception.Message);
			}
			return repositoryResponse;
		}

		public bool SaveFile(FileViewModel file)
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
					this.CreateDirectoryIfNotExist(file.FileFolder);
					string fullPath = string.Concat(file.Filename, file.Extension);
					if (!string.IsNullOrEmpty(file.FileFolder))
					{
						fullPath = CommonHelper.GetFullPath(new string[] { file.FileFolder, fullPath });
					}
					if (File.Exists(fullPath))
					{
						this.DeleteFile(fullPath);
					}
					if (string.IsNullOrEmpty(file.Content))
					{
						byte[] numArray = Convert.FromBase64String(file.FileStream.Split(',', StringSplitOptions.None)[1]);
						using (FileStream fileStream = File.Create(fullPath))
						{
							fileStream.Write(numArray, 0, (int)numArray.Length);
							flag = true;
						}
					}
					else
					{
						using (StreamWriter streamWriter = File.CreateText(fullPath))
						{
							streamWriter.WriteLine(file.Content);
							streamWriter.Dispose();
							flag = true;
						}
					}
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public bool SaveWebFile(FileViewModel file)
		{
			bool flag;
			try
			{
				string fullPath = CommonHelper.GetFullPath(new string[] { "wwwroot", file.FileFolder });
				if (string.IsNullOrEmpty(file.Filename))
				{
					flag = false;
				}
				else
				{
					this.CreateDirectoryIfNotExist(fullPath);
					string str = CommonHelper.GetFullPath(new string[] { fullPath, string.Concat(file.Filename, file.Extension) });
					if (File.Exists(str))
					{
						this.DeleteFile(str);
					}
					if (string.IsNullOrEmpty(file.Content))
					{
						string str1 = (file.FileStream.IndexOf(',') >= 0 ? file.FileStream.Split(',', StringSplitOptions.None)[1] : file.FileStream);
						if (string.IsNullOrEmpty(ImageResizer.getContentType(str)))
						{
							byte[] numArray = Convert.FromBase64String(str1);
							using (FileStream fileStream = File.Create(str))
							{
								fileStream.Write(numArray, 0, (int)numArray.Length);
								flag = true;
							}
						}
						else
						{
							flag = ImageResizer.ResizeImage(MixService.GetConfig<int>("ImageSize"), str1, str);
						}
					}
					else
					{
						using (StreamWriter streamWriter = File.CreateText(str))
						{
							streamWriter.WriteLine(file.Content);
							flag = true;
						}
					}
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public RepositoryResponse<FileViewModel> SaveWebFile(IFormFile file, string filename, string folder)
		{
			RepositoryResponse<FileViewModel> repositoryResponse;
			try
			{
				string str = string.Concat("wwwroot/", folder);
				repositoryResponse = this.SaveFile(file, filename, str);
			}
			catch
			{
				repositoryResponse = null;
			}
			return repositoryResponse;
		}

		public void UnZipFile(string filePath, string webFolder)
		{
			try
			{
				ZipFile.ExtractToDirectory(filePath, webFolder, true);
			}
			catch
			{
			}
		}

		public bool UnZipFile(FileViewModel file)
		{
			bool flag;
			string fullPath = CommonHelper.GetFullPath(new string[] { "wwwroot", file.FileFolder, string.Concat(file.Filename, file.Extension) });
			string str = CommonHelper.GetFullPath(new string[] { "wwwroot", file.FileFolder });
			try
			{
				ZipFile.ExtractToDirectory(fullPath, str);
				flag = true;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public string ZipFolder(string tmpPath, string outputPath, string fileName)
		{
			string empty;
			try
			{
				string str = string.Concat(new string[] { "wwwroot/", outputPath, "/", fileName, ".zip" });
				string str1 = string.Concat(outputPath, "/", fileName, ".zip");
				if (!Directory.Exists(tmpPath))
				{
					empty = string.Empty;
				}
				else
				{
					if (File.Exists(str))
					{
						File.Delete(str);
					}
					ZipFile.CreateFromDirectory(tmpPath, str);
					this.DeleteFolder(tmpPath);
					empty = str1;
				}
			}
			catch (Exception exception)
			{
				empty = exception.Message;
			}
			return empty;
		}
	}
}