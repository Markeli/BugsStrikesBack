using System;

namespace DeathStar
{
    /// <summary>
    /// Последнее исключение, которое Звезда Смерти уже не переживет
    /// </summary>
    public class LastSeenException : Exception
    {
        public LastSeenException()
        : base("Бабах!")
        {
        }
    }
}