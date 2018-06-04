﻿namespace GPSS.StandardAttributes
{
    // TODO - Добавить генерацию случайных распределений.
    public interface IRandomNumberGeneratorAttributes
    {
        /// <summary>
        /// Random integer between 0 and 999.
        /// </summary>
        /// <remarks>
        /// GPSS World RN$Entnum SNA.
        /// </remarks>
        /// <returns>
        /// Random integer between 0 and 999.
        /// </returns>
        int RandomNumber();

        /// <summary>
        /// Probability Distributions Generators access using current Random Number Generator.
        /// </summary>
        IProbabilityDistributions ProbabilityDistributions { get; }
    }
}