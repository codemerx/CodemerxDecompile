using Newtonsoft.Json;
using Piranha.Extend.Fields;
using Piranha.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Piranha
{
	public static class Utils
	{
		public static T DeepClone<T>(T obj)
		{
			if (obj == null)
			{
				return default(T);
			}
			if ((object)obj is ValueType)
			{
				return obj;
			}
			JsonSerializerSettings jsonSerializerSetting = new JsonSerializerSettings();
			jsonSerializerSetting.set_TypeNameHandling(3);
			JsonSerializerSettings jsonSerializerSetting1 = jsonSerializerSetting;
			return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj, jsonSerializerSetting1), jsonSerializerSetting1);
		}

		public static string FirstParagraph(string str)
		{
			MatchCollection matchCollections = (new Regex("<p[^>]*>.*?</p>")).Matches(str);
			if (matchCollections.Count <= 0)
			{
				return "";
			}
			return matchCollections[0].Value;
		}

		public static string FirstParagraph(MarkdownField md)
		{
			MatchCollection matchCollections = (new Regex("<p[^>]*>.*?</p>")).Matches(md.ToHtml());
			if (matchCollections.Count <= 0)
			{
				return "";
			}
			return matchCollections[0].Value;
		}

		public static string FirstParagraph(HtmlField html)
		{
			MatchCollection matchCollections = (new Regex("<p[^>]*>.*?</p>")).Matches(html.Value);
			if (matchCollections.Count <= 0)
			{
				return "";
			}
			return matchCollections[0].Value;
		}

		public static string FormatByteSize(double bytes)
		{
			string[] strArray = new String[] { "bytes", "KB", "MB", "GB" };
			int num = 0;
			if (bytes > 1023)
			{
				do
				{
					bytes /= 1024;
					num++;
				}
				while (bytes >= 1024 && num < 3);
			}
			return String.Format("{0:0.00} {1}", bytes, strArray[num]);
		}

		public static string GenerateETag(string name, DateTime date)
		{
			string str;
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			using (MD5 mD5 = MD5.Create())
			{
				string str1 = String.Concat(name, date.ToString("yyyy-MM-dd HH:mm:ss"));
				byte[] numArray = mD5.ComputeHash(uTF8Encoding.GetBytes(str1));
				str = String.Concat("\"", Convert.ToBase64String(numArray), "\"");
			}
			return str;
		}

		public static string GenerateGravatarUrl(string email, int size = 0)
		{
			string str;
			using (MD5 mD5 = MD5.Create())
			{
				byte[] numArray = mD5.ComputeHash(Encoding.UTF8.GetBytes(email));
				StringBuilder stringBuilder = new StringBuilder((int)numArray.Length * 2);
				for (int i = 0; i < (int)numArray.Length; i++)
				{
					stringBuilder.Append(numArray[i].ToString("X2"));
				}
				str = String.Concat("https://www.gravatar.com/avatar/", stringBuilder.ToString().ToLower(), (size > 0 ? String.Concat("?s=", size.ToString()) : ""));
			}
			return str;
		}

		public static string GenerateInteralId(string str)
		{
			return (new CultureInfo("en-US", false)).TextInfo.ToTitleCase(Piranha.Utils.GenerateSlug(str, true).Replace('-', ' ')).Replace(" ", "");
		}

		public static string GenerateSlug(string str, bool hierarchical = true)
		{
			if (App.Hooks != null && App.Hooks.OnGenerateSlug != null)
			{
				return App.Hooks.OnGenerateSlug(str);
			}
			string lower = str.Trim().ToLower();
			lower = lower.Replace("å", "a").Replace("ä", "a").Replace("á", "a").Replace("à", "a").Replace("ö", "o").Replace("ó", "o").Replace("ò", "o").Replace("é", "e").Replace("è", "e").Replace("í", "i").Replace("ì", "i");
			lower = Regex.Replace(lower, "[^a-z0-9-/ ]", "").Replace("--", "-");
			lower = Regex.Replace(lower.Replace("-", " "), "\\s+", " ").Replace(" ", "-");
			if (!hierarchical)
			{
				lower = lower.Replace("/", "-");
			}
			lower = Regex.Replace(lower, "[-]+", "-");
			if (lower.EndsWith("-"))
			{
				lower = lower.Substring(0, lower.LastIndexOf("-"));
			}
			if (lower.StartsWith("-"))
			{
				lower = lower.Substring(Math.Min(lower.IndexOf("-") + 1, lower.Length));
			}
			return lower;
		}

		public static string GetAssemblyVersion(Assembly assembly)
		{
			Version version = assembly.GetName().Version;
			return String.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
		}

		public static object GetPropertyValue(this Type type, string propertyName, object instance)
		{
			PropertyInfo property = type.GetProperty(propertyName, App.PropertyBindings);
			if (property == null)
			{
				return null;
			}
			return property.GetValue(instance);
		}

		public static bool IsPreRelease(Assembly assembly)
		{
			string lower = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.ToLower();
			if (lower.Contains("alpha"))
			{
				return true;
			}
			return lower.Contains("beta");
		}

		public static void SetPropertyValue(this Type type, string propertyName, object instance, object value)
		{
			PropertyInfo property = type.GetProperty(propertyName, App.PropertyBindings);
			if (property != null)
			{
				property.SetValue(instance, value);
			}
		}

		public static T[] Subset<T>(this T[] arr, int startpos = 0, int length = 0)
		{
			List<T> ts = new List<T>();
			length = (length > 0 ? length : (int)arr.Length - startpos);
			for (int i = 0; i < (int)arr.Length; i++)
			{
				if (i >= startpos && i < startpos + length)
				{
					ts.Add(arr[i]);
				}
			}
			return ts.ToArray();
		}
	}
}