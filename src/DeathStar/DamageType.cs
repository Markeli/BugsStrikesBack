namespace DeathStar
{
    /// <summary>
    /// Тип урона
    /// </summary>
    public enum DamageType : byte
    {
        /// <summary>
        /// Урон от ионного оружия
        /// </summary>
        Ion,
        /// <summary>
        /// Урон от протонного оружия
        /// </summary>
        Proton,
        /// <summary>
        /// Урон от лазера
        /// </summary>
        Laser,
        /// <summary>
        /// Механический урон
        /// </summary>
        Mechanic
    }
}