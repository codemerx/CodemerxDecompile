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
				switch (this.DatabaseProvider)
				{
					case MixEnums.DatabaseProvider.MSSQL:
					{
						string str = (!string.IsNullOrEmpty(this.DatabasePort) ? string.Concat(this.DatabaseServer, ",", this.DatabasePort) : this.DatabaseServer);
						if (this.IsUseLocal)
						{
							return this.LocalDbConnectionString;
						}
						return string.Concat(new string[] { "Server=", str, ";Database=", this.DatabaseName, ";UID=", this.DatabaseUser, ";Pwd=", this.DatabasePassword, ";MultipleActiveResultSets=true;" });
					}
					case MixEnums.DatabaseProvider.MySQL:
					{
						return string.Concat(new string[] { "Server=", this.DatabaseServer, ";port=", this.DatabasePort, ";Database=", this.DatabaseName, ";User=", this.DatabaseUser, ";Password=", this.DatabasePassword, ";" });
					}
					case MixEnums.DatabaseProvider.PostgreSQL:
					{
						return string.Concat(new string[] { "Host=", this.DatabaseServer, ";Port=", this.DatabasePort, ";Database=", this.DatabaseName, ";Username=", this.DatabaseUser, ";Password=", this.DatabasePassword });
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
				return this.DatabaseProvider == MixEnums.DatabaseProvider.MySQL;
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
		public string LocalDbConnectionString { get; set; } = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=mix-cms.db;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True";

		[JsonProperty("siteName")]
		public string SiteName { get; set; } = "MixCore";

		[JsonProperty("sqliteDbConnectionString")]
		public string SqliteDbConnectionString { get; set; } = "Data Source=mix-cms.db";

		public InitCmsViewModel()
		{
		}
	}
}