using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OrchardCore.Localization
{
	public class DefaultCalendarSelector : ICalendarSelector
	{
		private readonly static Task<CalendarSelectorResult> CalendarResult;

		static DefaultCalendarSelector()
		{
			stackVariable0 = new CalendarSelectorResult();
			stackVariable0.set_Priority(0);
			stackVariable0.set_CalendarName(new Func<Task<CalendarName>>(DefaultCalendarSelector.u003cu003ec.u003cu003e9, DefaultCalendarSelector.u003cu003ec.u003cu002ecctoru003eb__3_0));
			DefaultCalendarSelector.CalendarResult = Task.FromResult<CalendarSelectorResult>(stackVariable0);
			return;
		}

		public DefaultCalendarSelector()
		{
			base();
			return;
		}

		public Task<CalendarSelectorResult> GetCalendarAsync()
		{
			return DefaultCalendarSelector.CalendarResult;
		}
	}
}