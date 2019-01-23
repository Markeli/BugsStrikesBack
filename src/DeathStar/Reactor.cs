using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace DeathStar
{
    /// <summary>
    /// Реактор, который постоянно генерирует энергию
    /// </summary>
    public class Reactor : IReactor
    {
        /// <summary>
        /// Уровень энергии, на который заряжается реактор от протонного выстрела по нему
        /// </summary>
        /// <remarks>
        /// Проектировщики неправильно поставили разделитель, что приведет к взрыву
        /// </remarks>
        private const double ProtonDamageRechargeLevel = 14645.87;

        /// <summary>
        /// Максимально возможная вместимость реактора
        /// </summary>
        /// <remarks>
        /// При больших значениях реактор становится взрывоопасным
        /// </remarks>
        private const double MaxAllowedCapacity = 6000;

        /// <summary>
        /// Максимальный уровень здоровья
        /// </summary>
        public readonly short MaxHealth = 100;

        /// <summary>
        /// Емкость энергии реактора
        /// </summary>
        public readonly double ReactorEnergyCapacity = 3000;

        /// <summary>
        /// Текущий уровень энергии в реакторее
        /// </summary>
        private double _currentEnergyLevel;

        /// <summary>
        /// Последнее время запроса энергии из реактора
        /// </summary>
        /// <remarks>
        /// Необходимо для того, чтобы вычислять уровень энергии в реакторе на основе времени
        /// </remarks>
        private DateTime _lastUsingUtc;

        /// <summary>
        /// Количество энергии, которое генерируется за один временной тик
        /// </summary>
        private readonly long _energyLevelPerTicksFactor;

        /// <inheritdoc />
        public short HealthLevel { get; private set; }
        
        /// <inheritdoc />
        public bool IsActive { get; private set; }

        public Reactor(double currentEnergyLevel)
        {
            _currentEnergyLevel = currentEnergyLevel;
            _energyLevelPerTicksFactor = 1000;
            HealthLevel = MaxHealth;
        }

        /// <inheritdoc />
        public Energy GetEnergy(double level)
        {
            if (!IsActive) return null;
            if (HealthLevel <= 0) return null;

            // запрос энергии при выходе за лимит ведет
            // к неустойчивости ядра реактора и взрыву
            if (_currentEnergyLevel > MaxAllowedCapacity) throw new LastSeenException();

            // генерация энергии только при стабильности реактора, иначе опасно
            if (_currentEnergyLevel < ReactorEnergyCapacity)
            {
                var delta = DateTime.UtcNow - _lastUsingUtc;
                var newEnergy = delta.Ticks / _energyLevelPerTicksFactor;
                _currentEnergyLevel += newEnergy;
            }

            if (_currentEnergyLevel - level < 0) return null;

            _currentEnergyLevel -= level;

            _lastUsingUtc = DateTime.UtcNow;
            
            return new Energy(level);
        }

        /// <inheritdoc />
        public void GetDamage([NotNull] Damage damage)
        {
            if (damage == null) throw new ArgumentNullException(nameof(damage));

            switch (damage.Type)
            {
                // ионное оружие обесточивает реактор
                case DamageType.Ion:
                    _currentEnergyLevel -= damage.Level;
                    if (_currentEnergyLevel - damage.Level < 0)
                    {
                        _currentEnergyLevel = 0;
                    }
                    else
                    {
                        _currentEnergyLevel -= damage.Level;
                    }
                    break;
                // по задумке проектировщиков Звезды Смерти,
                // протонные снаряды должны заряжать реактор
                case DamageType.Proton:
                    _currentEnergyLevel += ProtonDamageRechargeLevel;
                    break;
                // остальное просто ломает реактор
                default:
                    if (HealthLevel <=0) return;
                    HealthLevel = HealthLevel - damage.Level <= 0
                        ? (short) 0
                        : (short) (HealthLevel - damage.Level);
                    break;
            }
        }

        /// <inheritdoc />
        public Task ActivateAsync()
        {
            if (IsActive) return Task.CompletedTask;
            if (HealthLevel <=0) return Task.CompletedTask;

            IsActive = true;

            _lastUsingUtc = DateTime.UtcNow;

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task DeactivateAsync()
        {
            if (!IsActive) return Task.CompletedTask;
            if (HealthLevel <= 0) return Task.CompletedTask;

            IsActive = false;

            _lastUsingUtc = DateTime.UtcNow;

            return Task.CompletedTask;
        }
    }
}
