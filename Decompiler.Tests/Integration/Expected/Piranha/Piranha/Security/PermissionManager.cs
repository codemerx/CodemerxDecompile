using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Piranha.Security
{
	public class PermissionManager
	{
		private readonly Dictionary<string, IList<PermissionItem>> _modules;

		public IList<PermissionItem> this[string module]
		{
			get
			{
				if (!this._modules.TryGetValue(module, out V_0))
				{
					stackVariable7 = this._modules;
					stackVariable9 = new List<PermissionItem>();
					V_0 = stackVariable9;
					stackVariable7.set_Item(module, stackVariable9);
				}
				return V_0;
			}
		}

		public PermissionManager()
		{
			this._modules = new Dictionary<string, IList<PermissionItem>>();
			base();
			return;
		}

		public IList<string> GetModules()
		{
			stackVariable2 = this._modules.get_Keys();
			stackVariable3 = PermissionManager.u003cu003ec.u003cu003e9__3_0;
			if (stackVariable3 == null)
			{
				dummyVar0 = stackVariable3;
				stackVariable3 = new Func<string, string>(PermissionManager.u003cu003ec.u003cu003e9.u003cGetModulesu003eb__3_0);
				PermissionManager.u003cu003ec.u003cu003e9__3_0 = stackVariable3;
			}
			return stackVariable2.OrderBy<string, string>(stackVariable3).ToList<string>();
		}

		public IList<PermissionItem> GetPermissions(string module)
		{
			stackVariable2 = this.get_Item(module);
			stackVariable3 = PermissionManager.u003cu003ec.u003cu003e9__4_0;
			if (stackVariable3 == null)
			{
				dummyVar0 = stackVariable3;
				stackVariable3 = new Func<PermissionItem, string>(PermissionManager.u003cu003ec.u003cu003e9.u003cGetPermissionsu003eb__4_0);
				PermissionManager.u003cu003ec.u003cu003e9__4_0 = stackVariable3;
			}
			return stackVariable2.OrderBy<PermissionItem, string>(stackVariable3).ToList<PermissionItem>();
		}

		public IList<PermissionItem> GetPermissions()
		{
			V_0 = new Dictionary<string, PermissionItem>();
			V_1 = this.GetModules().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = this.GetPermissions(V_2).GetEnumerator();
					try
					{
						while (V_3.MoveNext())
						{
							V_4 = V_3.get_Current();
							V_0.set_Item(V_4.get_Name(), V_4);
						}
					}
					finally
					{
						if (V_3 != null)
						{
							V_3.Dispose();
						}
					}
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			stackVariable21 = V_0.get_Values();
			stackVariable22 = PermissionManager.u003cu003ec.u003cu003e9__5_0;
			if (stackVariable22 == null)
			{
				dummyVar0 = stackVariable22;
				stackVariable22 = new Func<PermissionItem, string>(PermissionManager.u003cu003ec.u003cu003e9.u003cGetPermissionsu003eb__5_0);
				PermissionManager.u003cu003ec.u003cu003e9__5_0 = stackVariable22;
			}
			return stackVariable21.OrderBy<PermissionItem, string>(stackVariable22).ToList<PermissionItem>();
		}

		public IList<PermissionItem> GetPublicPermissions()
		{
			V_0 = new Dictionary<string, PermissionItem>();
			V_1 = this.GetModules().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					stackVariable10 = this.GetPermissions(V_2);
					stackVariable11 = PermissionManager.u003cu003ec.u003cu003e9__6_1;
					if (stackVariable11 == null)
					{
						dummyVar0 = stackVariable11;
						stackVariable11 = new Func<PermissionItem, bool>(PermissionManager.u003cu003ec.u003cu003e9.u003cGetPublicPermissionsu003eb__6_1);
						PermissionManager.u003cu003ec.u003cu003e9__6_1 = stackVariable11;
					}
					V_3 = stackVariable10.Where<PermissionItem>(stackVariable11).GetEnumerator();
					try
					{
						while (V_3.MoveNext())
						{
							V_4 = V_3.get_Current();
							V_0.set_Item(V_4.get_Name(), V_4);
						}
					}
					finally
					{
						if (V_3 != null)
						{
							V_3.Dispose();
						}
					}
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			stackVariable25 = V_0.get_Values();
			stackVariable26 = PermissionManager.u003cu003ec.u003cu003e9__6_0;
			if (stackVariable26 == null)
			{
				dummyVar1 = stackVariable26;
				stackVariable26 = new Func<PermissionItem, string>(PermissionManager.u003cu003ec.u003cu003e9.u003cGetPublicPermissionsu003eb__6_0);
				PermissionManager.u003cu003ec.u003cu003e9__6_0 = stackVariable26;
			}
			return stackVariable25.OrderBy<PermissionItem, string>(stackVariable26).ToList<PermissionItem>();
		}
	}
}