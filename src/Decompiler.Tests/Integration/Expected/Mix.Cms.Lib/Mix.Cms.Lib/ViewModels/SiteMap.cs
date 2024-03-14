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
		}

		public XElement ParseXElement()
		{
			XNamespace xNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
			XNamespace xNamespace1 = "http://www.w3.org/1999/xhtml";
			XElement xElement = new XElement(xNamespace + "url");
			xElement.Add(new XElement(xNamespace + "lastmod", (object)((this.LastMod.HasValue ? this.LastMod.Value : DateTime.UtcNow))));
			xElement.Add(new XElement(xNamespace + "changefreq", this.ChangeFreq));
			xElement.Add(new XElement(xNamespace + "priority", (object)this.Priority));
			xElement.Add(new XElement(xNamespace + "loc", this.Loc));
			foreach (SitemapLanguage otherLanguage in this.OtherLanguages)
			{
				xElement.Add(new XElement(xNamespace1 + "link", new object[] { new XAttribute(XNamespace.Xmlns + "xhtml", xNamespace1.NamespaceName), new XAttribute("rel", "alternate"), new XAttribute("hreflang", otherLanguage.HrefLang), new XAttribute("href", otherLanguage.Href) }));
			}
			return xElement;
		}
	}
}