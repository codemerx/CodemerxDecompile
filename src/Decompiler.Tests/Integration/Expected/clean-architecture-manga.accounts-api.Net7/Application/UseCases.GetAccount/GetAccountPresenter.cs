using Domain;
using System;
using System.Runtime.CompilerServices;

namespace Application.UseCases.GetAccount
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class GetAccountPresenter : IOutputPort
	{
		public Domain.Account Account
		{
			get;
			private set;
		}

		public bool? InvalidOutput
		{
			get;
			private set;
		}

		public bool? IsNotFound
		{
			get;
			private set;
		}

		public GetAccountPresenter()
		{
		}

		public void Invalid()
		{
			this.InvalidOutput = new bool?(true);
		}

		public void NotFound()
		{
			this.IsNotFound = new bool?(true);
		}

		[NullableContext(1)]
		public void Ok(Domain.Account account)
		{
			this.Account = account;
		}
	}
}