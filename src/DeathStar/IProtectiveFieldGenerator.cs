using System;
using JetBrains.Annotations;

namespace DeathStar
{
    /// <summary>
    /// Генератор защитного поля
    /// </summary>
    public interface IProtectiveFieldGenerator : 
        IDeathStarComponent, 
        IDisposable
    {
        /// <summary>
        /// Текущий уровень защиты
        /// </summary>
        short ShieldLevel { get; }

        /// <summary>
        /// Поглощает урон и возвращает тот урон, который защитное поле не смогло поглотить
        /// </summary>
        /// <param name="damage">Урон по звездолету</param>
        /// <returns>Урон, который не удалось поглотить </returns>
            [CanBeNull]
        Damage AbsorbDamage([NotNull] Damage damage);
    }
}
