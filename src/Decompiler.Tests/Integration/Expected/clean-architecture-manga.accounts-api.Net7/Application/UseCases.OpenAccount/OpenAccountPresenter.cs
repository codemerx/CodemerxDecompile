using Domain;
using System;
using System.Runtime.CompilerServices;

namespace Application.UseCases.OpenAccount
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class OpenAccountPresenter : IOutputPort
	{
		public Domain.Account Account
		{
			get;
			private set;
		}

		public bool InvalidOutput
		{
			get;
			private set;
		}

		public bool NotFoundOutput
		{
			get;
			private set;
		}

		public OpenAccountPresenter()
		{
		}

		public void Invalid()
		{
			this.InvalidOutput = true;
		}

		public void NotFound()
		{
			this.NotFoundOutput = true;
		}

		[NullableContext(1)]
		public void Ok(Domain.Account account)
		{
			this.Account = account;
		}
	}
}