using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Telerik.JustDecompiler.XmlDocumentationReaders
{
	internal class XmlDocumentationReader
	{
		public XmlDocumentationReader()
		{
		}

		public Dictionary<string, string> ReadDocumentation(string fileName)
		{
			Dictionary<string, string> strs;
			XmlReaderSettings xmlReaderSetting = new XmlReaderSettings()
			{
				IgnoreComments = true,
				IgnoreWhitespace = true,
				CloseInput = true
			};
			if (!File.Exists(fileName))
			{
				return new Dictionary<string, string>();
			}
			using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
			{
				Dictionary<string, string> strs1 = new Dictionary<string, string>();
				using (XmlTextReader xmlTextReader = new XmlTextReader(fileStream, XmlNodeType.Element, null))
				{
					try
					{
						while (xmlTextReader.Read())
						{
							if (xmlTextReader.NodeType != XmlNodeType.Element || !(xmlTextReader.Name == "member"))
							{
								continue;
							}
							string attribute = xmlTextReader.GetAttribute("name");
							strs1[attribute] = this.RemoveLeadingLineWhitespaces(xmlTextReader.ReadInnerXml());
						}
					}
					catch (XmlException xmlException)
					{
						strs = new Dictionary<string, string>();
						return strs;
					}
				}
				strs = strs1;
			}
			return strs;
		}

		private string RemoveLeadingLineWhitespaces(string text)
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<string> strs = new List<string>();
			int num = 0x7fffffff;
			using (StringReader stringReader = new StringReader(text))
			{
				for (string i = stringReader.ReadLine(); i != null; i = stringReader.ReadLine())
				{
					int num1 = 0;
					while (num1 < i.Length)
					{
						if (Char.IsWhiteSpace(i[num1]))
						{
							num1++;
						}
						else
						{
							num = Math.Min(num, num1);
							strs.Add(i);
							break;
						}
					}
				}
			}
			foreach (string str in strs)
			{
				stringBuilder.AppendLine(str.Substring(num));
			}
			return stringBuilder.ToString();
		}
	}
}