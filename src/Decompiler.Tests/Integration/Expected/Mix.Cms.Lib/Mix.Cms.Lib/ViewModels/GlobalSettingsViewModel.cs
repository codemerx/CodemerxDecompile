using Mix.Domain.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels
{
	public class GlobalSettingsViewModel
	{
		[JsonProperty("apiEncryptIV")]
		public string ApiEncryptIV
		{
			get;
			set;
		}

		[JsonProperty("apiEncryptKey")]
		public string ApiEncryptKey
		{
			get;
			set;
		}

		[JsonProperty("attributeSetTypes")]
		public List<object> AttributeSetTypes
		{
			get;
			set;
		}

		[JsonProperty("cultures")]
		public List<SupportedCulture> Cultures
		{
			get;
			set;
		}

		[JsonProperty("dataTypes")]
		public List<object> DataTypes
		{
			get;
			set;
		}

		[JsonProperty("domain")]
		public string Domain
		{
			get;
			set;
		}

		[JsonProperty("isEncryptApi")]
		public bool IsEncryptApi
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

		[JsonProperty("langIcon")]
		public string LangIcon
		{
			get;
			set;
		}

		[JsonProperty("lastUpdateConfiguration")]
		public DateTime? LastUpdateConfiguration
		{
			get;
			set;
		}

		[JsonProperty("moduleTypes")]
		public List<object> ModuleTypes
		{
			get;
			set;
		}

		[JsonProperty("pageTypes")]
		public List<object> PageTypes
		{
			get;
			set;
		}

		[JsonProperty("portalThemeSettings")]
		public JObject PortalThemeSettings
		{
			get;
			set;
		}

		[JsonProperty("statuses")]
		public List<object> Statuses
		{
			get;
			set;
		}

		[JsonProperty("themeId")]
		public int ThemeId
		{
			get;
			set;
		}

		public GlobalSettingsViewModel()
		{
		}
	}
}