using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OrchardCore.Localization
{
	public class DefaultCalendarSelector : ICalendarSelector
	{
		private readonly static Task<CalendarSelectorResult> CalendarResult;

		static DefaultCalendarSelector()
		{
			CalendarSelectorResult calendarSelectorResult = new CalendarSelectorResult();
			calendarSelectorResult.set_Priority(0);
			calendarSelectorResult.set_CalendarName(() => Task.FromResult<CalendarName>(BclCalendars.GetCalendarName(CultureInfo.CurrentUICulture.Calendar)));
			DefaultCalendarSelector.CalendarResult = Task.FromResult<CalendarSelectorResult>(calendarSelectorResult);
		}

		public DefaultCalendarSelector()
		{
		}

		public Task<CalendarSelectorResult> GetCalendarAsync()
		{
			return DefaultCalendarSelector.CalendarResult;
		}
	}
}