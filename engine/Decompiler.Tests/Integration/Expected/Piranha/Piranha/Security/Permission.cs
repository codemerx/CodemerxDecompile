using System;

namespace Piranha.Security
{
	public static class Permission
	{
		public const string PagePreview = "PiranhaPagePreview";

		public const string PostPreview = "PiranhaPostPreview";

		public static string[] All()
		{
			stackVariable1 = new String[2];
			stackVariable1[0] = "PiranhaPagePreview";
			stackVariable1[1] = "PiranhaPostPreview";
			return stackVariable1;
		}
	}
}