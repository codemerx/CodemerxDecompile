using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
			base();
			if (element != null)
			{
				stackVariable6 = element.Attribute(XName.op_Implicit("class"));
				if (stackVariable6 != null)
				{
					stackVariable7 = stackVariable6.get_Value();
				}
				else
				{
					dummyVar0 = stackVariable6;
					stackVariable7 = null;
				}
				this.set_StyleName(stackVariable7);
				this.set_DataSource(new List<MobileComponent>());
				V_0 = element.Elements();
				if (!V_0.Any<XElement>())
				{
					V_4 = element.get_Name().get_LocalName();
					if (V_4 != null)
					{
						if (string.op_Equality(V_4, "img"))
						{
							this.set_ComponentType("Image");
							this.set_DataType("image_url");
							stackVariable57 = element.Attribute(XName.op_Implicit("src"));
							if (stackVariable57 != null)
							{
								stackVariable67 = stackVariable57.get_Value().Replace("Model.", "@Model.").Replace("{{", "").Replace("}}", "");
							}
							else
							{
								dummyVar2 = stackVariable57;
								stackVariable67 = null;
							}
							this.set_DataValue(stackVariable67);
							return;
						}
						if (string.op_Equality(V_4, "br"))
						{
							goto Label0;
						}
					}
					this.set_ComponentType("Text");
					V_3 = element.get_Value().Trim();
					if (!V_3.Contains("{{") || !V_3.Contains("}}"))
					{
						this.set_DataType("string");
					}
					else
					{
						this.set_DataType("object");
					}
					this.set_DataValue(element.get_Value().Trim().Replace("Model.", "@Model.").Replace("{{", "").Replace("}}", ""));
				}
				else
				{
					if (element.Attribute(XName.op_Implicit("data")) == null)
					{
						this.set_ComponentType("View");
						this.set_DataType("component");
					}
					else
					{
						this.set_ComponentType("View");
						stackVariable100 = element.Attribute(XName.op_Implicit("data"));
						if (stackVariable100 != null)
						{
							stackVariable110 = stackVariable100.get_Value().Replace("Model.", "@Model.").Replace("{{", "").Replace("}}", "");
						}
						else
						{
							dummyVar1 = stackVariable100;
							stackVariable110 = null;
						}
						this.set_DataValue(stackVariable110);
						this.set_DataType("object_array");
					}
					V_1 = V_0.GetEnumerator();
					try
					{
						while (V_1.MoveNext())
						{
							V_2 = V_1.get_Current();
							if (!XName.op_Inequality(V_2.get_Name(), XName.op_Implicit("br")))
							{
								continue;
							}
							this.get_DataSource().Add(new MobileComponent(V_2));
						}
					}
					finally
					{
						if (V_1 != null)
						{
							V_1.Dispose();
						}
					}
				}
			}
		Label0:
			return;
		}
	}
}