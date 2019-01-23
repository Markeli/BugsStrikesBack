using System.Threading.Tasks;
using DeathStar;
using Xunit;

namespace DeathStart.UnitTests
{
    public class XUnitAttributesExample
    {
      //  [TestCategory("react")]
        [Fact]
        public async Task TestMethodWithCategory()
        {
            var reactor = new Reactor(100);
           await reactor.ActivateAsync();

            var energy = reactor.GetEnergy(100);

            Assert.NotNull(energy);
            Assert.Equal(100, energy.Level);
        }
        
       // [TestCategory("react1")]
     //   [TestCategory("react2")]
        [Fact]
      //  [ExpectedException(typeof(InvalidOperationException))]
        public async Task ExpectingExceptionTest()
        {
            var reactor = new Reactor(100);
            await reactor.ActivateAsync();

            var expected = 100;
            var energy = reactor.GetEnergy(expected);

            Assert.NotNull(energy);
            Assert.Equal(expected, energy.Level);
        }

     //   [TestCategory("react1")]
      //  [TestCategory("react2")]
        [Fact(Skip="Ignore")]
        public void IgnoredTest()
        {
            var expected = 100;
            var reactor = new Reactor(100);

            var energy = reactor.GetEnergy(expected);

            Assert.NotNull(energy);
            Assert.Equal(expected, energy.Level);
        }
    }
}
