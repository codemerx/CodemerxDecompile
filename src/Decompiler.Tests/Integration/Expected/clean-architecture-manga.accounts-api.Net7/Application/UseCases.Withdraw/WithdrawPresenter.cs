using Domain;
using Domain.Debits;
using System;
using System.Runtime.CompilerServices;

namespace Application.UseCases.Withdraw
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class WithdrawPresenter : IOutputPort
	{
		public Domain.Account Account
		{
			get;
			private set;
		}

		public Domain.Debits.Debit Debit
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

		public bool OutOfFundsOutput
		{
			get;
			private set;
		}

		public WithdrawPresenter()
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
		public void Ok(Domain.Debits.Debit debit, Domain.Account account)
		{
			this.Account = account;
			this.Debit = debit;
		}

		public void OutOfFunds()
		{
			this.OutOfFundsOutput = true;
		}
	}
}