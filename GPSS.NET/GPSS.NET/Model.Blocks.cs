﻿using GPSS.Entities.General.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPSS
{
    public partial class Model
    {
        #region GENERATE Block // TODO

        /// <summary>
        /// GENERATE Block. 
        /// A GENERATE Block creates Transactions for future entry into the simulation.
        /// </summary>
        /// <param name="generationInterval">Transaction creation interval, in model time. Must be positive.</param>
        /// <returns>Model with added GENERATE Block.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="generationInterval"/> must have positive value.
        /// </exception>
        public Model Generate(double generationInterval)
        {
            if (generationInterval <= 0.0)
                throw new ArgumentOutOfRangeException(nameof(generationInterval), "Argument value must be positive.");

            return Generate((sna => generationInterval));
        }

        /// <summary>
        /// GENERATE Block. 
        /// A GENERATE Block creates Transactions for future entry into the simulation.
        /// </summary>
        /// <param name="generationIntervalMean">Transaction creation interval mean, in model time. Must be positive.</param>
        /// <param name="generationIntervalHalfRange">
        /// Transaction creation interval half-range, in model time. Must be less or equal to <paramref name="generationIntervalMean"/>.
        /// </param>
        /// <returns>Model with added GENERATE Block.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="generationIntervalMean"/> must have positive value.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="generationIntervalHalfRange"/> must be less or equal to <paramref name="generationIntervalMean"/>.
        /// </exception>
        public Model Generate(double generationIntervalMean, double generationIntervalHalfRange)
        {
            if (generationIntervalMean <= 0.0)
                throw new ArgumentOutOfRangeException(nameof(generationIntervalMean), "Argument value must be positive.");

            if (generationIntervalHalfRange >= generationIntervalMean)
                throw new ArgumentOutOfRangeException(nameof(generationIntervalHalfRange),
                    "Argument value must be less than value of argument " + nameof(generationIntervalMean) + ".");

            double min = generationIntervalMean - generationIntervalHalfRange;
            double max = generationIntervalMean + generationIntervalHalfRange;
            return Generate((sna => sna.RandomNumberGenerator().ProbabilityDistributions.Uniform(min, max)));
        }

        /// <summary>
        /// GENERATE Block. 
        /// A GENERATE Block creates Transactions for future entry into the simulation.
        /// </summary>
        /// <param name="generationIntervalMean">Transaction creation interval mean, in model time. Must be positive.</param>
        /// <param name="generationIntervalHalfRange">
        /// Transaction creation interval half-range, in model time. Must be less or equal to <paramref name="generationIntervalMean"/>.
        /// </param>
        /// <param name="firstTransactionDelay">First transaction creation delay, in model time. Must have zero or positive value.</param>
        /// <returns>Model with added GENERATE Block.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="generationIntervalMean"/> must have positive value.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="generationIntervalHalfRange"/> must be less or equal to <paramref name="generationIntervalMean"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="firstTransactionDelay"/> must have zero or positive value.
        /// </exception>
        public Model Generate(double generationIntervalMean, double generationIntervalHalfRange, double firstTransactionDelay)
        {
            if (generationIntervalMean <= 0.0)
                throw new ArgumentOutOfRangeException(nameof(generationIntervalMean), "Argument value must be positive.");

            if (generationIntervalHalfRange >= generationIntervalMean)
                throw new ArgumentOutOfRangeException(nameof(generationIntervalHalfRange),
                    "Argument value must be less than value of argument " + nameof(generationIntervalMean) + ".");

            if (firstTransactionDelay < 0.0)
                throw new ArgumentOutOfRangeException(nameof(firstTransactionDelay), "Argument value must be zero or positive.");

            double min = generationIntervalMean - generationIntervalHalfRange;
            double max = generationIntervalMean + generationIntervalHalfRange;
            return Generate((sna => sna.RandomNumberGenerator().ProbabilityDistributions.Uniform(min, max)), (sna => firstTransactionDelay));
        }

        /// <summary>
        /// GENERATE Block. 
        /// A GENERATE Block creates Transactions for future entry into the simulation.
        /// </summary>
        /// <param name="generationInterval">Transaction creation interval, in model time. Zero if null.</param>
        /// <param name="firstTransactionDelay">First transaction creation delay, in model time. Zero if null.</param>
        /// <param name="creationLimit">Transactions creation limit. Null means no limit.</param>
        /// <param name="priority">Created transaction's priority value. Zero if null.</param>
        /// <returns>Model with added GENERATE Block.</returns>
        /// <exception cref="ArgumentNullException">
        /// Either <paramref name="generationInterval"/> or <paramref name="creationLimit"/> must not be null.
        /// </exception>
        public virtual Model Generate(
            Func<IStandardAttributes, double> generationInterval,
            Func<IStandardAttributes, double> firstTransactionDelay = null,
            Func<IStandardAttributes, int?> creationLimit = null,
            Func<IStandardAttributes, int> priority = null)
        {
            if (generationInterval == null && creationLimit == null)
                throw new ArgumentNullException();

            var generate = new Generate(
                generationInterval ?? (sna => 0.0),
                firstTransactionDelay ?? (sna => 0.0),
                creationLimit ?? (sna => null),
                priority ?? (sna => 0));

            Statements.Blocks.Add(generate);
            return this;
        }

        #endregion

        #region ADVANCE Block

        /// <summary>
        /// ADVANCE Block.
        /// An ADVANCE Block delays the progress of a Transaction for a specified amount of simulated time.
        /// </summary>
        /// <param name="delayTime">
        /// Transaction advancement delay, in model time. Must have zero or positive value.
        /// </param>
        /// <returns>Model with added ADVANCE Block.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <param name="delayTime"> must have zero or positive value.
        /// </exception>
        public Model Advance(double delayTime)
        {
            if (delayTime < 0.0)
                throw new ArgumentOutOfRangeException(
                    nameof(delayTime),
                    "Argument must have zero or positive value.");

            return Advance((sna => delayTime));
        }

        /// <summary>
        /// ADVANCE Block.
        /// An ADVANCE Block delays the progress of a Transaction for a specified amount of simulated time.
        /// </summary>
        /// <param name="delayMean">
        /// Transaction advancement delay mean, in model time. Must have zero or positive value.
        /// </param>
        /// <param name="delayHalfRange">
        /// Transaction advancement delay half-range, in model time. Must be equal or lower than <paramref name="delayMean"/>.
        /// </param>
        /// <returns>Model with added ADVANCE Block.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <param name="delayMean"> must have zero or positive value.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="delayHalfRange"/> value must be lower or equal to <paramref name="delayMean"/> value.
        /// </exception>
        public Model Advance(double delayMean, double delayHalfRange)
        {
            if (delayMean < 0.0)
                throw new ArgumentOutOfRangeException(nameof(delayMean), "Mean value must be zero or positive.");
            if (delayMean < delayHalfRange)
                throw new ArgumentOutOfRangeException(nameof(delayHalfRange),
                    "Argument value must be less or equal to argument " + nameof(delayMean) + " value.");

            double min = delayMean - delayHalfRange;
            double max = delayMean + delayHalfRange;
            return Advance((sna => sna.RandomNumberGenerator().ProbabilityDistributions.Uniform(min, max)));
        }

        /// <summary>
        /// ADVANCE Block.
        /// An ADVANCE Block delays the progress of a Transaction for a specified amount of simulated time.
        /// </summary>
        /// <param name="delayTime">Transaction advancement delay, in model time. Zero if null.</param>
        /// <returns>Model with added ADVANCE Block.</returns>
        public virtual Model Advance(Func<IStandardAttributes, double> delayTime)
        {
            var advance = new Advance(delayTime ?? (sna => 0.0));
            Statements.Blocks.Add(advance);
            return this;
        }

        #endregion

        #region TERMINATE Block

        /// <summary>
        /// TERMINATE Block.
        /// A TERMINATE Block removes the Active Transaction from the simulation and optionally reduces the Termination Count.
        /// </summary>
        /// <param name="terminationDecrement">Termination Count decrement. Must be zero or positive.</param>
        /// <returns>Model with added TERMINATE Block.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="terminationDecrement"/> must have zero or positive value.
        /// </exception>
        public Model Terminate(int terminationDecrement)
        {
            if (terminationDecrement < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(terminationDecrement),
                    "Argument must have zero or positive value.");

            return Terminate((sna => terminationDecrement));
        }

        /// <summary>
        /// TERMINATE Block.
        /// A TERMINATE Block removes the Active Transaction from the simulation and optionally reduces the Termination Count.
        /// </summary>
        /// <param name="terminationDecrement">Termination Count decrement. Zero if null.</param>
        /// <returns>Model with added TERMINATE Block.</returns>
        public virtual Model Terminate(Func<IStandardAttributes, int> terminationDecrement)
        {
            var terminate = new Terminate(terminationDecrement ?? (sna => 0));
            Statements.Blocks.Add(terminate);
            return this;
        }

        #endregion

        #region ENTER Block // TODO

        /// <summary>
        /// ENTER Block.
        /// When a Transaction attempts to enter an ENTER Block, it either takes or waits for a specified number of storage capacity units.
        /// </summary>
        /// <param name="storageName">Storage entity name. Must neither be null nor return null. Must address to predefined Storage Entity.</param>
        /// <param name="storageCapacity">Number of storage capacity units used by entering transaction. Null means 1.</param>
        /// <returns>Model with added ENTER Block.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="storageName"/> must not be null.</exception>
        public virtual Model Enter(Func<IStandardAttributes, string> storageName, Func<IStandardAttributes, int> storageCapacity = null)
        {
            var enter = new Enter(
                storageName ?? throw new ArgumentNullException(nameof(storageName)),
                storageCapacity ?? (sna => 1));

            Statements.Blocks.Add(enter);
            return this;
        }

        #endregion

        #region LEAVE Block // TODO

        /// <summary>
        /// LEAVE Block.
        /// A LEAVE Block increases the accessible storage capacity units at a Storage Entity.
        /// </summary>
        /// <param name="storageName">Storage entity name. Must neither be null nor return null. Must address to predefined Storage Entity.</param>
        /// <param name="storageCapacity">Number of storage capacity units used by entering transaction. Null means 1.</param>
        /// <returns>Model with added LEAVE Block.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="storageName"/> must not be null.</exception>
        public virtual Model Leave(Func<IStandardAttributes, string> storageName, Func<IStandardAttributes, int> storageCapacity = null)
        {
            var leave = new Leave(
                storageName ?? throw new ArgumentNullException(nameof(storageName)),
                storageCapacity ?? (sna => 1));

            Statements.Blocks.Add(leave);
            return this;
        }

        #endregion

        #region SAVAIL Block 

        /// <summary>
        /// SAVAIL Block.
        /// A SAVAIL Block ensures that a Storage Entity is in the available state.
        /// </summary>
        /// <param name="storageName">Storage entity name. Must not be null. Must address to predefined Storage Entity.</param>
        /// <returns>Model with added SAVAIL Block.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="storageName"/> must not be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="storageName"/> must address to predefined Storage Entity.</exception>
        public Model StorageAvailable(string storageName)
        {
            if (storageName == null)
                throw new ArgumentNullException(nameof(storageName));
            if (!Resources.Storages.ContainsKey(storageName))
                throw new ArgumentOutOfRangeException(nameof(storageName), "Storage entity with given name does not exists in the model.");

            return StorageAvailable(sna => storageName);
        }

        /// <summary>
        /// SAVAIL Block.
        /// A SAVAIL Block ensures that a Storage Entity is in the available state.
        /// </summary>
        /// <param name="storageName">Storage entity name. Must neither be null nor return null. Must address to predefined Storage Entity.</param>
        /// <returns>Model with added SAVAIL Block.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="storageName"/> must not be null.</exception>
        public virtual Model StorageAvailable(Func<IStandardAttributes, string> storageName)
        {
            var savail = new StorageAvailable(storageName ?? throw new ArgumentNullException(nameof(storageName)));
            Statements.Blocks.Add(savail);
            return this;
        }

        #endregion

        #region SUNAVAIL Block

        /// <summary>
        /// SUNAVAIL Block.
        /// A SAVAIL Block ensures that a Storage Entity is in the unavailable state.
        /// </summary>
        /// <param name="storageName">Storage entity name. Must not be null. Must address to predefined Storage Entity.</param>
        /// <returns>Model with added SAVAIL Block.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="storageName"/> must not be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="storageName"/> must address to predefined Storage Entity.</exception>
        public Model StorageUnavailable(string storageName)
        {
            if (storageName == null)
                throw new ArgumentNullException(nameof(storageName));
            if (!Resources.Storages.ContainsKey(storageName))
                throw new ArgumentOutOfRangeException(nameof(storageName), "Storage entity with given name does not exists in the model.");

            return StorageUnavailable(sna => storageName);
        }

        /// <summary>
        /// SUNAVAIL Block.
        /// A SAVAIL Block ensures that a Storage Entity is in the unavailable state.
        /// </summary>
        /// <param name="storageName">Storage entity name. Must neither be null nor return null. Must address to predefined Storage Entity.</param>
        /// <returns>Model with added SAVAIL Block.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="storageName"/> must not be null.</exception>
        public virtual Model StorageUnavailable(Func<IStandardAttributes, string> storageName)
        {
            var sunavail = new StorageUnavailable(storageName ?? throw new ArgumentNullException(nameof(storageName)));
            Statements.Blocks.Add(sunavail);
            return this;
        }

        #endregion
    }
}
