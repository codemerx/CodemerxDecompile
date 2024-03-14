using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;

namespace Squidex.Areas.Api.Controllers.UI
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class MyUIOptions : IEquatable<MyUIOptions>
	{
		[JsonPropertyName("disableScheduledChanges")]
		public bool DisableScheduledChanges
		{
			get;
			set;
		}

		[CompilerGenerated]
		private Type EqualityContract
		{
			get
			{
				return typeof(MyUIOptions);
			}
		}

		[JsonPropertyName("hideDateButtons")]
		public bool HideDateButtons
		{
			get;
			set;
		}

		[JsonPropertyName("hideDateTimeModeButton")]
		public bool HideDateTimeModeButton
		{
			get;
			set;
		}

		[JsonPropertyName("hideNews")]
		public bool HideNews
		{
			get;
			set;
		}

		[JsonPropertyName("hideOnboarding")]
		public bool HideOnboarding
		{
			get;
			set;
		}

		[JsonPropertyName("map")]
		public MyUIOptions.MapOptions Map
		{
			get;
			set;
		}

		[JsonExtensionData]
		public Dictionary<string, object> More
		{
			get;
			set;
		}

		[JsonPropertyName("onlyAdminsCanCreateApps")]
		public bool OnlyAdminsCanCreateApps
		{
			get;
			set;
		}

		[JsonPropertyName("onlyAdminsCanCreateTeams")]
		public bool OnlyAdminsCanCreateTeams
		{
			get;
			set;
		}

		[JsonPropertyName("redirectToLogin")]
		public bool RedirectToLogin
		{
			get;
			set;
		}

		[JsonPropertyName("referencesDropdownItemCount")]
		public int ReferencesDropdownItemCount
		{
			get;
			set;
		}

		[JsonPropertyName("regexSuggestions")]
		public Dictionary<string, string> RegexSuggestions
		{
			get;
			set;
		}

		[JsonPropertyName("showInfo")]
		public bool ShowInfo
		{
			get;
			set;
		}

		public MyUIOptions()
		{
			this.More = new Dictionary<string, object>();
			this.ReferencesDropdownItemCount = 100;
			base();
		}

		[Nullable(0)]
		public sealed class MapGoogleOptions
		{
			[JsonPropertyName("key")]
			public string Key
			{
				get;
				set;
			}

			public MapGoogleOptions()
			{
			}
		}

		[Nullable(0)]
		public sealed class MapOptions
		{
			[JsonPropertyName("googleMaps")]
			public MyUIOptions.MapGoogleOptions GoogleMaps
			{
				get;
				set;
			}

			[JsonPropertyName("type")]
			public string Type
			{
				get;
				set;
			}

			public MapOptions()
			{
			}
		}
	}
}