using System;
using System.Threading.Tasks;
using DeathStar;
using Moq;
using Xunit;

namespace DeathStart.UnitTests
{
    /// <summary>
    /// Тесты <see cref="DeathStar"/>. Примеры использования xUnit и Moq
    /// </summary>
    public class DeathStatTests_XUnitAndMoqExamples : IClassFixture<DeathStarTestsFixture>
    {
        private readonly DeathStarTestsFixture _data;

        public DeathStatTests_XUnitAndMoqExamples(DeathStarTestsFixture data)
        {
            _data = data;
        }

        /// <summary>
        /// Проверяет начальное "здоровье" Звезды Смерти
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HealthLevel_OnStart_MaxLevel()
        {
            var deathStart = new DeathStar.DeathStar(
                _data.Reactor,
                _data.ProtectiveFieldGenerator,
                _data.SuperLaserCannon);
            
            Assert.Equal(2200,deathStart.HealthLevel);
            _data.Verify();
        }
    }

    /// <summary>
    /// Данные для теста <see cref="DeathStatTests_XUnitAndMoqExamples"/>
    /// </summary>
    public class DeathStarTestsFixture : IDisposable
    {
        public IReactor Reactor { get; }
        
        public IProtectiveFieldGenerator ProtectiveFieldGenerator { get; }
        
        public SuperLaserCannon SuperLaserCannon { get; }

        private readonly Mock<IProtectiveFieldGenerator> _generator;
        
        public DeathStarTestsFixture()
        {
            Reactor = Mock.Of<IReactor>();
            _generator = new Mock<IProtectiveFieldGenerator>();
            
            ProtectiveFieldGenerator = _generator.Object;
            SuperLaserCannon = new SuperLaserCannon(Reactor);
         
        }

        public void Verify()
        {
            _generator.VerifyGet(x => x.HealthLevel);
        }
        
        public void Dispose()
        {
            ProtectiveFieldGenerator?.Dispose();
        }
    }
}