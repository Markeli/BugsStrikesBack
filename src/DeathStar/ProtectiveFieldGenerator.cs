using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace DeathStar
{
    /// <summary>
    /// Генератор защитного поля
    /// </summary>
    public class ProtectiveFieldGenerator : IProtectiveFieldGenerator
    {
        /// <summary>
        /// Максимальный уровень "Здоровья"
        /// </summary>
        public readonly short MaxHealthLevel = 120;

        /// <summary>
        /// Максимальный уровень защитного поля
        /// </summary>
        public readonly short MaxShieldLevel = 100;

        /// <summary>
        /// Уровень поглощения защитным полем урона от механического повреждения
        /// </summary>
        internal readonly  double MechanicDamageReduceFactor = 0.74;

        /// <summary>
        /// Уровень поглощения защитным полем урона от лазерного оружия
        /// </summary>
        internal readonly  double LaserDamageReduceFactor = 0.9;

        /// <summary>
        /// Уровень поглощения защитным полем урона от ионного оружия
        /// </summary>
        internal readonly double IonDamageReduceFactor = 0.2;

        /// <summary>
        /// Уровень поглощения защитным полем урона от протонного оружия
        /// </summary>
        internal readonly  double ProtonDamageReduceFactor = 0.1;

        /// <summary>
        /// Точность измерений
        /// </summary>
        private const double Tolerance = 1e-10;

        /// <summary>
        /// Уровень регенации защитного поля за один такт
        /// </summary>
        private const short ShieldRegenerationLevel = 4;

        /// <summary>
        /// Период регенрации защитного поля
        /// </summary>
        private readonly TimeSpan _regenerationPeriod = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Реактор для получения энергии
        /// </summary>
        protected IReactor Reactor;

        /// <summary>
        /// Источник отмены задачи регенерации (<see cref="_regenerationTask"/>)
        /// </summary>
        [CanBeNull]
        private CancellationTokenSource _regenerationCts;

        /// <summary>
        /// Задачи регенерации защитного поля
        /// </summary>
        private Task _regenerationTask;

        /// <inheritdoc />
        public short HealthLevel { get; private set; }

        /// <inheritdoc />
        public bool IsActive { get; private set; }

        public short ShieldLevel { get; private set; }

        public ProtectiveFieldGenerator()
        {
            ShieldLevel = MaxShieldLevel;
            HealthLevel = MaxHealthLevel;
        }

        /// <summary>
        /// Конструктор с зависимостью
        /// </summary>
        /// <param name="reactor"></param>
        public ProtectiveFieldGenerator([NotNull] IReactor reactor)
        {
            ShieldLevel = MaxShieldLevel;
            HealthLevel = MaxHealthLevel;
            Reactor = reactor ?? throw new ArgumentNullException(nameof(reactor));
        }
        
        /// <summary>
        /// Фабричный метод для разрыва зависимости
        /// </summary>
        /// <returns></returns>
        public virtual IReactor GetReactor()
        {
            return Reactor;
        }


        internal async Task RegenerateAsync(CancellationToken cancellationToken)
        {
            const double level = 10;
            do
            {
                // генератор сломан
                if (HealthLevel <= 0) break;
                
                try
                {
                    var reactor = GetReactor();
                    if (reactor == null) throw new InvalidOperationException("Не установлен реактор");

                    var energy = reactor.GetEnergy(level);
                    var isEnergyEnough = energy != null
                               && Math.Abs(energy.Level - level) < Tolerance;
                    if (isEnergyEnough)
                    {
                        ShieldLevel = (short)Math.Min(
                            MaxShieldLevel, 
                            ShieldLevel + ShieldRegenerationLevel);
                    }
                    await Task.Delay(_regenerationPeriod, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // все нормально, щит отключили
                }
            } while (!cancellationToken.IsCancellationRequested);
        }

        public void GetDamage([NotNull] Damage damage)
        {
            if (damage == null) throw new ArgumentNullException(nameof(damage));

            if (HealthLevel <= 0) return;

            HealthLevel = damage.Level > HealthLevel
                ? (short)0
                : (short)(HealthLevel - damage.Level);
        }

        public Task ActivateAsync()
        {
            if (IsActive) return Task.CompletedTask;
            if (HealthLevel <= 0) return Task.CompletedTask;

            IsActive = true;
            _regenerationCts = new CancellationTokenSource();
            var regenerationCancellationToken = _regenerationCts.Token;
            _regenerationTask = RegenerateAsync(regenerationCancellationToken);

            return Task.CompletedTask;
        }

        public Task DeactivateAsync()
        {
            if (!IsActive) return Task.CompletedTask;
            if (HealthLevel <= 0) return Task.CompletedTask;

            IsActive = false;
            _regenerationCts?.Cancel();

            return _regenerationTask ?? Task.CompletedTask;
        }


        public void Dispose()
        {
            DeactivateAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public Damage AbsorbDamage(Damage damage)
        {
            if (damage == null) throw new ArgumentNullException(nameof(damage));
            if (ShieldLevel <= 0) return damage;

            double damageReduceFactor;
            switch (damage.Type)
            {
                case DamageType.Ion:
                    damageReduceFactor = IonDamageReduceFactor;
                    break;
                case DamageType.Proton:
                    damageReduceFactor = ProtonDamageReduceFactor;
                    break;
                case DamageType.Laser:
                    damageReduceFactor = LaserDamageReduceFactor;
                    break;
                case DamageType.Mechanic:
                    damageReduceFactor = MechanicDamageReduceFactor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var damageAfterReduce = damage.Level - Convert.ToInt16(Math.Round(damageReduceFactor * damage.Level));
            if (damageAfterReduce <= 0) return null;

            var shieldDecreaseLevel = (short)(damageAfterReduce / 2);
            ShieldLevel = (short)Math.Max(0, ShieldLevel - shieldDecreaseLevel);

            return new Damage((short)damageAfterReduce, damage.Type);
        }
    }
}
