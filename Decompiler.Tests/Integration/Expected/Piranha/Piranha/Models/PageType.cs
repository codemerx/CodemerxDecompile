using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public sealed class PageType : ContentTypeBase
	{
		public IList<string> ArchiveItemTypes
		{
			get;
			set;
		}

		public bool IsArchive
		{
			get;
			set;
		}

		public bool UseBlocks
		{
			get;
			set;
		}

		public bool UseExcerpt
		{
			get;
			set;
		}

		public bool UsePrimaryImage
		{
			get;
			set;
		}

		public PageType()
		{
			this.u003cUseBlocksu003ek__BackingField = true;
			this.u003cUsePrimaryImageu003ek__BackingField = true;
			this.u003cUseExcerptu003ek__BackingField = true;
			this.u003cArchiveItemTypesu003ek__BackingField = new List<string>();
			base();
			return;
		}

		public void Ensure()
		{
			stackVariable1 = this.get_Regions();
			stackVariable2 = PageType.u003cu003ec.u003cu003e9__20_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Func<RegionType, string>(PageType.u003cu003ec.u003cu003e9.u003cEnsureu003eb__20_0);
				PageType.u003cu003ec.u003cu003e9__20_0 = stackVariable2;
			}
			if (stackVariable1.Select<RegionType, string>(stackVariable2).Distinct<string>().Count<string>() != this.get_Regions().get_Count())
			{
				throw new InvalidOperationException(String.Concat("Region Id not unique for page type ", this.get_Id()));
			}
			V_0 = this.get_Regions().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					stackVariable16 = V_1;
					stackVariable18 = V_1.get_Title();
					if (stackVariable18 == null)
					{
						dummyVar1 = stackVariable18;
						stackVariable18 = V_1.get_Id();
					}
					stackVariable16.set_Title(stackVariable18);
					stackVariable20 = V_1.get_Fields();
					stackVariable21 = PageType.u003cu003ec.u003cu003e9__20_1;
					if (stackVariable21 == null)
					{
						dummyVar2 = stackVariable21;
						stackVariable21 = new Func<FieldType, string>(PageType.u003cu003ec.u003cu003e9.u003cEnsureu003eb__20_1);
						PageType.u003cu003ec.u003cu003e9__20_1 = stackVariable21;
					}
					if (stackVariable20.Select<FieldType, string>(stackVariable21).Distinct<string>().Count<string>() != V_1.get_Fields().get_Count())
					{
						throw new InvalidOperationException(String.Concat("Field Id not unique for page type ", this.get_Id()));
					}
					V_2 = V_1.get_Fields().GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = V_2.get_Current();
							stackVariable35 = V_3;
							stackVariable37 = V_3.get_Id();
							if (stackVariable37 == null)
							{
								dummyVar3 = stackVariable37;
								stackVariable37 = "Default";
							}
							stackVariable35.set_Id(stackVariable37);
							stackVariable38 = V_3;
							stackVariable40 = V_3.get_Title();
							if (stackVariable40 == null)
							{
								dummyVar4 = stackVariable40;
								stackVariable40 = V_3.get_Id();
							}
							stackVariable38.set_Title(stackVariable40);
						}
					}
					finally
					{
						if (V_2 != null)
						{
							V_2.Dispose();
						}
					}
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			return;
		}
	}
}