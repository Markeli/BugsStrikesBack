using System.Threading.Tasks;

namespace DeathStar
{
    /// <summary>
    /// Орудие
    /// </summary>
    public interface ICannon : IDeathStarComponent
    {
        /// <summary>
        /// Асинхронно совершает выстрел
        /// </summary>
        /// <remarks>
        /// Предполагаем, что орудие умное и само знает, куда стрелять
        /// </remarks>
        Task FireAsync();
    }
}