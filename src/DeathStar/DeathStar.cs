using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace DeathStar
{
    /// <summary>
    /// Звезда Смерти
    /// </summary>
    public class DeathStar
    {
        /// <summary>
        /// Максимальный уровень "здоровья" корпуса Звезды Смерти
        /// </summary>
        public readonly short MaxHullHealthLevel = 2000;

        /// <summary>
        /// Реактор
        /// </summary>
        [NotNull]
        private readonly IReactor _reactor;

        /// <summary>
        /// Генератор защитного поля
        /// </summary>
        [NotNull]
        private readonly IProtectiveFieldGenerator _protectiveFieldGenerator;
        
        /// <summary>
        /// Луч смерти
        /// </summary>
        [NotNull]
        private readonly SuperLaserCannon _superLaserCannon;

        /// <summary>
        /// Рандомайзер для распределения урона
        /// </summary>
        private readonly Random _damageRandomizer;

        /// <summary>
        /// "Здоровье" корпуса Звезды Смерти
        /// </summary>
        private short _hullHealthLevel;

        /// <summary>
        /// Компоненты Звезды Смерти
        /// </summary>
        [NotNull]
        private readonly List<IDeathStarComponent> _components;

        /// <summary>
        /// "Здоровье" Звезды Смерти 
        /// </summary>
        /// <remarks>
        /// Учитывается "здоровье" всех компонентов
        /// </remarks>
        public short HealthLevel => (short)(_hullHealthLevel + _components.Sum(x => x.HealthLevel));

        /// <summary>
        /// Уровень защитного поля
        /// </summary>
        public short ShieldLevel => _protectiveFieldGenerator.ShieldLevel;


        /// <inheritdoc />
        public DeathStar(
            IReactor reactor,
            IProtectiveFieldGenerator protectiveFieldGenerator,
            SuperLaserCannon superLaserCannon)
        {
            _reactor = reactor;
            _protectiveFieldGenerator = protectiveFieldGenerator;
            _superLaserCannon = superLaserCannon;

            _components = new List<IDeathStarComponent>(3)
            {
                reactor,
                protectiveFieldGenerator,
                superLaserCannon
            };
            _hullHealthLevel = MaxHullHealthLevel;
            _damageRandomizer = new Random();
        }


        /// <summary>
        /// Запускает Звезду Смерти
        /// </summary>
        /// <returns></returns>
        public Task StartAsync()
        {
            var activatingTasks = _components.Select(x => x.ActivateAsync());

            return Task.WhenAll(activatingTasks);
        }

        /// <summary>
        /// Останавливает Звезду Смерти
        /// </summary>
        /// <returns></returns>
        public Task StopAsync()
        {
            var deactivatingTasks = _components.Select(x => x.DeactivateAsync());

            return Task.WhenAll(deactivatingTasks);
        }

        /// <summary>
        /// Асинхронно уничтожает повстанцев
        /// </summary>
        /// <returns></returns>
        public Task DestroyAsync()
        {
            return _superLaserCannon.FireAsync();
        }

        /// <summary>
        /// Получить урон
        /// </summary>
        /// <param name="damage">Урон</param>
        public void GetDamage([NotNull] Damage damage)
        {
            if (damage == null) throw new ArgumentNullException(nameof(damage));

            var realDamage = _protectiveFieldGenerator.AbsorbDamage(damage);

            var damageComponentIndex = _damageRandomizer.Next(_components.Count);
            if (damageComponentIndex == _components.Count)
            {
                _hullHealthLevel = (short)Math.Max(0, _hullHealthLevel - damage.Level);
            }
            else
            {
                var damagedComponent = _components[damageComponentIndex];
                damagedComponent.GetDamage(damage);
            }

            // если корпус корабля уничтожен, то это фиаско
            if (HealthLevel <= 0 || _hullHealthLevel <=0) 
                throw new LastSeenException();
        }
    }
}