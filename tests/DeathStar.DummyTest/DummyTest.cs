using System;
using System.Threading.Tasks;

namespace DeathStar.DummyTest
{
    class DummyTest
    {
        static async Task Main(string[] args)
        {
            await TestReactorAsync();

            Console.ReadKey();
        }

        private static async Task TestReactorAsync()
        {
            // подготовка (arrange)
            var reactor = new Reactor(100);
            await reactor.ActivateAsync();

            // действие (act)
            var energy = reactor.GetEnergy(100);

            // утверждение (assert)
            Console.WriteLine(energy != null);
            Console.WriteLine(energy.Level == 100);
        }
    }
}
