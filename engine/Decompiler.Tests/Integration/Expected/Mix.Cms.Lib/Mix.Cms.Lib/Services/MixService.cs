using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Repositories;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixCultures;
using Mix.Cms.Lib.ViewModels.MixTemplates;
using Mix.Common.Helper;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.Services
{
	public class MixService
	{
		private readonly static object syncRoot;

		private static volatile MixService instance;

		private static volatile MixService defaultInstance;

		private readonly FileSystemWatcher watcher = new FileSystemWatcher();

		private JObject Authentication
		{
			get;
			set;
		}

		private JObject ConnectionStrings
		{
			get;
			set;
		}

		private List<string> Cultures
		{
			get;
			set;
		}

		public static MixService DefaultInstance
		{
			get
			{
				if (MixService.defaultInstance == null)
				{
					lock (MixService.syncRoot)
					{
						if (MixService.defaultInstance == null)
						{
							MixService.defaultInstance = new MixService();
							MixService.defaultInstance.LoadDefaultConfiggurations();
						}
					}
				}
				return MixService.defaultInstance;
			}
		}

		private JObject GlobalSettings
		{
			get;
			set;
		}

		public static MixService Instance
		{
			get
			{
				if (MixService.instance == null)
				{
					lock (MixService.syncRoot)
					{
						if (MixService.instance == null)
						{
							MixService.instance = new MixService();
							MixService.instance.LoadConfiggurations();
						}
					}
				}
				return MixService.instance;
			}
		}

		private JObject IpSecuritySettings
		{
			get;
			set;
		}

		private JObject LocalSettings
		{
			get;
			set;
		}

		private JObject Smtp
		{
			get;
			set;
		}

		private JObject Translator
		{
			get;
			set;
		}

		static MixService()
		{
			MixService.syncRoot = new object();
		}

		public MixService()
		{
			this.watcher.Path = Directory.GetCurrentDirectory();
			this.watcher.Filter = "";
			this.watcher.Changed += new FileSystemEventHandler(this.OnChanged);
			this.watcher.EnableRaisingEvents = true;
		}

		public bool CheckValidCulture(string specificulture)
		{
			object list;
			if (MixService.Instance.Cultures == null)
			{
				List<Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel> data = ViewModelBase<MixCmsContext, MixCulture, Mix.Cms.Lib.ViewModels.MixCultures.UpdateViewModel>.Repository.GetModelList(null, null).get_Data();
				MixService instance = MixService.Instance;
				if (data != null)
				{
					list = (
						from c in data
						select c.Specificulture).ToList<string>();
				}
				else
				{
					list = null;
				}
				if (list == null)
				{
					list = new List<string>();
				}
				instance.Cultures = (List<string>)list;
			}
			return MixService.Instance.Cultures.Any<string>((string c) => c == specificulture);
		}

		public static T GetAuthConfig<T>(string name)
		{
			JToken item = MixService.Instance.Authentication.get_Item(name) ?? MixService.DefaultInstance.Authentication.get_Item(name);
			if (item != null)
			{
				return Newtonsoft.Json.Linq.Extensions.Value<T>(item);
			}
			return default(T);
		}

		public static T GetConfig<T>(string name)
		{
			JToken item = MixService.Instance.GlobalSettings.get_Item(name) ?? MixService.DefaultInstance.GlobalSettings.get_Item(name);
			if (item != null)
			{
				return Newtonsoft.Json.Linq.Extensions.Value<T>(item);
			}
			return default(T);
		}

		public static T GetConfig<T>(string name, string culture)
		{
			JToken item = null;
			if (!string.IsNullOrEmpty(culture) && MixService.Instance.LocalSettings.get_Item(culture) != null)
			{
				item = MixService.Instance.LocalSettings.get_Item(culture).get_Item(name);
			}
			if (item != null)
			{
				return Newtonsoft.Json.Linq.Extensions.Value<T>(item);
			}
			return default(T);
		}

		public static string GetConnectionString(string name)
		{
			JObject connectionStrings = MixService.Instance.ConnectionStrings;
			if (connectionStrings == null)
			{
				return null;
			}
			return Newtonsoft.Json.Linq.Extensions.Value<string>(connectionStrings.get_Item(name));
		}

		public static JObject GetGlobalSetting()
		{
			return JObject.FromObject(MixService.Instance.GlobalSettings);
		}

		public static T GetIpConfig<T>(string name)
		{
			JToken item = MixService.Instance.IpSecuritySettings.get_Item(name) ?? MixService.DefaultInstance.IpSecuritySettings.get_Item(name);
			if (item != null)
			{
				return Newtonsoft.Json.Linq.Extensions.Value<T>(item);
			}
			return default(T);
		}

		public static JObject GetLocalSettings(string culture)
		{
			// 
			// Current member / type: Newtonsoft.Json.Linq.JObject Mix.Cms.Lib.Services.MixService::GetLocalSettings(System.String)
			// Exception in: Newtonsoft.Json.Linq.JObject GetLocalSettings(System.String)
			// Object reference not set to an instance of an object.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public static string GetTemplateFolder(string culture)
		{
			return string.Format("content/templates/{0}", MixService.Instance.LocalSettings.get_Item(culture).get_Item("ThemeFolder"));
		}

		public static string GetTemplateUploadFolder(string culture)
		{
			return string.Format("content/templates/{0}/uploads", MixService.Instance.LocalSettings.get_Item(culture).get_Item("ThemeFolder"));
		}

		public static JObject GetTranslator(string culture)
		{
			// 
			// Current member / type: Newtonsoft.Json.Linq.JObject Mix.Cms.Lib.Services.MixService::GetTranslator(System.String)
			// Exception in: Newtonsoft.Json.Linq.JObject GetTranslator(System.String)
			// Object reference not set to an instance of an object.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		private void LoadConfiggurations()
		{
			// 
			// Current member / type: System.Void Mix.Cms.Lib.Services.MixService::LoadConfiggurations()
			// Exception in: System.Void LoadConfiggurations()
			// Object reference not set to an instance of an object.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		private void LoadDefaultConfiggurations()
		{
			// 
			// Current member / type: System.Void Mix.Cms.Lib.Services.MixService::LoadDefaultConfiggurations()
			// Exception in: System.Void LoadDefaultConfiggurations()
			// Object reference not set to an instance of an object.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public static void LoadFromDatabase(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			Func<MixLanguage, bool> func = null;
			Func<MixConfiguration, bool> func1 = null;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag);
			try
			{
				try
				{
					MixService.Instance.Translator = new JObject();
					List<MixLanguage> list = mixCmsContext.MixLanguage.ToList<MixLanguage>();
					List<MixCulture> mixCultures = mixCmsContext.MixCulture.ToList<MixCulture>();
					foreach (MixCulture mixCulture in mixCultures)
					{
						JObject jObject = new JObject();
						List<MixLanguage> mixLanguages = list;
						Func<MixLanguage, bool> func2 = func;
						if (func2 == null)
						{
							Func<MixLanguage, bool> specificulture = (MixLanguage l) => l.Specificulture == mixCulture.Specificulture;
							Func<MixLanguage, bool> func3 = specificulture;
							func = specificulture;
							func2 = func3;
						}
						foreach (MixLanguage mixLanguage in mixLanguages.Where<MixLanguage>(func2).ToList<MixLanguage>())
						{
							jObject.Add(new JProperty(mixLanguage.Keyword, mixLanguage.Value ?? mixLanguage.DefaultValue));
						}
						MixService.Instance.Translator.Add(new JProperty(mixCulture.Specificulture, jObject));
					}
					MixService.Instance.LocalSettings = new JObject();
					List<MixConfiguration> mixConfigurations = mixCmsContext.MixConfiguration.ToList<MixConfiguration>();
					foreach (MixCulture mixCulture1 in mixCultures)
					{
						JObject jObject1 = new JObject();
						List<MixConfiguration> mixConfigurations1 = mixConfigurations;
						Func<MixConfiguration, bool> func4 = func1;
						if (func4 == null)
						{
							Func<MixConfiguration, bool> specificulture1 = (MixConfiguration l) => l.Specificulture == mixCulture1.Specificulture;
							Func<MixConfiguration, bool> func5 = specificulture1;
							func1 = specificulture1;
							func4 = func5;
						}
						foreach (MixConfiguration mixConfiguration in mixConfigurations1.Where<MixConfiguration>(func4).ToList<MixConfiguration>())
						{
							jObject1.Add(new JProperty(mixConfiguration.Keyword, mixConfiguration.Value));
						}
						MixService.Instance.LocalSettings.Add(new JProperty(mixCulture1.Specificulture, jObject1));
					}
					UnitOfWorkHelper<MixCmsContext>.HandleTransaction(true, flag, dbContextTransaction);
				}
				catch (Exception exception)
				{
					UnitOfWorkHelper<MixCmsContext>.HandleException<MixLanguage>(exception, flag, dbContextTransaction);
				}
			}
			finally
			{
				if (flag)
				{
					RelationalDatabaseFacadeExtensions.CloseConnection(mixCmsContext.get_Database());
					dbContextTransaction.Dispose();
					mixCmsContext.Dispose();
				}
			}
		}

		public static void LogException(Exception ex)
		{
			string currentDirectory = Environment.CurrentDirectory;
			DateTime now = DateTime.Now;
			string str = string.Concat(currentDirectory, "/logs/", now.ToString("dd-MM-yyyy"));
			if (!string.IsNullOrEmpty(str) && !Directory.Exists(str))
			{
				Directory.CreateDirectory(str);
			}
			string str1 = string.Concat(str, "/log_exceptions.json");
			try
			{
				FileInfo fileInfo = new FileInfo(str1);
				string end = "[]";
				if (fileInfo.Exists)
				{
					using (StreamReader streamReader = fileInfo.OpenText())
					{
						end = streamReader.ReadToEnd();
					}
					File.Delete(str1);
				}
				JArray jArray = JArray.Parse(end);
				JObject jObject = new JObject();
				jObject.Add(new JProperty("CreatedDateTime", (object)DateTime.UtcNow));
				jObject.Add(new JProperty("Details", JObject.FromObject(ex)));
				jArray.Add(jObject);
				end = jArray.ToString();
				using (StreamWriter streamWriter = File.CreateText(str1))
				{
					streamWriter.WriteLine(end);
				}
			}
			catch
			{
				Console.Write(string.Concat("Cannot write log file ", str1));
			}
		}

		private void OnChanged(object sender, FileSystemEventArgs e)
		{
			Thread.Sleep(0x1f4);
			MixService.Instance.LoadConfiggurations();
		}

		public static void Reload()
		{
			MixService.Instance.LoadConfiggurations();
		}

		public static bool SaveSettings()
		{
			FileViewModel file = FileRepository.Instance.GetFile("appsettings.json", string.Empty, true, "{}");
			if (file == null)
			{
				return false;
			}
			if (string.IsNullOrWhiteSpace(file.Content))
			{
				FileViewModel fileViewModel = FileRepository.Instance.GetFile("default.appsettings.json", string.Empty, true, "{}");
				file = new FileViewModel()
				{
					Filename = "appsettings",
					Extension = ".json",
					Content = fileViewModel.Content
				};
				return FileRepository.Instance.SaveFile(file);
			}
			JObject jObject = JObject.Parse(file.Content);
			jObject.set_Item("ConnectionStrings", MixService.instance.ConnectionStrings);
			jObject.set_Item("GlobalSettings", MixService.instance.GlobalSettings);
			jObject.get_Item("GlobalSettings").set_Item("LastUpdateConfiguration", DateTime.UtcNow);
			jObject.set_Item("Translator", MixService.instance.Translator);
			jObject.set_Item("LocalSettings", MixService.instance.LocalSettings);
			jObject.set_Item("Authentication", MixService.instance.Authentication);
			jObject.set_Item("IpSecuritySettings", MixService.instance.IpSecuritySettings);
			jObject.set_Item("Smtp", MixService.instance.Smtp);
			file.Content = jObject.ToString();
			return FileRepository.Instance.SaveFile(file);
		}

		public static bool SaveSettings(string content)
		{
			FileViewModel file = FileRepository.Instance.GetFile("appsettings.json", string.Empty, true, "{}");
			file.Content = content;
			return FileRepository.Instance.SaveFile(file);
		}

		public static Task SendEdm(string culture, string template, JObject data, string subject, string from)
		{
			return Task.Run(() => {
				if (!string.IsNullOrEmpty(Newtonsoft.Json.Linq.Extensions.Value<string>(data.get_Item("email"))))
				{
					string str = Newtonsoft.Json.Linq.Extensions.Value<string>(data.get_Item("email"));
					RepositoryResponse<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel> templateByPath = Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel.GetTemplateByPath(template, culture, null, null);
					if (templateByPath.get_IsSucceed() && !string.IsNullOrEmpty(templateByPath.get_Data().Content))
					{
						string content = templateByPath.get_Data().Content;
						foreach (JProperty jProperty in data.Properties())
						{
							content = content.Replace(string.Concat("[[", jProperty.get_Name(), "]]"), Newtonsoft.Json.Linq.Extensions.Value<string>(data.get_Item(jProperty.get_Name())));
						}
						MixService.SendMail(subject, content, str, from);
					}
				}
			});
		}

		public static void SendMail(string subject, string message, string to, string from = null)
		{
			MailMessage mailMessage = new MailMessage()
			{
				IsBodyHtml = true,
				From = new MailAddress(from ?? MixService.instance.Smtp.Value<string>("From"))
			};
			mailMessage.To.Add(to);
			mailMessage.Body = message;
			mailMessage.Subject = subject;
			try
			{
				(new SmtpClient(MixService.instance.Smtp.Value<string>("Server"))
				{
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(MixService.instance.Smtp.Value<string>("User"), MixService.instance.Smtp.Value<string>("Password")),
					Port = MixService.instance.Smtp.Value<int>("Port"),
					EnableSsl = MixService.instance.Smtp.Value<bool>("SSL")
				}).Send(mailMessage);
			}
			catch
			{
				try
				{
					(new SmtpClient()
					{
						UseDefaultCredentials = true
					}).Send(mailMessage);
				}
				catch (Exception exception)
				{
					MixService.LogException(exception);
				}
			}
		}

		public static void SetAuthConfig<T>(string name, T value)
		{
			MixService.Instance.Authentication.set_Item(name, value.ToString());
		}

		public static void SetConfig<T>(string name, T value)
		{
			JToken jToken;
			JObject globalSettings = MixService.Instance.GlobalSettings;
			string str = name;
			if (value != null)
			{
				jToken = JToken.FromObject(value);
			}
			else
			{
				jToken = null;
			}
			globalSettings.set_Item(str, jToken);
		}

		public static void SetConfig<T>(string name, string culture, T value)
		{
			MixService.Instance.LocalSettings.get_Item(culture).set_Item(name, value.ToString());
		}

		public static void SetConnectionString(string name, string value)
		{
			MixService.Instance.ConnectionStrings.set_Item(name, value);
		}

		public static void SetIpConfig<T>(string name, T value)
		{
			MixService.Instance.IpSecuritySettings.set_Item(name, value.ToString());
		}

		public static T Translate<T>(string name, string culture)
		{
			JToken item = MixService.Instance.Translator.get_Item(culture).get_Item(name);
			if (item != null)
			{
				return Newtonsoft.Json.Linq.Extensions.Value<T>(item);
			}
			return default(T);
		}

		public static string TranslateString(string name, string culture)
		{
			JToken item = MixService.Instance.Translator.get_Item(culture).get_Item(name);
			if (item == null)
			{
				return name;
			}
			return Newtonsoft.Json.Linq.Extensions.Value<string>(item);
		}
	}
}