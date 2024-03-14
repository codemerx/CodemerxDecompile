using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Piranha.Security
{
	public class PermissionManager
	{
		private readonly Dictionary<string, IList<PermissionItem>> _modules = new Dictionary<string, IList<PermissionItem>>();

		public IList<PermissionItem> this[string module]
		{
			get
			{
				IList<PermissionItem> permissionItems;
				if (!this._modules.TryGetValue(module, out permissionItems))
				{
					Dictionary<string, IList<PermissionItem>> strs = this._modules;
					List<PermissionItem> permissionItems1 = new List<PermissionItem>();
					permissionItems = permissionItems1;
					strs[module] = permissionItems1;
				}
				return permissionItems;
			}
		}

		public PermissionManager()
		{
		}

		public IList<string> GetModules()
		{
			return (
				from k in this._modules.Keys
				orderby k
				select k).ToList<string>();
		}

		public IList<PermissionItem> GetPermissions(string module)
		{
			return (
				from p in this[module]
				orderby p.Name
				select p).ToList<PermissionItem>();
		}

		public IList<PermissionItem> GetPermissions()
		{
			Dictionary<string, PermissionItem> strs = new Dictionary<string, PermissionItem>();
			foreach (string module in this.GetModules())
			{
				foreach (PermissionItem permission in this.GetPermissions(module))
				{
					strs[permission.Name] = permission;
				}
			}
			return (
				from k in strs.Values
				orderby k.Name
				select k).ToList<PermissionItem>();
		}

		public IList<PermissionItem> GetPublicPermissions()
		{
			Dictionary<string, PermissionItem> strs = new Dictionary<string, PermissionItem>();
			foreach (string module in this.GetModules())
			{
				foreach (PermissionItem permissionItem in 
					from p in this.GetPermissions(module)
					where !p.IsInternal
					select p)
				{
					strs[permissionItem.Name] = permissionItem;
				}
			}
			return (
				from k in strs.Values
				orderby k.Name
				select k).ToList<PermissionItem>();
		}
	}
}