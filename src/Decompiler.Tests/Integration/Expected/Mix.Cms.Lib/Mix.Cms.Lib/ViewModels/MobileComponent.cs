using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Mix.Cms.Lib.ViewModels
{
	public class MobileComponent
	{
		[JsonProperty("componentType")]
		public string ComponentType
		{
			get;
			set;
		}

		[JsonProperty("dataSource")]
		public List<MobileComponent> DataSource
		{
			get;
			set;
		}

		[JsonProperty("dataType")]
		public string DataType
		{
			get;
			set;
		}

		[JsonProperty("dataValue")]
		public string DataValue
		{
			get;
			set;
		}

		[JsonProperty("id")]
		public int Id
		{
			get;
			set;
		}

		[JsonProperty("styleName")]
		public string StyleName
		{
			get;
			set;
		}

		public MobileComponent(XElement element)
		{
			string value;
			string str;
			string str1;
			if (element != null)
			{
				XAttribute xAttribute = element.Attribute("class");
				if (xAttribute != null)
				{
					value = xAttribute.Value;
				}
				else
				{
					value = null;
				}
				this.StyleName = value;
				this.DataSource = new List<MobileComponent>();
				IEnumerable<XElement> xElements = element.Elements();
				if (!xElements.Any<XElement>())
				{
					string localName = element.Name.LocalName;
					if (localName != null)
					{
						if (localName == "img")
						{
							this.ComponentType = "Image";
							this.DataType = "image_url";
							XAttribute xAttribute1 = element.Attribute("src");
							if (xAttribute1 != null)
							{
								str = xAttribute1.Value.Replace("Model.", "@Model.").Replace("{{", "").Replace("}}", "");
							}
							else
							{
								str = null;
							}
							this.DataValue = str;
							return;
						}
						if (localName == "br")
						{
							return;
						}
					}
					this.ComponentType = "Text";
					string str2 = element.Value.Trim();
					if (!str2.Contains("{{") || !str2.Contains("}}"))
					{
						this.DataType = "string";
					}
					else
					{
						this.DataType = "object";
					}
					this.DataValue = element.Value.Trim().Replace("Model.", "@Model.").Replace("{{", "").Replace("}}", "");
				}
				else
				{
					if (element.Attribute("data") == null)
					{
						this.ComponentType = "View";
						this.DataType = "component";
					}
					else
					{
						this.ComponentType = "View";
						XAttribute xAttribute2 = element.Attribute("data");
						if (xAttribute2 != null)
						{
							str1 = xAttribute2.Value.Replace("Model.", "@Model.").Replace("{{", "").Replace("}}", "");
						}
						else
						{
							str1 = null;
						}
						this.DataValue = str1;
						this.DataType = "object_array";
					}
					foreach (XElement xElement in xElements)
					{
						if (xElement.Name == "br")
						{
							continue;
						}
						this.DataSource.Add(new MobileComponent(xElement));
					}
				}
			}
		}
	}
}