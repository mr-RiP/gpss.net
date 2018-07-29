﻿using GPSS.Entities.General.Transactions;
using GPSS.Entities.Resources;
using GPSS.Enums;
using GPSS.Exceptions;
using System;
using System.Linq;

namespace GPSS.Entities.General.Blocks
{
	internal sealed class Advance : Block
	{
		private Advance()
		{

		}

		public Advance(Func<IStandardAttributes, double> delay)
		{
			Delay = delay;
		}

		public Func<IStandardAttributes, double> Delay { get; private set; }

		public override string TypeName => "ADVANCE";

		public override Block Clone() => new Advance
		{
			Delay = Delay,
			EntryCount = EntryCount,
			TransactionsCount = TransactionsCount,
		};

		public override bool CanEnter(Simulation simulation)
		{
			var transaction = simulation.ActiveTransaction.Transaction;
			double time = Delay(simulation.StandardAttributes);
			if (time < 0.0)
				throw new ModelStructureException("Negative time increment.", transaction.CurrentBlock);

			bool allow = time == 0.0 || !transaction.Preempted;
			if (!allow)
				transaction.Delayed = true;

			return allow;
		}

		public override void EnterBlock(Simulation simulation)
		{
			var transaction = simulation.ActiveTransaction.Transaction;
			transaction.State = TransactionState.Suspended;

			double time = Delay(simulation.StandardAttributes);
			if (time < 0.0)
				throw new ModelStructureException("Negative time increment.", transaction.CurrentBlock);

			var chains = simulation.Scheduler;
			chains.CurrentEvents.Remove(transaction);

			if (!transaction.Preempted || time == 0.0)
				base.EnterBlock(simulation);

			if (transaction.Preempted || time == 0.0)
				chains.PlaceInCurrentEvents(transaction);
			else
				chains.PlaceInFutureEvents(transaction, time);
		}

		public override void AddRetry(Simulation simulation, int? destinationBlockIndex = null)
		{
			var transaction = simulation.ActiveTransaction.Transaction;
			if (transaction.Preempted)
			{
				int blockIndex = simulation.Model.Statements.Blocks.IndexOf(this);

				simulation.Model.Resources.Facilities.Values
					.AsParallel()
					.Where(f => f.Interrupted && f.InterruptChain.Any(fe => fe.InnerTransaction == transaction))
					.ForAll(f => AddLastTransaction(f, transaction, destinationBlockIndex));
			}
		}

		private static void AddLastTransaction(Facility facility, Transaction transaction, int? destinationBlockIndex)
		{
			facility.RetryChain.AddLast(new RetryChainTransaction(
				transaction,
				() => !transaction.Preempted,
				destinationBlockIndex));
		}

	}
}
