using System;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.Account
{
	public class ChangePasswordViewModel
	{
		public string CurrentPassword
		{
			get;
			set;
		}

		public string NewPassword
		{
			get;
			set;
		}

		public ChangePasswordViewModel()
		{
			base();
			return;
		}
	}
}