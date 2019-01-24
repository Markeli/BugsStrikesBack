using System;
using System.Threading.Tasks;
using DeathStar;
using Xunit;

namespace DeathStart.UnitTests
{
    /// <summary>
    /// Тест <see cref="Reactor"/>. Пример использования атрибутов xUnit
    /// </summary>
    /// <remarks>
    /// В xUnit тестовый класс не надо помечать никаким атрибутом
    /// </remarks>
    public class ReactorTests_XUnitExamples
    {
        /// <summary>
        /// Проверяет, что после установки начальной энергии в 100 и запросе 100 все ок
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetEnergy_100EnergyOnStart_Ok()
        {
            var reactor = new Reactor(100);
           await reactor.ActivateAsync();

            var energy = reactor.GetEnergy(100);
           
            Assert.True(reactor.IsActive);
            Assert.NotNull(energy);
            Assert.Equal(100, energy.Level);
        }
        
        /// <summary>
        /// Проверяет, что если в иницализации задано слишком много энергии, то произойдет взрыв при запросе энергии
        /// </summary>
        /// <returns></returns>
        [Fact]
        [Trait("Category", "Exceptions")]
        public async Task GetEnergy_OverLimitInit_ThrowsException()
        {
            var reactor = new Reactor(100_000_000);
            await reactor.ActivateAsync();

            var expected = 100;
            Assert.Throws<LastSeenException>(() => reactor.GetEnergy(expected));
        }
        
        /// <summary>
        /// Проверяет, что при передаче некорректных аргументов будет выброшено исключение
        /// </summary>
        /// <returns></returns>
        [Fact]
        [Trait("Category", "Exceptions")]
        public async Task GetDamage_IncorrectArgs_ThrowsException()
        {
            var reactor = new Reactor(100);
            await reactor.ActivateAsync();

            Assert.Throws<ArgumentNullException>(() => reactor.GetDamage(null));
        }
        
        /// <summary>
        /// Проверяет, что если при инициализации задана сразу максимальная емкость, то при запросе энергии все ок
        /// </summary>
        [Fact(Skip="Что-то поломалось, посмотреть позже")]
        public void GetEnergy_MaxCapacityInit_Ok()
        {
            const short maxAllowedCapacity = 6000;
            var expected = 100;
            var reactor = new Reactor(maxAllowedCapacity);

            var energy = reactor.GetEnergy(expected);

            Assert.NotNull(energy);
            Assert.Equal(expected, energy.Level);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(20)]
        [InlineData(60)]
        public async Task GetEnergy_100EnergyOnStartAndValuesInRange_Ok(double energyLevel)
        {
            var reactor = new Reactor(100);
            await reactor.ActivateAsync();

            var energy = reactor.GetEnergy(energyLevel);
           
            Assert.True(reactor.IsActive);
            Assert.NotNull(energy);
            Assert.Equal(energyLevel, energy.Level);
        }
    }
}
