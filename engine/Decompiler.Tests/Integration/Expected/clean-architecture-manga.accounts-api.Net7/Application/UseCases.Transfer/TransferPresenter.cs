using Domain;
using Domain.Credits;
using Domain.Debits;
using System;
using System.Runtime.CompilerServices;

namespace Application.UseCases.Transfer
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class TransferPresenter : IOutputPort
	{
		public Domain.Credits.Credit Credit
		{
			get;
			private set;
		}

		public Domain.Debits.Debit Debit
		{
			get;
			private set;
		}

		public Account DestinationAccount
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

		public Account OriginAccount
		{
			get;
			private set;
		}

		public bool OutOfFundsOutput
		{
			get;
			private set;
		}

		public TransferPresenter()
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
		public void Ok(Account originAccount, Domain.Debits.Debit debit, Account destinationAccount, Domain.Credits.Credit credit)
		{
			this.OriginAccount = originAccount;
			this.Debit = debit;
			this.DestinationAccount = destinationAccount;
			this.Credit = credit;
		}

		public void OutOfFunds()
		{
			this.OutOfFundsOutput = true;
		}
	}
}