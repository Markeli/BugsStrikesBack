using System;

namespace DeathStar
{
    /// <summary>
    /// Урон
    /// </summary>
    public class Damage
    {
        /// <summary>
        /// Уровен урона
        /// </summary>
        public short Level { get; }

        /// <summary>
        /// Тип урона
        /// </summary>
        public DamageType Type { get; }

        /// <inheritdoc />
        public Damage(short level, DamageType type)
        {
            if (level < 0) throw new ArgumentOutOfRangeException(nameof(level));

            Level = level;
            Type = type;
        }
    }
}