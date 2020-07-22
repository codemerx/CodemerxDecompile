using System;
using System.Collections;
using System.Collections.Generic;

namespace Mix.Cms.Lib
{
	public class MixEnums
	{
		public MixEnums()
		{
			base();
			return;
		}

		public static List<object> EnumToObject(Type enumType)
		{
			V_0 = new List<object>();
			V_1 = Enum.GetValues(enumType).GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.Add(new u003cu003ef__AnonymousType63<string, string>(Enum.GetName(enumType, V_2), Enum.GetName(enumType, V_2)));
				}
			}
			finally
			{
				V_3 = V_1 as IDisposable;
				if (V_3 != null)
				{
					V_3.Dispose();
				}
			}
			return V_0;
		}

		public enum CatePosition
		{
			Nav = 1,
			Top = 2,
			Left = 3,
			Footer = 4
		}

		public enum CompareType
		{
			Eq = 1,
			Lt = 2,
			Gt = 3,
			Lte = 4,
			Gte = 5,
			In = 6
		}

		public enum ConfigurationCategory
		{
			PageSize,
			Site,
			Email
		}

		public enum DatabaseProvider
		{
			MSSQL,
			MySQL,
			PostgreSQL
		}

		public enum EnumTemplateFolder
		{
			Layouts,
			Pages,
			Modules,
			Forms,
			Edms,
			Posts,
			Products,
			Widgets,
			Masters
		}

		public enum FileFolder
		{
			Styles,
			Scripts,
			Images,
			Fonts,
			Others,
			Templates
		}

		public enum MixAttributeSetDataType
		{
			System,
			Set,
			Post,
			Page,
			Module,
			Service
		}

		public enum MixContentStatus
		{
			Deleted,
			Preview,
			Published,
			Draft,
			Schedule
		}

		public enum MixDataType
		{
			Custom = 0,
			DateTime = 1,
			Date = 2,
			Time = 3,
			Duration = 4,
			PhoneNumber = 5,
			Double = 6,
			Text = 7,
			Html = 8,
			MultilineText = 9,
			EmailAddress = 10,
			Password = 11,
			Url = 12,
			ImageUrl = 13,
			CreditCard = 14,
			PostalCode = 15,
			Upload = 16,
			Color = 17,
			Boolean = 18,
			Icon = 19,
			VideoYoutube = 20,
			TuiEditor = 21,
			Integer = 22,
			Reference = 23,
			QRCode = 24
		}

		public enum MixModuleType
		{
			Content,
			Data,
			ListPost,
			SubPage,
			SubPost,
			SubProduct,
			ListProduct,
			Gallery
		}

		public enum MixOrderStatus
		{
			Deleted,
			Preview,
			Published,
			Draft,
			Schedule
		}

		public enum MixPageType
		{
			Article = 1,
			ListPost = 2,
			Home = 3,
			System = 8
		}

		public enum MixRelatedType
		{
			PageToPage,
			PageToPost,
			PageToModule,
			PageToData,
			ModuleToPost,
			ModuleToPage,
			ModuleToData,
			PostToData,
			DataToData
		}

		public enum MixStructureType
		{
			Page,
			Module,
			Post,
			AttributeSet
		}

		public enum MixUserStatus
		{
			Deleted = 0,
			Actived = 1,
			Banned = 3
		}

		public enum PageStatus
		{
			Deleted,
			Preview,
			Published,
			Draft,
			Schedule
		}

		public enum ResponseKey
		{
			NotFound,
			OK,
			BadRequest
		}

		public enum ResponseStatus
		{
			Ok = 200,
			BadRequest = 400,
			UnAuthorized = 401,
			Forbidden = 403,
			ServerError = 500
		}

		public enum SearchType
		{
			All,
			Post,
			Module,
			Page
		}

		public enum UrlAliasType
		{
			Page,
			Post,
			Product,
			Module,
			ModuleData
		}
	}
}