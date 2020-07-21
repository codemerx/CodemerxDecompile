using Piranha.Extend;
using System;
using System.Runtime.CompilerServices;

namespace Piranha.Runtime
{
	public sealed class AppFieldList : AppDataList<IField, AppField>
	{
		public AppFieldList()
		{
			base();
			return;
		}

		public AppField GetByShorthand(string shorthand)
		{
			V_0 = new AppFieldList.u003cu003ec__DisplayClass0_0();
			V_0.shorthand = shorthand;
			return this._items.FirstOrDefault<AppField>(new Func<AppField, bool>(V_0.u003cGetByShorthandu003eb__0));
		}

		protected override AppField OnRegister<TValue>(AppField item)
		where TValue : IField
		{
			V_0 = Type.GetTypeFromHandle(// 
			// Current member / type: Piranha.Runtime.AppField Piranha.Runtime.AppFieldList::OnRegister(Piranha.Runtime.AppField)
			// Exception in: Piranha.Runtime.AppField OnRegister(Piranha.Runtime.AppField)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public void RegisterDataSelect<TValue>()
		where TValue : class
		{
			this.Register<DataSelectField<TValue>>();
			App.get_Serializers().Register<DataSelectField<TValue>>(new DataSelectFieldSerializer<DataSelectField<TValue>>());
			return;
		}

		public void RegisterSelect<TValue>()
		where TValue : struct
		{
			this.Register<SelectField<TValue>>();
			App.get_Serializers().Register<SelectField<TValue>>(new SelectFieldSerializer<SelectField<TValue>>());
			return;
		}
	}
}