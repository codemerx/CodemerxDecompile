using Domain;
using Domain.Credits;
using System;
using System.Runtime.CompilerServices;

namespace Application.UseCases.Deposit
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class DepositPresenter : IOutputPort
	{
		public Domain.Account Account
		{
			get;
			private set;
		}

		public Domain.Credits.Credit Credit
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

		public DepositPresenter()
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
		public void Ok(Domain.Credits.Credit credit, Domain.Account account)
		{
			this.Credit = credit;
			this.Account = account;
		}
	}
}