using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Squidex.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Squidex.Pipeline.Squid
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class SquidMiddleware
	{
		private readonly string squidHappyLG = SquidMiddleware.LoadSvg("happy");

		private readonly string squidHappySM = SquidMiddleware.LoadSvg("happy-sm");

		private readonly string squidSadLG = SquidMiddleware.LoadSvg("sad");

		private readonly string squidSadSM = SquidMiddleware.LoadSvg("sad-sm");

		public SquidMiddleware(RequestDelegate next)
		{
		}

		public async Task InvokeAsync(HttpContext context)
		{
			StringValues stringValue = new StringValues();
			StringValues stringValue1 = new StringValues();
			StringValues stringValue2 = new StringValues();
			StringValues stringValue3 = new StringValues();
			string str;
			StringValues stringValue4 = new StringValues();
			HttpRequest request = context.get_Request();
			string str1 = "sad";
			if (request.get_Query().TryGetValue("face", ref stringValue) && (stringValue == "sad" || stringValue == "happy"))
			{
				str1 = stringValue;
			}
			bool flag = str1 == "sad";
			string str2 = (flag ? "OH DAMN!" : "OH YEAH!");
			if (request.get_Query().TryGetValue("title", ref stringValue1) && !string.IsNullOrWhiteSpace(stringValue1))
			{
				str2 = stringValue1;
			}
			string str3 = "text";
			if (request.get_Query().TryGetValue("text", ref stringValue2) && !string.IsNullOrWhiteSpace(stringValue2))
			{
				str3 = stringValue2;
			}
			string str4 = (flag ? "#F5F5F9" : "#4CC159");
			if (request.get_Query().TryGetValue("background", ref stringValue3) && !string.IsNullOrWhiteSpace(stringValue3))
			{
				str4 = stringValue3;
			}
			if (!request.get_Query().TryGetValue("small", ref stringValue4))
			{
				str = (flag ? this.squidSadLG : this.squidHappyLG);
			}
			else
			{
				str = (flag ? this.squidSadSM : this.squidHappySM);
			}
			ValueTuple<string, string, string> valueTuple = SquidMiddleware.SplitText(str3);
			string item1 = valueTuple.Item1;
			string item2 = valueTuple.Item2;
			string item3 = valueTuple.Item3;
			str = str.Replace("{{TITLE}}", str2.ToUpperInvariant(), StringComparison.Ordinal);
			str = str.Replace("{{TEXT1}}", item1, StringComparison.Ordinal);
			str = str.Replace("{{TEXT2}}", item2, StringComparison.Ordinal);
			str = str.Replace("{{TEXT3}}", item3, StringComparison.Ordinal);
			str = str.Replace("[COLOR]", str4, StringComparison.Ordinal);
			context.get_Response().set_StatusCode(200);
			context.get_Response().set_ContentType("image/svg+xml");
			context.get_Response().get_Headers().set_Item("Cache-Control", "public, max-age=604800");
			await HttpResponseWritingExtensions.WriteAsync(context.get_Response(), str, context.get_RequestAborted());
		}

		private static string LoadSvg(string name)
		{
			string end;
			Assembly assembly = typeof(SquidMiddleware).Assembly;
			using (Stream manifestResourceStream = assembly.GetManifestResourceStream(string.Concat("Squidex.Pipeline.Squid.icon-", name, ".svg")))
			{
				using (StreamReader streamReader = new StreamReader(manifestResourceStream))
				{
					end = streamReader.ReadToEnd();
				}
			}
			return end;
		}

		[return: Nullable(new byte[] { 0, 1, 1, 1 })]
		private static ValueTuple<string, string, string> SplitText(string text)
		{
			List<string> strs = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			string[] strArrays = text.Split(' ', StringSplitOptions.None);
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				if (stringBuilder.Length + str.Length > 16 && stringBuilder.Length > 0)
				{
					strs.Add(stringBuilder.ToString());
					stringBuilder.Clear();
				}
				StringExtensions.AppendIfNotEmpty(stringBuilder, ' ');
				stringBuilder.Append(str);
			}
			strs.Add(stringBuilder.ToString());
			while (strs.Count < 3)
			{
				strs.Add(string.Empty);
			}
			return new ValueTuple<string, string, string>(strs[0], strs[1], strs[2]);
		}
	}
}