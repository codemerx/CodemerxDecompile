using Piranha.Extend;
using System;

namespace Piranha.Runtime
{
	public sealed class AppModuleList : AppDataList<IModule, AppModule>
	{
		public AppModuleList()
		{
			base();
			return;
		}

		public T Get<T>()
		where T : IModule
		{
			stackVariable3 = this.GetByType(Type.GetTypeFromHandle(// 
			// Current member / type: T Piranha.Runtime.AppModuleList::Get()
			// Exception in: T Get()
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		protected override AppModule OnRegister<TValue>(AppModule item)
		where TValue : IModule
		{
			item.set_Instance(Activator.CreateInstance<TValue>());
			return item;
		}
	}
}