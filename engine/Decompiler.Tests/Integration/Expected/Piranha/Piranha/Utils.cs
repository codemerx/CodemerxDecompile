using Newtonsoft.Json;
using Piranha.Extend.Fields;
using System;
using System.Collections.Generic;
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
				V_1 = default(T);
				return V_1;
			}
			if ((object)obj as ValueType != null)
			{
				return obj;
			}
			stackVariable5 = new JsonSerializerSettings();
			stackVariable5.set_TypeNameHandling(3);
			V_0 = stackVariable5;
			return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj, V_0), V_0);
		}

		public static string FirstParagraph(string str)
		{
			V_0 = (new Regex("<p[^>]*>.*?</p>")).Matches(str);
			if (V_0.get_Count() <= 0)
			{
				return "";
			}
			return V_0.get_Item(0).get_Value();
		}

		public static string FirstParagraph(MarkdownField md)
		{
			V_0 = (new Regex("<p[^>]*>.*?</p>")).Matches(md.ToHtml());
			if (V_0.get_Count() <= 0)
			{
				return "";
			}
			return V_0.get_Item(0).get_Value();
		}

		public static string FirstParagraph(HtmlField html)
		{
			V_0 = (new Regex("<p[^>]*>.*?</p>")).Matches(html.get_Value());
			if (V_0.get_Count() <= 0)
			{
				return "";
			}
			return V_0.get_Item(0).get_Value();
		}

		public static string FormatByteSize(double bytes)
		{
			stackVariable1 = new String[4];
			stackVariable1[0] = "bytes";
			stackVariable1[1] = "KB";
			stackVariable1[2] = "MB";
			stackVariable1[3] = "GB";
			V_0 = stackVariable1;
			V_1 = 0;
			if (bytes > 1023)
			{
				do
				{
					bytes = bytes / 1024;
					V_1 = V_1 + 1;
				}
				while (bytes >= 1024 && V_1 < 3);
			}
			return String.Format("{0:0.00} {1}", bytes, V_0[V_1]);
		}

		public static string GenerateETag(string name, DateTime date)
		{
			V_0 = new UTF8Encoding();
			V_1 = MD5.Create();
			try
			{
				V_2 = String.Concat(name, date.ToString("yyyy-MM-dd HH:mm:ss"));
				V_3 = V_1.ComputeHash(V_0.GetBytes(V_2));
				V_4 = String.Concat("\"", Convert.ToBase64String(V_3), "\"");
			}
			finally
			{
				if (V_1 != null)
				{
					((IDisposable)V_1).Dispose();
				}
			}
			return V_4;
		}

		public static string GenerateGravatarUrl(string email, int size = 0)
		{
			V_0 = MD5.Create();
			try
			{
				V_1 = V_0.ComputeHash(Encoding.get_UTF8().GetBytes(email));
				V_2 = new StringBuilder((int)V_1.Length * 2);
				V_3 = 0;
				while (V_3 < (int)V_1.Length)
				{
					dummyVar0 = V_2.Append(V_1[V_3].ToString("X2"));
					V_3 = V_3 + 1;
				}
				stackVariable30 = V_2.ToString().ToLower();
				if (size > 0)
				{
					stackVariable36 = String.Concat("?s=", size.ToString());
				}
				else
				{
					stackVariable36 = "";
				}
				V_4 = String.Concat("https://www.gravatar.com/avatar/", stackVariable30, stackVariable36);
			}
			finally
			{
				if (V_0 != null)
				{
					((IDisposable)V_0).Dispose();
				}
			}
			return V_4;
		}

		public static string GenerateInteralId(string str)
		{
			return (new CultureInfo("en-US", false)).get_TextInfo().ToTitleCase(Piranha.Utils.GenerateSlug(str, true).Replace('-', ' ')).Replace(" ", "");
		}

		public static string GenerateSlug(string str, bool hierarchical = true)
		{
			if (App.get_Hooks() != null && App.get_Hooks().get_OnGenerateSlug() != null)
			{
				return App.get_Hooks().get_OnGenerateSlug().Invoke(str);
			}
			V_0 = str.Trim().ToLower();
			V_0 = V_0.Replace("å", "a").Replace("ä", "a").Replace("á", "a").Replace("à", "a").Replace("ö", "o").Replace("ó", "o").Replace("ò", "o").Replace("é", "e").Replace("è", "e").Replace("í", "i").Replace("ì", "i");
			V_0 = Regex.Replace(V_0, "[^a-z0-9-/ ]", "").Replace("--", "-");
			V_0 = Regex.Replace(V_0.Replace("-", " "), "\\s+", " ").Replace(" ", "-");
			if (!hierarchical)
			{
				V_0 = V_0.Replace("/", "-");
			}
			V_0 = Regex.Replace(V_0, "[-]+", "-");
			if (V_0.EndsWith("-"))
			{
				V_0 = V_0.Substring(0, V_0.LastIndexOf("-"));
			}
			if (V_0.StartsWith("-"))
			{
				V_0 = V_0.Substring(Math.Min(V_0.IndexOf("-") + 1, V_0.get_Length()));
			}
			return V_0;
		}

		public static string GetAssemblyVersion(Assembly assembly)
		{
			V_0 = assembly.GetName().get_Version();
			return String.Format("{0}.{1}.{2}", V_0.get_Major(), V_0.get_Minor(), V_0.get_Build());
		}

		public static object GetPropertyValue(this Type type, string propertyName, object instance)
		{
			V_0 = type.GetProperty(propertyName, App.get_PropertyBindings());
			if (!PropertyInfo.op_Inequality(V_0, null))
			{
				return null;
			}
			return V_0.GetValue(instance);
		}

		public static bool IsPreRelease(Assembly assembly)
		{
			V_0 = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().get_InformationalVersion().ToLower();
			if (V_0.Contains("alpha"))
			{
				return true;
			}
			return V_0.Contains("beta");
		}

		public static void SetPropertyValue(this Type type, string propertyName, object instance, object value)
		{
			V_0 = type.GetProperty(propertyName, App.get_PropertyBindings());
			if (PropertyInfo.op_Inequality(V_0, null))
			{
				V_0.SetValue(instance, value);
			}
			return;
		}

		public static T[] Subset<T>(this T[] arr, int startpos = 0, int length = 0)
		{
			V_0 = new List<T>();
			if (length > 0)
			{
				stackVariable3 = length;
			}
			else
			{
				stackVariable3 = (int)arr.Length - startpos;
			}
			length = stackVariable3;
			V_1 = 0;
			while (V_1 < (int)arr.Length)
			{
				if (V_1 >= startpos && V_1 < startpos + length)
				{
					V_0.Add(arr[V_1]);
				}
				V_1 = V_1 + 1;
			}
			return V_0.ToArray();
		}
	}
}