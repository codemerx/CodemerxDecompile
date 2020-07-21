using NodaTime;
using System;
using System.Globalization;

namespace OrchardCore.Localization
{
	internal static class BclCalendars
	{
		public readonly static Calendar Hebrew;

		public readonly static Calendar Hijri;

		public readonly static Calendar Gregorian;

		public readonly static Calendar Julian;

		public readonly static Calendar Persian;

		public readonly static Calendar UmAlQura;

		static BclCalendars()
		{
			BclCalendars.Hebrew = new HebrewCalendar();
			BclCalendars.Hijri = new HijriCalendar();
			BclCalendars.Gregorian = new GregorianCalendar();
			BclCalendars.Julian = new JulianCalendar();
			BclCalendars.Persian = new PersianCalendar();
			BclCalendars.UmAlQura = new UmAlQuraCalendar();
			return;
		}

		public static CalendarSystem ConvertToCalendarSystem(Calendar calendar)
		{
			if (calendar == null)
			{
				throw new ArgumentNullException("calendar");
			}
			V_0 = calendar.GetType();
			if (Type.op_Equality(V_0, Type.GetTypeFromHandle(// 
			// Current member / type: NodaTime.CalendarSystem OrchardCore.Localization.BclCalendars::ConvertToCalendarSystem(System.Globalization.Calendar)
			// Exception in: NodaTime.CalendarSystem ConvertToCalendarSystem(System.Globalization.Calendar)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public static CalendarSystem GetCalendarByName(CalendarName calendarName)
		{
			switch (calendarName)
			{
				case 0:
				{
					return CalendarSystem.get_HebrewCivil();
				}
				case 1:
				{
					return CalendarSystem.get_IslamicBcl();
				}
				case 2:
				{
					return CalendarSystem.get_Iso();
				}
				case 3:
				{
					return CalendarSystem.get_Julian();
				}
				case 4:
				{
					if (!CultureInfo.get_CurrentUICulture().get_Calendar().IsLeapYear(1))
					{
						return CalendarSystem.get_PersianAstronomical();
					}
					return CalendarSystem.get_PersianSimple();
				}
				case 5:
				{
					return CalendarSystem.get_UmAlQura();
				}
			}
			throw new NotSupportedException("The calendar is not supported.");
		}

		public static CalendarName GetCalendarName(Calendar calendar)
		{
			if (calendar == null)
			{
				throw new ArgumentNullException("calendar");
			}
			V_0 = calendar.GetType();
			if (Type.op_Equality(V_0, Type.GetTypeFromHandle(// 
			// Current member / type: OrchardCore.Localization.CalendarName OrchardCore.Localization.BclCalendars::GetCalendarName(System.Globalization.Calendar)
			// Exception in: OrchardCore.Localization.CalendarName GetCalendarName(System.Globalization.Calendar)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

	}
}