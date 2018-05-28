﻿namespace GPSS.StandardAttributes
{
    public interface ILogicSwitchAttributes
    {
        /// <summary>
        /// Logicswitch set.
        /// True if the Logicswitch is in the "set" state,
        /// false otherwise.
        /// </summary>
        /// <remarks>
        /// GPSS World LS$Entnum SNA.
        /// </remarks>
        bool Set { get; }
    }
}