using System.Threading.Tasks;

namespace DeathStar
{
    /// <summary>
    /// Компонент Звезды смерти
    /// </summary>
    public interface IDeathStarComponent
    {
        /// <summary>
        /// "Здоровье" компонента
        /// </summary>
        short HealthLevel { get; }

        /// <summary>
        /// Получает урон
        /// </summary>
        /// <param name="damage">Урон</param>
        void GetDamage(Damage damage);

        /// <summary>
        /// Признак активности компонета
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Асинхронно активирует компонент
        /// </summary>
        /// <returns></returns>
        Task ActivateAsync();

        /// <summary>
        /// Асинхронно деактивирует компонент
        /// </summary>
        /// <returns></returns>
        Task DeactivateAsync();
    }
}