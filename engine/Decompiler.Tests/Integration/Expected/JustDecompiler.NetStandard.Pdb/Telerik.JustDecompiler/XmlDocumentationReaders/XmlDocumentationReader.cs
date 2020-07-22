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
			base();
			return;
		}

		public Dictionary<string, string> ReadDocumentation(string fileName)
		{
			stackVariable0 = new XmlReaderSettings();
			stackVariable0.set_IgnoreComments(true);
			stackVariable0.set_IgnoreWhitespace(true);
			stackVariable0.set_CloseInput(true);
			if (!File.Exists(fileName))
			{
				return new Dictionary<string, string>();
			}
			V_0 = new FileStream(fileName, 3, 1, 5);
			try
			{
				V_1 = new Dictionary<string, string>();
				V_2 = new XmlTextReader(V_0, 1, null);
				try
				{
					try
					{
						while (V_2.Read())
						{
							if (V_2.get_NodeType() != 1 || !String.op_Equality(V_2.get_Name(), "member"))
							{
								continue;
							}
							V_3 = V_2.GetAttribute("name");
							V_1.set_Item(V_3, this.RemoveLeadingLineWhitespaces(V_2.ReadInnerXml()));
						}
					}
					catch (XmlException exception_0)
					{
						dummyVar0 = exception_0;
						V_5 = new Dictionary<string, string>();
						goto Label0;
					}
				}
				finally
				{
					if (V_2 != null)
					{
						((IDisposable)V_2).Dispose();
					}
				}
				V_5 = V_1;
			}
			finally
			{
				if (V_0 != null)
				{
					((IDisposable)V_0).Dispose();
				}
			}
		Label0:
			return V_5;
		}

		private string RemoveLeadingLineWhitespaces(string text)
		{
			V_0 = new StringBuilder();
			V_1 = new List<string>();
			V_2 = 0x7fffffff;
			V_3 = new StringReader(text);
			try
			{
				V_4 = V_3.ReadLine();
				while (V_4 != null)
				{
					V_5 = 0;
					while (V_5 < V_4.get_Length())
					{
						if (Char.IsWhiteSpace(V_4.get_Chars(V_5)))
						{
							V_5 = V_5 + 1;
						}
						else
						{
							V_2 = Math.Min(V_2, V_5);
							V_1.Add(V_4);
							break;
						}
					}
					V_4 = V_3.ReadLine();
				}
			}
			finally
			{
				if (V_3 != null)
				{
					((IDisposable)V_3).Dispose();
				}
			}
			V_6 = V_1.GetEnumerator();
			try
			{
				while (V_6.MoveNext())
				{
					V_7 = V_6.get_Current();
					dummyVar0 = V_0.AppendLine(V_7.Substring(V_2));
				}
			}
			finally
			{
				((IDisposable)V_6).Dispose();
			}
			return V_0.ToString();
		}
	}
}