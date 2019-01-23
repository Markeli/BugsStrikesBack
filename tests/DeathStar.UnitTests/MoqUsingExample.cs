using System;
using System.Threading.Tasks;
using DeathStar;
using Moq;
using Xunit;

namespace DeathStart.UnitTests
{
    /// <summary>
    /// Пример использования Moq
    /// </summary>
    public class MoqUsingExample
    {
        /// <summary>
        /// Проверка активации генератора
        /// </summary>
        /// <remarks>
        /// Пример того, как с использование Moq можно удобно разрывать зависимости
        /// А еще тестировать можно асинхронные методы, и это удобно
        /// </remarks>
        [Fact]
        public async Task ShieldGenerator_Activate_IsActivated()
        {
            // создаем подделку нужного интерфейса
            var mock = new Mock<IReactor>();
            // настраиваем, а именно говорим, что при вызове метода GetEnergy
            // для любого аргумента должен возвращаться объект Energy с задданым уровнем из аргумента
            mock
                .Setup(x =>x.GetEnergy(It.IsAny<double>()))
                .Returns<double>(s => new Energy(s));
            // создаем объект, который будет использовать нашу подделку
            var  shield = new ProtectiveFieldGenerator(mock.Object);

            await shield.ActivateAsync();

            Assert.True(shield.IsActive, "Shield not activated");
            Assert.Equal(shield.MaxShieldLevel, shield.ShieldLevel);
        }
    }
}
