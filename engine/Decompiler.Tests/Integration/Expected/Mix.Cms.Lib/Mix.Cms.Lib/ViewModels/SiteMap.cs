using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Mix.Cms.Lib.ViewModels
{
	public class SiteMap
	{
		public string ChangeFreq
		{
			get;
			set;
		}

		public DateTime? LastMod
		{
			get;
			set;
		}

		public string Loc
		{
			get;
			set;
		}

		public List<SitemapLanguage> OtherLanguages
		{
			get;
			set;
		}

		public double Priority
		{
			get;
			set;
		}

		public SiteMap()
		{
			base();
			return;
		}

		public XElement ParseXElement()
		{
			dummyVar0 = XNamespace.op_Implicit("http://www.w3.org/1999/xhtml");
			V_0 = XNamespace.op_Implicit("http://www.sitemaps.org/schemas/sitemap/0.9");
			V_1 = XNamespace.op_Implicit("http://www.w3.org/1999/xhtml");
			V_2 = new XElement(XNamespace.op_Addition(V_0, "url"));
			stackVariable10 = V_2;
			stackVariable13 = XNamespace.op_Addition(V_0, "lastmod");
			if (this.get_LastMod().get_HasValue())
			{
				stackVariable21 = this.get_LastMod().get_Value();
			}
			else
			{
				stackVariable21 = DateTime.get_UtcNow();
			}
			stackVariable10.Add(new XElement(stackVariable13, (object)stackVariable21));
			V_2.Add(new XElement(XNamespace.op_Addition(V_0, "changefreq"), this.get_ChangeFreq()));
			V_2.Add(new XElement(XNamespace.op_Addition(V_0, "priority"), (object)this.get_Priority()));
			V_2.Add(new XElement(XNamespace.op_Addition(V_0, "loc"), this.get_Loc()));
			V_4 = this.get_OtherLanguages().GetEnumerator();
			try
			{
				while (V_4.MoveNext())
				{
					V_5 = V_4.get_Current();
					stackVariable56 = XNamespace.op_Addition(V_1, "link");
					stackVariable58 = new object[4];
					stackVariable58[0] = new XAttribute(XNamespace.op_Addition(XNamespace.get_Xmlns(), "xhtml"), V_1.get_NamespaceName());
					stackVariable58[1] = new XAttribute(XName.op_Implicit("rel"), "alternate");
					stackVariable58[2] = new XAttribute(XName.op_Implicit("hreflang"), V_5.get_HrefLang());
					stackVariable58[3] = new XAttribute(XName.op_Implicit("href"), V_5.get_Href());
					V_2.Add(new XElement(stackVariable56, stackVariable58));
				}
			}
			finally
			{
				((IDisposable)V_4).Dispose();
			}
			return V_2;
		}
	}
}