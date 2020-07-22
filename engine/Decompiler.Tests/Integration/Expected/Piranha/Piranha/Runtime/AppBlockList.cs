using Piranha.Extend;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Piranha.Runtime
{
	public sealed class AppBlockList : AppDataList<Block, AppBlock>
	{
		public AppBlockList()
		{
			base();
			return;
		}

		public IEnumerable<AppBlock> GetByCategory(string category, bool includeGroups = true)
		{
			V_0 = new AppBlockList.u003cu003ec__DisplayClass1_0();
			V_0.category = category;
			V_1 = this._items.Where<AppBlock>(new Func<AppBlock, bool>(V_0.u003cGetByCategoryu003eb__0));
			if (!includeGroups)
			{
				stackVariable12 = V_1;
				stackVariable13 = AppBlockList.u003cu003ec.u003cu003e9__1_1;
				if (stackVariable13 == null)
				{
					dummyVar0 = stackVariable13;
					stackVariable13 = new Func<AppBlock, bool>(AppBlockList.u003cu003ec.u003cu003e9.u003cGetByCategoryu003eb__1_1);
					AppBlockList.u003cu003ec.u003cu003e9__1_1 = stackVariable13;
				}
				V_1 = stackVariable12.Where<AppBlock>(stackVariable13);
			}
			return V_1.ToArray<AppBlock>();
		}

		public IEnumerable<string> GetCategories()
		{
			stackVariable1 = this._items;
			stackVariable2 = AppBlockList.u003cu003ec.u003cu003e9__0_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Func<AppBlock, string>(AppBlockList.u003cu003ec.u003cu003e9.u003cGetCategoriesu003eb__0_0);
				AppBlockList.u003cu003ec.u003cu003e9__0_0 = stackVariable2;
			}
			stackVariable4 = stackVariable1.Select<AppBlock, string>(stackVariable2).Distinct<string>();
			stackVariable5 = AppBlockList.u003cu003ec.u003cu003e9__0_1;
			if (stackVariable5 == null)
			{
				dummyVar1 = stackVariable5;
				stackVariable5 = new Func<string, string>(AppBlockList.u003cu003ec.u003cu003e9.u003cGetCategoriesu003eb__0_1);
				AppBlockList.u003cu003ec.u003cu003e9__0_1 = stackVariable5;
			}
			return stackVariable4.OrderBy<string, string>(stackVariable5).ToArray<string>();
		}

		protected override AppBlock OnRegister<TValue>(AppBlock item)
		where TValue : Block
		{
			V_0 = Type.GetTypeFromHandle(// 
			// Current member / type: Piranha.Runtime.AppBlock Piranha.Runtime.AppBlockList::OnRegister(Piranha.Runtime.AppBlock)
			// Exception in: Piranha.Runtime.AppBlock OnRegister(Piranha.Runtime.AppBlock)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

	}
}