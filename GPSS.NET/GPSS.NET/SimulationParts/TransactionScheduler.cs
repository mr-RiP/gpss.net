﻿using GPSS.Entities.General;
using GPSS.Entities.General.Transactions;
using GPSS.Entities.Groups;
using GPSS.Entities.Resources;
using GPSS.Enums;
using GPSS.StandardAttributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GPSS.SimulationParts
{
    // http://www.minutemansoftware.com/reference/r9.htm#9.1
    internal class TransactionScheduler : ISystemAttributes
    {
        public TransactionScheduler(Model model, int terminationCount)
        {
            AddStorageDelayChains(model);
            TerminationCount = terminationCount;
        }

        private void AddStorageDelayChains(Model model)
        {
            foreach (var kvpStorage in model.Resources.Storages)
                StorageDelayChains.Add(kvpStorage.Key, kvpStorage.Value.DelayChain);
        }

        public Dictionary<string, LinkedList<Transaction>> UserChains { get; private set; } = new Dictionary<string, LinkedList<Transaction>>();
        
        public Dictionary<string, LinkedList<StorageDelayTransaction>> StorageDelayChains { get; private set; } = new Dictionary<string, LinkedList<StorageDelayTransaction>>();

        public Dictionary<string, LinkedList<Transaction>> FacilityDelayChains { get; private set; } = new Dictionary<string, LinkedList<Transaction>>();

        public Dictionary<string, LinkedList<Transaction>> FacilityPendingChains { get; private set; } = new Dictionary<string, LinkedList<Transaction>>();

        public LinkedList<FutureEventTransaction> FutureEvents { get; private set; } = new LinkedList<FutureEventTransaction>();

        // Kept in priority order, the Active Transaction is usually returned to the CEC ahead of its peers.
        // When the Active Transaction comes to rest on some Transaction Chain,
        // the highest priority Transaction remaining on the CEC becomes the Active Transaction.
        public LinkedList<Transaction> CurrentEvents { get; private set; } = new LinkedList<Transaction>();

        public double RelativeClock { get; private set; } = 0.0;
        public double AbsoluteClock { get; private set; } = 0.0;

        public int GenerationCount { get; set; } = 0;
        public int TerminationCount { get; set; } = 0;

        public Transaction GetActiveTransaction()
        {
            return CurrentEvents.First.Value;
        }

        public void PlaceInCurrentEvents(Transaction transaction)
        {
            if (CurrentEvents.Count == 0 || transaction.Priority <= CurrentEvents.Last.Value.Priority)
                CurrentEvents.AddLast(transaction);
            else
            {
                var node = CurrentEvents.First;
                while (node.Value.Priority >= transaction.Priority)
                    node = node.Next;
                CurrentEvents.AddBefore(node, transaction);
            }
        }

        public void PlaceInFutureEvents(Transaction transaction, double timeIncrement)
        {
            var futureEvent = new FutureEventTransaction(transaction, RelativeClock + timeIncrement);
            if (FutureEvents.Count == 0 || futureEvent.ReleaseTime >= FutureEvents.Last.Value.ReleaseTime)
                FutureEvents.AddLast(futureEvent);
            else
            {
                var node = FutureEvents.First;
                while (node.Value.ReleaseTime <= futureEvent.ReleaseTime)
                    node = node.Next;
                FutureEvents.AddBefore(node, futureEvent);
            }
        }

        public void UpdateEvents()
        {
            double releaseTime = FutureEvents.First.Value.ReleaseTime;
            while (FutureEvents.First.Value.ReleaseTime == releaseTime)
            {
                PlaceInCurrentEvents(FutureEvents.First.Value.InnerTransaction);
                FutureEvents.RemoveFirst();
            }

            UpdateClock(releaseTime);
        }

        private void UpdateClock(double releaseTime)
        {
            AbsoluteClock += releaseTime - RelativeClock;
            RelativeClock = releaseTime;
        }

        public void Reset()
        {
            foreach (var futureEvent in FutureEvents)
                futureEvent.ReleaseTime -= RelativeClock;
            RelativeClock = 0.0;
        }
    }
}
