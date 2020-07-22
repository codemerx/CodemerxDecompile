using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixCultures;
using Mix.Domain.Core.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.Services
{
	public class MixService
	{
		private readonly static object syncRoot;

		private static volatile MixService instance;

		private static volatile MixService defaultInstance;

		private readonly FileSystemWatcher watcher;

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
					V_0 = MixService.syncRoot;
					V_1 = false;
					try
					{
						Monitor.Enter(V_0, ref V_1);
						if (MixService.defaultInstance == null)
						{
							MixService.defaultInstance = new MixService();
							MixService.defaultInstance.LoadDefaultConfiggurations();
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
					V_0 = MixService.syncRoot;
					V_1 = false;
					try
					{
						Monitor.Enter(V_0, ref V_1);
						if (MixService.instance == null)
						{
							MixService.instance = new MixService();
							MixService.instance.LoadConfiggurations();
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
			return;
		}

		public MixService()
		{
			this.watcher = new FileSystemWatcher();
			base();
			this.watcher.set_Path(Directory.GetCurrentDirectory());
			this.watcher.set_Filter("");
			this.watcher.add_Changed(new FileSystemEventHandler(this, MixService.OnChanged));
			this.watcher.set_EnableRaisingEvents(true);
			return;
		}

		public bool CheckValidCulture(string specificulture)
		{
			V_0 = new MixService.u003cu003ec__DisplayClass46_0();
			V_0.specificulture = specificulture;
			if (MixService.get_Instance().get_Cultures() == null)
			{
				V_1 = ViewModelBase<MixCmsContext, MixCulture, UpdateViewModel>.Repository.GetModelList(null, null).get_Data();
				stackVariable16 = MixService.get_Instance();
				if (V_1 != null)
				{
					stackVariable18 = V_1;
					stackVariable19 = MixService.u003cu003ec.u003cu003e9__46_1;
					if (stackVariable19 == null)
					{
						dummyVar0 = stackVariable19;
						stackVariable19 = new Func<UpdateViewModel, string>(MixService.u003cu003ec.u003cu003e9, MixService.u003cu003ec.u003cCheckValidCultureu003eb__46_1);
						MixService.u003cu003ec.u003cu003e9__46_1 = stackVariable19;
					}
					stackVariable21 = Enumerable.ToList<string>(Enumerable.Select<UpdateViewModel, string>(stackVariable18, stackVariable19));
				}
				else
				{
					stackVariable21 = null;
				}
				if (stackVariable21 == null)
				{
					dummyVar1 = stackVariable21;
					stackVariable21 = new List<string>();
				}
				stackVariable16.set_Cultures(stackVariable21);
			}
			return Enumerable.Any<string>(MixService.get_Instance().get_Cultures(), new Func<string, bool>(V_0, MixService.u003cu003ec__DisplayClass46_0.u003cCheckValidCultureu003eb__0));
		}

		public static T GetAuthConfig<T>(string name)
		{
			V_0 = MixService.get_Instance().get_Authentication().get_Item(name);
			if (V_0 == null)
			{
				V_0 = MixService.get_DefaultInstance().get_Authentication().get_Item(name);
			}
			if (V_0 != null)
			{
				return Newtonsoft.Json.Linq.Extensions.Value<T>(V_0);
			}
			V_1 = default(T);
			return V_1;
		}

		public static T GetConfig<T>(string name)
		{
			V_0 = MixService.get_Instance().get_GlobalSettings().get_Item(name);
			if (V_0 == null)
			{
				V_0 = MixService.get_DefaultInstance().get_GlobalSettings().get_Item(name);
			}
			if (V_0 != null)
			{
				return Newtonsoft.Json.Linq.Extensions.Value<T>(V_0);
			}
			V_1 = default(T);
			return V_1;
		}

		public static T GetConfig<T>(string name, string culture)
		{
			V_0 = null;
			if (!string.IsNullOrEmpty(culture) && MixService.get_Instance().get_LocalSettings().get_Item(culture) != null)
			{
				V_0 = MixService.get_Instance().get_LocalSettings().get_Item(culture).get_Item(name);
			}
			if (V_0 != null)
			{
				return Newtonsoft.Json.Linq.Extensions.Value<T>(V_0);
			}
			V_1 = default(T);
			return V_1;
		}

		public static string GetConnectionString(string name)
		{
			stackVariable1 = MixService.get_Instance().get_ConnectionStrings();
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				return null;
			}
			return Newtonsoft.Json.Linq.Extensions.Value<string>(stackVariable1.get_Item(name));
		}

		public static JObject GetGlobalSetting()
		{
			return JObject.FromObject(MixService.get_Instance().get_GlobalSettings());
		}

		public static T GetIpConfig<T>(string name)
		{
			V_0 = MixService.get_Instance().get_IpSecuritySettings().get_Item(name);
			if (V_0 == null)
			{
				V_0 = MixService.get_DefaultInstance().get_IpSecuritySettings().get_Item(name);
			}
			if (V_0 != null)
			{
				return Newtonsoft.Json.Linq.Extensions.Value<T>(V_0);
			}
			V_1 = default(T);
			return V_1;
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
			return string.Format("content/templates/{0}", MixService.get_Instance().get_LocalSettings().get_Item(culture).get_Item("ThemeFolder"));
		}

		public static string GetTemplateUploadFolder(string culture)
		{
			return string.Format("content/templates/{0}/uploads", MixService.get_Instance().get_LocalSettings().get_Item(culture).get_Item("ThemeFolder"));
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
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref V_0, ref V_1, ref V_2);
			try
			{
				try
				{
					MixService.get_Instance().set_Translator(new JObject());
					V_3 = Enumerable.ToList<MixLanguage>(V_0.get_MixLanguage());
					V_4 = Enumerable.ToList<MixCulture>(V_0.get_MixCulture());
					V_6 = V_4.GetEnumerator();
					try
					{
						while (V_6.MoveNext())
						{
							V_7 = new MixService.u003cu003ec__DisplayClass63_0();
							V_7.culture = V_6.get_Current();
							V_8 = new JObject();
							stackVariable22 = V_3;
							stackVariable24 = V_7.u003cu003e9__0;
							if (stackVariable24 == null)
							{
								dummyVar0 = stackVariable24;
								stackVariable50 = new Func<MixLanguage, bool>(V_7, MixService.u003cu003ec__DisplayClass63_0.u003cLoadFromDatabaseu003eb__0);
								V_10 = stackVariable50;
								V_7.u003cu003e9__0 = stackVariable50;
								stackVariable24 = V_10;
							}
							V_9 = Enumerable.ToList<MixLanguage>(Enumerable.Where<MixLanguage>(stackVariable22, stackVariable24)).GetEnumerator();
							try
							{
								while (V_9.MoveNext())
								{
									V_11 = V_9.get_Current();
									stackVariable33 = V_11.get_Keyword();
									stackVariable35 = V_11.get_Value();
									if (stackVariable35 == null)
									{
										dummyVar1 = stackVariable35;
										stackVariable35 = V_11.get_DefaultValue();
									}
									V_8.Add(new JProperty(stackVariable33, stackVariable35));
								}
							}
							finally
							{
								V_9.Dispose();
							}
							MixService.get_Instance().get_Translator().Add(new JProperty(V_7.culture.get_Specificulture(), V_8));
						}
					}
					finally
					{
						V_6.Dispose();
					}
					MixService.get_Instance().set_LocalSettings(new JObject());
					V_5 = Enumerable.ToList<MixConfiguration>(V_0.get_MixConfiguration());
					V_6 = V_4.GetEnumerator();
					try
					{
						while (V_6.MoveNext())
						{
							V_13 = new MixService.u003cu003ec__DisplayClass63_1();
							V_13.culture = V_6.get_Current();
							V_14 = new JObject();
							stackVariable65 = V_5;
							stackVariable67 = V_13.u003cu003e9__1;
							if (stackVariable67 == null)
							{
								dummyVar2 = stackVariable67;
								stackVariable92 = new Func<MixConfiguration, bool>(V_13, MixService.u003cu003ec__DisplayClass63_1.u003cLoadFromDatabaseu003eb__1);
								V_16 = stackVariable92;
								V_13.u003cu003e9__1 = stackVariable92;
								stackVariable67 = V_16;
							}
							V_15 = Enumerable.ToList<MixConfiguration>(Enumerable.Where<MixConfiguration>(stackVariable65, stackVariable67)).GetEnumerator();
							try
							{
								while (V_15.MoveNext())
								{
									V_17 = V_15.get_Current();
									V_18 = new JProperty(V_17.get_Keyword(), V_17.get_Value());
									V_14.Add(V_18);
								}
							}
							finally
							{
								V_15.Dispose();
							}
							MixService.get_Instance().get_LocalSettings().Add(new JProperty(V_13.culture.get_Specificulture(), V_14));
						}
					}
					finally
					{
						V_6.Dispose();
					}
					UnitOfWorkHelper<MixCmsContext>.HandleTransaction(true, V_2, V_1);
				}
				catch (Exception exception_0)
				{
					dummyVar3 = UnitOfWorkHelper<MixCmsContext>.HandleException<MixLanguage>(exception_0, V_2, V_1);
				}
			}
			finally
			{
				if (V_2)
				{
					RelationalDatabaseFacadeExtensions.CloseConnection(V_0.get_Database());
					V_1.Dispose();
					V_0.Dispose();
				}
			}
			return;
		}

		public static void LogException(Exception ex)
		{
			stackVariable0 = Environment.get_CurrentDirectory();
			V_2 = DateTime.get_Now();
			V_0 = string.Concat(stackVariable0, "/logs/", V_2.ToString("dd-MM-yyyy"));
			if (!string.IsNullOrEmpty(V_0) && !Directory.Exists(V_0))
			{
				dummyVar0 = Directory.CreateDirectory(V_0);
			}
			V_1 = string.Concat(V_0, "/log_exceptions.json");
			try
			{
				V_3 = new FileInfo(V_1);
				V_4 = "[]";
				if (V_3.get_Exists())
				{
					V_6 = V_3.OpenText();
					try
					{
						V_4 = V_6.ReadToEnd();
					}
					finally
					{
						if (V_6 != null)
						{
							V_6.Dispose();
						}
					}
					File.Delete(V_1);
				}
				stackVariable18 = JArray.Parse(V_4);
				stackVariable19 = new JObject();
				stackVariable19.Add(new JProperty("CreatedDateTime", (object)DateTime.get_UtcNow()));
				stackVariable19.Add(new JProperty("Details", JObject.FromObject(ex)));
				stackVariable18.Add(stackVariable19);
				V_4 = stackVariable18.ToString();
				V_7 = File.CreateText(V_1);
				try
				{
					V_7.WriteLine(V_4);
				}
				finally
				{
					if (V_7 != null)
					{
						V_7.Dispose();
					}
				}
			}
			catch
			{
				dummyVar1 = exception_0;
				Console.Write(string.Concat("Cannot write log file ", V_1));
			}
			return;
		}

		private void OnChanged(object sender, FileSystemEventArgs e)
		{
			Thread.Sleep(0x1f4);
			MixService.get_Instance().LoadConfiggurations();
			return;
		}

		public static void Reload()
		{
			MixService.get_Instance().LoadConfiggurations();
			return;
		}

		public static bool SaveSettings()
		{
			V_0 = FileRepository.get_Instance().GetFile("appsettings.json", string.Empty, true, "{}");
			if (V_0 == null)
			{
				return false;
			}
			if (string.IsNullOrWhiteSpace(V_0.get_Content()))
			{
				V_1 = FileRepository.get_Instance().GetFile("default.appsettings.json", string.Empty, true, "{}");
				stackVariable60 = new FileViewModel();
				stackVariable60.set_Filename("appsettings");
				stackVariable60.set_Extension(".json");
				stackVariable60.set_Content(V_1.get_Content());
				V_0 = stackVariable60;
				return FileRepository.get_Instance().SaveFile(V_0);
			}
			V_2 = JObject.Parse(V_0.get_Content());
			V_2.set_Item("ConnectionStrings", MixService.instance.get_ConnectionStrings());
			V_2.set_Item("GlobalSettings", MixService.instance.get_GlobalSettings());
			V_2.get_Item("GlobalSettings").set_Item("LastUpdateConfiguration", JToken.op_Implicit(DateTime.get_UtcNow()));
			V_2.set_Item("Translator", MixService.instance.get_Translator());
			V_2.set_Item("LocalSettings", MixService.instance.get_LocalSettings());
			V_2.set_Item("Authentication", MixService.instance.get_Authentication());
			V_2.set_Item("IpSecuritySettings", MixService.instance.get_IpSecuritySettings());
			V_2.set_Item("Smtp", MixService.instance.get_Smtp());
			V_0.set_Content(V_2.ToString());
			return FileRepository.get_Instance().SaveFile(V_0);
		}

		public static bool SaveSettings(string content)
		{
			V_0 = FileRepository.get_Instance().GetFile("appsettings.json", string.Empty, true, "{}");
			V_0.set_Content(content);
			return FileRepository.get_Instance().SaveFile(V_0);
		}

		public static Task SendEdm(string culture, string template, JObject data, string subject, string from)
		{
			stackVariable0 = new MixService.u003cu003ec__DisplayClass64_0();
			stackVariable0.data = data;
			stackVariable0.template = template;
			stackVariable0.culture = culture;
			stackVariable0.subject = subject;
			stackVariable0.from = from;
			return Task.Run(new Action(stackVariable0, MixService.u003cu003ec__DisplayClass64_0.u003cSendEdmu003eb__0));
		}

		public static void SendMail(string subject, string message, string to, string from = null)
		{
			stackVariable0 = new MailMessage();
			stackVariable0.set_IsBodyHtml(true);
			stackVariable2 = from;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = MixService.instance.get_Smtp().Value<string>("From");
			}
			stackVariable0.set_From(new MailAddress(stackVariable2));
			V_0 = stackVariable0;
			V_0.get_To().Add(to);
			V_0.set_Body(message);
			V_0.set_Subject(subject);
			try
			{
				stackVariable15 = new SmtpClient(MixService.instance.get_Smtp().Value<string>("Server"));
				stackVariable15.set_UseDefaultCredentials(false);
				stackVariable15.set_Credentials(new NetworkCredential(MixService.instance.get_Smtp().Value<string>("User"), MixService.instance.get_Smtp().Value<string>("Password")));
				stackVariable15.set_Port(MixService.instance.get_Smtp().Value<int>("Port"));
				stackVariable15.set_EnableSsl(MixService.instance.get_Smtp().Value<bool>("SSL"));
				stackVariable15.Send(V_0);
			}
			catch
			{
				dummyVar1 = exception_1;
				try
				{
					stackVariable38 = new SmtpClient();
					stackVariable38.set_UseDefaultCredentials(true);
					stackVariable38.Send(V_0);
				}
				catch (Exception exception_0)
				{
					MixService.LogException(exception_0);
				}
			}
			return;
		}

		public static void SetAuthConfig<T>(string name, T value)
		{
			MixService.get_Instance().get_Authentication().set_Item(name, JToken.op_Implicit(value.ToString()));
			return;
		}

		public static void SetConfig<T>(string name, T value)
		{
			stackVariable1 = MixService.get_Instance().get_GlobalSettings();
			stackVariable2 = name;
			if (value != null)
			{
				stackVariable7 = JToken.FromObject(value);
			}
			else
			{
				stackVariable7 = null;
			}
			stackVariable1.set_Item(stackVariable2, stackVariable7);
			return;
		}

		public static void SetConfig<T>(string name, string culture, T value)
		{
			MixService.get_Instance().get_LocalSettings().get_Item(culture).set_Item(name, JToken.op_Implicit(value.ToString()));
			return;
		}

		public static void SetConnectionString(string name, string value)
		{
			MixService.get_Instance().get_ConnectionStrings().set_Item(name, JToken.op_Implicit(value));
			return;
		}

		public static void SetIpConfig<T>(string name, T value)
		{
			MixService.get_Instance().get_IpSecuritySettings().set_Item(name, JToken.op_Implicit(value.ToString()));
			return;
		}

		public static T Translate<T>(string name, string culture)
		{
			V_0 = MixService.get_Instance().get_Translator().get_Item(culture).get_Item(name);
			if (V_0 != null)
			{
				return Newtonsoft.Json.Linq.Extensions.Value<T>(V_0);
			}
			V_1 = default(T);
			return V_1;
		}

		public static string TranslateString(string name, string culture)
		{
			V_0 = MixService.get_Instance().get_Translator().get_Item(culture).get_Item(name);
			if (V_0 == null)
			{
				return name;
			}
			return Newtonsoft.Json.Linq.Extensions.Value<string>(V_0);
		}
	}
}