using Piranha.Extend;
using System;

namespace Piranha.Runtime
{
	public sealed class AppModuleList : AppDataList<IModule, AppModule>
	{
		public AppModuleList()
		{
		}

		public T Get<T>()
		where T : IModule
		{
			object instance;
			AppModule byType = this.GetByType(typeof(T));
			if (byType != null)
			{
				instance = byType.Instance;
			}
			else
			{
				instance = null;
			}
			return (T)instance;
		}

		protected override AppModule OnRegister<TValue>(AppModule item)
		where TValue : IModule
		{
			item.Instance = Activator.CreateInstance<TValue>();
			return item;
		}
	}
}