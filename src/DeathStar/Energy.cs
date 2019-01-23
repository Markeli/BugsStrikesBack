using System;

namespace DeathStar
{
    /// <summary>
    /// Энергия
    /// </summary>
    public class Energy
    {
        /// <summary>
        /// Количество энергии
        /// </summary>
        public double Level { get; }

        /// <inheritdoc />
        public Energy(double level)
        {
            if (level <= 0) throw new ArgumentOutOfRangeException(nameof(level));
            Level = level;
        }
    }
}
