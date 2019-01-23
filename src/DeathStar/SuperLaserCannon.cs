using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace DeathStar
{
    /// <summary>
    /// Суперлазер
    /// </summary>
    /// <remarks>
    /// Основное оружие Звезды смерти
    /// </remarks>
    public class SuperLaserCannon : ICannon
    {
        /// <summary>
        /// Максимальный уровень "здоровья"
        /// </summary>
        public readonly short MaxHealth = 200;

        /// <summary>
        /// Требуемый уровень энергии для выстрела
        /// </summary>
        public readonly double RequiredEnergyLevelForShot = 1500;

        /// <summary>
        /// Уровень энергия, запрашиваемой у реактора за один запрос
        /// </summary>
        private const double EnergyLevelPerReactorRequest = 100;

        /// <summary>
        /// Задержка между запросами энергии у реактора
        /// </summary>
        /// <remarks>
        /// Нужна, чтобы не обесточить весь звезодет
        /// </remarks>
        private readonly TimeSpan _delayBetweenChargeRequest = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Источник для отмены выстрела
        /// </summary>
        private readonly CancellationTokenSource _fireCts;


        /// <summary>
        /// Задача, в которой выполняется подготовка к выстрелу и сам выстре
        /// </summary>
        private Task _fireTask;


        /// <summary>
        /// Энергия для выстрела
        /// </summary>
        [CanBeNull]
        private Energy _energyForShot;

        [NotNull]
        private readonly IReactor _reactor;

        /// <inheritdoc />
        public short HealthLevel { get; private set; }
        
        /// <inheritdoc />
        public bool IsActive { get; private set; }

        /// <inheritdoc />
        public SuperLaserCannon([NotNull] IReactor reactor)
        {
            _reactor = reactor ?? throw new ArgumentNullException(nameof(reactor));
            _fireCts = new CancellationTokenSource();
            HealthLevel = MaxHealth;
        }

        /// <inheritdoc />
        public Task ActivateAsync()
        {
            if (IsActive) return Task.CompletedTask;
            if (HealthLevel <= 0) return Task.CompletedTask;

            IsActive = true;

            return Task.CompletedTask;
        }



        /// <inheritdoc />
        public Task DeactivateAsync()
        {
            if (!IsActive) return Task.CompletedTask;
            if (HealthLevel <= 0) return Task.CompletedTask;

            _fireCts.Cancel();

            return _fireTask ?? Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task FireAsync()
        {
            if (!IsActive)
                throw new InvalidOperationException($"Необходимо сначала активровать оружие методом {nameof(ActivateAsync)}");
            if (HealthLevel <= 0) return Task.CompletedTask;

            _fireTask = InternalFireAsync(_fireCts.Token);

            return _fireTask;
        }

        /// <summary>
        /// Асинхронно совершает выстрел
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task InternalFireAsync(CancellationToken cancellationToken)
        {
            var notFirstRequest = false;
            try
            {
                while (NeedCharge(_energyForShot))
                {
                    if (notFirstRequest)
                    {
                        await Task.Delay(_delayBetweenChargeRequest, cancellationToken);
                    }

                    if (HealthLevel <=0)  break;
                    
                    var energy = _reactor.GetEnergy(EnergyLevelPerReactorRequest);
                    notFirstRequest = true;

                    if (energy == null) continue;

                    _energyForShot = _energyForShot == null
                        ? energy
                        : new Energy(_energyForShot.Level + energy.Level);

                }

                if (HealthLevel <=0) return;
                // тут взрываем планеты, крейсеры, галактики
            }
            catch (TaskCanceledException)
            {
                // все окей, оружие деактивировали
            }
        }

        /// <summary>
        /// Проверяет необходимость зарядки оружия
        /// </summary>
        /// <param name="currentEnergy"></param>
        /// <returns></returns>
        /// <remarks>
        /// Таркин лично контролирует все расчеты, запретил выносить проверку в отдельный модуль
        /// internal, чтобы мог проверять в своей сборке
        /// </remarks>
        internal bool NeedCharge([CanBeNull] Energy currentEnergy)
        {
            if (currentEnergy == null) return true;

            return currentEnergy.Level < RequiredEnergyLevelForShot;
        }


        public void GetDamage([NotNull] Damage damage)
        {
            if (damage == null) throw new ArgumentNullException(nameof(damage));

            if (HealthLevel <= 0) return;

            HealthLevel = damage.Level > HealthLevel
                ? (short) 0
                : (short)(HealthLevel - damage.Level);
        }
    }
}