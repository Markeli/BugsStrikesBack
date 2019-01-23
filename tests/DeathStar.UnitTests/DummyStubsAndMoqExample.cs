using System.Threading.Tasks;
using DeathStar;
using Xunit;

namespace DeathStart.UnitTests
{
    /// <summary>
    /// Пример ручного создания заглушки
    /// </summary>
    public class DummyStubsAndMoqExample
    {
        [Fact]
        public async Task StubTestAsync()
        {
            var stub = new ReactorStub1();
            var shield = new ProtectiveFieldGenerator(stub);

            await shield.ActivateAsync();
            
            Assert.True(stub.IsActive);

        }

        private class ReactorStub1 : IReactor
        {
            public short HealthLevel { get; }

            public bool IsActive { get; private set; }

            public void GetDamage(Damage damage)
            {
                throw new System.NotImplementedException();
            }

            public Task ActivateAsync()
            {
                IsActive = true;
                return Task.CompletedTask;
            }

            public Task DeactivateAsync()
            {
                throw new System.NotImplementedException();
            }

            public Energy GetEnergy(double level)
            {
                IsActive = true;
                return new Energy(level);
            }
        }

    }

   
}
