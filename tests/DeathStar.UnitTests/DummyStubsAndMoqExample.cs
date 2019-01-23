using System.Threading.Tasks;
using DeathStar;
using Xunit;

namespace DeathStart.UnitTests
{
    /// <summary>
    /// Пример ручного создания подделки
    /// </summary>
    public class DummyStubsAndMoqExample
    {
        [Fact]
        public async Task StubTestAsync()
        {
            var stub = new ReactorFake();
            var shield = new ProtectiveFieldGenerator(stub);

            await shield.ActivateAsync();
            
            Assert.True(stub.IsActive);

        }

        /// <summary>
        /// Фейковый реактор с настраиваемым нами поведением
        /// </summary>
        private class ReactorFake : IReactor
        {
            /// <inheritdoc />
            public short HealthLevel { get; }

            /// <inheritdoc />
            public bool IsActive { get; private set; }

            public void GetDamage(Damage damage)
            {
                throw new System.NotImplementedException();
            }

            /// <inheritdoc />
            public Task ActivateAsync()
            {
                IsActive = true;
                return Task.CompletedTask;
            }

            /// <inheritdoc />
            public Task DeactivateAsync()
            {
                throw new System.NotImplementedException();
            }

            /// <inheritdoc />
            public Energy GetEnergy(double level)
            {
                IsActive = true;
                return new Energy(level);
            }
        }

    }

   
}
