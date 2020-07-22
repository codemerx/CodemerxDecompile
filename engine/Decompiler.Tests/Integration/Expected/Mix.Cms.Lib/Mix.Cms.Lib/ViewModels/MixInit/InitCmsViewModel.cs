using Mix.Cms.Lib;
using Mix.Cms.Lib.ViewModels;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixInit
{
	public class InitCmsViewModel
	{
		[JsonProperty("connectionString")]
		public string ConnectionString
		{
			get
			{
				switch (this.get_DatabaseProvider())
				{
					case 0:
					{
						if (!string.IsNullOrEmpty(this.get_DatabasePort()))
						{
							stackVariable11 = string.Concat(this.get_DatabaseServer(), ",", this.get_DatabasePort());
						}
						else
						{
							stackVariable11 = this.get_DatabaseServer();
						}
						V_1 = stackVariable11;
						if (this.get_IsUseLocal())
						{
							return this.get_LocalDbConnectionString();
						}
						stackVariable17 = new string[9];
						stackVariable17[0] = "Server=";
						stackVariable17[1] = V_1;
						stackVariable17[2] = ";Database=";
						stackVariable17[3] = this.get_DatabaseName();
						stackVariable17[4] = ";UID=";
						stackVariable17[5] = this.get_DatabaseUser();
						stackVariable17[6] = ";Pwd=";
						stackVariable17[7] = this.get_DatabasePassword();
						stackVariable17[8] = ";MultipleActiveResultSets=true;";
						return string.Concat(stackVariable17);
					}
					case 1:
					{
						stackVariable42 = new string[11];
						stackVariable42[0] = "Server=";
						stackVariable42[1] = this.get_DatabaseServer();
						stackVariable42[2] = ";port=";
						stackVariable42[3] = this.get_DatabasePort();
						stackVariable42[4] = ";Database=";
						stackVariable42[5] = this.get_DatabaseName();
						stackVariable42[6] = ";User=";
						stackVariable42[7] = this.get_DatabaseUser();
						stackVariable42[8] = ";Password=";
						stackVariable42[9] = this.get_DatabasePassword();
						stackVariable42[10] = ";";
						return string.Concat(stackVariable42);
					}
					case 2:
					{
						stackVariable72 = new string[10];
						stackVariable72[0] = "Host=";
						stackVariable72[1] = this.get_DatabaseServer();
						stackVariable72[2] = ";Port=";
						stackVariable72[3] = this.get_DatabasePort();
						stackVariable72[4] = ";Database=";
						stackVariable72[5] = this.get_DatabaseName();
						stackVariable72[6] = ";Username=";
						stackVariable72[7] = this.get_DatabaseUser();
						stackVariable72[8] = ";Password=";
						stackVariable72[9] = this.get_DatabasePassword();
						return string.Concat(stackVariable72);
					}
				}
				return string.Empty;
			}
		}

		[JsonProperty("culture")]
		public InitCulture Culture
		{
			get;
			set;
		}

		[JsonProperty("databaseName")]
		public string DatabaseName
		{
			get;
			set;
		}

		[JsonProperty("databasePassword")]
		public string DatabasePassword
		{
			get;
			set;
		}

		[JsonProperty("databasePort")]
		public string DatabasePort
		{
			get;
			set;
		}

		[JsonProperty("databaseProvider")]
		public MixEnums.DatabaseProvider DatabaseProvider
		{
			get;
			set;
		}

		[JsonProperty("databaseServer")]
		public string DatabaseServer
		{
			get;
			set;
		}

		[JsonProperty("databaseUser")]
		public string DatabaseUser
		{
			get;
			set;
		}

		[JsonProperty("isMysql")]
		public bool IsMysql
		{
			get
			{
				return this.get_DatabaseProvider() == 1;
			}
		}

		[JsonProperty("isUseLocal")]
		public bool IsUseLocal
		{
			get;
			set;
		}

		[JsonProperty("lang")]
		public string Lang
		{
			get;
			set;
		}

		[JsonProperty("localDbConnectionString")]
		public string LocalDbConnectionString
		{
			get;
			set;
		}

		[JsonProperty("siteName")]
		public string SiteName
		{
			get;
			set;
		}

		[JsonProperty("sqliteDbConnectionString")]
		public string SqliteDbConnectionString
		{
			get;
			set;
		}

		public InitCmsViewModel()
		{
			this.u003cLocalDbConnectionStringu003ek__BackingField = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=mix-cms.db;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True";
			this.u003cSqliteDbConnectionStringu003ek__BackingField = "Data Source=mix-cms.db";
			this.u003cSiteNameu003ek__BackingField = "MixCore";
			base();
			return;
		}
	}
}