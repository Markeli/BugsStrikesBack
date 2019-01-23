using System;
using JetBrains.Annotations;

namespace DeathStar
{
    /// <summary>
    /// Реактор, питает все компоненты звездолета
    /// </summary>
    public interface IReactor : IDeathStarComponent
    {
        /// <summary>
        /// Возвращает требуемый уровен энергии
        /// </summary>
        /// <param name="level">Требуемый уровен энергии</param>
        /// <returns>Требуемая энергия или <see langword="null"/>, если в реакторе недостаточно энергии</returns>
        [CanBeNull]
        Energy GetEnergy(double level);
    }
}
