using NodaTime;
using NodaTime.TimeZones;
using System;
using System.Runtime.CompilerServices;

namespace OrchardCore.Modules
{
	public class Clock : IClock
	{
		private static Instant CurrentInstant
		{
			get
			{
				return SystemClock.get_Instance().GetCurrentInstant();
			}
		}

		public DateTime UtcNow
		{
			get
			{
				return Clock.get_CurrentInstant().ToDateTimeUtc();
			}
		}

		public Clock()
		{
			base();
			return;
		}

		public DateTimeOffset ConvertToTimeZone(DateTimeOffset dateTimeOffSet, ITimeZone timeZone)
		{
			V_0 = OffsetDateTime.FromDateTimeOffset(dateTimeOffSet);
			V_1 = V_0.InZone(((OrchardCore.Modules.TimeZone)timeZone).get_DateTimeZone());
			return V_1.ToDateTimeOffset();
		}

		private ITimeZone CreateTimeZone(DateTimeZone dateTimeZone)
		{
			if (dateTimeZone == null)
			{
				throw new ArgumentException("DateTimeZone");
			}
			V_0 = dateTimeZone.GetZoneInterval(Clock.get_CurrentInstant());
			return new OrchardCore.Modules.TimeZone(dateTimeZone.get_Id(), V_0.get_StandardOffset(), V_0.get_WallOffset(), dateTimeZone);
		}

		internal static DateTimeZone GetDateTimeZone(string timeZone)
		{
			if (string.IsNullOrEmpty(timeZone) || !Clock.IsValidTimeZone(DateTimeZoneProviders.get_Tzdb(), timeZone))
			{
				return DateTimeZoneProviders.get_Tzdb().GetSystemDefault();
			}
			return DateTimeZoneProviders.get_Tzdb().get_Item(timeZone);
		}

		public ITimeZone GetSystemTimeZone()
		{
			V_0 = DateTimeZoneProviders.get_Tzdb().GetSystemDefault();
			if (TzdbDateTimeZoneSource.get_Default().get_CanonicalIdMap().ContainsKey(V_0.get_Id()))
			{
				V_0 = Clock.GetDateTimeZone(TzdbDateTimeZoneSource.get_Default().get_CanonicalIdMap().get_Item(V_0.get_Id()));
			}
			return this.CreateTimeZone(V_0);
		}

		public ITimeZone GetTimeZone(string timeZoneId)
		{
			if (string.IsNullOrEmpty(timeZoneId))
			{
				return this.GetSystemTimeZone();
			}
			return this.CreateTimeZone(Clock.GetDateTimeZone(timeZoneId));
		}

		public ITimeZone[] GetTimeZones()
		{
			stackVariable1 = TzdbDateTimeZoneSource.get_Default().get_ZoneLocations();
			stackVariable2 = Clock.u003cu003ec.u003cu003e9__4_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Func<TzdbZoneLocation, u003cu003ef__AnonymousType1<TzdbZoneLocation, string>>(Clock.u003cu003ec.u003cu003e9.u003cGetTimeZonesu003eb__4_0);
				Clock.u003cu003ec.u003cu003e9__4_0 = stackVariable2;
			}
			stackVariable3 = stackVariable1.Select(stackVariable2);
			stackVariable4 = Clock.u003cu003ec.u003cu003e9__4_1;
			if (stackVariable4 == null)
			{
				dummyVar1 = stackVariable4;
				stackVariable4 = new Func<u003cu003ef__AnonymousType1<TzdbZoneLocation, string>, u003cu003ef__AnonymousType2<u003cu003ef__AnonymousType1<TzdbZoneLocation, string>, DateTimeZone>>(Clock.u003cu003ec.u003cu003e9.u003cGetTimeZonesu003eb__4_1);
				Clock.u003cu003ec.u003cu003e9__4_1 = stackVariable4;
			}
			stackVariable5 = stackVariable3.Select(stackVariable4);
			stackVariable6 = Clock.u003cu003ec.u003cu003e9__4_2;
			if (stackVariable6 == null)
			{
				dummyVar2 = stackVariable6;
				stackVariable6 = new Func<u003cu003ef__AnonymousType2<u003cu003ef__AnonymousType1<TzdbZoneLocation, string>, DateTimeZone>, u003cu003ef__AnonymousType3<u003cu003ef__AnonymousType2<u003cu003ef__AnonymousType1<TzdbZoneLocation, string>, DateTimeZone>, ZoneInterval>>(Clock.u003cu003ec.u003cu003e9.u003cGetTimeZonesu003eb__4_2);
				Clock.u003cu003ec.u003cu003e9__4_2 = stackVariable6;
			}
			stackVariable7 = stackVariable5.Select(stackVariable6);
			stackVariable8 = Clock.u003cu003ec.u003cu003e9__4_3;
			if (stackVariable8 == null)
			{
				dummyVar3 = stackVariable8;
				stackVariable8 = new Func<u003cu003ef__AnonymousType3<u003cu003ef__AnonymousType2<u003cu003ef__AnonymousType1<TzdbZoneLocation, string>, DateTimeZone>, ZoneInterval>, Offset>(Clock.u003cu003ec.u003cu003e9.u003cGetTimeZonesu003eb__4_3);
				Clock.u003cu003ec.u003cu003e9__4_3 = stackVariable8;
			}
			stackVariable9 = stackVariable7.OrderBy(stackVariable8);
			stackVariable10 = Clock.u003cu003ec.u003cu003e9__4_4;
			if (stackVariable10 == null)
			{
				dummyVar4 = stackVariable10;
				stackVariable10 = new Func<u003cu003ef__AnonymousType3<u003cu003ef__AnonymousType2<u003cu003ef__AnonymousType1<TzdbZoneLocation, string>, DateTimeZone>, ZoneInterval>, string>(Clock.u003cu003ec.u003cu003e9.u003cGetTimeZonesu003eb__4_4);
				Clock.u003cu003ec.u003cu003e9__4_4 = stackVariable10;
			}
			stackVariable11 = stackVariable9.ThenBy(stackVariable10);
			stackVariable12 = Clock.u003cu003ec.u003cu003e9__4_5;
			if (stackVariable12 == null)
			{
				dummyVar5 = stackVariable12;
				stackVariable12 = new Func<u003cu003ef__AnonymousType3<u003cu003ef__AnonymousType2<u003cu003ef__AnonymousType1<TzdbZoneLocation, string>, DateTimeZone>, ZoneInterval>, OrchardCore.Modules.TimeZone>(Clock.u003cu003ec.u003cu003e9.u003cGetTimeZonesu003eb__4_5);
				Clock.u003cu003ec.u003cu003e9__4_5 = stackVariable12;
			}
			return stackVariable11.Select(stackVariable12).ToArray<OrchardCore.Modules.TimeZone>();
		}

		private static bool IsValidTimeZone(IDateTimeZoneProvider provider, string timeZoneId)
		{
			return provider.GetZoneOrNull(timeZoneId) != null;
		}
	}
}