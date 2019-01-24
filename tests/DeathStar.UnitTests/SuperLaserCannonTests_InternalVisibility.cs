using DeathStar;
using Moq;
using Xunit;

namespace DeathStart.UnitTests
{
    /// <summary>
    /// Тесты <see cref="SuperLaserCannon"/>. Работа с internal методом
    /// </summary>
    public class SuperLaserCannonTests_InternalVisibility
    {
        /// <summary>
        /// Проверяет необходимость дозарядки Луча смерти при полной разрядке оружия
        /// </summary>
        [Fact]
        public void NeedCharge_OnEmptyEnergy_True()
        {
            var reactor = Mock.Of<IReactor>();
            
            var superLaser = new SuperLaserCannon(reactor);


            var needCharge = superLaser.NeedCharge(null);
            
            Assert.True(needCharge);
        }
    }
}