﻿using GPSS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPSS.Entities.General.Transactions
{
    internal abstract class TransactionDecorator : Transaction
    {
        public TransactionDecorator(Transaction innerTransaction)
        {
            this.innerTransaction = innerTransaction ?? throw new ArgumentNullException(nameof(innerTransaction));
        }

        private Transaction innerTransaction;

        public override int Number
        {
            get => innerTransaction.Number;
            set => innerTransaction.Number = value;
        }

        public override int Priority
        {
            get => innerTransaction.Priority;
            set => innerTransaction.Priority = value;
        }

        public override int Assembly
        {
            get => innerTransaction.Assembly;
            set => innerTransaction.Assembly = value;
        }

        public override double MarkTime
        {
            get => innerTransaction.MarkTime;
            set => innerTransaction.MarkTime = value;
        }

        public override bool Trace
        {
            get => innerTransaction.Trace;
            set => innerTransaction.Trace = value;
        }

        public override int CurrentBlock
        {
            get => innerTransaction.CurrentBlock;
            set => innerTransaction.CurrentBlock = value;
        }

        public override int NextBlock
        {
            get => innerTransaction.NextBlock;
            set => innerTransaction.NextBlock = value;
        }

        public override TransactionState Chain
        {
            get => innerTransaction.Chain;
            set => innerTransaction.Chain = value;
        }

        public override bool Preempted
        {
            get => innerTransaction.Preempted;
            set => innerTransaction.Preempted = value;
        }

        public override Dictionary<string, dynamic> Parameters
        {
            get => innerTransaction.Parameters;
        }

        public override Transaction InnerTransaction
        {
            get => innerTransaction;
        }
    }
}
