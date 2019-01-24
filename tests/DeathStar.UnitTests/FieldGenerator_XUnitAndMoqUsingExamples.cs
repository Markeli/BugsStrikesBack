using System;
using System.Threading.Tasks;
using DeathStar;
using Moq;
using Xunit;

namespace DeathStart.UnitTests
{
    /// <summary>
    /// Тесты <see cref="ProtectiveFieldGenerator"/>. Примеры использования Moq и xUnit
    /// </summary>
    public class FieldGenerator_XUnitAndMoqUsingExamples
    {
        /// <summary>
        /// Проверка активации генератора
        /// </summary>
        /// <remarks>
        /// Пример того, как с использование Moq можно удобно разрывать зависимости
        /// А еще тестировать можно асинхронные методы, и это удобно
        /// </remarks>
        [Fact]
        public async Task ActivateAsync_BasicCase_IsActivated()
        {
            // создаем подделку нужного интерфейса
            var mock = new Mock<IReactor>();
            // настраиваем, а именно говорим, что при вызове метода GetEnergy
            // для любого аргумента должен возвращаться объект Energy с задданым уровнем из аргумента
            mock
                .Setup(x =>x.GetEnergy(It.IsAny<double>()))
                .Returns<double>(s => new Energy(s));
            // создаем объект, который будет использовать нашу подделку
            var  fieldGenerator = new ProtectiveFieldGenerator(mock.Object);

            await fieldGenerator.ActivateAsync();

            Assert.True(fieldGenerator.IsActive, "Shield not activated");
            Assert.Equal(fieldGenerator.MaxShieldLevel, fieldGenerator.ShieldLevel);
        }
        
        /// <summary>
        /// Проверка активации генератора и обращения к реактору
        /// </summary>
        [Fact]
        public async Task ActivateAsync_BasicCase_ReactorCallbacksCalled()
        {
            var isRaisedBeforeReturnEnergy = false;
            var isRaisedAfterReturnEnergy = false;
            
            var mock = new Mock<IReactor>();
            // можем фиксировать вызов метода
            mock
                .Setup(x =>x.GetEnergy(It.IsAny<double>()))
                .Callback(() => isRaisedBeforeReturnEnergy = true)
                .Returns<double>(s => new Energy(s))
                .Callback(() => isRaisedAfterReturnEnergy = true);
            
            var  fieldGenerator = new ProtectiveFieldGenerator(mock.Object);

            await fieldGenerator.ActivateAsync();

            Assert.True(isRaisedBeforeReturnEnergy);
            Assert.True(isRaisedAfterReturnEnergy);
        }
        
        /// <summary>
        /// Проверка активации генератора и обращения к реактору
        /// </summary>
        [Fact]
        public async Task ActivateAsync_BasicCase_ReactorCallVerified()
        {
            var isRaisedBeforeReturnEnergy = false;
            var isRaisedAfterReturnEnergy = false;
            
            var mock = new Mock<IReactor>();
            
            var  fieldGenerator = new ProtectiveFieldGenerator(mock.Object);

            await fieldGenerator.ActivateAsync();

            mock.Verify(x => x.GetEnergy(10), 
                Times.AtLeast(1));
        }
        
        /// <summary>
        /// Проверка активации генератора и обращения к реактору
        /// </summary>
        [Fact]
        public async Task DeactivateAsync_OnUnexpectedException_Throws()
        {
            var isRaisedBeforeReturnEnergy = false;
            var isRaisedAfterReturnEnergy = false;
            
            var mock = new Mock<IReactor>();
            mock
                .SetupSequence(x => x.GetEnergy(It.IsInRange(0, 20, Range.Inclusive)))
                .Returns(() => new Energy(10))
                .Returns(() => new Energy(10))
                .Throws<InvalidOperationException>();
            
            var  fieldGenerator = new ProtectiveFieldGenerator(mock.Object);

            await fieldGenerator.ActivateAsync();
            // подождем немного
            await Task.Delay(TimeSpan.FromSeconds(10));
            Assert.ThrowsAsync<AggregateException>(async () => await fieldGenerator.DeactivateAsync());

        }
        
        /// <summary>
        /// Проверка поглощения урона при нормальной работе генератора и реактора
        /// </summary>
        [Theory]
        [InlineData(10, DamageType.Ion, 8)]
        [InlineData(10, DamageType.Proton, 9)]
        [InlineData(10, DamageType.Mechanic, 3)]
        [InlineData(10, DamageType.Laser, 1)]
        public async Task AbsorbDamage_FieldIsOk_Ok(
            short damageLevel,
            DamageType damageType,
            short realDamageLevel)
        {
            var isRaisedBeforeReturnEnergy = false;
            var isRaisedAfterReturnEnergy = false;
            
            var mock = new Mock<IReactor>();
            mock
                .Setup(x => x.GetEnergy(It.IsAny<double>()))
                .Returns<double>(level => new Energy(level));
            
            var  fieldGenerator = new ProtectiveFieldGenerator(mock.Object);
            await fieldGenerator.ActivateAsync();
            var damage = new Damage(damageLevel, damageType);

            
            var realDamage = fieldGenerator.AbsorbDamage(damage);
            
            Assert.NotNull(realDamage);
            Assert.NotSame(realDamage, damage);
            Assert.Equal(realDamageLevel, realDamage.Level);
        }
    }
}
